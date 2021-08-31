using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<double> Calculate3DDistanceBetweenPositionOfDeclarationsAndDeclaredGoal(List<Declaration> declarations)
        {
            List<double> distance3DBetweenPositionOfDeclarationsAndDeclaredGoal = new List<double>();
            foreach (Declaration declaration in declarations)
            {
                distance3DBetweenPositionOfDeclarationsAndDeclaredGoal.Add(CoordinateHelpers.Calculate3DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal, true));
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
                        identifier = $"Goal{declarations[index].GoalNumber}_{declarations[index].DeclaredGoal.TimeStamp:HH:mm:ss}->Goal{declarations[iterator].GoalNumber}_{declarations[iterator].DeclaredGoal.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declarations[index].GoalNumber}->Goal{declarations[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declarations[index].DeclaredGoal, declarations[iterator].DeclaredGoal);
                    distance2DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenDeclaredGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenDeclaredGoals(List<Declaration> declarations)
        {
            List<(string identifier, double distance)> distance3DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declarations.Count; index++)
            {
                bool useTimeStamp = declarations.Where(x => x.GoalNumber == declarations[index].GoalNumber).Count() > 1;
                for (int iterator = index + 1; iterator < declarations.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"Goal{declarations[index].GoalNumber}_{declarations[index].DeclaredGoal.TimeStamp:HH:mm:ss}->Goal{declarations[iterator].GoalNumber}_{declarations[iterator].DeclaredGoal.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declarations[index].GoalNumber}->Goal{declarations[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declarations[index].DeclaredGoal, declarations[iterator].DeclaredGoal, true);
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
                        identifier = $"Marker{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->Marker{markerDrops[iterator].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Marker{markerDrops[index].MarkerNumber}->Marker{markerDrops[iterator].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation);
                    distance2DBetweenMarkers.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkers;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkers(List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkers = new List<(string identifier, double distance)>();
            for (int index = 0; index < markerDrops.Count; index++)
            {
                bool useTimeStamp = markerDrops.Where(x => x.MarkerNumber == markerDrops[index].MarkerNumber).Count() > 1;
                for (int iterator = index + 1; iterator < markerDrops.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"Marker{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->Marker{markerDrops[index].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Marker{markerDrops[index].MarkerNumber}->Marker{markerDrops[index].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation, true);
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
                    string identifier = $"Goal{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}->Marker{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation);
                    distance2DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkerAndGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkerAndGoals(List<Declaration> declarations, List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkerAndGoals = new List<(string identifier, double distance)>();
            foreach (Declaration declaration in declarations)
            {
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    string identifier = $"Goal{declaration.GoalNumber}_{declaration.DeclaredGoal.TimeStamp:HH:mm:ss}->Marker{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation, true);
                    distance3DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance3DBetweenMarkerAndGoals;
        }

        public static void EstimateLaunchAndLandingTime(Track track, bool useGPSAltitude, out Coordinate launchPoint, out Coordinate landingPoint, out bool isDangerousFlyingDetected)
        {
            if (track is null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            launchPoint = null;
            landingPoint = null;
            isDangerousFlyingDetected = true;
            List<double> altitudesFiltered;

            List<Coordinate> trackPointsClean = new List<Coordinate>();
            for (int index = 0; index < track.TrackPoints.Count - 1; index++)
            {
                if (Math.Abs(track.TrackPoints[index].Latitude) <= double.Epsilon || Math.Abs(track.TrackPoints[index].Longitude) <= double.Epsilon)
                    continue;
                if (Math.Abs(track.TrackPoints[index + 1].Latitude) <= double.Epsilon || Math.Abs(track.TrackPoints[index + 1].Longitude) <= double.Epsilon)
                    continue;
                if (useGPSAltitude)
                {
                    if (Math.Abs(track.TrackPoints[index + 1].AltitudeGPS - track.TrackPoints[index].AltitudeGPS) > 25.0)
                        continue;
                }
                else
                {
                    if (Math.Abs(track.TrackPoints[index + 1].AltitudeBarometric - track.TrackPoints[index].AltitudeBarometric) > 25.0)
                        continue;
                }
                trackPointsClean.Add(track.TrackPoints[index]);
            }

            if (useGPSAltitude)
                altitudesFiltered = trackPointsClean.Select(x => x.AltitudeGPS).ToList();
            else
                altitudesFiltered = trackPointsClean.Select(x => x.AltitudeBarometric).ToList();
            int filterLength = 5;
            int halfFilterLength = filterLength / 2;//integer division is inteded
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
            List<double> altitudeDerivative = new List<double>();
            for (int index = 0; index < trackPointsClean.Count - 1; index++)
            {
                if (useGPSAltitude)
                    altitudeDerivative.Add((trackPointsClean[index + 1].AltitudeGPS - trackPointsClean[index].AltitudeGPS) / (trackPointsClean[index + 1].TimeStamp.Subtract(trackPointsClean[index].TimeStamp).TotalSeconds));
                else
                    altitudeDerivative.Add((trackPointsClean[index + 1].AltitudeBarometric - trackPointsClean[index].AltitudeBarometric) / (trackPointsClean[index + 1].TimeStamp.Subtract(trackPointsClean[index].TimeStamp).TotalSeconds));

                altitudeFilteredDerivative.Add((index, altitudesFiltered[index + 1] - altitudesFiltered[index]));
            }
            isDangerousFlyingDetected = altitudeDerivative.Any(x => Math.Abs(x) > 10.0);

            int firstPeak = altitudeFilteredDerivative.FindIndex(x => x.altitudeDifference > 2.0);
            int counter = 0;
            int launchPointIndex = 0;
            for (int index = firstPeak; index >= 0; index--)
            {
                if (altitudeFilteredDerivative[index].altitudeDifference <= 0)
                    counter++;
                else
                    counter = 0;
                if (counter == 10)
                {
                    launchPointIndex = altitudeFilteredDerivative[index + 10].index;
                    break;
                }
            }
            //int launchPointIndex = altitudeDerivative.Take( firstPeak).Last( x => x.altitudeDifference == 0).index;
            launchPoint = trackPointsClean[launchPointIndex];
            int lastPeak = altitudeFilteredDerivative.FindLastIndex(x => x.altitudeDifference < -2.0);
            counter = 0;
            int landingPointIndex = altitudeFilteredDerivative[altitudeDerivative.Count - 1].index;
            for (int index = lastPeak; index < altitudeFilteredDerivative.Count; index++)
            {
                if (altitudeFilteredDerivative[index].altitudeDifference >= 0)
                    counter++;
                else
                    counter = 0;
                if (counter == 10)
                {
                    landingPointIndex = altitudeFilteredDerivative[index - 10].index;
                    break;
                }
            }
            //int landingPointIndex = altitudeDerivative.Skip(lastPeak).First( x => x.altitudeDifference == 0).index;
            landingPoint = trackPointsClean[landingPointIndex];
        }
    }
}
