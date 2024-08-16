using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition.Penalties;

public static class PenaltyCalculation
{
    private static readonly ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(PenaltyCalculation));

    /// <summary>
    /// Checks if pilot flight in blue PZ and calculate the penalty
    /// <para>For each track point in the PZ, the penalty will be: infringement [ft] * logger interval / 100  </para>
    /// <para>The penalties of all track points are summed up and round to the tens digit</para>
    /// </summary>
    /// <param name="lowerAltitudeOfBluePZ">the lower altitude of the blue PZ in [m]</param>
    /// <param name="tracks">the list of tracks to check</param>
    /// <param name="useGPSAltitude">true:use GPS altitude; false: use barometric altitude</param>
    /// <param name="penalties">output parameter. a list containing the pilot number, number of violating track points and the penalty</param>
    public static void CheckForBluePZAndCalculatePenaltyPoints(double lowerAltitudeOfBluePZ, bool useGPSAltitude, List<Track> tracks, out List<(int pilotNumber, int numberOfViolatingTrackPoints, TimeSpan durationInBluePZ,double maxAlitudeInFeet, int penalty)> penalties)
    {
        penalties = new();
        double penalty;
        int numberOfViolatingTrackPoints;
        double trackPointInterval;
        foreach (Track track in tracks)
        {
            penalty = 0.0;
            numberOfViolatingTrackPoints = 0;
            trackPointInterval = 1.0;//set to default of 1s
            //if the track has no LoggerInterval or it cannot be parsed, use the default value
            if (track.AdditionalPropertiesFromIGCFile.ContainsKey("LoggerInterval"))
            {
                if (!double.TryParse(track.AdditionalPropertiesFromIGCFile["LoggerInterval"], out trackPointInterval))
                {
                    Logger?.LogWarning("Cannot parse LoggerInterval for track of pilot no. {PilotNumber}, the default value of {trackPointInterval}s is used", track.Pilot.PilotNumber, trackPointInterval);
                }
            }
            else
            {
                Logger?.LogWarning("The track of pilot no. {PilotNumber} has no logger interval stored, the default value of {trackPointInterval}s is used", track.Pilot.PilotNumber, trackPointInterval);
            }
            List<Coordinate> violatingTrackPoints;
            if (useGPSAltitude)
                violatingTrackPoints = track.TrackPoints.Where(x => x.AltitudeGPS > lowerAltitudeOfBluePZ).ToList();
            else
                violatingTrackPoints = track.TrackPoints.Where(x => x.AltitudeBarometric > lowerAltitudeOfBluePZ).ToList();
            numberOfViolatingTrackPoints = violatingTrackPoints.Count;
            foreach (Coordinate coordinate in violatingTrackPoints)
            {
                double altitude = useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric;
                double infrigmentInFeet = CoordinateHelpers.ConvertToFeet(altitude - lowerAltitudeOfBluePZ);
                penalty += infrigmentInFeet * trackPointInterval / 100.0;
            }
            double maxAltitudeInFeet = useGPSAltitude ? CoordinateHelpers.ConvertToFeet(violatingTrackPoints.Max(x => x.AltitudeGPS)) : CoordinateHelpers.ConvertToFeet(violatingTrackPoints.Max(x => x.AltitudeBarometric));
            penalty = Math.Round(penalty / 10.0, 0, MidpointRounding.AwayFromZero) * 10.0;//Round to tens digit
            penalties.Add((track.Pilot.PilotNumber, numberOfViolatingTrackPoints, TimeSpan.FromSeconds(trackPointInterval * numberOfViolatingTrackPoints), maxAltitudeInFeet, (int)penalty));
        }
    }


    /// <summary>
    /// Checks for 2D distance infringements of minimum or maximum allowed distances
    /// <para>calculates the infringement in [%], the penalty is 20 points per percent of infringement</para>
    /// <para>if the infringement is greater than 25%, the pilot is the scored with 'no result', however is this <b>not</b> reflected in the returned penalty</para>
    /// </summary>
    /// <param name="referenceCoordinate">the coordinate to which distances are calculated</param>
    /// <param name="minimumDistance">the minimum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
    /// <param name="maximumDistance">the maximum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
    /// <param name="coordinatesToCheck">the list to be checked, each entry consist of a pilot Number and coordinate pair</param>
    /// <param name="penalties">output parameter. a list containing pilot number, distance, infringement and penalty points</param>
    /// <returns>true:success; false:error</returns>
    public static bool CheckFor2DDistanceInfringementAndCalculatePenaltyPoints(Coordinate referenceCoordinate, double minimumDistance, double maximumDistance, List<(int pilotNumber, Coordinate coordinate)> coordinatesToCheck, out List<(int pilotNumber, double distance, double infringement, int penalty)> penalties)
    {
        penalties = new List<(int pilotNumber, double distance, double infrigement, int penalty)>();
        if (referenceCoordinate is null)
        {
            Logger?.LogError("Reference coordinate cannot be null");
            return false;
        }
        if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
        {
            if (minimumDistance > maximumDistance)
            {
                Logger?.LogError("Minimum distance cannot be larger than maximum distance");
                return false;
            }
        }
        double infringement;
        double penalty;
        double distance;
        foreach ((int pilotNumber, Coordinate coordinate) coordinate in coordinatesToCheck)
        {
            infringement = 0.0;
            distance = CoordinateHelpers.Calculate2DDistanceVincentyWSG84(referenceCoordinate, coordinate.coordinate);
            if (!double.IsNaN(minimumDistance))
                if (distance < minimumDistance)
                    infringement = (1.0 - (distance / minimumDistance)) * 100.0;
            if (!double.IsNaN(maximumDistance))
                if (distance > maximumDistance)
                    infringement = ((distance / maximumDistance) - 1.0) * 100.0;
            penalty = Math.Round(infringement, 0, MidpointRounding.AwayFromZero) * 20.0;
            penalties.Add((coordinate.pilotNumber, distance, infringement, (int)penalty));
        }

        return true;
    }

    /// <summary>
    /// Checks for 3D distance infringements of minimum or maximum allowed distances
    /// <para>calculates the infringement in [%], the penalty is 20 points per percent of infringement</para>
    /// <para>if the infringement is greater than 25%, the pilot is the scored with 'no result', however is this <b>not</b> reflected in the returned penalty</para>
    /// </summary>
    /// <param name="referenceCoordinate">the coordinate to which distances are calculated</param>
    /// <param name="minimumDistance">the minimum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
    /// <param name="maximumDistance">the maximum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
    /// <param name="useGPSAltitude">true: uses GPS altitude; false: uses barometric altitude</param>
    /// <param name="coordinatesToCheck">the list to be checked, each entry consist of a pilot Number and coordinate pair</param>
    /// <param name="penalties">output parameter. a list containing pilot number, distance, infringement and penalty points</param>
    /// <returns>true:success; false:error</returns>
    public static bool CheckFor3DDistanceInfringementAndCalculatePenaltyPoints(Coordinate referenceCoordinate, double minimumDistance, double maximumDistance, bool useGPSAltitude, List<(int pilotNumber, Coordinate coordinate)> coordinatesToCheck, out List<(int pilotNumber, double distance, double infringement, int penalty)> penalties)
    {
        penalties = new List<(int pilotNumber, double distance, double infrigement, int penalty)>();
        if (referenceCoordinate is null)
        {
            Logger?.LogError("Reference coordinate cannot be null");
            return false;
        }
        if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
        {
            if (minimumDistance > maximumDistance)
            {
                Logger?.LogError("Minimum distance cannot be larger than maximum distance");
                return false;
            }
        }
        double infringement;
        double penalty;
        double distance;
        foreach ((int pilotNumber, Coordinate coordinate) coordinate in coordinatesToCheck)
        {
            infringement = 0.0;
            distance = CoordinateHelpers.Calculate3DDistance(referenceCoordinate, coordinate.coordinate, useGPSAltitude);
            if (!double.IsNaN(minimumDistance))
                if (distance < minimumDistance)
                    infringement = (1.0 - (distance / minimumDistance)) * 100.0;
            if (!double.IsNaN(maximumDistance))
                if (distance > maximumDistance)
                    infringement = ((distance / maximumDistance) - 1.0) * 100.0;
            penalty = Math.Round(infringement, 0, MidpointRounding.AwayFromZero) * 20.0;
            penalties.Add((coordinate.pilotNumber, distance, infringement, (int)penalty));
        }

        return true;
        //TODO check if minimum or maximum limits have been violated using 3D distance
        //if so calculate infringement in %
        //minimum: (1 - distance/limit) * 100
        //maximum: (distance/limit - 1) * 100
        //penalty: round infringement[%] to one digit * 20
    }

    public static bool CheckForVerticalDistanceInfringementAndCalculatePenaltyPoints(Coordinate referenceCoordinate, double minimumDistance, double maximumDistance, bool useGPSAltitude, List<(int pilotNumber, Coordinate coordinate)> coordinatesToCheck, out List<(int pilotNumber, double distance, double infringement, int penalty)> penalties)
    {
        penalties = new List<(int pilotNumber, double distance, double infrigement, int penalty)>();
        if (referenceCoordinate is null)
        {
            Logger?.LogError("Reference coordinate cannot be null");
            return false;
        }
        if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
        {
            if (minimumDistance > maximumDistance)
            {
                Logger?.LogError("Minimum distance cannot be larger than maximum distance");
                return false;
            }
        }
        double infringementInFeet;
        double penalty;
        double distance;
        foreach ((int pilotNumber, Coordinate coordinate) coordinate in coordinatesToCheck)
        {
            infringementInFeet = 0.0;
            if (useGPSAltitude)
            {
                distance = referenceCoordinate.AltitudeGPS - coordinate.coordinate.AltitudeGPS;
            }
            else
            {
                distance = referenceCoordinate.AltitudeBarometric - coordinate.coordinate.AltitudeBarometric;
            }
            if (!double.IsNaN(minimumDistance))
                if (distance < minimumDistance)
                    infringementInFeet = CoordinateHelpers.ConvertToFeet((1.0 - (distance / minimumDistance)) * 100.0);
            if (!double.IsNaN(maximumDistance))
                if (distance > maximumDistance)
                    infringementInFeet = CoordinateHelpers.ConvertToFeet(((distance / maximumDistance) - 1.0) * 100.0);
            penalty = Math.Round(infringementInFeet, 0, MidpointRounding.AwayFromZero) * 20.0;
            penalties.Add((coordinate.pilotNumber, distance, infringementInFeet, (int)penalty));
        }
        return true;
        //TODO check if minimum or maximum limits have been violated using altitude only
        //if so calculate infringement in %
        //minimum: (1 - distance/limit) * 100
        //maximum: (distance/limit - 1) * 100
        //penalty: round infringement[%] to one digit * 20
    }


    public static void CheckForDangerousFlyingAndCalculatePenaltyPoints(Track track, bool useGPSAltitude, out bool isDangerousFlyingDetected, out List<Coordinate> relatedCoordinates, out double minVerticalVelocity, out double maxVerticalVelocity, out TimeSpan totalDuration, out int penaltyPoints, double maxAbsVerticalVelocityLimit = 8.0, int minDurationSeconds = 5)
    {
        isDangerousFlyingDetected = false;
        maxVerticalVelocity = double.NaN;
        minVerticalVelocity = double.NaN;
        totalDuration = TimeSpan.Zero;
        penaltyPoints = 0;
        relatedCoordinates = [];
        if (double.IsFinite(maxAbsVerticalVelocityLimit) && minDurationSeconds > 0)
        {

            TrackHelpers.CleanTrackPoints(track, useGPSAltitude, 15.0, out List<Coordinate> cleanedUpTrackPoints);

            List<(DateTime timestamp, double altitudeDiff)> altitudeDerivative = [];
            TimeSpan trackPointInterval = TimeSpan.MaxValue;
            for (int index = 0; index < cleanedUpTrackPoints.Count - 1; index++)
            {

                if (useGPSAltitude)
                {
                    double derivative = (cleanedUpTrackPoints[index + 1].AltitudeGPS - cleanedUpTrackPoints[index].AltitudeGPS) / (cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp).TotalSeconds);
                    if (!double.IsNaN(derivative) && !double.IsInfinity(derivative))
                    {
                        if (cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp) < trackPointInterval)
                        {
                            trackPointInterval = cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp);
                        }
                        altitudeDerivative.Add((cleanedUpTrackPoints[index].TimeStamp, derivative));
                    }
                }
                else
                {
                    double derivative = (cleanedUpTrackPoints[index + 1].AltitudeBarometric - cleanedUpTrackPoints[index].AltitudeBarometric) / (cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp).TotalSeconds);
                    if (!double.IsNaN(derivative) && !double.IsInfinity(derivative))
                    {
                        if (cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp) < trackPointInterval)
                        {
                            trackPointInterval = cleanedUpTrackPoints[index + 1].TimeStamp.Subtract(cleanedUpTrackPoints[index].TimeStamp);
                        }
                        altitudeDerivative.Add((cleanedUpTrackPoints[index].TimeStamp, derivative));
                    }
                }
            }

            List<(DateTime timestamp, double altitudeDiff)> violatingPoints = altitudeDerivative.Where(x => Math.Abs(x.altitudeDiff) > maxAbsVerticalVelocityLimit).ToList();
            int consecutiveTrackPointsToCheck = (int)Math.Ceiling(minDurationSeconds / (double)trackPointInterval.Seconds);
            for (int index = 0; index < violatingPoints.Count - consecutiveTrackPointsToCheck; index++)
            {
                if (violatingPoints[index + consecutiveTrackPointsToCheck].timestamp.Subtract(violatingPoints[index].timestamp) <= TimeSpan.FromSeconds(minDurationSeconds))
                {
                    isDangerousFlyingDetected = true;
                    foreach ((DateTime timestamp, double altitudeDiff) in violatingPoints.Skip(index).Take(consecutiveTrackPointsToCheck))
                    {
                        Coordinate trackPoint = cleanedUpTrackPoints.FirstOrDefault(x => x.TimeStamp == timestamp);
                        if (!relatedCoordinates.Contains(trackPoint))
                        {
                            relatedCoordinates.Add(trackPoint);
                        }
                    }
                }
            }

            minVerticalVelocity = altitudeDerivative.Min(x => x.altitudeDiff);
            maxVerticalVelocity = altitudeDerivative.Max(x => x.altitudeDiff);
            totalDuration = TimeSpan.FromSeconds(relatedCoordinates.Count * trackPointInterval.TotalSeconds);
            if (isDangerousFlyingDetected)
            {
                penaltyPoints = (int)Math.Round((Math.Max(maxVerticalVelocity, Math.Abs(minVerticalVelocity)) - maxAbsVerticalVelocityLimit) * 250, 0, MidpointRounding.AwayFromZero);
            }
        }
        //TODO check if vertical velocity is +/- 8m/s  for 5 consecutive seconds
        //penalty: max abs vertical velocity - max abs allowed vertical velocity *250 (use integer)
    }

    public static void CheckForCloseProximityAndCalculatePenaltyPoints(TimeSpan gracePeriodAfterLaunch, TimeSpan gracePeriodBeforeLanding, bool useGPSAltitude, List<Track> tracks)
    {
        int analyzeWindowInSeconds = 60;
        for (int outerIndex = 0; outerIndex < tracks.Count; outerIndex++)
        {
            Track referenceTrack = tracks[outerIndex];
            Coordinate referenceLaunchPoint;
            Coordinate referenceLandingPoint;
            if (!TrackHelpers.EstimateLaunchAndLandingTime(referenceTrack, useGPSAltitude, out referenceLaunchPoint, out referenceLandingPoint))
            {
                Logger?.LogWarning("Failed to estimate launch and landing points for pilot no. {PilotNumber}, the first and last track point will be used instead", referenceTrack.Pilot.PilotNumber);
                referenceLaunchPoint = referenceTrack.TrackPoints[0];
                referenceLandingPoint = referenceTrack.TrackPoints[referenceTrack.TrackPoints.Count - 1];
            }
            List<Coordinate> referenceCoordinates = referenceTrack.TrackPoints.Where(x => x.TimeStamp > referenceLaunchPoint.TimeStamp + gracePeriodAfterLaunch).Where(x => x.TimeStamp < referenceLandingPoint.TimeStamp - gracePeriodBeforeLanding).ToList();//Get all track points x seconds after launch and y seconds before landing
            for (int innerIndex = outerIndex + 1; innerIndex < tracks.Count; innerIndex++)
            {
                Track otherTrack = tracks[innerIndex];
                Coordinate otherLaunchPoint;
                Coordinate otherLandingPoint;
                if (!TrackHelpers.EstimateLaunchAndLandingTime(otherTrack, useGPSAltitude, out otherLaunchPoint, out otherLandingPoint))
                {
                    Logger?.LogWarning("Failed to estimate launch and landing points for pilot no. {PilotNumber}, the first and last track point will be used instead", otherTrack.Pilot.PilotNumber);
                    otherLaunchPoint = otherTrack.TrackPoints[0];
                    otherLandingPoint = otherTrack.TrackPoints[otherTrack.TrackPoints.Count - 1];
                }
                List<Coordinate> otherCoordinates = otherTrack.TrackPoints.Where(x => x.TimeStamp > otherLaunchPoint.TimeStamp + gracePeriodAfterLaunch).Where(x => x.TimeStamp < otherLandingPoint.TimeStamp - gracePeriodBeforeLanding).ToList();//Get all track points x seconds after launch and y 
                List<(List<Coordinate> references, List<Coordinate> others)> violations = new List<(List<Coordinate> a, List<Coordinate> b)>();

                foreach (Coordinate referenceCoordinate in referenceCoordinates)
                {
                    List<Coordinate> coordinates = otherCoordinates.Where(x => x.TimeStamp > referenceCoordinate.TimeStamp - TimeSpan.FromSeconds(analyzeWindowInSeconds / 2))
                        .Where(x => x.TimeStamp < referenceCoordinate.TimeStamp + TimeSpan.FromSeconds(analyzeWindowInSeconds / 2)).
                        Where(x => CoordinateHelpers.Calculate3DDistance(referenceCoordinate, x, useGPSAltitude) < 300).ToList();//get all coordinates with in the analyzing window and less than 75m distance
                    if (coordinates.Count > 0)
                    {
                        if (violations.Count == 0)
                            violations.Add((new List<Coordinate>(), new List<Coordinate>()));
                        violations[^1].references.Add(referenceCoordinate);
                        violations[^1].others.AddRange(coordinates);
                    }
                    else
                    if (violations.Count > 0)
                    {
                        if (violations[^1].references.Count > 0)
                        {
                            violations.Add((new List<Coordinate>(), new List<Coordinate>()));
                        }
                    }
                }
                for (int index = 0; index < violations.Count; index++)
                {
                    List<Coordinate> distinctOthers = violations[index].others.Distinct().ToList();
                    violations[index].others.Clear();
                    violations[index].others.AddRange(distinctOthers);
                }
            }
        }
        //TODO use only trackpoints x seconds after launch and y seconds before landing
        //for each trackpoint: find trackpoint in other track where timestamp is nearest
        //check 3D distance between the two trackpoints
        //Limit 1: more than 3m / s at less than 25m
        //Limit 2: more than 5m / s at less than 50m
        //Limit 3: more than 8m / s at less than 75m
        //Limit 4: more than 8m / s vertical ascend speed
    }
}
