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
    }
}
