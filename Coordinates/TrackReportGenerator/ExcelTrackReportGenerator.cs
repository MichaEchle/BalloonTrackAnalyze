﻿using Competition.Penalties;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TrackReportGenerator
{
    public static class ExcelTrackReportGenerator
    {
        private static readonly ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(ExcelTrackReportGenerator));

        public static bool GenerateTrackReport(string filename, Track track, bool skipCoordinatesWithOutLocation, bool useGPSAltitude, double maxAllowedAltitude)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            int iterator = 0;
            FileInfo fileInfo = new(filename);
            if (fileInfo.Exists)
            {
                Logger?.LogInformation("File '{filename}' already exists, a new file name will be generated", fileInfo.Name);
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
                Logger?.LogInformation("New file name created by adding '_{iterator}' to create unique file name '{fileInfo.Name}'", iterator, fileInfo.Name);
            }
            using ExcelPackage package = new(fileInfo);

            Logger?.LogInformation("Start generating track report, the process may require some seconds. Report will be saved in '{fileInfo.Name}'.", fileInfo.Name);
            ExcelWorksheet wsTrackpoints = package.Workbook.Worksheets.Add("Trackpoints");
            if (!WriteTrackPoints(wsTrackpoints, track, skipCoordinatesWithOutLocation, useGPSAltitude, out List<(int easting, int norting)> trackChartPoints, out List<(DateTime timestamp, double altitude)> altitudeChartPoints))
            {
                Logger?.LogError("Failed to generate track report");
                return false;
            }

            ExcelWorksheet wsDeclarationsAndMarkerDrops = package.Workbook.Worksheets.Add("Decl. and Markers");
            if (!WriteDeclaratrionsAndMarkerDrops(wsDeclarationsAndMarkerDrops, track, useGPSAltitude))
            {
                Logger?.LogError("Failed to generate track report");
                return false;
            }


            ExcelWorksheet wsCharts = package.Workbook.Worksheets.Add("Charts");
            if (!CreateCharts(wsCharts, trackChartPoints, altitudeChartPoints))
            {
                Logger?.LogError("Failed to generate track report");
                return false;
            }

            ExcelWorksheet wsIncidents = package.Workbook.Worksheets.Add("Incidents");
            if (!WriteIncidentsInOrder(wsIncidents, track))
            {
                Logger?.LogError("Failed to generate track report");
                return false;

            }

            ExcelWorksheet wsViolations = package.Workbook.Worksheets.Add("Violations");
            if (!WriteViolations(wsViolations, track, useGPSAltitude, maxAllowedAltitude))
            {
                Logger?.LogError("Failed to generate track report");
                return false;
            }

            package.Save();
            Logger?.LogInformation("Successfully generated and saved report at '{fileInfo.FullName}'", fileInfo.FullName);

            return true;
        }


        private static bool WriteTrackPoints(ExcelWorksheet wsTrackpoints, Track track, bool skipCoordinatesWithOutLocation, bool useGPSAltitude, out List<(int easting, int norting)> trackChartPoints, out List<(DateTime timestamp, double altitude)> altitudeChartPoints)
        {
            trackChartPoints = [];
            altitudeChartPoints = [];
            try
            {
                Logger?.LogInformation("Writing track points ...");
                wsTrackpoints.Cells[1, 1].Value = "Pilot Number";
                wsTrackpoints.Cells[1, 2].Value = track.Pilot?.PilotNumber;
                wsTrackpoints.Cells[2, 1].Value = "Pilot Identifier";
                wsTrackpoints.Cells[2, 2].Value = track.Pilot?.PilotIdentifier;
                if (track.AdditionalPropertiesFromIGCFile.TryGetValue("SensBoxSerialNumber", out string value))
                {
                    wsTrackpoints.Cells[3, 1].Value = "Sens Box Serial Number";
                    wsTrackpoints.Cells[3, 2].Value = value;
                }

                if (!TrackHelpers.EstimateLaunchAndLandingTime(track, useGPSAltitude, out Coordinate launchPoint, out Coordinate landingPoint))
                {
                    Logger?.LogError("Launch or landing point not correctly calculated");
                }
                wsTrackpoints.Cells[1, 5].Value = "Timestamp";
                wsTrackpoints.Cells[1, 6].Value = "Long";
                wsTrackpoints.Cells[1, 7].Value = "Lat";
                wsTrackpoints.Cells[1, 8].Value = "Long [°]";
                wsTrackpoints.Cells[1, 9].Value = "Lat [°]";
                wsTrackpoints.Cells[1, 10].Value = "UTM Zone";
                wsTrackpoints.Cells[1, 11].Value = "East";
                wsTrackpoints.Cells[1, 12].Value = "North";
                wsTrackpoints.Cells[1, 13].Value = "Alt [m]";
                wsTrackpoints.Cells[1, 14].Value = "Alt [ft]";
                wsTrackpoints.Cells["E1:N1"].Style.Font.Bold = true;

                (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) degreeMinuteFormat;
                string longitudeBeautified;
                string latitudeBeautified;
                CoordinateSharp.Coordinate coordinateSharp;

                wsTrackpoints.Cells[2, 4].Value = "Launch Point";
                wsTrackpoints.Cells[2, 4].Style.Font.Bold = true;

                wsTrackpoints.Cells[2, 5].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                wsTrackpoints.Cells[2, 5].Value = launchPoint.TimeStamp;
                wsTrackpoints.Cells[2, 6].Value = launchPoint.Longitude;
                wsTrackpoints.Cells[2, 7].Value = launchPoint.Latitude;
                degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(launchPoint.Longitude);
                longitudeBeautified = $"{(launchPoint.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                wsTrackpoints.Cells[2, 8].Value = longitudeBeautified;
                degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(launchPoint.Latitude);
                latitudeBeautified = $"{(launchPoint.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                wsTrackpoints.Cells[2, 9].Value = latitudeBeautified;
                coordinateSharp = new CoordinateSharp.Coordinate(launchPoint.Latitude, launchPoint.Longitude);
                wsTrackpoints.Cells[2, 10].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                wsTrackpoints.Cells[2, 11].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                wsTrackpoints.Cells[2, 12].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                wsTrackpoints.Cells[2, 13].Value = launchPoint.AltitudeGPS;
                wsTrackpoints.Cells[2, 14].Value = Math.Round(CoordinateHelpers.ConvertToFeet(launchPoint.AltitudeGPS), 0, MidpointRounding.AwayFromZero);


                wsTrackpoints.Cells[3, 4].Value = "Landing Point";
                wsTrackpoints.Cells[3, 4].Style.Font.Bold = true;
                wsTrackpoints.Cells[3, 5].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                wsTrackpoints.Cells[3, 5].Value = landingPoint.TimeStamp;
                wsTrackpoints.Cells[3, 6].Value = landingPoint.Longitude;
                wsTrackpoints.Cells[3, 7].Value = landingPoint.Latitude;
                degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(landingPoint.Longitude);
                longitudeBeautified = $"{(landingPoint.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                wsTrackpoints.Cells[3, 8].Value = longitudeBeautified;
                degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(landingPoint.Latitude);
                latitudeBeautified = $"{(landingPoint.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                wsTrackpoints.Cells[3, 9].Value = latitudeBeautified;
                coordinateSharp = new CoordinateSharp.Coordinate(landingPoint.Latitude, landingPoint.Longitude);
                wsTrackpoints.Cells[3, 10].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                wsTrackpoints.Cells[3, 11].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                wsTrackpoints.Cells[3, 12].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                wsTrackpoints.Cells[3, 13].Value = landingPoint.AltitudeGPS;
                wsTrackpoints.Cells[3, 14].Value = Math.Round(CoordinateHelpers.ConvertToFeet(landingPoint.AltitudeGPS), 0, MidpointRounding.AwayFromZero);


                ExcelRange takeOffTouchDownRange = wsTrackpoints.Cells["E1:N3"];
                ExcelTable takeOffTouchDownTable = wsTrackpoints.Tables.Add(takeOffTouchDownRange, "TakeOff_TouchDown");
                takeOffTouchDownTable.TableStyle = TableStyles.Light16;

                wsTrackpoints.Cells[6, 1].Value = "Timestamp";
                wsTrackpoints.Cells[6, 2].Value = "Long";
                wsTrackpoints.Cells[6, 3].Value = "Lat";
                wsTrackpoints.Cells[6, 4].Value = "Long [°]";
                wsTrackpoints.Cells[6, 5].Value = "Lat [°]";
                wsTrackpoints.Cells[6, 6].Value = "UTM Zone";
                wsTrackpoints.Cells[6, 7].Value = "East";
                wsTrackpoints.Cells[6, 8].Value = "North";
                wsTrackpoints.Cells[6, 9].Value = "Alt [m]";
                wsTrackpoints.Cells[6, 10].Value = "Alt [ft]";
                wsTrackpoints.Cells["A6:J6"].Style.Font.Bold = true;


                int index = 7;
                List<Coordinate> coordinates = [.. track.TrackPoints.OrderBy(x => x.TimeStamp)];

                foreach (Coordinate coordinate in coordinates)
                {
                    if ((skipCoordinatesWithOutLocation) && (Math.Abs(coordinate.Longitude) < double.Epsilon))
                        continue;
                    wsTrackpoints.Cells[index, 1].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsTrackpoints.Cells[index, 1].Value = coordinate.TimeStamp;
                    wsTrackpoints.Cells[index, 2].Value = coordinate.Longitude;
                    wsTrackpoints.Cells[index, 3].Value = coordinate.Latitude;
                    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Longitude);
                    longitudeBeautified = $"{(coordinate.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    wsTrackpoints.Cells[index, 4].Value = longitudeBeautified;
                    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Latitude);
                    latitudeBeautified = $"{(coordinate.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    wsTrackpoints.Cells[index, 5].Value = latitudeBeautified;
                    coordinateSharp = new CoordinateSharp.Coordinate(coordinate.Latitude, coordinate.Longitude);
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

                ExcelRange range = wsTrackpoints.Cells[6, 1, index - 1, 10];
                ExcelTable table = wsTrackpoints.Tables.Add(range, "Trackpoints");
                table.TableStyle = TableStyles.Light16;
                wsTrackpoints.Cells.Style.Font.Size = 10;

                wsTrackpoints.Cells.AutoFitColumns();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to write track points");
                return false;
            }

            return true;

        }

        private static bool WriteDeclaratrionsAndMarkerDrops(ExcelWorksheet wsDeclarationsAndMarkerDrops, Track track, bool useGPSAltitude)
        {
            try
            {
                Logger?.LogInformation("Writing declarations and marker drops ...");
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
                wsDeclarationsAndMarkerDrops.Cells[1, 1, 1, 23].Style.Font.Bold = true;
                List<Declaration> declarations = [.. track.Declarations.OrderBy(x => x.GoalNumber)];

                List<double> distance2D = TrackHelpers.Calculate2DDistanceBetweenPositionOfDeclarationAndDeclaredGoal(declarations);
                List<double> distance3D = TrackHelpers.Calculate3DDistanceBetweenPositionOfDeclarationAndDeclaredGoal(declarations, useGPSAltitude);
                int index = 2;
                foreach (Declaration declaration in declarations)
                {
                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = declaration.GoalNumber;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = declaration.PositionAtDeclaration.TimeStamp;
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = declaration.PositionAtDeclaration.Longitude;
                    wsDeclarationsAndMarkerDrops.Cells[index, 4].Value = declaration.PositionAtDeclaration.Latitude;
                    (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaration.PositionAtDeclaration.Longitude);
                    string longitudeBeautified = $"{(declaration.PositionAtDeclaration.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaration.PositionAtDeclaration.Latitude);
                    string latitudeBeautified = $"{(declaration.PositionAtDeclaration.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = latitudeBeautified;
                    CoordinateSharp.Coordinate coordinateSharp = new(declaration.PositionAtDeclaration.Latitude, declaration.PositionAtDeclaration.Longitude);
                    wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationsAndMarkerDrops.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 9].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 10].Value = declaration.PositionAtDeclaration.AltitudeGPS;
                    wsDeclarationsAndMarkerDrops.Cells[index, 11].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaration.PositionAtDeclaration.AltitudeGPS), 0, MidpointRounding.AwayFromZero);

                    wsDeclarationsAndMarkerDrops.Cells[index, 13].Value = declaration.DeclaredGoal.Longitude;
                    wsDeclarationsAndMarkerDrops.Cells[index, 14].Value = declaration.DeclaredGoal.Latitude;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaration.DeclaredGoal.Longitude);
                    longitudeBeautified = $"{(declaration.DeclaredGoal.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 15].Value = longitudeBeautified;
                    (degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds) = CoordinateHelpers.ConvertToDegreeMinutes(declaration.DeclaredGoal.Latitude);
                    latitudeBeautified = $"{(declaration.DeclaredGoal.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degrees, degreeMinutes, degreeSeconds, degreeTenthSeconds)}";
                    wsDeclarationsAndMarkerDrops.Cells[index, 16].Value = latitudeBeautified;
                    coordinateSharp = new CoordinateSharp.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);
                    wsDeclarationsAndMarkerDrops.Cells[index, 17].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsDeclarationsAndMarkerDrops.Cells[index, 18].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 19].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 20].Value = declaration.DeclaredGoal.AltitudeGPS;
                    wsDeclarationsAndMarkerDrops.Cells[index, 21].Value = Math.Round(CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
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
                List<MarkerDrop> markerDrops = [.. track.MarkerDrops.OrderBy(x => x.MarkerNumber)];
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
                    CoordinateSharp.Coordinate coordinateSharp = new(markerDrop.MarkerLocation.Latitude, markerDrop.MarkerLocation.Longitude);
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
                int indexDistance = index;
                List<(string identifier, double distance)> distance2DGoals = TrackHelpers.Calculate2DDistanceBetweenDeclaredGoals(declarations);
                List<(string identifier, double distance)> distance3DGoals = TrackHelpers.Calculate3DDistanceBetweenDeclaredGoals(declarations, useGPSAltitude);
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Dist. Goals";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distGoalsStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "ID";
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
                List<(string identifier, double distance)> distance2DGoalsToMarkers = TrackHelpers.Calculate2DDistanceBetweenMarkerAndGoals(declarations, markerDrops);
                List<(string identifier, double distance)> distance3DGoalsToMarkers = TrackHelpers.Calculate3DDistanceBetweenMarkerAndGoals(declarations, markerDrops, useGPSAltitude);

                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Dist. Goals to Markers";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distGoalToMarkStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "ID";
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
                List<(string identifier, double distance)> distance3DMarkers = TrackHelpers.Calculate3DDistanceBetweenMarkers(markerDrops, useGPSAltitude);
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "Dist. Markers";
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Style.Font.Bold = true;
                index++;
                int distMarkStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = "ID";
                wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 1, index, 3].Style.Font.Bold = true;
                index++;
                for (int markerIndex = 0; markerIndex < distance2DMarkers.Count; markerIndex++)
                {

                    wsDeclarationsAndMarkerDrops.Cells[index, 1].Value = distance2DMarkers[markerIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 2].Value = Math.Round(distance2DMarkers[markerIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 3].Value = Math.Round(distance3DMarkers[markerIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange distMarkRange = wsDeclarationsAndMarkerDrops.Cells[distMarkStartRow, 1, index - 1, 3];
                ExcelTable distMarkTable = wsDeclarationsAndMarkerDrops.Tables.Add(distMarkRange, "Distance_Markers");
                distMarkTable.TableStyle = TableStyles.Light16;


                index = indexDistance;
                if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out Coordinate landingPoint))
                {
                    Logger?.LogError("Launch or landing point not correctly calculated");
                }
                List<(string identifier, double distance)> distance2DLaunchToGoals = TrackHelpers.Calculate2DDistanceBetweenLaunchPointAndGoals(launchPoint, declarations);
                List<(string identifier, double distance)> distance3DLaunchToGoals = TrackHelpers.Calculate3DDistanceBetweenLaunchPointAndGoals(launchPoint, declarations, useGPSAltitude);
                wsDeclarationsAndMarkerDrops.Cells[index, 5, index, 7].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = "Dist. Launch to Goals";
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Style.Font.Bold = true;
                index++;
                int distLaunchStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = "ID";
                wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 5, index, 7].Style.Font.Bold = true;
                index++;
                for (int goalIndex = 0; goalIndex < distance2DLaunchToGoals.Count; goalIndex++)
                {
                    wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = distance2DLaunchToGoals[goalIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = Math.Round(distance2DLaunchToGoals[goalIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = Math.Round(distance3DLaunchToGoals[goalIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange distLaunchRange = wsDeclarationsAndMarkerDrops.Cells[distLaunchStartRow, 5, index - 1, 7];
                ExcelTable distLaunchTable = wsDeclarationsAndMarkerDrops.Tables.Add(distLaunchRange, "Distance_Launch");
                distLaunchTable.TableStyle = TableStyles.Light16;

                index += 3;

                List<(string identifier, double distance)> distance2DLandingToGoals = TrackHelpers.Calculate2DDistanceBetweenLandingPointAndGoals(landingPoint, declarations);
                List<(string identifier, double distance)> distance3DLandingToGoals = TrackHelpers.Calculate3DDistanceBetweenLandingPointAndGoals(landingPoint, declarations, useGPSAltitude);
                wsDeclarationsAndMarkerDrops.Cells[index, 5, index, 7].Merge = true;
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = "Dist. Landing to Goals";
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Style.Font.Bold = true;
                index++;
                int distLandingStartRow = index;
                wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = "ID";
                wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = "Dist 2D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = "Dist 3D [m]";
                wsDeclarationsAndMarkerDrops.Cells[index, 5, index, 7].Style.Font.Bold = true;
                index++;
                for (int goalIndex = 0; goalIndex < distance2DLandingToGoals.Count; goalIndex++)
                {
                    wsDeclarationsAndMarkerDrops.Cells[index, 5].Value = distance2DLandingToGoals[goalIndex].identifier;
                    wsDeclarationsAndMarkerDrops.Cells[index, 6].Value = Math.Round(distance2DLandingToGoals[goalIndex].distance, 0, MidpointRounding.AwayFromZero);
                    wsDeclarationsAndMarkerDrops.Cells[index, 7].Value = Math.Round(distance3DLandingToGoals[goalIndex].distance, 0, MidpointRounding.AwayFromZero);
                    index++;
                }
                ExcelRange distLandingRange = wsDeclarationsAndMarkerDrops.Cells[distLandingStartRow, 5, index - 1, 7];
                ExcelTable distLandingTable = wsDeclarationsAndMarkerDrops.Tables.Add(distLandingRange, "Distance_Landing");
                distLandingTable.TableStyle = TableStyles.Light16;

                wsDeclarationsAndMarkerDrops.Cells.Style.Font.Size = 10;
                wsDeclarationsAndMarkerDrops.Cells.AutoFitColumns();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to write declarations and marker drops");
                return false;
            }
            return true;
        }

        private static bool CreateCharts(ExcelWorksheet wsCharts, List<(int easting, int norting)> trackChartPoints,
        List<(DateTime timestamp, double altitude)> altitudeChartPoints)
        {
            try
            {
                Logger?.LogInformation("Create track charts ...");
                wsCharts.Cells[1, 1].Value = "Easting";
                wsCharts.Cells[1, 2].Value = "Northing";
                wsCharts.Cells[1, 3].Value = "Time";
                wsCharts.Cells[1, 4].Value = "Alt [m]";
                int index = 2;
                if (trackChartPoints.Count == altitudeChartPoints.Count)
                {
                    for (int chartIndex = 0; chartIndex < trackChartPoints.Count; chartIndex++)
                    {
                        wsCharts.Cells[index, 1].Value = trackChartPoints[chartIndex].easting;
                        wsCharts.Cells[index, 2].Value = trackChartPoints[chartIndex].norting;
                        wsCharts.Cells[index, 3].Style.Numberformat.Format = "HH:mm:ss";
                        wsCharts.Cells[index, 3].Value = altitudeChartPoints[chartIndex].timestamp;
                        wsCharts.Cells[index, 4].Value = altitudeChartPoints[chartIndex].altitude;
                        index++;
                    }

                    ExcelScatterChart trackChart = wsCharts.Drawings.AddScatterChart("Track", eScatterChartType.XYScatterLinesNoMarkers);
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
                    Logger?.LogError("Failed to generate charts: track and altitude chart are expected to have the same number of points");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to generate charts");
                return false;
            }
            return true;
        }

        private static bool WriteIncidentsInOrder(ExcelWorksheet wsIncidients, Track track)
        {
            try
            {
                Logger?.LogInformation("Write incidents in order of occurrence ...");
                if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out Coordinate landingPoint))
                {
                    Logger?.LogError("Launch or landing point not correctly calculated");
                }
                List<(DateTime timeStamp, string incident)> incidents = [(launchPoint.TimeStamp, "Take Off"), (landingPoint.TimeStamp, "Touch Down")];
                foreach (Declaration declaration in track.Declarations)
                {
                    incidents.Add((declaration.PositionAtDeclaration.TimeStamp, $"Dec. Goal{declaration.GoalNumber}"));
                }
                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    incidents.Add((markerDrop.MarkerLocation.TimeStamp, $"Marker{markerDrop.MarkerNumber}"));
                }

                incidents = [.. incidents.OrderBy(x => x.timeStamp)];

                wsIncidients.Cells[1, 1].Value = "Time";
                wsIncidients.Cells[1, 2].Value = "Incident";
                int index = 2;
                foreach ((DateTime timeStamp, string incidient) in incidents)
                {
                    wsIncidients.Cells[index, 1].Value = timeStamp;
                    wsIncidients.Cells[index, 1].Style.Numberformat.Format = "HH:mm:ss";
                    wsIncidients.Cells[index, 2].Value = incidient;
                    index++;
                }

                ExcelRange incidentsRange = wsIncidients.Cells[1, 1, index - 1, 2];
                ExcelTable incidentsTable = wsIncidients.Tables.Add(incidentsRange, "Incidents");
                incidentsTable.TableStyle = TableStyles.Light16;

                wsIncidients.Cells.Style.Font.Size = 10;
                wsIncidients.Cells.AutoFitColumns();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to write events in order");
                return false;
            }
            return true;
        }

        private static bool WriteViolations(ExcelWorksheet wsViolations, Track track, bool useGPSAltitude, double maxAllowedAltitude)
        {
            try
            {
                Logger?.LogInformation("List violations ...");
                PenaltyCalculation.CheckForDangerousFlyingAndCalculatePenaltyPoints(track, useGPSAltitude, out bool isDangerousFlyingDetected, out List<Coordinate> relatedCoordinates, out double maxDropRate, out double maxClimbRate, out TimeSpan totalDuration, out int penaltyPointsDangerousFlying);
                wsViolations.Cells[1, 1].Value = "Dangerous Flying";
                wsViolations.Cells[2, 1].Value = isDangerousFlyingDetected ? "yes" : "no";
                wsViolations.Cells[1, 2].Value = "Vertical Limit [m/s]";
                wsViolations.Cells[2, 2].Value = "+/-8";
                wsViolations.Cells[1, 3].Value = "Trigger Threshold [s]";
                wsViolations.Cells[2, 3].Value = "5";
                wsViolations.Cells[1, 4].Value = "Duration";
                wsViolations.Cells[2, 4].Value = totalDuration;
                wsViolations.Cells[2, 4].Style.Numberformat.Format = "HH:mm:ss";
                wsViolations.Cells[1, 5].Value = "Max Drop Rate [m/s]";
                wsViolations.Cells[2, 5].Value = maxDropRate;
                wsViolations.Cells[1, 6].Value = "Max Climb Rate [m/s]";
                wsViolations.Cells[2, 6].Value = maxClimbRate;
                wsViolations.Cells[1, 7].Value = "Penalties";
                wsViolations.Cells[2, 7].Value = penaltyPointsDangerousFlying;
                wsViolations.Cells[1, 1, 1, 7].Style.Font.Bold = true;

                ExcelRange dangerousFlyingSummaryRange = wsViolations.Cells[1, 1, 2, 7];
                ExcelTable dangerousFlyingSummaryTable = wsViolations.Tables.Add(dangerousFlyingSummaryRange, "Dangerous_Flying_Sum");
                dangerousFlyingSummaryTable.TableStyle = TableStyles.Light16;


                wsViolations.Cells[4, 1].Value = "Timestamp";
                wsViolations.Cells[4, 2].Value = "Long";
                wsViolations.Cells[4, 3].Value = "Lat";
                wsViolations.Cells[4, 4].Value = "Long [°]";
                wsViolations.Cells[4, 5].Value = "Lat [°]";
                wsViolations.Cells[4, 6].Value = "UTM Zone";
                wsViolations.Cells[4, 7].Value = "East";
                wsViolations.Cells[4, 8].Value = "North";
                wsViolations.Cells[4, 9].Value = "Alt [m]";
                wsViolations.Cells[4, 10].Value = "Alt [ft]";
                wsViolations.Cells[4, 1, 4, 10].Style.Font.Bold = true;
                int index = 5;
                (int degrees, int degreeMinutes, int degreeSeconds, int degreeTenthSeconds) degreeMinuteFormat;
                string longitudeBeautified;
                string latitudeBeautified;
                CoordinateSharp.Coordinate coordinateSharp;
                foreach (Coordinate coordinate in relatedCoordinates)
                {
                    wsViolations.Cells[index, 1].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    wsViolations.Cells[index, 1].Value = coordinate.TimeStamp;
                    wsViolations.Cells[index, 2].Value = coordinate.Longitude;
                    wsViolations.Cells[index, 3].Value = coordinate.Latitude;
                    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Longitude);
                    longitudeBeautified = $"{(coordinate.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    wsViolations.Cells[index, 4].Value = longitudeBeautified;
                    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Latitude);
                    latitudeBeautified = $"{(coordinate.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    wsViolations.Cells[index, 5].Value = latitudeBeautified;
                    coordinateSharp = new CoordinateSharp.Coordinate(coordinate.Latitude, coordinate.Longitude);
                    wsViolations.Cells[index, 6].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    wsViolations.Cells[index, 7].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    wsViolations.Cells[index, 8].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    wsViolations.Cells[index, 9].Value = coordinate.AltitudeGPS;
                    wsViolations.Cells[index, 10].Value = Math.Round(CoordinateHelpers.ConvertToFeet(coordinate.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    index++;
                }


                ExcelRange dangerousFlyingRange = wsViolations.Cells[4, 1, index - 1, 10];
                ExcelTable dangerousFlyingTable = wsViolations.Tables.Add(dangerousFlyingRange, "Dangerous_Flying");
                dangerousFlyingTable.TableStyle = TableStyles.Light16;

                //12
                if (double.IsFinite(maxAllowedAltitude))
                {
                    PenaltyCalculation.CheckForBluePZAndCalculatePenaltyPoints(maxAllowedAltitude, useGPSAltitude, [track], out List<(int pilotNumber, int numberOfViolatingTrackPoints, TimeSpan durationInBluePZ,double maxAlitudeInFeet, int penalty)> penalties);
                    wsViolations.Cells[1, 12].Value = "Max Allowed Altitude [ft]";
                    wsViolations.Cells[2, 12].Value = Math.Round(CoordinateHelpers.ConvertToFeet(maxAllowedAltitude), 0, MidpointRounding.AwayFromZero);
                    wsViolations.Cells[1, 13].Value = "Max Altitude [ft]";
                    wsViolations.Cells[2, 13].Value = Math.Round(penalties.FirstOrDefault().maxAlitudeInFeet, 0, MidpointRounding.AwayFromZero);
                    wsViolations.Cells[1, 14].Value = "Duration";
                    wsViolations.Cells[2, 14].Value = penalties.FirstOrDefault().durationInBluePZ;
                    wsViolations.Cells[2, 14].Style.Numberformat.Format = "HH:mm:ss";
                    wsViolations.Cells[1, 15].Value = "Penalties";
                    wsViolations.Cells[2, 15].Value = penalties.FirstOrDefault().penalty;
                    wsViolations.Cells[1, 12, 1, 15].Style.Font.Bold = true;
                    ExcelRange maxAltitudeSummaryRange = wsViolations.Cells[1, 12, 2, 15];
                    ExcelTable maxAltitudeSummaryTable = wsViolations.Tables.Add(maxAltitudeSummaryRange, "Max_Altitude_Sum");
                    maxAltitudeSummaryTable.TableStyle = TableStyles.Light16;

                    //index = 5;
                    //wsViolations.Cells[4, 12].Value = "Timestamp";
                    //wsViolations.Cells[4, 13].Value = "Long";
                    //wsViolations.Cells[4, 14].Value = "Lat";
                    //wsViolations.Cells[4, 15].Value = "Long [°]";
                    //wsViolations.Cells[4, 16].Value = "Lat [°]";
                    //wsViolations.Cells[4, 17].Value = "UTM Zone";
                    //wsViolations.Cells[4, 18].Value = "East";
                    //wsViolations.Cells[4, 19].Value = "North";
                    //wsViolations.Cells[4, 20].Value = "Alt [m]";
                    //wsViolations.Cells[4, 21].Value = "Alt [ft]";
                    //wsViolations.Cells[4, 12, 4, 21].Style.Font.Bold = true;
                    //foreach (Coordinate coordinate in trackPointsAbove)
                    //{
                    //    wsViolations.Cells[index, 12].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    //    wsViolations.Cells[index, 12].Value = coordinate.TimeStamp;
                    //    wsViolations.Cells[index, 13].Value = coordinate.Longitude;
                    //    wsViolations.Cells[index, 14].Value = coordinate.Latitude;
                    //    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Longitude);
                    //    longitudeBeautified = $"{(coordinate.Longitude < 0.0 ? "W" : "E")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    //    wsViolations.Cells[index, 15].Value = longitudeBeautified;
                    //    degreeMinuteFormat = CoordinateHelpers.ConvertToDegreeMinutes(coordinate.Latitude);
                    //    latitudeBeautified = $"{(coordinate.Latitude < 0.0 ? "S" : "N")} {CoordinateHelpers.BeautifyDegreeMinutes(degreeMinuteFormat.degrees, degreeMinuteFormat.degreeMinutes, degreeMinuteFormat.degreeSeconds, degreeMinuteFormat.degreeTenthSeconds)}";
                    //    wsViolations.Cells[index, 16].Value = latitudeBeautified;
                    //    coordinateSharp = new CoordinateSharp.Coordinate(coordinate.Latitude, coordinate.Longitude);
                    //    wsViolations.Cells[index, 17].Value = coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone;
                    //    wsViolations.Cells[index, 18].Value = Math.Round(coordinateSharp.UTM.Easting, 0, MidpointRounding.AwayFromZero);
                    //    wsViolations.Cells[index, 19].Value = Math.Round(coordinateSharp.UTM.Northing, 0, MidpointRounding.AwayFromZero);
                    //    wsViolations.Cells[index, 20].Value = coordinate.AltitudeGPS;
                    //    wsViolations.Cells[index, 21].Value = Math.Round(CoordinateHelpers.ConvertToFeet(coordinate.AltitudeGPS), 0, MidpointRounding.AwayFromZero);
                    //    index++;
                    //}
                    //ExcelRange maxAltitudeRange = wsViolations.Cells[4, 12, index - 1, 21];
                    //ExcelTable maxAltitudeTable = wsViolations.Tables.Add(maxAltitudeRange, "Max_Altitude");
                    //maxAltitudeTable.TableStyle = TableStyles.Light16;
                }

                wsViolations.Cells.Style.Font.Size = 10;
                wsViolations.Cells.AutoFitColumns();

            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to list violations");
                return false;
            }
            return true;
        }
    }
}
