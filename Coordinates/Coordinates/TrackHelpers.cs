using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Coordinates
{
    public static class TrackHelpers
    {
        public static List<double> Calculate2DDistanceBetweenPositionOfDeclarationsAndDeclaredGoal(List<Declaration> declarations)
        {
            List<double> distance2DBetweenPositionOfDeclarationsAndDeclaredGoal = new List<double>();
            foreach (Declaration declaration in declarations)
            {
                distance2DBetweenPositionOfDeclarationsAndDeclaredGoal.Add(CoordinateHelpers.Calculate2DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal));
            }
            return distance2DBetweenPositionOfDeclarationsAndDeclaredGoal;
        }

        public static List<double> Calculate3DDistanceBetweenPositionOfDeclarationsAndDeclaredGoal(List<Declaration> declarations, bool useGPSAltitude)
        {
            List<double> distance3DBetweenPositionOfDeclarationsAndDeclaredGoal = new List<double>();
            foreach (Declaration declaration in declarations)
            {
                distance3DBetweenPositionOfDeclarationsAndDeclaredGoal.Add(CoordinateHelpers.Calculate3DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal, useGPSAltitude));
            }
            return distance3DBetweenPositionOfDeclarationsAndDeclaredGoal;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenDeclaredGoals(List<Declaration> declarations)
        {
            List<(string identifier, double distance)> distance2DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declarations.Count; index++)
            {
                bool useTimeStamp = declarations.Where(x => x.GoalNumber == declarations[index].GoalNumber).Count() > 1;
                for (int iterator = index + 1; iterator < declarations.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"G{declarations[index].GoalNumber}_{declarations[index].DeclaredGoal.TimeStamp:HH:mm:ss}->G{declarations[iterator].GoalNumber}_{declarations[iterator].DeclaredGoal.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"G{declarations[index].GoalNumber}->G{declarations[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declarations[index].DeclaredGoal, declarations[iterator].DeclaredGoal);
                    distance2DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenDeclaredGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenDeclaredGoals(List<Declaration> declarations, bool useGPSAltitude)
        {
            List<(string identifier, double distance)> distance3DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declarations.Count; index++)
            {
                bool useTimeStamp = declarations.Where(x => x.GoalNumber == declarations[index].GoalNumber).Count() > 1;
                for (int iterator = index + 1; iterator < declarations.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"G{declarations[index].GoalNumber}_{declarations[index].DeclaredGoal.TimeStamp:HH:mm:ss}->G{declarations[iterator].GoalNumber}_{declarations[iterator].DeclaredGoal.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"G{declarations[index].GoalNumber}->G{declarations[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declarations[index].DeclaredGoal, declarations[iterator].DeclaredGoal, useGPSAltitude);
                    distance3DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance3DBetweenDeclaredGoals;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenMarkers(List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance2DBetweenMarkers = new List<(string identifier, double distance)>();
            for (int index = 0; index < markerDrops.Count; index++)
            {
                bool useTimeStamp = markerDrops.Where(x => x.MarkerNumber == markerDrops[index].MarkerNumber).Count() > 1;
                for (int iterator = index + 1; iterator < markerDrops.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"M{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->M{markerDrops[iterator].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"M{markerDrops[index].MarkerNumber}->M{markerDrops[iterator].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation);
                    distance2DBetweenMarkers.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkers;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkers(List<MarkerDrop> markerDrops, bool useGPSAltitude)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkers = new List<(string identifier, double distance)>();
            for (int index = 0; index < markerDrops.Count; index++)
            {
                bool useTimeStamp = markerDrops.Where(x => x.MarkerNumber == markerDrops[index].MarkerNumber).Count() > 1;
                for (int iterator = index + 1; iterator < markerDrops.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"M{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->M{markerDrops[index].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"M{markerDrops[index].MarkerNumber}->M{markerDrops[index].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation, useGPSAltitude);
                    distance3DBetweenMarkers.Add((identifier, distance));
                }
            }
            return distance3DBetweenMarkers;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenMarkerAndGoals(List<Declaration> declarations, List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance2DBetweenMarkerAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    string identifier = $"G{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}->M{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation);
                    distance2DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkerAndGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkerAndGoals(List<Declaration> declarations, List<MarkerDrop> markerDrops, bool useGPSAltitude)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkerAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    string identifier = $"G{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}->M{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation, useGPSAltitude);
                    distance3DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance3DBetweenMarkerAndGoals;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenLaunchPointAndGoals(Coordinate launchPoint, List<Declaration> declarations)
        {
            List<(string identifier, double distance)> distance2DBetweenLaunchPointAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                string idenifier = $"TO->G{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}";
                double distance = CoordinateHelpers.Calculate2DDistance(launchPoint, declaration.DeclaredGoal);
                distance2DBetweenLaunchPointAndGoals.Add((idenifier, distance));
            }
            return distance2DBetweenLaunchPointAndGoals;
        }
        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenLaunchPointAndGoals(Coordinate launchPoint, List<Declaration> declarations, bool useGPSAltitude)
        {
            List<(string identifier, double distance)> distance2DBetweenLaunchPointAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                string idenifier = $"TO->G{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}";
                double distance = CoordinateHelpers.Calculate3DDistance(launchPoint, declaration.DeclaredGoal, useGPSAltitude);
                distance2DBetweenLaunchPointAndGoals.Add((idenifier, distance));
            }
            return distance2DBetweenLaunchPointAndGoals;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenLandingPointAndGoals(Coordinate landingPoint, List<Declaration> declarations)
        {
            List<(string identifier, double distance)> distance2DBetweenLandingPointAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                string idenifier = $"TD->Goal{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}";
                double distance = CoordinateHelpers.Calculate2DDistance(landingPoint, declaration.DeclaredGoal);
                distance2DBetweenLandingPointAndGoals.Add((idenifier, distance));
            }
            return distance2DBetweenLandingPointAndGoals;
        }
        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenLandingPointAndGoals(Coordinate landingPoint, List<Declaration> declarations, bool useGPSAltitude)
        {
            List<(string identifier, double distance)> distance2DBetweenLandingPointAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                string idenifier = $"TD->Goal{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}";
                double distance = CoordinateHelpers.Calculate3DDistance(landingPoint, declaration.DeclaredGoal, useGPSAltitude);
                distance2DBetweenLandingPointAndGoals.Add((idenifier, distance));
            }
            return distance2DBetweenLandingPointAndGoals;
        }


        public static void EstimateLaunchAndLandingTime(Track track, bool useGPSAltitude, out Coordinate launchPoint, out Coordinate landingPoint)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            launchPoint = null;
            landingPoint = null;
            List<double> altitudesFiltered;

            List<Coordinate> cleanedUpTrackPoints;
            CleanTrackPoints(track, useGPSAltitude, 15.0, out cleanedUpTrackPoints);

            if (useGPSAltitude)
                altitudesFiltered = cleanedUpTrackPoints.Select(x => x.AltitudeGPS).ToList();
            else
                altitudesFiltered = cleanedUpTrackPoints.Select(x => x.AltitudeBarometric).ToList();
            int filterLength = 5;
            int halfFilterLength = filterLength / 2;//integer division is intended
            for (int index = 0; index < altitudesFiltered.Count; index++)//moving average
            {

                if (index - halfFilterLength < 0)
                    continue;
                if (index + halfFilterLength > altitudesFiltered.Count - 1)
                    continue;
                int filterStart = index - halfFilterLength;
                altitudesFiltered[index] = Math.Round(altitudesFiltered.GetRange(filterStart, filterLength).Average(), 0, MidpointRounding.AwayFromZero);
            }
            List<(int index, double altitudeDifference)> altitudeFilteredDerivative = new List<(int index, double altitudeDifference)>();

            for (int index = 0; index < cleanedUpTrackPoints.Count - 1; index++)
            {
                altitudeFilteredDerivative.Add((index, altitudesFiltered[index + 1] - altitudesFiltered[index]));
            }


            int firstPeak = altitudeFilteredDerivative.FindIndex(x => x.altitudeDifference > 2.0);
            int counter = 0;
            int launchPointIndex = 0;
            for (int index = firstPeak; index >= 1; index--)
            {
                if ((altitudeFilteredDerivative[index].altitudeDifference <= 0) && (Math.Abs(CoordinateHelpers.Calculate2DDistance(cleanedUpTrackPoints[altitudeFilteredDerivative[index].index], cleanedUpTrackPoints[altitudeFilteredDerivative[index].index - 1])) <= 2))
                    counter++;
                else
                    counter = 0;
                if (counter == 20)
                {
                    launchPointIndex = altitudeFilteredDerivative[index + 20].index;
                    break;
                }
            }
            //int launchPointIndex = altitudeDerivative.Take( firstPeak).Last( x => x.altitudeDifference == 0).index;
            launchPoint = cleanedUpTrackPoints[launchPointIndex];
            int lastPeak = altitudeFilteredDerivative.FindLastIndex(x => x.altitudeDifference < -2.0);
            counter = 0;
            int landingPointIndex = altitudeFilteredDerivative[altitudeFilteredDerivative.Count - 1].index;
            for (int index = lastPeak; index < altitudeFilteredDerivative.Count - 1; index++)
            {
                if ((altitudeFilteredDerivative[index].altitudeDifference >= 0) && (Math.Abs(CoordinateHelpers.Calculate2DDistance(cleanedUpTrackPoints[altitudeFilteredDerivative[index].index], cleanedUpTrackPoints[altitudeFilteredDerivative[index].index + 1])) >= 2))
                    counter++;
                else
                    counter = 0;
                if (counter == 20)
                {
                    landingPointIndex = altitudeFilteredDerivative[index - 20].index;
                    break;
                }
            }
            //int landingPointIndex = altitudeDerivative.Skip(lastPeak).First( x => x.altitudeDifference == 0).index;
            landingPoint = cleanedUpTrackPoints[landingPointIndex];
        }

        public static void CheckForDangerousFlying(Track track, bool useGPSAltitude, out bool isDangerousFlyingDetected, out List<Coordinate> relatedCoordinates, out double minVerticalVelocity, out double maxVerticalVelocity, out TimeSpan totalDuration, out int penaltyPoints, double maxAbsVerticalVelocity = 8.0, int minDurationSeconds = 5)
        {
            isDangerousFlyingDetected = false;
            maxVerticalVelocity = double.NaN;
            minVerticalVelocity = double.NaN;
            totalDuration = TimeSpan.Zero;
            penaltyPoints = 0;
            relatedCoordinates = new List<Coordinate>();
            if (double.IsFinite(maxAbsVerticalVelocity) && minDurationSeconds > 0)
            {

                List<Coordinate> cleanedUpTrackPoints;
                CleanTrackPoints(track, useGPSAltitude, 15.0, out cleanedUpTrackPoints);

                List<(DateTime timestamp, double altitudeDiff)> altitudeDerivative = new List<(DateTime timestamp, double altitudeDiff)>();
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

                List<(DateTime timestamp, double altitudeDiff)> violatingPoints = altitudeDerivative.Where(x => Math.Abs(x.altitudeDiff) > maxAbsVerticalVelocity).ToList();
                int consecutiveTrackPointsToCheck = (int)Math.Ceiling(minDurationSeconds / (double)trackPointInterval.Seconds);
                for (int index = 0; index < violatingPoints.Count - consecutiveTrackPointsToCheck; index++)
                {
                    if (violatingPoints[index + consecutiveTrackPointsToCheck].timestamp.Subtract(violatingPoints[index].timestamp) <= TimeSpan.FromSeconds(minDurationSeconds))
                    {
                        isDangerousFlyingDetected = true;
                        foreach ((DateTime timestamp, double altitudeDiff) consecutiveViolatingPoints in violatingPoints.Skip(index).Take(consecutiveTrackPointsToCheck))
                        {
                            Coordinate trackPoint = cleanedUpTrackPoints.FirstOrDefault(x => x.TimeStamp == consecutiveViolatingPoints.timestamp);
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
                    penaltyPoints = (int)Math.Round((Math.Max(maxVerticalVelocity, Math.Abs(minVerticalVelocity)) - maxAbsVerticalVelocity) * 250, 0, MidpointRounding.AwayFromZero);
                }
            }

        }

        public static void CheckFlyingAboveSpecifedAltitude(Track track, bool useGPSAltitude, double maxAllowedAltitude, out List<Coordinate> trackPointsAbove, out TimeSpan totalDuration, out double maxAltitude, out int penaltyPoints)
        {
            totalDuration = TimeSpan.Zero;
            trackPointsAbove = new List<Coordinate>();
            maxAltitude = double.NaN;
            penaltyPoints = 0;
            if (double.IsFinite(maxAllowedAltitude))
            {
                if (useGPSAltitude)
                {
                    trackPointsAbove = track.TrackPoints.Where(x => x.AltitudeGPS > maxAllowedAltitude).ToList();
                    maxAltitude = track.TrackPoints.Max(x => x.AltitudeGPS);
                }
                else
                {
                    trackPointsAbove = track.TrackPoints.Where(x => x.AltitudeBarometric > maxAllowedAltitude).ToList();
                    maxAltitude = track.TrackPoints.Max(x => x.AltitudeBarometric);
                }
                if (trackPointsAbove.Count > 1)
                {
                    TimeSpan trackPointInterval = TimeSpan.MaxValue;
                    for (int index = 0; index < trackPointsAbove.Count - 1; index++)
                    {
                        if (trackPointsAbove[index + 1].TimeStamp.Subtract(trackPointsAbove[index].TimeStamp) < trackPointInterval)
                        {
                            trackPointInterval = trackPointsAbove[index + 1].TimeStamp.Subtract(trackPointsAbove[index].TimeStamp);
                        }
                    }
                    totalDuration = TimeSpan.FromSeconds(trackPointsAbove.Count * trackPointInterval.TotalSeconds);
                    double tempPenaltyPoints = 0.0;
                    foreach (Coordinate coordinate in trackPointsAbove)
                    {
                        if (useGPSAltitude)
                            tempPenaltyPoints += (CoordinateHelpers.ConvertToFeet( coordinate.AltitudeGPS - maxAllowedAltitude)) * trackPointInterval.TotalSeconds / 100.0;

                        else
                            tempPenaltyPoints += (CoordinateHelpers.ConvertToFeet(coordinate.AltitudeBarometric - maxAllowedAltitude)) * trackPointInterval.TotalSeconds / 100.0;

                    }
                    penaltyPoints = (int)Math.Ceiling(tempPenaltyPoints / 10.0) * 10;
                }
            }
        }

        private static void CleanTrackPoints(Track track, bool useGPSAltitude, double maxAbsVeritcalVelocityConsideredReasonable, out List<Coordinate> cleanedUpTrackPoints)
        {
            cleanedUpTrackPoints = new List<Coordinate>();
            for (int index = 0; index < track.TrackPoints.Count - 1; index++)
            {
                if (Math.Abs(track.TrackPoints[index].Latitude) <= double.Epsilon || Math.Abs(track.TrackPoints[index].Longitude) <= double.Epsilon)
                    continue;
                if (useGPSAltitude)
                {
                    if (Math.Abs(track.TrackPoints[index + 1].AltitudeGPS - track.TrackPoints[index].AltitudeGPS) > maxAbsVeritcalVelocityConsideredReasonable)
                        continue;
                }
                else
                {
                    if (Math.Abs(track.TrackPoints[index + 1].AltitudeBarometric - track.TrackPoints[index].AltitudeBarometric) > maxAbsVeritcalVelocityConsideredReasonable)
                        continue;
                }
                cleanedUpTrackPoints.Add(track.TrackPoints[index]);
            }
        }
    }
}
