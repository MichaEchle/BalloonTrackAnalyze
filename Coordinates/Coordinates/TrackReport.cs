using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Coordinates
{
    public static class TrackReport
    {

        private static List<double> Calculate2DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(List<DeclaredGoal> declaredGoals)
        {
            List<double> distance2DBetweenPositionOfDeclarationsAndGoalDeclared = new List<double>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                distance2DBetweenPositionOfDeclarationsAndGoalDeclared.Add(CoordinateHelpers.Calculate2DDistance(declaredGoal.PositionAtDeclaration, declaredGoal.GoalDeclared));
            }
            return distance2DBetweenPositionOfDeclarationsAndGoalDeclared;
        }

        private static List<double> Calculate3DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(List<DeclaredGoal> declaredGoals)
        {
            List<double> distance3DBetweenPositionOfDeclarationsAndGoalDeclared = new List<double>();
            foreach (DeclaredGoal declaredGoal in declaredGoals)
            {
                distance3DBetweenPositionOfDeclarationsAndGoalDeclared.Add(CoordinateHelpers.Calculate3DDistance(declaredGoal.PositionAtDeclaration, declaredGoal.GoalDeclared, true));
            }
            return distance3DBetweenPositionOfDeclarationsAndGoalDeclared;
        }

        private static List<(string identifier, double distance)> Calculate2DDistanceBetweenDeclaredGoals(List<DeclaredGoal> declaredGoals)
        {
            List<(string identifier, double distance)> distance2DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declaredGoals.Count; index++)
            {
                for (int iterator = index + 1; iterator < declaredGoals.Count; iterator++)
                {
                    string identifier;
                    if (declaredGoals[index].GoalNumber == declaredGoals[iterator].GoalNumber)
                        identifier = $"Goal{declaredGoals[index].GoalNumber}_{declaredGoals[index].GoalDeclared.TimeStamp:HH:mm:ss}->Goal{declaredGoals[iterator].GoalNumber}_{declaredGoals[iterator].GoalDeclared.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declaredGoals[index].GoalNumber}->Goal{declaredGoals[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(declaredGoals[index].GoalDeclared, declaredGoals[iterator].GoalDeclared);
                    distance2DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance2DBetweenDeclaredGoals;
        }

        private static List<(string identifier, double distance)> Calculate3DDistanceBetweenDeclaredGoals(List<DeclaredGoal> declaredGoals)
        {
            List<(string identifier, double distance)> distance3DBetweenDeclaredGoals = new List<(string identifier, double distance)>();
            for (int index = 0; index < declaredGoals.Count; index++)
            {
                for (int iterator = index + 1; iterator < declaredGoals.Count; iterator++)
                {
                    string identifier;
                    if (declaredGoals[index].GoalNumber == declaredGoals[iterator].GoalNumber)
                        identifier = $"Goal{declaredGoals[index].GoalNumber}_{declaredGoals[index].GoalDeclared.TimeStamp:HH:mm:ss}->Goal{declaredGoals[iterator].GoalNumber}_{declaredGoals[iterator].GoalDeclared.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Goal{declaredGoals[index].GoalNumber}->Goal{declaredGoals[iterator].GoalNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(declaredGoals[index].GoalDeclared, declaredGoals[iterator].GoalDeclared, true);
                    distance3DBetweenDeclaredGoals.Add((identifier, distance));
                }
            }
            return distance3DBetweenDeclaredGoals;
        }

        private static List<(string identifier, double distance)> Calculate2DDistanceBetweenMarkers(List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance2DBetweenMarkers = new List<(string identifier, double distance)>();
            for (int index = 0; index < markerDrops.Count; index++)
            {
                for (int iterator = index + 1; iterator < markerDrops.Count; iterator++)
                {
                    string identifier;
                    if (markerDrops[index].MarkerNumber == markerDrops[iterator].MarkerNumber)
                        identifier = $"Marker{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->Marker{markerDrops[iterator].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Marker{markerDrops[index].MarkerNumber}->Marker{markerDrops[iterator].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate2DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation);
                    distance2DBetweenMarkers.Add((identifier, distance));
                }
            }
            return distance2DBetweenMarkers;
        }

        private static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkers(List<MarkerDrop> markerDrops)
        {
            List<(string identifier, double distance)> distance3DBetweenMarkers = new List<(string identifier, double distance)>();
            for (int index = 0; index < markerDrops.Count; index++)
            {
                for (int iterator = index + 1; iterator < markerDrops.Count; iterator++)
                {
                    string identifier;
                    if (markerDrops[index].MarkerNumber == markerDrops[iterator].MarkerNumber)
                        identifier = $"Marker{markerDrops[index].MarkerNumber}_{markerDrops[index].MarkerLocation.TimeStamp:HH:mm:ss}->Marker{markerDrops[index].MarkerNumber}_{markerDrops[iterator].MarkerLocation.TimeStamp:HH:mm:ss}";
                    else
                        identifier = $"Marker{markerDrops[index].MarkerNumber}->Marker{markerDrops[index].MarkerNumber}";
                    double distance = CoordinateHelpers.Calculate3DDistance(markerDrops[index].MarkerLocation, markerDrops[iterator].MarkerLocation, true);
                    distance3DBetweenMarkers.Add((identifier, distance));
                }
            }
            return distance3DBetweenMarkers;
        }

        private static List<(string identifier, double distance)> Calculate2DDistanceBetweenMarkerAndGoals(List<DeclaredGoal> declaredGoals, List<MarkerDrop> markerDrops)
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

        private static List<(string identifier, double distance)> Calculate3DDistanceBetweenMarkerAndGoals(List<DeclaredGoal> declaredGoals, List<MarkerDrop> markerDrops)
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

        public static bool GenerateTrackReport(string filename, Track track, bool skipCoordinatesWithOutLocation)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int iterator = 1;
            FileInfo fileInfo = new FileInfo(filename);
            while (fileInfo.Exists)
            {
                fileInfo = new FileInfo(filename.Split(".").First() + $"_{iterator}." + filename.Split(".").Last());
                iterator++;
            }
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {


                ExcelWorksheet wsTrackpoints = package.Workbook.Worksheets.Add("Trackpoints");
                wsTrackpoints.Cells[1, 1].Value = "Pilot Number";
                wsTrackpoints.Cells[1, 2].Value = track.Pilot?.PilotNumber;
                wsTrackpoints.Cells[2, 1].Value = "Pilot Identifier";
                wsTrackpoints.Cells[2, 2].Value = track.Pilot?.PilotIdentifier;

                wsTrackpoints.Cells[4, 1].Value = "Timestamp";
                wsTrackpoints.Cells[4, 2].Value = "Long";
                wsTrackpoints.Cells[4, 3].Value = "Lat";
                wsTrackpoints.Cells[4, 4].Value = "Long [°]";
                wsTrackpoints.Cells[4, 5].Value = "Lat [°]";
                wsTrackpoints.Cells[4, 6].Value = "UTM Zone";
                wsTrackpoints.Cells[4, 7].Value = "East";
                wsTrackpoints.Cells[4, 8].Value = "North";
                wsTrackpoints.Cells[4, 9].Value = "Alt [m]";
                wsTrackpoints.Cells[4, 10].Value = "Alt [ft]";
                wsTrackpoints.Cells["A4:J4"].Style.Font.Bold = true;


                int index = 5;
                List<Coordinate> coordinates = track.TrackPoints.OrderBy(x => x.TimeStamp).ToList();
                List<(int easting, int norting)> trackChartPoints = new List<(int easting, int norting)>();
                List<(DateTime timestamp, double altitude)> altitudeChartPoints = new List<(DateTime timestamp, double altitude)>();
                foreach (Coordinate coordinate in coordinates)
                {
                    if ((skipCoordinatesWithOutLocation) && (Math.Abs(coordinate.Longitude) < double.Epsilon))
                        continue;
                    wsTrackpoints.Cells[index, 1].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsTrackpoints.Cells[index, 1].Value = coordinate.TimeStamp;
                    wsTrackpoints.Cells[index, 2].Value = coordinate.Longitude;
                    wsTrackpoints.Cells[index, 3].Value = coordinate.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Longitude);
                    string longitudeBeautified = $"{(coordinate.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsTrackpoints.Cells[index, 4].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Latitude);
                    string latitudeBeautified = $"{(coordinate.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsTrackpoints.Cells[index, 5].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(coordinate.Latitude, coordinate.Longitude);
                    wsTrackpoints.Cells[index, 6].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsTrackpoints.Cells[index, 7].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsTrackpoints.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsTrackpoints.Cells[index, 9].Value = coordinate.AltitudeGPS;
                    wsTrackpoints.Cells[index, 10].Value = Math.Round(CoordinateHelpers.ConvertToFeet(coordinate.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    if ((index % 10 == 0) && (Math.Abs(coordinate.Longitude) > double.Epsilon))
                    {
                        trackChartPoints.Add(((int)Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero), (int)Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero)));
                        altitudeChartPoints.Add((coordinate.TimeStamp, coordinate.AltitudeGPS));
                    }
                    index++;
                }
                wsTrackpoints.Cells.Style.Font.Size = 10;

                wsTrackpoints.Cells.AutoFitColumns();

                ExcelWorksheet wsDeclarationAndMarker = package.Workbook.Worksheets.Add("Decl. and Markers");
                wsDeclarationAndMarker.Cells[1, 1].Value = "Goal No";
                wsDeclarationAndMarker.Cells[1, 2].Value = "Timestamp";
                wsDeclarationAndMarker.Cells[1, 3].Value = "Decl. Long";
                wsDeclarationAndMarker.Cells[1, 4].Value = "Decl. Lat";
                wsDeclarationAndMarker.Cells[1, 5].Value = "Decl. Long [°]";
                wsDeclarationAndMarker.Cells[1, 6].Value = "Decl. Lat [°]";
                wsDeclarationAndMarker.Cells[1, 7].Value = "Decl. UTM Zone";
                wsDeclarationAndMarker.Cells[1, 8].Value = "Decl. East";
                wsDeclarationAndMarker.Cells[1, 9].Value = "Decl. North";
                wsDeclarationAndMarker.Cells[1, 10].Value = "Decl. Alt [m]";
                wsDeclarationAndMarker.Cells[1, 11].Value = "Decl. Alt [ft]";

                wsDeclarationAndMarker.Cells[1, 13].Value = "Goal Long";
                wsDeclarationAndMarker.Cells[1, 14].Value = "Goal Lat";
                wsDeclarationAndMarker.Cells[1, 15].Value = "Goal Long [°]";
                wsDeclarationAndMarker.Cells[1, 16].Value = "Goal Lat [°]";
                wsDeclarationAndMarker.Cells[1, 17].Value = "Goal UTM Zone";
                wsDeclarationAndMarker.Cells[1, 18].Value = "Goal East";
                wsDeclarationAndMarker.Cells[1, 19].Value = "Goal North";
                wsDeclarationAndMarker.Cells[1, 20].Value = "Goal Alt [m]";
                wsDeclarationAndMarker.Cells[1, 21].Value = "Goal Alt [ft]";

                wsDeclarationAndMarker.Cells[1, 22].Value = "Dist 2D [m]";
                wsDeclarationAndMarker.Cells[1, 23].Value = "Dist 3D [m]";
                wsDeclarationAndMarker.Cells["A1:W1"].Style.Font.Bold = true;
                List<DeclaredGoal> declaredGoals = track.DeclaredGoals.OrderBy(x => x.GoalNumber).ToList();

                List<double> distance2D = Calculate2DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(declaredGoals);
                List<double> distance3D = Calculate3DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(declaredGoals);
                index = 2;
                foreach (DeclaredGoal declaredGoal in declaredGoals)
                {
                    wsDeclarationAndMarker.Cells[index, 1].Value = declaredGoal.GoalNumber;
                    wsDeclarationAndMarker.Cells[index, 2].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsDeclarationAndMarker.Cells[index, 2].Value = declaredGoal.PositionAtDeclaration.TimeStamp;
                    wsDeclarationAndMarker.Cells[index, 3].Value = declaredGoal.PositionAtDeclaration.Longitude;
                    wsDeclarationAndMarker.Cells[index, 4].Value = declaredGoal.PositionAtDeclaration.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.PositionAtDeclaration.Longitude);
                    string longitudeBeautified = $"{(declaredGoal.PositionAtDeclaration.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 5].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.PositionAtDeclaration.Latitude);
                    string latitudeBeautified = $"{(declaredGoal.PositionAtDeclaration.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 6].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(declaredGoal.PositionAtDeclaration.Latitude, declaredGoal.PositionAtDeclaration.Longitude);
                    wsDeclarationAndMarker.Cells[index, 7].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationAndMarker.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 9].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 10].Value = declaredGoal.PositionAtDeclaration.AltitudeGPS;
                    wsDeclarationAndMarker.Cells[index, 11].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaredGoal.PositionAtDeclaration.AltitudeGPS), 0, MidpointRounding.AwayFromZero);

                    wsDeclarationAndMarker.Cells[index, 13].Value = declaredGoal.GoalDeclared.Longitude;
                    wsDeclarationAndMarker.Cells[index, 14].Value = declaredGoal.GoalDeclared.Latitude;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.GoalDeclared.Longitude);
                    longitudeBeautified = $"{(declaredGoal.GoalDeclared.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 15].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.GoalDeclared.Latitude);
                    latitudeBeautified = $"{(declaredGoal.GoalDeclared.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 16].Value = latitudeBeautified;
                    coordinateSharp = new CoordinateSharp.Coordinate(declaredGoal.GoalDeclared.Latitude, declaredGoal.GoalDeclared.Longitude);
                    wsDeclarationAndMarker.Cells[index, 17].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationAndMarker.Cells[index, 18].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 19].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 20].Value = declaredGoal.GoalDeclared.AltitudeGPS;
                    wsDeclarationAndMarker.Cells[index, 21].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaredGoal.GoalDeclared.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 22].Value = Math.Round(distance2D[index - 2], 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 23].Value = Math.Round(distance3D[index - 2], 0, MidpointRounding.AwayFromZero);
                    index++;
                }

                index += 3;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Marker No";
                wsDeclarationAndMarker.Cells[index, 2].Value = "Timestamp";
                wsDeclarationAndMarker.Cells[index, 3].Value = "Mark. Long";
                wsDeclarationAndMarker.Cells[index, 4].Value = "Mark. Lat";
                wsDeclarationAndMarker.Cells[index, 5].Value = "Mark. Long [°]";
                wsDeclarationAndMarker.Cells[index, 6].Value = "Mark. Lat [°]";
                wsDeclarationAndMarker.Cells[index, 7].Value = "Mark. UTM Zone";
                wsDeclarationAndMarker.Cells[index, 8].Value = "Mark. East";
                wsDeclarationAndMarker.Cells[index, 9].Value = "Mark. North";
                wsDeclarationAndMarker.Cells[index, 10].Value = "Mark. Alt [m]";
                wsDeclarationAndMarker.Cells[index, 11].Value = "Mark. Alt [ft]";
                wsDeclarationAndMarker.Cells[index, 1, index, 11].Style.Font.Bold = true;
                index++;
                List<MarkerDrop> markerDrops = track.MarkerDrops.OrderBy(x => x.MarkerNumber).ToList();
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    wsDeclarationAndMarker.Cells[index, 1].Value = markerDrop.MarkerNumber;
                    wsDeclarationAndMarker.Cells[index, 2].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsDeclarationAndMarker.Cells[index, 2].Value = markerDrop.MarkerLocation.TimeStamp;
                    wsDeclarationAndMarker.Cells[index, 3].Value = markerDrop.MarkerLocation.Longitude;
                    wsDeclarationAndMarker.Cells[index, 4].Value = markerDrop.MarkerLocation.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(markerDrop.MarkerLocation.Longitude);
                    string longitudeBeautified = $"{(markerDrop.MarkerLocation.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 5].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(markerDrop.MarkerLocation.Latitude);
                    string latitudeBeautified = $"{(markerDrop.MarkerLocation.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationAndMarker.Cells[index, 6].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(markerDrop.MarkerLocation.Latitude, markerDrop.MarkerLocation.Longitude);
                    wsDeclarationAndMarker.Cells[index, 7].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationAndMarker.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 9].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 10].Value = markerDrop.MarkerLocation.AltitudeGPS;
                    wsDeclarationAndMarker.Cells[index, 11].Value = Math.Round(CoordinateHelpers.ConvertToFeet(markerDrop.MarkerLocation.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    index++;
                }

                index += 3;
                List<(string identifier, double distance)> distance2DGoals = Calculate2DDistanceBetweenDeclaredGoals(declaredGoals);
                List<(string identifier, double distance)> distance3DGoals = Calculate3DDistanceBetweenDeclaredGoals(declaredGoals);
                wsDeclarationAndMarker.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Distance Goals";
                wsDeclarationAndMarker.Cells[index, 1].Style.Font.Bold = true;
                index++;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Identifier";
                wsDeclarationAndMarker.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationAndMarker.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationAndMarker.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DGoals.Count; goalsIndex++)
                {

                    wsDeclarationAndMarker.Cells[index, 1].Value = distance2DGoals[goalsIndex].identifier;
                    wsDeclarationAndMarker.Cells[index, 2].Value =Math.Round( distance2DGoals[goalsIndex].distance,0,MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 3].Value = Math.Round(distance3DGoals[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }

                index += 3;
                List<(string identifier, double distance)> distance2DGoalsToMarkers = Calculate2DDistanceBetweenMarkerAndGoals(declaredGoals,markerDrops);
                List<(string identifier, double distance)> distance3DGoalsToMarkers = Calculate3DDistanceBetweenMarkerAndGoals(declaredGoals, markerDrops);

                wsDeclarationAndMarker.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Distance Goals to Markers";
                wsDeclarationAndMarker.Cells[index, 1].Style.Font.Bold = true;
                index++;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Identifier";
                wsDeclarationAndMarker.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationAndMarker.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationAndMarker.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DGoalsToMarkers.Count; goalsIndex++)
                {

                    wsDeclarationAndMarker.Cells[index, 1].Value = distance2DGoalsToMarkers[goalsIndex].identifier;
                    wsDeclarationAndMarker.Cells[index, 2].Value = Math.Round(distance2DGoalsToMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 3].Value = Math.Round(distance3DGoalsToMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }

                index += 3;
                List<(string identifier, double distance)> distance2DMarkers = Calculate2DDistanceBetweenMarkers(markerDrops);
                List<(string identifier, double distance)> distance3DMarkers = Calculate3DDistanceBetweenMarkers( markerDrops);
                wsDeclarationAndMarker.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Distance Markers";
                wsDeclarationAndMarker.Cells[index, 1].Style.Font.Bold = true;
                index++;
                wsDeclarationAndMarker.Cells[index, 1].Value = "Identifier";
                wsDeclarationAndMarker.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationAndMarker.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationAndMarker.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DMarkers.Count; goalsIndex++)
                {

                    wsDeclarationAndMarker.Cells[index, 1].Value = distance2DMarkers[goalsIndex].identifier;
                    wsDeclarationAndMarker.Cells[index, 2].Value = Math.Round(distance2DMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationAndMarker.Cells[index, 3].Value = Math.Round(distance3DMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                wsDeclarationAndMarker.Cells.Style.Font.Size = 10;
                wsDeclarationAndMarker.Cells.AutoFitColumns();


                ExcelWorksheet wsCharts = package.Workbook.Worksheets.Add("Charts");
                wsCharts.Cells[1, 1].Value = "Northing";
                wsCharts.Cells[1, 2].Value = "Easting";
                wsCharts.Cells[1, 3].Value = "Time";
                wsCharts.Cells[1, 4].Value = "Alt [m]";
                index = 2;
                for (int chartIndex = 0; chartIndex < trackChartPoints.Count; chartIndex++)
                {
                    wsCharts.Cells[index, 1].Value = trackChartPoints[chartIndex].norting;
                    wsCharts.Cells[index, 2].Value = trackChartPoints[chartIndex].easting;
                    wsCharts.Cells[index, 3].Style.Numberformat.Format = "HH:mm:ss";
                    wsCharts.Cells[index, 3].Value = altitudeChartPoints[chartIndex].timestamp;
                    wsCharts.Cells[index, 4].Value = altitudeChartPoints[chartIndex].altitude;
                    index++;
                }
                ExcelLineChart trackChart = wsCharts.Drawings.AddLineChart("Track", eLineChartType.Line);
                trackChart.Title.Text= "2D Track(every 10th trackpoint only)";
                trackChart.Series.Add(wsCharts.Cells[2,2,index-1,2],wsCharts.Cells[2,1,index-1,1]);
                trackChart.SetPosition(1, 0, 6, 0);
                trackChart.SetSize(500, 500);
                trackChart.Legend.Remove();

                ExcelLineChart altChart = wsCharts.Drawings.AddLineChart("Alt", eLineChartType.Line);
                altChart.Title.Text = "Altitude (every 10th trackpoint only)";
                altChart.Series.Add(wsCharts.Cells[2, 4, index-1, 4],wsCharts.Cells[2,3,index-1,3]);
                altChart.SetPosition(27, 0, 6, 0);
                altChart.SetSize(500, 500);
                altChart.Legend.Remove();

                package.Save();
            }

            return true;
        }

    }
}
