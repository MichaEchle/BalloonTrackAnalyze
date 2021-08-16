using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinates
{
    public static class TrackHelpers
    {
        public static List<double> Calculate2DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(List<DeclaredGoal> declaredGoals)
        {
            List<double> distance2DBetweenPositionOfDeclarationsAndGoalDeclared = new List<double>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                distance2DBetweenPositionOfDeclarationsAndGoalDeclared.Add(CoordinateHelpers.Calculate2DDistance(declaredGoal.PositionAtDeclaration, declaredGoal.GoalDeclared));
            }
            return distance2DBetweenPositionOfDeclarationsAndGoalDeclared;
        }

        public static List<double> Calculate3DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(List<DeclaredGoal> declaredGoals)
        {
            List<double> distance3DBetweenPositionOfDeclarationsAndGoalDeclared = new List<double>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                distance3DBetweenPositionOfDeclarationsAndGoalDeclared.Add(CoordinateHelpers.Calculate3DDistance(declaredGoal.PositionAtDeclaration, declaredGoal.GoalDeclared, true));
            }
            return distance3DBetweenPositionOfDeclarationsAndGoalDeclared;
        }

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenDeclaredGoals(List<DeclaredGoal> declaredGoals)
        {
            List<(string identifier, double distance)> distance2DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declaredGoals.Count; index++)
            {
                bool useTimeStamp = declaredGoals.Where(x => x.GoalNumber == declaredGoals[index].GoalNumber).Count() > 1;
                for (int iterator = index + 1; iterator < declaredGoals.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"Goal{declaredGoals[index].GoalNumber}_{declaredGoals[index].GoalDeclared.TimeStamp:HH:mm:ss}->Goal{declaredGoals[iterator].GoalNumber}_{declaredGoals[iterator].GoalDeclared.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declaredGoals[index].GoalNumber}->Goal{declaredGoals[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declaredGoals[index].GoalDeclared, declaredGoals[iterator].GoalDeclared);
                    distance2DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenDeclaredGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenDeclaredGoals(List<DeclaredGoal> declaredGoals)
        {
            List<(string identifier, double distance)> distance3DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declaredGoals.Count; index++)
            {
                bool useTimeStamp = declaredGoals.Where(x => x.GoalNumber == declaredGoals[index].GoalNumber).Count() > 1;
                for (int iterator = index + 1; iterator < declaredGoals.Count; iterator++)
                {
                    string identifier;
                    if (useTimeStamp)
                        identifier = $"Goal{declaredGoals[index].GoalNumber}_{declaredGoals[index].GoalDeclared.TimeStamp:HH:mm:ss}->Goal{declaredGoals[iterator].GoalNumber}_{declaredGoals[iterator].GoalDeclared.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declaredGoals[index].GoalNumber}->Goal{declaredGoals[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declaredGoals[index].GoalDeclared, declaredGoals[iterator].GoalDeclared, true);
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

        public static List<(string identifier, double distance)> Calculate2DDistanceBetweenMarkerAndGoals(List<DeclaredGoal> declaredGoals, List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance2DBetweenMarkerAndGoals = new List<(string identifier, double distance)>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    string identifier = $"Goal{declaredGoal.GoalNumber}_{declaredGoal.GoalDeclared.TimeStamp:HH:mm:ss}->Marker{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declaredGoal.GoalDeclared, markerDrop.MarkerLocation);
                    distance2DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkerAndGoals;
        }

        public static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkerAndGoals(List<DeclaredGoal> declaredGoals, List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkerAndGoals = new List<(string identifier, double distance)>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    string identifier = $"Goal{declaredGoal.GoalNumber}_{declaredGoal.GoalDeclared.TimeStamp:HH:mm:ss}->Marker{markerDrop.MarkerNumber}_{markerDrop.MarkerLocation.TimeStamp:HH:mm:ss}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declaredGoal.GoalDeclared, markerDrop.MarkerLocation, true);
                    distance3DBetweenMarkerAndGoals.Add((identifier, distance));
                }
            }
            return distance3DBetweenMarkerAndGoals;
        }
    }
}
