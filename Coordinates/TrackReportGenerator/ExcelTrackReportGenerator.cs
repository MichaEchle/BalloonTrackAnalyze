using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coordinates;
using LoggerComponent;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table;

namespace TrackReportGenerator
{
    public static class ExcelTrackReportGenerator
    {
        public static bool GenerateTrackReport(string filename, Track track, bool skipCoordinatesWithOutLocation)
        {
            string functionErrorMessage = "Failed to generate track report";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int iterator = 0;
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, $"File '{fileInfo.Name}' already exists, a new file name will be generated");
            }
            bool logNewName = false;
            while (fileInfo.Exists)
            {
                logNewName = true;
                iterator++;
                fileInfo = new FileInfo(filename.Split(".").First() + $"_{iterator}." + filename.Split(".").Last());
            }
            if (logNewName)
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, $"New file name created by adding '_{iterator}' to create unique file name '{fileInfo.Name}'");
            }
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {

                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, $"Start generating track report, the process may require some seconds. Report will be saved in '{fileInfo.Name}'.");
                ExcelWorksheet wsTrackpoints = package.Workbook.Worksheets.Add("Trackpoints");
                List<(int easting, int norting)> trackChartPoints;
                List<(DateTime timestamp, double altitude)> altitudeChartPoints;
                if (!WriteTrackPoints(wsTrackpoints, track, skipCoordinatesWithOutLocation, out trackChartPoints, out altitudeChartPoints))
                {
                    Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, functionErrorMessage);
                    return false;
                }

                ExcelWorksheet wsDeclarationsAndMarkerDrops = package.Workbook.Worksheets.Add("Decl. and Markers");
                if (!WriteDeclaratrionsMarkerDrops(wsDeclarationsAndMarkerDrops, track))
                {
                    Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, functionErrorMessage);
                    return false;
                }


                ExcelWorksheet wsCharts = package.Workbook.Worksheets.Add("Charts");
                if (!CreateCharts(wsCharts, trackChartPoints, altitudeChartPoints))
                {
                    Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, functionErrorMessage);
                    return false;
                }

                package.Save();
                Logger.Log("TrackReportGenerator", LogSeverityType.Info, $"Successfully generated and saved report at '{fileInfo.FullName}'");
            }

            return true;
        }


        private static bool WriteTrackPoints(ExcelWorksheet wsTrackpoints, Track track, bool skipCoordinatesWithOutLocation, out List<(int easting, int norting)> trackChartPoints, out List<(DateTime timestamp, double altitude)> altitudeChartPoints)
        {
            trackChartPoints = new List<(int easting, int norting)>();
            altitudeChartPoints = new List<(DateTime timestamp, double altitude)>();
            try
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, "Writing track points ...");
                wsTrackpoints.Cells[1, 1].Value = "Pilot Number";
                wsTrackpoints.Cells[1, 2].Value = track.Pilot?.PilotNumber;
                wsTrackpoints.Cells[2, 1].Value = "Pilot Identifier";
                wsTrackpoints.Cells[2, 2].Value = track.Pilot?.PilotIdentifier;
                if (track.AdditionalPropertiesFromIGCFile.ContainsKey("SensBoxSerialNumber"))
                {
                    wsTrackpoints.Cells[3, 1].Value = "Sens Box Serial Number";
                    wsTrackpoints.Cells[3, 2].Value = track.AdditionalPropertiesFromIGCFile["SensBoxSerialNumber"];
                }

                wsTrackpoints.Cells[5, 1].Value = "Timestamp";
                wsTrackpoints.Cells[5, 2].Value = "Long";
                wsTrackpoints.Cells[5, 3].Value = "Lat";
                wsTrackpoints.Cells[5, 4].Value = "Long [°]";
                wsTrackpoints.Cells[5, 5].Value = "Lat [°]";
                wsTrackpoints.Cells[5, 6].Value = "UTM Zone";
                wsTrackpoints.Cells[5, 7].Value = "East";
                wsTrackpoints.Cells[5, 8].Value = "North";
                wsTrackpoints.Cells[5, 9].Value = "Alt [m]";
                wsTrackpoints.Cells[5, 10].Value = "Alt [ft]";
                wsTrackpoints.Cells["A5:J5"].Style.Font.Bold = true;


                int index = 6;
                List<Coordinate> coordinates = track.TrackPoints.OrderBy(x => x.TimeStamp).ToList();

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

                ExcelRange range = wsTrackpoints.Cells[4, 1, index - 1, 10];
                ExcelTable table = wsTrackpoints.Tables.Add(range, "Trackpoints");
                table.TableStyle = TableStyles.Light16;
                wsTrackpoints.Cells.Style.Font.Size = 10;

                wsTrackpoints.Cells.AutoFitColumns();
            }
            catch (Exception ex)
            {
                Logger.Log("TrackReportGenerator", LogSeverityType.Error, $"Failed to write track points: {ex.Message}");
                return false;
            }

            return true;

        }

        private static bool WriteDeclaratrionsMarkerDrops(ExcelWorksheet wsDeclarationsAndMarkerDrops, Track track)
        {
            try
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, "Writing declarations and marker drops ...");
                wsDeclarationsAndMarkerDrops.Cells[1, 1].Value = "Goal No";
                wsDeclarationsAndMarkerDrops.Cells[1, 2].Value = "Timestamp";
                wsDeclarationsAndMarkerDrops.Cells[1, 3].Value = "Decl. Long";
                wsDeclarationsAndMarkerDrops.Cells[1, 4].Value = "Decl. Lat";
                wsDeclarationsAndMarkerDrops.Cells[1, 5].Value = "Decl. Long [°]";
                wsDeclarationsAndMarkerDrops.Cells[1, 6].Value = "Decl. Lat [°]";
                wsDeclarationsAndMarkerDrops.Cells[1, 7].Value = "Decl. UTM Zone";
                wsDeclarationsAndMarkerDrops.Cells[1, 8].Value = "Decl. East";
                wsDeclarationsAndMarkerDrops.Cells[1, 9].Value = "Decl. North";
                wsDeclarationsAndMarkerDrops.Cells[1, 10].Value = "Decl. Alt [m]";
                wsDeclarationsAndMarkerDrops.Cells[1, 11].Value = "Decl. Alt [ft]";

                wsDeclarationsAndMarkerDrops.Cells[1, 13].Value = "Goal Long";
                wsDeclarationsAndMarkerDrops.Cells[1, 14].Value = "Goal Lat";
                wsDeclarationsAndMarkerDrops.Cells[1, 15].Value = "Goal Long [°]";
                wsDeclarationsAndMarkerDrops.Cells[1, 16].Value = "Goal Lat [°]";
                wsDeclarationsAndMarkerDrops.Cells[1, 17].Value = "Goal UTM Zone";
                wsDeclarationsAndMarkerDrops.Cells[1, 18].Value = "Goal East";
                wsDeclarationsAndMarkerDrops.Cells[1, 19].Value = "Goal North";
                wsDeclarationsAndMarkerDrops.Cells[1, 20].Value = "Goal Alt [m]";
                wsDeclarationsAndMarkerDrops.Cells[1, 21].Value = "Goal Alt [ft]";

                wsDeclarationsAndMarkerDrops.Cells[1, 22].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[1, 23].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[1,1,1,23].Style.Font.Bold = true;
                List<DeclaredGoal> declaredGoals = track.DeclaredGoals.OrderBy(x => x.GoalNumber).ToList();

                List<double> distance2D = TrackHelpers.Calculate2DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(declaredGoals);
                List<double> distance3D = TrackHelpers.Calculate3DDistanceBetweenPositionOfDeclarationsAndGoalDeclared(declaredGoals);
                int index = 2;
                foreach (DeclaredGoal declaredGoal in declaredGoals)
                {
                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = declaredGoal.GoalNumber;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = declaredGoal.PositionAtDeclaration.TimeStamp;
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = declaredGoal.PositionAtDeclaration.Longitude;
                    wsDeclarationsAndMarkerDrops.Cells[index, 4].Value = declaredGoal.PositionAtDeclaration.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.PositionAtDeclaration.Longitude);
                    string longitudeBeautified = $"{(declaredGoal.PositionAtDeclaration.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.PositionAtDeclaration.Latitude);
                    string latitudeBeautified = $"{(declaredGoal.PositionAtDeclaration.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(declaredGoal.PositionAtDeclaration.Latitude, declaredGoal.PositionAtDeclaration.Longitude);
                    wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationsAndMarkerDrops.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 9].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 10].Value = declaredGoal.PositionAtDeclaration.AltitudeGPS;
                    wsDeclarationsAndMarkerDrops.Cells[index, 11].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaredGoal.PositionAtDeclaration.AltitudeGPS), 0, MidpointRounding.AwayFromZero);

                    wsDeclarationsAndMarkerDrops.Cells[index, 13].Value = declaredGoal.GoalDeclared.Longitude;
                    wsDeclarationsAndMarkerDrops.Cells[index, 14].Value = declaredGoal.GoalDeclared.Latitude;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.GoalDeclared.Longitude);
                    longitudeBeautified = $"{(declaredGoal.GoalDeclared.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 15].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaredGoal.GoalDeclared.Latitude);
                    latitudeBeautified = $"{(declaredGoal.GoalDeclared.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 16].Value = latitudeBeautified;
                    coordinateSharp = new CoordinateSharp.Coordinate(declaredGoal.GoalDeclared.Latitude, declaredGoal.GoalDeclared.Longitude);
                    wsDeclarationsAndMarkerDrops.Cells[index, 17].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationsAndMarkerDrops.Cells[index, 18].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 19].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 20].Value = declaredGoal.GoalDeclared.AltitudeGPS;
                    wsDeclarationsAndMarkerDrops.Cells[index, 21].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaredGoal.GoalDeclared.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 22].Value = Math.Round(distance2D[index - 2], 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 23].Value = Math.Round(distance3D[index - 2], 0, MidpointRounding.AwayFromZero);
                    index++;
                }

                ExcelRange declRange = wsDeclarationsAndMarkerDrops.Cells[1, 1, index - 1, 23];
                ExcelTable declTable = wsDeclarationsAndMarkerDrops.Tables.Add(declRange, "Declarations");
                declTable.TableStyle = TableStyles.Light16;
                index += 3;
                int markerStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Marker No";
                wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = "Timestamp";
                wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = "Mark. Long";
                wsDeclarationsAndMarkerDrops.Cells[index, 4].Value = "Mark. Lat";
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = "Mark. Long [°]";
                wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = "Mark. Lat [°]";
                wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = "Mark. UTM Zone";
                wsDeclarationsAndMarkerDrops.Cells[index, 8].Value = "Mark. East";
                wsDeclarationsAndMarkerDrops.Cells[index, 9].Value = "Mark. North";
                wsDeclarationsAndMarkerDrops.Cells[index, 10].Value = "Mark. Alt [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 11].Value = "Mark. Alt [ft]";
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 11].Style.Font.Bold = true;
                index++;
                List<MarkerDrop> markerDrops = track.MarkerDrops.OrderBy(x => x.MarkerNumber).ToList();
                foreach (MarkerDrop markerDrop in markerDrops)
                {
                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = markerDrop.MarkerNumber;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = markerDrop.MarkerLocation.TimeStamp;
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = markerDrop.MarkerLocation.Longitude;
                    wsDeclarationsAndMarkerDrops.Cells[index, 4].Value = markerDrop.MarkerLocation.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(markerDrop.MarkerLocation.Longitude);
                    string longitudeBeautified = $"{(markerDrop.MarkerLocation.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(markerDrop.MarkerLocation.Latitude);
                    string latitudeBeautified = $"{(markerDrop.MarkerLocation.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(markerDrop.MarkerLocation.Latitude, markerDrop.MarkerLocation.Longitude);
                    wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationsAndMarkerDrops.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 9].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 10].Value = markerDrop.MarkerLocation.AltitudeGPS;
                    wsDeclarationsAndMarkerDrops.Cells[index, 11].Value = Math.Round(CoordinateHelpers.ConvertToFeet(markerDrop.MarkerLocation.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange markRange = wsDeclarationsAndMarkerDrops.Cells[markerStartRow, 1, index - 1, 11];
                ExcelTable markTable = wsDeclarationsAndMarkerDrops.Tables.Add(markRange, "Marker_Drops");
                markTable.TableStyle = TableStyles.Light16;

                index += 3;
                List<(string identifier, double distance)> distance2DGoals = TrackHelpers.Calculate2DDistanceBetweenDeclaredGoals(declaredGoals);
                List<(string identifier, double distance)> distance3DGoals = TrackHelpers.Calculate3DDistanceBetweenDeclaredGoals(declaredGoals);
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Distance Goals";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distGoalsStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Identifier";
                wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DGoals.Count; goalsIndex++)
                {

                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = distance2DGoals[goalsIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = Math.Round(distance2DGoals[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = Math.Round(distance3DGoals[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange goalToGoalRange = wsDeclarationsAndMarkerDrops.Cells[distGoalsStartRow, 1, index - 1, 3];
                ExcelTable goalToGoalTable = wsDeclarationsAndMarkerDrops.Tables.Add(goalToGoalRange, "Distance_Goals");
                goalToGoalTable.TableStyle = TableStyles.Light16;

                index += 3;
                List<(string identifier, double distance)> distance2DGoalsToMarkers = TrackHelpers.Calculate2DDistanceBetweenMarkerAndGoals(declaredGoals, markerDrops);
                List<(string identifier, double distance)> distance3DGoalsToMarkers = TrackHelpers.Calculate3DDistanceBetweenMarkerAndGoals(declaredGoals, markerDrops);

                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Distance Goals to Markers";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distGoalToMarkStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Identifier";
                wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DGoalsToMarkers.Count; goalsIndex++)
                {

                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = distance2DGoalsToMarkers[goalsIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = Math.Round(distance2DGoalsToMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = Math.Round(distance3DGoalsToMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange distGoalToMarkRange = wsDeclarationsAndMarkerDrops.Cells[distGoalToMarkStartRow, 1, index - 1, 3];
                ExcelTable distGoalToMarkTable = wsDeclarationsAndMarkerDrops.Tables.Add(distGoalToMarkRange, "Distance_Goals_to_Markers");
                distGoalToMarkTable.TableStyle = TableStyles.Light16;


                index += 3;
                List<(string identifier, double distance)> distance2DMarkers = TrackHelpers.Calculate2DDistanceBetweenMarkers(markerDrops);
                List<(string identifier, double distance)> distance3DMarkers = TrackHelpers.Calculate3DDistanceBetweenMarkers(markerDrops);
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Distance Markers";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distMarkStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Identifier";
                wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int goalsIndex = 0; goalsIndex < distance2DMarkers.Count; goalsIndex++)
                {

                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = distance2DMarkers[goalsIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = Math.Round(distance2DMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = Math.Round(distance3DMarkers[goalsIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange distMarkRange = wsDeclarationsAndMarkerDrops.Cells[distMarkStartRow, 1, index - 1, 3];
                ExcelTable distMarkTable = wsDeclarationsAndMarkerDrops.Tables.Add(distMarkRange, "Distance_Markers");
                distMarkTable.TableStyle = TableStyles.Light16;

                wsDeclarationsAndMarkerDrops.Cells.Style.Font.Size = 10;
                wsDeclarationsAndMarkerDrops.Cells.AutoFitColumns();
            }
            catch (Exception ex)
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, $"Failed to write declarations and marker drops: {ex.Message}");
                return false;
            }
            return true;
        }

        private static bool CreateCharts(ExcelWorksheet wsCharts, List<(int easting, int norting)> trackChartPoints,
        List<(DateTime timestamp, double altitude)> altitudeChartPoints)
        {
            try
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Info, "Create track charts ...");
                wsCharts.Cells[1, 1].Value = "Northing";
                wsCharts.Cells[1, 2].Value = "Easting";
                wsCharts.Cells[1, 3].Value = "Time";
                wsCharts.Cells[1, 4].Value = "Alt [m]";
                int index = 2;
                if (trackChartPoints.Count == altitudeChartPoints.Count)
                {
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
                    trackChart.Title.Text = "2D Track (every 10th trackpoint only)";
                    trackChart.Series.Add(wsCharts.Cells[2, 2, index - 1, 2], wsCharts.Cells[2, 1, index - 1, 1]);
                    trackChart.SetPosition(1, 0, 6, 0);
                    trackChart.SetSize(500, 500);
                    trackChart.Legend.Remove();

                    ExcelLineChart altChart = wsCharts.Drawings.AddLineChart("Alt", eLineChartType.Line);
                    altChart.Title.Text = "Altitude (every 10th trackpoint only)";
                    altChart.Series.Add(wsCharts.Cells[2, 4, index - 1, 4], wsCharts.Cells[2, 3, index - 1, 3]);
                    altChart.SetPosition(27, 0, 6, 0);
                    altChart.SetSize(500, 500);
                    altChart.Legend.Remove();
                }
                else
                {
                    Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, $"Failed to generate charts: track and altitude chart are expected to have the same number of points");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ExcelTrackReportGenerator", LogSeverityType.Error, $"Failed to generate charts: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}
