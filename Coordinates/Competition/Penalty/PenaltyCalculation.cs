using Coordinates;
using LoggerComponent;
using System;

namespace Competition.Penalties
{
    public static class PenaltyCalculation
    {
        /// <summary>
        /// Checks if pilot flight in blue PZ and calculate the penalty
        /// <para>For each track point in the PZ, the penalty will be: infringement [ft] * logger interval / 100  </para>
        /// <para>The penalties of all track points are summed up and round to the tens digit</para>
        /// </summary>
        /// <param name="lowerAltitudeOfBluePZ">the lower altitude of the blue PZ in [m]</param>
        /// <param name="tracks">the list of tracks to check</param>
        /// <param name="useGPSAltitude">true:use GPS altitude; false: use barometric altitude</param>
        /// <param name="penalties">output parameter. a list containing the pilot number, number of violating track points and the penalty</param>
        public static void CheckForBluePZAndCalculatePenaltyPoints(double lowerAltitudeOfBluePZ, bool useGPSAltitude, List<Track> tracks, out List<(int pilotNumber, int numberOfViolatingTrackPoints, int penalty)> penalties)
        {
            penalties = new List<(int pilotNumber, int numberOfViolatingTrackPoints, int penalty)>();
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
                        Logger.Log(LogSeverityType.Warning, $"Cannot parse LoggerInterval for track of pilot no. {track.Pilot.PilotNumber}, the default value of {trackPointInterval}s is used");
                    }
                }
                else
                {
                    Logger.Log(LogSeverityType.Warning, $"The track of pilot no. {track.Pilot.PilotNumber} has no logger interval stored, the default value of {trackPointInterval}s is used");
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
                penalty = Math.Round(penalty / 10.0, 0, MidpointRounding.AwayFromZero) * 10.0;//Round to tens digit
                penalties.Add((track.Pilot.PilotNumber, numberOfViolatingTrackPoints, (int)penalty));
            }
        }


        /// <summary>
        /// Checks if minimum or maximum distances has been infringed
        /// <para>calculates the infringement in [%], the penalty is 20 points per percent of infringement</para>
        /// <para>if the infringement is greater than 25%, the pilot is the scored with 'no result', however is this <b>not</b> reflected in the returned penalty</para>
        /// </summary>
        /// <param name="referenceCoordinate">the coordinate to which distances are calculated</param>
        /// <param name="minimumDistance">the minimum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
        /// <param name="maximumDistance">the maximum allowed distance in [m]. Use <see cref="double.NaN"/> to omit</param>
        /// <param name="coordinateToCheck">the list to be checked, each entry consist of a pilot Number and coordinate pair</param>
        /// <param name="penalties">output parameter. a list containing pilot number, distance, infringement and penalty points</param>
        /// <returns>true:success; false:error</returns>
        public static bool CheckFor2DDistanceInfringementAndCalculatePenaltyPoints(Coordinate referenceCoordinate, double minimumDistance, double maximumDistance, List<(int pilotNumber, Coordinate coordinate)> coordinateToCheck, out List<(int pilotNumber, double distance, double infrigement, int penalty)> penalties)
        {
            penalties = new List<(int pilotNumber, double distance, double infrigement, int penalty)>();
            if (referenceCoordinate is null)
            {
                Logger.Log(LogSeverityType.Error, "Reference coordinate cannot be null");
                return false;
            }
            if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
            {
                if (minimumDistance > maximumDistance)
                {
                    Logger.Log(LogSeverityType.Error, "Minimum distance cannot be larger than maximum distance");
                    return false;
                }
            }
            double infringement;
            double penalty;
            double distance;
            foreach ((int pilotNumber, Coordinate coordinate) coordinate in coordinateToCheck)
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

        public static void CheckFor3DDistanceInfringementAndCalculatePenaltyPoints()
        {
            //TODO check if minimum or maximum limits have been violated using 3D distance
            //if so calculate infringement in %
            //minimum: (1 - distance/limit) * 100
            //maximum: (distance/limit - 1) * 100
            //penalty: round infringement[%] to one digit * 20
            throw new NotImplementedException();
        }

        public static void CheckForVerticalDistanceInfringementAndCalculatePenaltyPoints()
        {
            //TODO check if minimum or maximum limits have been violated using altitude only
            //if so calculate infringement in %
            //minimum: (1 - distance/limit) * 100
            //maximum: (distance/limit - 1) * 100
            //penalty: round infringement[%] to one digit * 20
            throw new NotImplementedException();
        }

        public static void CheckForDangerousFlyingAndCalculatePenaltyPoints()
        {
            //TODO check if vertical velocity is +/- 8m/s  for 5 consecutive seconds
            //penalty: max abs vertical velocity - max abs allowed vertical velocity *250 (use integer)
            throw new NotImplementedException();
        }

        public static void CheckForCloseProximityAndCalculatePenaltyPoints()
        {
            //TODO use only trackpoints x seconds after launch and y seconds before landing
            //for each trackpoint: find trackpoint in other track where timestamp is nearest
            //check 3D distance between the two trackpoints
            //Limit 1: more than 3m / s at less than 25m
            //Limit 2: more than 5m / s at less than 50m
            //Limit 3: more than 8m / s at less than 75m
            //Limit 4: more than 8m / s vertical ascend speed
            throw new NotImplementedException();
        }
    }
}
