using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Coordinates.Parsers
{
    public static class BalloonLiveParser
    {
        private static ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(BalloonLiveParser));


        private static bool MarkerDrop_HasAdditionalLatitudeDecimals = false;
        private static int MarkerDrop_StartOfAdditionalLatitudeDecimals = -1;
        private static int MarkerDrop_EndOfAdditionalLatitudeDecimals = -1;

        private static bool MarkerDrop_HasAdditionalLongitudeDecimals = false;
        private static int MarkerDrop_StartOfAdditionalLongitudeDecimals = -1;
        private static int MarkerDrop_EndOfAdditionalLongitudeDecimals = -1;

        private static bool Declaration_HasAdditionalLatitudeDecimals = false;
        private static int Declaration_StartOfAdditionalLatitudeDecimals = -1;
        private static int Declaration_EndOfAdditionalLatitudeDecimals = -1;

        private static bool Declaration_HasAdditionalLongitudeDecimals = false;
        private static int Declaration_StartOfAdditionalLongitudeDecimals = -1;
        private static int Declaration_EndOfAdditionalLongitudeDecimals = -1;

        private static bool TrackPoint_HasAdditionalLatitudeDecimals = false;
        private static int TrackPoint_StartOfAdditionalLatitudeDecimals = -1;
        private static int TrackPoint_EndOfAdditionalLatitudeDecimals = -1;

        private static bool TrackPoint_HasAdditionalLongitudeDecimals = false;
        private static int TrackPoint_StartOfAdditionalLongitudeDecimals = -1;
        private static int TrackPoint_EndOfAdditionalLongitudeDecimals = -1;

        #region API

        /// <summary>
        /// Parses a .igc file and pass back a track object
        /// </summary>
        /// <param name="fileNameAndPath">the file path and name of the .igc file</param>
        /// <param name="track">output parameter. the parsed track from the file</param>
        /// <param name="referenceCoordinate">provide a reference coordinate to be used complete the missing information of a goal declaration
        /// <para>a goal declaration not in the zone 6/7 format is ambiguous</para>
        /// <para>this is an optional parameter, the parse will use either marker drop 1 or the position at declaration if not reference point has been provided</para></param>
        /// /// <param name="defaultGoalAlitude">the default declaration altitude in meters to be used when the pilot didn't declared one</param>
        /// <returns>true:success; false:error</returns>
        public static bool ParseFile(string fileNameAndPath, out Track track, Coordinate referenceCoordinate = null, double defaultGoalAlitude = 0.0)
        {

            track = null;
            try
            {
                Logger?.LogInformation("Parsing file '{filePathAndName}'", fileNameAndPath);
                FileInfo fileInfo = new(fileNameAndPath);
                if (!fileInfo.Exists)
                {
                    Logger?.LogError("Failed to parse the file '{filePathAndName}': the file does not exists", fileNameAndPath);
                    return false;
                }

                if (!fileInfo.Extension.EndsWith("igc"))
                {
                    Logger?.LogError("Failed to parse the file '{filePathAndName}': the file extension '{fileExtension}' is not supported", fileNameAndPath, fileInfo.Extension);
                    return false;
                }
                track = new Track();
                int pilotNumber = -1;
                string pilotIdentifier = "";
                DateTime date = new();
                bool declaredAltitudeIsInFeet = true;
                List<string> lines = [];
                using (StreamReader reader = new(fileNameAndPath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        lines.Add(line);
                    }
                }
                string[] identifierLine = lines.Where(x => x.StartsWith("AXBL")).ToArray();
                if (identifierLine.Length == 0)
                {
                    identifierLine = lines.Where(x => x.StartsWith("AXXX")).ToArray();
                    if (identifierLine.Length == 0)
                    {
                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Line with Pilot Identifier 'AXBL' or 'AXXX' is missing", fileNameAndPath);
                        return false;
                    }
                    if (identifierLine.Length > 1)
                    {
                        Logger?.LogWarning("More the one line with Pilot Identifier ('AXXX') were found. First occurrence will be used.");
                    }
                    else
                    {
                        pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("BalloonLive", "");
                    }
                }
                if (identifierLine.Length > 1)
                {
                    Logger?.LogWarning("More the one line with Pilot Identifier ('AXBL') were found. First occurrence will be used.");
                }
                else
                {
                    pilotIdentifier = identifierLine[0][4..12];
                }


                string[] headerLines = lines.Where(x => x.StartsWith('H')).ToArray();
                bool markerDrop_HasAdditionalLatitudeDecimals;
                int markerDrop_StartOfAdditionalLatitudeDecimals;
                int markerDrop_EndOfAdditionalLatitudeDecimals;
                bool markerDrop_HasAdditionalLongitudeDecimals;
                int markerDrop_StartOfAdditionalLongitudeDecimals;
                int markerDrop_EndOfAdditionalLongitudeDecimals;
                bool declaration_HasAdditionalLatitudeDecimals;
                int declaration_StartOfAdditionalLatitudeDecimals;
                int declaration_EndOfAdditionalLatitudeDecimals;
                bool declaration_HasAdditionalLongitudeDecimals;
                int declaration_StartOfAdditionalLongitudeDecimals;
                int declaration_EndOfAdditionalLongitudeDecimals;
                if (!ParseHeaders(headerLines, out pilotNumber, out date, out markerDrop_HasAdditionalLatitudeDecimals, out markerDrop_StartOfAdditionalLatitudeDecimals, out markerDrop_EndOfAdditionalLatitudeDecimals, out markerDrop_HasAdditionalLongitudeDecimals, out markerDrop_StartOfAdditionalLongitudeDecimals, out markerDrop_EndOfAdditionalLongitudeDecimals, out declaration_HasAdditionalLatitudeDecimals, out declaration_StartOfAdditionalLatitudeDecimals, out declaration_EndOfAdditionalLatitudeDecimals, out declaration_HasAdditionalLongitudeDecimals, out declaration_StartOfAdditionalLongitudeDecimals, out declaration_EndOfAdditionalLongitudeDecimals))
                {
                    Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse header lines", fileNameAndPath);
                    return false;
                }

                string[] configLines = lines.Where(x => x.StartsWith("LXXX")).ToArray();
                foreach (string configLine in configLines)
                {
                    if (configLine.StartsWith("LXXX alt unit"))
                    {
                        if (configLine.Contains("feet"))
                        {
                            declaredAltitudeIsInFeet = true;
                        }
                        else
                        {
                            declaredAltitudeIsInFeet = false;
                        }
                    }
                    if (configLine.StartsWith("LXXX BLSSN"))
                    {
                        string sensBoxSerialNumber = configLine.Split('=').Last();
                        if (track.AdditionalPropertiesFromIGCFile.TryAdd("SensBoxSerialNumber", sensBoxSerialNumber))
                        {
                            Logger?.LogWarning("Failed to add SensBoxSerialNumber '{SensBoxSerialNumber}' to the track", sensBoxSerialNumber);
                        }
                    }
                }

                string iRecordLine = lines.Where(x => x.StartsWith('I')).FirstOrDefault();
                int offset = 3;
                bool trackPoint_HasAdditionalLatitudeDecimals = false;
                int trackPoint_StartOfAdditionalLatitudeDecimals = -1;
                int trackPoint_EndOfAdditionalLatitudeDecimals = -1;
                bool trackPoint_HasAdditionalLongitudeDecimals = false;
                int trackPoint_StartOfAdditionalLongitudeDecimals = -1;
                int trackPoint_EndOfAdditionalLongitudeDecimals = -1;
                if (!string.IsNullOrWhiteSpace(iRecordLine))
                {
                    if (!int.TryParse(iRecordLine[1..3], out int numberOfAdditions))
                    {

                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse number of additions from I-record", fileNameAndPath);
                        return false;
                    }

                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        offset = index * 7 + 3;
                        if (!int.TryParse(iRecordLine[offset..(offset + 2)], out int startPosition))
                        {
                            Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse start position of I-record addition no {additionNumber}", fileNameAndPath, index + 1);
                            return false;
                        }
                        offset += 2;
                        if (!int.TryParse(iRecordLine[(offset)..(offset + 2)], out int stopPosition))
                        {
                            Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse stop position of I-record addition no {additionNumber}", fileNameAndPath, index + 1);
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = iRecordLine[(offset)..(offset + 3)];

                        if (additionIdentifier.Equals("LAD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            trackPoint_HasAdditionalLatitudeDecimals = true;
                            trackPoint_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            trackPoint_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.Equals("LOD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            trackPoint_HasAdditionalLongitudeDecimals = true;
                            trackPoint_StartOfAdditionalLongitudeDecimals = startPosition - 1;//adjust to zero based index
                            trackPoint_EndOfAdditionalLongitudeDecimals = stopPosition;
                        }
                    }
                }
                string[] positionSourceEvents = lines.Where(x => x.StartsWith('E') && x.Contains("XS")).ToArray();
                bool isFirstSourceEvent = true;
                foreach (string positionSourceEvent in positionSourceEvents)
                {
                    if (!ParseSourceEvent(positionSourceEvent, date, out DateTime timeStamp, out bool isPrimarySource, out bool isBallonLiveSensor, out string blsSerialNumber))
                    {
                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse position source event", fileNameAndPath);
                        return false;
                    }
                    if (isFirstSourceEvent)
                    {
                        isFirstSourceEvent = false;
                        Logger?.LogInformation("Position source of track is: '{sourceType}' position source '{source}' {blsSerialNumber}", (isPrimarySource ? "primary" : "fallback"), (isBallonLiveSensor ? "Ballon Live Sensor" : "Phone Internal"), (isBallonLiveSensor ? $"with serial number '{blsSerialNumber}'" : ""));
                    }
                    else
                    {
                        Logger?.LogWarning("Caution, change of position source detected at '{timestamp}': {sourceType}' position source '{source}' {blsSerialNumber}", timeStamp.ToString("dd-MMM-yyyy HH:mm:ss"), (isPrimarySource ? "primary" : "fallback"), (isBallonLiveSensor ? "Ballon Live Sensor" : "Phone Internal"), (isBallonLiveSensor ? $"with serial number '{blsSerialNumber}'" : ""));
                        if (!track.AdditionalPropertiesFromIGCFile.TryAdd("Change of position source", "yes"))
                            Logger?.LogWarning("Failed to add 'Change of position source' to the track");
                    }
                }
                if (!track.AdditionalPropertiesFromIGCFile.TryAdd("Change of position source", "no"))
                    Logger?.LogWarning("Failed to add 'Change of position source' to the track");
                string[] trackPointLines = lines.Where(x => x.StartsWith('B')).ToArray();
                foreach (string trackPointLine in trackPointLines)
                {
                    Coordinate coordinate;
                    if (!ParseTrackPoint(trackPointLine, date,
                        trackPoint_HasAdditionalLatitudeDecimals,
                        trackPoint_StartOfAdditionalLatitudeDecimals,
                        trackPoint_EndOfAdditionalLatitudeDecimals,
                        trackPoint_HasAdditionalLongitudeDecimals,
                        trackPoint_StartOfAdditionalLongitudeDecimals,
                        trackPoint_EndOfAdditionalLongitudeDecimals, out coordinate))
                    {
                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse trackpoint", fileNameAndPath);
                        return false;
                    }
                    track.TrackPoints.Add(coordinate);
                }

                string[] markerDropLines = lines.Where(x => x.StartsWith('E') && x.Contains("XX0")).ToArray();
                foreach (string markerDropLine in markerDropLines)
                {
                    MarkerDrop markerDrop;
                    if (!ParseMarkerDrop(markerDropLine, date,
                        markerDrop_HasAdditionalLatitudeDecimals,
                        markerDrop_StartOfAdditionalLatitudeDecimals,
                        markerDrop_EndOfAdditionalLatitudeDecimals,
                        markerDrop_HasAdditionalLongitudeDecimals,
                        markerDrop_StartOfAdditionalLongitudeDecimals,
                        markerDrop_EndOfAdditionalLongitudeDecimals,
                        out markerDrop))
                    {
                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse marker drop", fileNameAndPath);
                        return false;
                    }
                    track.MarkerDrops.Add(markerDrop);
                }
                if (referenceCoordinate is null)
                {
                    if (track.MarkerDrops.Count > 0)
                    {
                        MarkerDrop markerDrop = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
                        if (markerDrop != null)
                        {
                            referenceCoordinate = markerDrop.MarkerLocation;
                        }
                        else
                        {
                            referenceCoordinate = track.MarkerDrops[0].MarkerLocation;
                            Logger?.LogWarning("No marker 1 dropped. Marker '{markerNumber}' will be used as goal declaration reference", track.MarkerDrops[0].MarkerNumber);
                        }
                    }
                    else
                    {
                        Logger?.LogWarning("No marker drops found. Position at declaration will be used as reference instead");
                    }
                }
                string[] goalDeclarationLines = lines.Where(x => x.StartsWith('E') && x.Contains("XL1")).ToArray();
                foreach (string goalDeclarationLine in goalDeclarationLines)
                {

                    Declaration declaration;
                    if (!ParseGoalDeclaration(goalDeclarationLine, 
                        date, 
                        declaredAltitudeIsInFeet,
                        defaultGoalAlitude,
                        referenceCoordinate,
                        declaration_HasAdditionalLatitudeDecimals,
                        declaration_StartOfAdditionalLatitudeDecimals,
                        declaration_EndOfAdditionalLatitudeDecimals,
                        declaration_HasAdditionalLongitudeDecimals,
                        declaration_StartOfAdditionalLongitudeDecimals,
                        declaration_EndOfAdditionalLongitudeDecimals,
                        out declaration))
                    {
                        Logger?.LogError("Failed to parse the file '{filePathAndName}': Failed to parse goal declaration", fileNameAndPath);
                        return false;
                    }
                    if (declaration != null)
                        track.Declarations.Add(declaration);
                }

                Pilot pilot = new(pilotNumber, pilotIdentifier);
                track.Pilot = pilot;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to parse the file '{filePathAndName}'", fileNameAndPath);
                return false;
            }
            Logger?.LogInformation("Successfully parsed file '{filePathAndName}'", fileNameAndPath);
            return true;
        }

        #endregion

        #region Private methods
        private static bool ParseHeaders(string[] headerLines, out int pilotNumber, out DateTime date,
            out bool markerDrop_HasAdditionalLatitudeDecimals,
            out int markerDrop_StartOfAdditionalLatitudeDecimals,
            out int markerDrop_EndOfAdditionalLatitudeDecimals,
            out bool markerDrop_HasAdditionalLongitudeDecimals,
            out int markerDrop_StartOfAdditionalLongitudeDecimals,
            out int markerDrop_EndOfAdditionalLongitudeDecimals, out bool declaration_HasAdditionalLatitudeDecimals,
            out int declaration_StartOfAdditionalLatitudeDecimals,
            out int declaration_EndOfAdditionalLatitudeDecimals,
            out bool declaration_HasAdditionalLongitudeDecimals,
            out int declaration_StartOfAdditionalLongitudeDecimals,
            out int declaration_EndOfAdditionalLongitudeDecimals)
        {
            date = DateTime.MinValue;
            pilotNumber = -1;
            string functionErrorMessage = "Failed to parse header lines: ";
            markerDrop_HasAdditionalLatitudeDecimals = false;
            markerDrop_StartOfAdditionalLatitudeDecimals = -1;
            markerDrop_EndOfAdditionalLatitudeDecimals = -1;
            markerDrop_HasAdditionalLongitudeDecimals = false;
            markerDrop_StartOfAdditionalLongitudeDecimals = -1;
            markerDrop_EndOfAdditionalLongitudeDecimals = -1;
            declaration_HasAdditionalLatitudeDecimals = false;
            declaration_StartOfAdditionalLatitudeDecimals = -1;
            declaration_EndOfAdditionalLatitudeDecimals = -1;
            declaration_HasAdditionalLongitudeDecimals = false;
            declaration_StartOfAdditionalLongitudeDecimals = -1;
            declaration_EndOfAdditionalLongitudeDecimals = -1;

            foreach (string headerLine in headerLines)
            {
                if (headerLine.StartsWith("HFDTE"))
                {
                    string line = headerLine.Replace("HFDTE", "");
                    if (!int.TryParse(line[0..2], out int day))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse day portion of date '{portion}' in '{line}'", line[0..2], line);
                        return false;
                    }
                    if (!int.TryParse(line[2..4], out int month))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse month portion of date '{portion}' in '{line}'", line[2..4], line);
                        return false;
                    }
                    if (!int.TryParse(line[4..6], out int year))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse year portion of date '{portion}' in '{line}'", line[4..6], line);
                        return false;
                    }
                    year += 2000;
                    date = new DateTime(year, month, day);
                }
                if (headerLine.StartsWith("HFPID"))
                {
                    string pilotNumberText = headerLine.Replace("HFPID", "");
                    if (!int.TryParse(pilotNumberText, out pilotNumber))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse pilot number '{pilotNumberText}' in '{headerLine}'", pilotNumberText, headerLine);
                        return false;
                    }
                }
                if (headerLine.StartsWith("HFXII:XX0:"))
                {
                    if (!int.TryParse(headerLine[10..12], out int numberOfAdditions))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse number of additions from I-record from portion '{portion}' in '{headerLine}'", headerLine[10..12], headerLine);
                        return false;
                    }
                    int offset = 12;
                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        offset = index * 7 + 12;
                        if (!int.TryParse(headerLine[offset..(offset + 2)], out int startPosition))
                        {
                            Logger?.LogError("Failed to parse header lines: Could not parse start position of I-record addition no '{additionNumber}' from portion '{portion}' in '{headerLine}'", index + 1, headerLine[offset..(offset + 2)], headerLine);
                            return false;
                        }
                        offset += 2;
                        if (!int.TryParse(headerLine[(offset)..(offset + 2)], out int stopPosition))
                        {
                            Logger?.LogError("Failed to parse header lines: Could not parse stop position of I-record addition no '{additionNumber}' in from portion '{portion}' '{headerLine}'", index + 1, headerLine[(offset)..(offset + 2)], headerLine);
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = headerLine[(offset)..(offset + 3)];

                        if (additionIdentifier.Equals("LAD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            markerDrop_HasAdditionalLatitudeDecimals = true;
                            markerDrop_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            markerDrop_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.Equals("LOD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            markerDrop_HasAdditionalLongitudeDecimals = true;
                            markerDrop_StartOfAdditionalLongitudeDecimals = startPosition - 1;//adjust to zero based index
                            markerDrop_EndOfAdditionalLongitudeDecimals = stopPosition;
                        }
                    }

                }
                if (headerLine.StartsWith("HFXII:XL1:"))
                {
                    if (!int.TryParse(headerLine[10..12], out int numberOfAdditions))
                    {
                        Logger?.LogError("Failed to parse header lines: Could not parse number of additions from I-record from portion '{portion}' in '{headerLine}'", headerLine[10..12], headerLine);
                        return false;
                    }

                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        int offset = index * 7 + 12;
                        if (!int.TryParse(headerLine[offset..(offset + 2)], out int startPosition))
                        {
                            Logger?.LogError("Failed to parse header lines: Could not parse start position of I-record addition no '{additionNumber}' from portion '{portion}' in '{headerLine}'", index + 1, headerLine[offset..(offset + 2)], headerLine);
                            return false;
                        }
                        offset += 2;
                        if (!int.TryParse(headerLine[(offset)..(offset + 2)], out int stopPosition))
                        {
                            Logger?.LogError("Failed to parse header lines: Could not parse stop position of I-record addition no '{additionNumber}' in from portion '{portion}' '{headerLine}'", index + 1, headerLine[(offset)..(offset + 2)], headerLine);
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = headerLine[(offset)..(offset + 3)];

                        if (additionIdentifier.Equals("LAD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            declaration_HasAdditionalLatitudeDecimals = true;
                            declaration_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            declaration_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.Equals("LOD", StringComparison.CurrentCultureIgnoreCase))
                        {
                            declaration_HasAdditionalLongitudeDecimals = true;
                            declaration_StartOfAdditionalLongitudeDecimals = startPosition - 1;//adjust to zero based index
                            declaration_EndOfAdditionalLongitudeDecimals = stopPosition;
                        }
                    }
                }

            }
            return true;
        }


        /// <summary>
        /// Parses a line containing a track point (BttttttnnnnnnnNeeeeeeeeEAbbbbbgggggaaassddd0000)
        /// <para>where t:timestamp n:northing e:easting b:barometric altitude [m] g:GPS altitude [m]</para>
        /// <para>a:accuracy s:number of satellites d: engine noise level and rpm 0:carrier return and line feed</para>
        /// <para> e.g. B1058394839658N00858176EA0000000537000225940000</para>
        /// </summary>
        /// <param name="line">the line in the file</param>
        /// <param name="date">the date from the header</param>
        /// <param name="coordinate">output parameter. a trackpoint as coordinate object</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseTrackPoint(string line, DateTime date,
            bool trackPoint_HasAdditionalLatitudeDecimals,
            int trackPoint_StartOfAdditionalLatitudeDecimals,
            int trackPoint_EndOfAdditionalLatitudeDecimals,
            bool trackPoint_HasAdditionalLongitudeDecimals,
            int trackPoint_StartOfAdditionalLongitudeDecimals,
            int trackPoint_EndOfAdditionalLongitudeDecimals,
            out Coordinate coordinate)
        {
            coordinate = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse track point: Cannot parse timestamp from '{line}'", line);
                return false;
            }

            double latitude;
            if (trackPoint_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[7..15], line[trackPoint_StartOfAdditionalLatitudeDecimals..trackPoint_EndOfAdditionalLatitudeDecimals], out latitude))
                {
                    Logger?.LogError("Failed to parse track point: Cannot parse latitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[7..15], line[TrackPoint_StartOfAdditionalLatitudeDecimals..TrackPoint_EndOfAdditionalLatitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[7..15], out latitude))
                {
                    Logger?.LogError("Failed to parse track point: Cannot parse latitude from portion '{portion}' in '{line}'", line[7..15], line);
                    return false;
                }
            }
            double longitude;
            if (trackPoint_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[15..24], line[trackPoint_StartOfAdditionalLongitudeDecimals..trackPoint_EndOfAdditionalLongitudeDecimals], out longitude))
                {
                    Logger?.LogError("Failed to parse track point: Cannot parse longitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[15..24], line[TrackPoint_StartOfAdditionalLongitudeDecimals..TrackPoint_EndOfAdditionalLongitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[15..24], out longitude))
                {
                    Logger?.LogError("Failed to parse track point: Cannot parse longitude from portion '{portion}' in '{line}'", line[15..24], line);
                    return false;
                }
            }




            if (!double.TryParse(line[25..30], out double altitudeBarometric))
            {
                Logger?.LogError("Failed to parse track point: Cannot parse barometric altitude from portion '{portion}' in '{line}'", line[25..30], line);
                return false;
            }

            if (!double.TryParse(line[30..35], out double altitudeGPS))
            {
                Logger?.LogError("Failed to parse track point: Cannot parse GPS altitude from portion '{portion}' in '{line}'", line[30..35], line);
                return false;
            }
            coordinate = new Coordinate(latitude, longitude, altitudeGPS, altitudeBarometric, timeStamp);
            return true;
        }


        /// <summary>
        /// Parses a line with a goal declaration (EttttttXL1ddññññ*/ëëëë*,hhhh#,nnnnnnnNeeeeeeeeEbbbbbgggggaaassddd0000)
        /// <para>where t:timestamp d:number of declared goal ñ:goal northing in utm ë:goal easting in utm (*:the exact format in specified in the header at 'LXXX declaration digits') h:declared height (#: the unit is specified in the header)
        /// <para>n:northing e:easting b:barometric altitude g:gps altitude</para>
        /// <para>a:accuracy s:number of satellites d: engine noise level and rpm 0:carrier return and line feed</para>
        /// <para>e.g E105850XL101123456/987654,1500,4839658N00858176EA0000000537000224940000</para>
        /// </summary>
        /// <param name="line">the line in the file</param>
        /// <param name="date">the date form the header</param>
        /// <param name="declaredAltitudeIsInFeet">true: declared height is in feet; false: declared height is in meter</param>
        /// <param name="referenceCoordinate">a reference coordinate to fill up the missing info from utm goal declaration. If the reference is null, the position of declaration will be used instead</param>
        /// <param name="declaration">output parameter. the declaration</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseGoalDeclaration(string line, DateTime date, bool declaredAltitudeIsInFeet,double defaultGoalAltitude,
            Coordinate referenceCoordinate,
            bool declaration_HasAdditionalLatitudeDecimals,
            int declaration_StartOfAdditionalLatitudeDecimals,
            int declaration_EndOfAdditionalLatitudeDecimals,
            bool declaration_HasAdditionalLongitudeDecimals,
            int declaration_StartOfAdditionalLongitudeDecimals,
            int declaration_EndOfAdditionalLongitudeDecimals,
            out Declaration declaration)
        {
            declaration = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse goal declaration: Cannot parse timestamp from '{line}'", line);
                return false;
            }
            if (!int.TryParse(line[10..12], out int goalNumber))
            {
                Logger?.LogError("Failed to parse goal declaration: Cannot parse goal number from portion '{portion}' in '{line}'", line[10..12], line);
                return false;
            }

            double declarationLatitude;
            if (declaration_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[12..20], line[declaration_StartOfAdditionalLatitudeDecimals..declaration_EndOfAdditionalLatitudeDecimals], out declarationLatitude))
                {
                    Logger?.LogError("Failed to parse goal declaration: Cannot parse latitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[12..20], line[Declaration_StartOfAdditionalLatitudeDecimals..Declaration_EndOfAdditionalLatitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[12..20], out declarationLatitude))
                {
                    Logger?.LogError("Failed to parse goal declaration: Cannot parse latitude from portion '{portion}' in '{line}'", line[12..20], line);
                    return false;
                }
            }
            double declarationLongitude;
            if (declaration_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[20..29], line[declaration_StartOfAdditionalLongitudeDecimals..declaration_EndOfAdditionalLongitudeDecimals], out declarationLongitude))
                {
                    Logger?.LogError("Failed to parse goal declaration: Cannot parse longitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[20..29], line[Declaration_StartOfAdditionalLongitudeDecimals..Declaration_EndOfAdditionalLongitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[20..29], out declarationLongitude))
                {
                    Logger?.LogError("Failed to parse goal declaration: Cannot parse longitude from portion '{portion}' in '{line}'", line[20..29], line);
                    return false;
                }
            }

            if (!double.TryParse(line[30..35], out double declarationPositonAltitudeBarometric))
            {
                Logger?.LogError("Failed to parse goal declaration: Cannot parse barometric altitude from portion '{portion}' in '{line}'", line[30..35], line);
                return false;
            }

            if (!double.TryParse(line[35..40], out double declarationPositionAltitudeGPS))
            {
                Logger?.LogError("Failed to parse goal declaration: Cannot parse GPS altitude from portion '{portion}' in '{line}'", line[35..40], line);
                return false;
            }
            string declarationText = line[43..^0];
            if (declarationText.Length > 2)
            {

                string[] parts = declarationText.Split(',');
                string[] locations = parts[0].Split('/');
                int eastingDigits;
                int eastingUTM;
                int northingDigits;
                int northingUTM;
                if (locations.Length == 2)
                {
                    if (!string.IsNullOrWhiteSpace(locations[0]))
                    {
                        eastingDigits = locations[0].Length;
                        if (!int.TryParse(locations[0], out eastingUTM))
                        {
                            Logger?.LogError("Failed to parse goal declaration: Cannot parse easting from portion '{portion}' in '{line}'", locations[0], line);
                            return false;
                        }
                    }
                    else
                    {
                        Logger?.LogError("Failed to parse goal declaration: Cannot parse easting from portion '{portion}' in '{line}'", locations[0], line);
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(locations[1]))
                    {
                        northingDigits = locations[1].Length;
                        if (!int.TryParse(locations[1], out northingUTM))
                        {
                            Logger?.LogError("Failed to parse goal declaration: Cannot parse northing from portion '{portion}' in '{line}'", locations[1], line);
                            return false;
                        }
                    }
                    else
                    {
                        Logger?.LogError("Failed to parse goal declaration: Cannot parse northing from portion '{portion}' in '{line}'", locations[1], line);
                        return false;
                    }

                }
                else
                {
                    Logger?.LogError("Failed to parse goal declaration: Cannot parse easting and northing from portion '{portion}' in '{line}'", declarationText, line);
                    return false;
                }

                bool hasPilotDelaredGoalAltitude = true;
                double declaredAltitudeInMeter;
                if (parts.Length == 2)
                {
                    if (!string.IsNullOrWhiteSpace(parts[1]))
                    {
                        string altitudePart = parts[1].Replace("ft", "").Replace("m", "");
                        if (!int.TryParse(altitudePart, out int declaredAltitude))
                        {
                            Logger?.LogError("Failed to parse goal declaration: Cannot parse altitude from portion '{portion}' in '{line}'", altitudePart, line);
                            return false;
                        }
                        if (declaredAltitudeIsInFeet)
                            declaredAltitudeInMeter = CoordinateHelpers.ConvertToMeter((double)declaredAltitude);
                        else
                            declaredAltitudeInMeter = (double)declaredAltitude;
                    }
                    else
                    {
                        hasPilotDelaredGoalAltitude = false;
                        declaredAltitudeInMeter = defaultGoalAltitude;
                        Logger?.LogWarning("No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed", goalNumber);
                    }
                }
                else
                {
                    hasPilotDelaredGoalAltitude = false;
                    declaredAltitudeInMeter = defaultGoalAltitude;
                    Logger?.LogWarning("No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed", goalNumber);
                }

                CoordinateSharp.Coordinate coordinateSharp;
                double goalNorthingUTM = double.NaN;
                double goalEastingUTM = double.NaN;
                Coordinate declaredGoal = null;
                Coordinate positionAtDeclaration = null;
                bool useDeclarationPosition = false;
                if (referenceCoordinate != null)
                {
                    coordinateSharp = new CoordinateSharp.Coordinate(referenceCoordinate.Latitude, referenceCoordinate.Longitude);

                    string utmGridZone = coordinateSharp.UTM.LatZone + coordinateSharp.UTM.LongZone;
                    if (northingDigits < 6)
                    {
                        goalNorthingUTM = northingUTM * 10;
                        goalNorthingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits + 1)) * Math.Pow(10, northingDigits + 1));
                    }
                    if (northingDigits == 6)
                    {
                        goalNorthingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
                    }

                    if (eastingDigits != 6)
                    {
                        goalEastingUTM = eastingUTM * 10;
                        goalEastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));

                    }

                    CoordinateSharp.UniversalTransverseMercator utm = new(utmGridZone, goalEastingUTM, goalNorthingUTM);

                    CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

                    declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);
                    positionAtDeclaration = new Coordinate(declarationLatitude, declarationLongitude, declarationPositionAltitudeGPS, declarationPositonAltitudeBarometric, timeStamp);
                    double distance = CoordinateHelpers.Calculate2DDistanceHavercos(declaredGoal, positionAtDeclaration);
                    if (distance > 70e3)
                    {

                        useDeclarationPosition = true;
                    }
                }
                if (useDeclarationPosition)
                {
                    coordinateSharp = new CoordinateSharp.Coordinate(declarationLatitude, declarationLongitude);
                    string utmGridZone = coordinateSharp.UTM.LatZone + coordinateSharp.UTM.LongZone;
                    if (northingDigits < 6)
                    {
                        goalNorthingUTM = northingUTM * 10;
                        goalNorthingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits + 1)) * Math.Pow(10, northingDigits + 1));
                    }
                    if (northingDigits == 6)
                    {
                        goalNorthingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
                    }

                    if (eastingDigits != 6)
                    {
                        goalEastingUTM = eastingUTM * 10;
                        goalEastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));

                    }

                    CoordinateSharp.UniversalTransverseMercator utm = new(utmGridZone, goalEastingUTM, goalNorthingUTM);

                    CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

                    declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);
                    positionAtDeclaration = new Coordinate(declarationLatitude, declarationLongitude, declarationPositionAltitudeGPS, declarationPositonAltitudeBarometric, timeStamp);
                    double distance = CoordinateHelpers.Calculate2DDistanceHavercos(declaredGoal, positionAtDeclaration);
                    if (distance > 70e3)
                    {
                        Logger?.LogWarning("Suspicious declaration of gaol {goalNumber}: {locations[0]}/{locations[1]}", goalNumber, locations[0], locations[1]);
                    }
                }

                declaration = new Declaration(goalNumber, declaredGoal, positionAtDeclaration, hasPilotDelaredGoalAltitude, eastingUTM, northingUTM);
            }
            else
            {
                Logger?.LogWarning("Declaration contained no coordinates and therefore has not been added to the track");
            }

            return true;
        }

        /// <summary>
        /// Parses a line with a marker drop (EttttttXX0ddnnnnnnnNeeeeeeeeEbbbbbgggggaaassddd0000)
        /// <para>where t:time stamp d:marker number n:northing e:easting b:barometric altitude g:gps altitude</para>
        /// <para>a:accuracy s:number of satellites d: engine noise level and rpm 0:carrier return and line feed</para>
        /// <para>e.g E110615XX0024839666N00858106EA0000000539000227870000</para>
        /// </summary>
        /// <param name="line">the line in the file</param>
        /// <param name="date">the data form the header</param>
        /// <param name="markerDrop">output parameter. the marker drop as object</param>
        /// <returns>true:success; false: error</returns>
        private static bool ParseMarkerDrop(string line, DateTime date,
            bool markerDrop_HasAdditionalLatitudeDecimals,
            int markerDrop_StartOfAdditionalLatitudeDecimals,
            int markerDrop_EndOfAdditionalLatitudeDecimals,
            bool markerDrop_HasAdditionalLongitudeDecimals,
            int markerDrop_StartOfAdditionalLongitudeDecimals,
            int markerDrop_EndOfAdditionalLongitudeDecimals,
            out MarkerDrop markerDrop)
        {
            markerDrop = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse marker drop: Cannot parse timestamp from '{line}'", line);
                return false;
            }
            if (!int.TryParse(line[10..12], out int markerNumber))
            {
                Logger?.LogError("Failed to parse marker drop: Cannot parse marker number from portion '{portion}' in '{line}'", line[10..12], line);
                return false;
            }

            double latitude;
            if (markerDrop_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[12..20], line[markerDrop_StartOfAdditionalLatitudeDecimals..markerDrop_EndOfAdditionalLatitudeDecimals], out latitude))
                {
                    Logger?.LogError("Failed to parse marker drop: Cannot parse latitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[12..20], line[MarkerDrop_StartOfAdditionalLatitudeDecimals..MarkerDrop_EndOfAdditionalLatitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[12..20], out latitude))
                {
                    Logger?.LogError("Failed to parse marker drop: Cannot parse latitude from portion '{portion}' in '{line}'", line[12..20], line);
                    return false;
                }
            }
            double longitude;
            if (markerDrop_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[20..29], line[markerDrop_StartOfAdditionalLongitudeDecimals..markerDrop_EndOfAdditionalLongitudeDecimals], out longitude))
                {
                    Logger?.LogError("Failed to parse marker drop: Cannot parse longitude from portion '{portion}' and additional decimals '{additionDecimals}' in '{line}'", line[20..29], line[MarkerDrop_StartOfAdditionalLongitudeDecimals..MarkerDrop_EndOfAdditionalLongitudeDecimals], line);
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[20..29], out longitude))
                {
                    Logger?.LogError("Failed to parse marker drop: Cannot parse longitude from portion '{portion}' in '{line}'", line[20..29], line);
                    return false;
                }
            }

            if (!double.TryParse(line[30..35], out double altitudeBarometric))
            {
                Logger?.LogError("Failed to parse marker drop: Cannot parse barometric altitude from portion '{portion}' in '{line}'", line[30..35], line);
                return false;
            }

            if (!double.TryParse(line[35..40], out double altitudeGPS))
            {
                Logger?.LogError("Failed to parse marker drop: Cannot parse GPS altitude from portion '{portion}' in '{line}'", line[35..40], line);
                return false;
            }
            Coordinate coordinate = new(latitude, longitude, altitudeGPS, altitudeBarometric, timeStamp);
            markerDrop = new MarkerDrop(markerNumber, coordinate);
            return true;
        }

        /// <summary>
        /// Parses and converts the latitude into decimal degree
        /// </summary>
        /// <param name="latitudeText">the northing porting (nnnnnnnN)</param>
        /// <param name="latitude">output parameter. the latitude in decimal degree</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseLatitude(string latitudeText, out double latitude)
        {
            latitude = double.NaN;
            double factor;
            if (latitudeText.EndsWith('N'))
                factor = 1.0;
            else if (latitudeText.EndsWith('S'))
                factor = -1.0;
            else
            {
                Logger?.LogError("Failed to parse latitude text. Unexpected suffix '{suffix}'", latitudeText[^1]);
                return false;
            }
            if (!double.TryParse(latitudeText[0..2], out double fullAngle))
            {
                Logger?.LogError("Failed to parse latitude full angle from '{latitudeText}'", latitudeText[0..2]);
                return false;
            }
            if (!double.TryParse(latitudeText[2..7], out double decimalAngle))
            {
                Logger?.LogError("Failed to parse latitude decimal angle '{latitudeText}'", latitudeText[2..7]);
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal angles

            latitude = factor * (fullAngle + decimalAngle);
            return true;
        }

        private static bool ParseLatitudeWithAdditionalDecimals(string standardLatitudeText, string additionalDecimals, out double latitude)
        {
            latitude = double.NaN;
            double factor;
            if (standardLatitudeText.EndsWith('N'))
                factor = 1.0;
            else if (standardLatitudeText.EndsWith('S'))
                factor = -1.0;
            else
            {
                Logger?.LogError("Failed to parse latitude text. Unexpected suffix '{suffix}'", standardLatitudeText[^1]);
                return false;
            }
            if (!double.TryParse(standardLatitudeText[0..2], out double fullAngle))
            {
                Logger?.LogError("Failed to parse latitude full angle from '{latitudeText}'", standardLatitudeText[0..2]);
                return false;
            }
            if (!double.TryParse(standardLatitudeText[2..7] + additionalDecimals, out double decimalAngle))
            {
                Logger?.LogError("Failed to parse latitude decimal angle '{latitudeText}'", standardLatitudeText[2..7] + additionalDecimals);
                return false;
            }
            if (decimalAngle > 0.0)
            {
                int divider = (standardLatitudeText[2..7] + additionalDecimals).Length - 2;//first two digits are integer the rest are decimal places
                decimalAngle /= (Math.Pow(10.0, divider) * 60.0);//divide two get decimal places right and convert from minutes to degrees
            }
            latitude = factor * (fullAngle + decimalAngle);
            return true;
        }

        /// <summary>
        /// Parses and converts the longitude into decimal degree
        /// </summary>
        /// <param name="longitudeText">the easting porting (eeeeeeeeE)</param>
        /// <param name="longitude">output parameter. the longitude in decimal degree</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseLongitude(string longitudeText, out double longitude)
        {
            longitude = double.NaN;
            double factor;
            if (longitudeText.EndsWith('E'))
                factor = 1.0;
            else if (longitudeText.EndsWith('W'))
                factor = -1.0;
            else
            {
                Logger?.LogError("Failed to parse longitude text. Unexpected suffix '{suffix}'", longitudeText[^1]);
                return false;
            }
            if (!double.TryParse(longitudeText[0..3], out double fullAngle))
            {
                Logger?.LogError("Failed to parse longitude full angle '{longitudeText}'", longitudeText[0..3]);
                return false;
            }
            if (!double.TryParse(longitudeText[3..8], out double decimalAngle))
            {
                Logger?.LogError("Failed to parse longitude decimal angle '{longitudeText}'", longitudeText[3..8]);
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal degree

            longitude = factor * (fullAngle + decimalAngle);
            return true;
        }

        private static bool ParseLongitudeWithAdditionalDecimals(string standardLongitudeText, string additinalDecimals, out double longitude)
        {
            longitude = double.NaN;
            double factor;
            if (standardLongitudeText.EndsWith('E'))
                factor = 1.0;
            else if (standardLongitudeText.EndsWith('W'))
                factor = -1.0;
            else
            {
                Logger?.LogError("Failed to parse longitude text. Unexpected suffix '{suffix}'", standardLongitudeText[^1]);
                return false;
            }
            if (!double.TryParse(standardLongitudeText[0..3], out double fullAngle))
            {
                Logger?.LogError("Failed to parse longitude full angle '{longitudeText}'", standardLongitudeText[0..3]);
                return false;
            }
            if (!double.TryParse(standardLongitudeText[3..8] + additinalDecimals, out double decimalAngle))
            {
                Logger?.LogError("Failed to parse longitude decimal angle '{longitudeText}'", standardLongitudeText[3..8] + additinalDecimals);
                return false;
            }
            if (decimalAngle > 0.0)
            {
                int divider = (standardLongitudeText[3..8] + additinalDecimals).Length - 2;//first two digits are integer the rest are decimal places
                decimalAngle /= (Math.Pow(10.0, divider) * 60.0);//divide two get decimal places right and convert from minutes to degrees
            }
            longitude = factor * (fullAngle + decimalAngle);
            return true;
        }

        /// <summary>
        /// Parses the time stamp and creates a date time object using the date specified
        /// </summary>
        /// <param name="line">the time stamp portion</param>
        /// <param name="date">the date from the header</param>
        /// <param name="timeStamp">output parameter. the time stamp (including the date)</param>
        /// <returns>true:success: false:error</returns>
        private static bool ParseTimeStamp(string line, DateTime date, out DateTime timeStamp)
        {
            string time = line[1..7];
            timeStamp = date;
            if (!int.TryParse(time[0..2], out int hours))
            {
                Logger?.LogError("Failed to parse time: Cannot parse hour portion '{portion}' in '{line}'", time[0..2], line);
                return false;
            }
            if (!int.TryParse(time[2..4], out int minutes))
            {
                Logger?.LogError("Failed to parse time: Cannot parse minute portion '{portion}' in '{line}'", time[2..4], line);
                return false;
            }
            if (!int.TryParse(time[4..6], out int seconds))
            {
                Logger?.LogError("Failed to parse time: Cannot parse second portion '{portion}' in '{line}'", time[4..6], line);
                return false;
            }
            timeStamp = date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return true;
        }

        private static void ResetProperties()
        {
            MarkerDrop_HasAdditionalLatitudeDecimals = false;
            MarkerDrop_StartOfAdditionalLatitudeDecimals = -1;
            MarkerDrop_EndOfAdditionalLatitudeDecimals = -1;

            MarkerDrop_HasAdditionalLongitudeDecimals = false;
            MarkerDrop_StartOfAdditionalLongitudeDecimals = -1;
            MarkerDrop_EndOfAdditionalLongitudeDecimals = -1;

            Declaration_HasAdditionalLatitudeDecimals = false;
            Declaration_StartOfAdditionalLatitudeDecimals = -1;
            Declaration_EndOfAdditionalLatitudeDecimals = -1;

            Declaration_HasAdditionalLongitudeDecimals = false;
            Declaration_StartOfAdditionalLongitudeDecimals = -1;
            Declaration_EndOfAdditionalLongitudeDecimals = -1;

            TrackPoint_HasAdditionalLatitudeDecimals = false;
            TrackPoint_StartOfAdditionalLatitudeDecimals = -1;
            TrackPoint_EndOfAdditionalLatitudeDecimals = -1;

            TrackPoint_HasAdditionalLongitudeDecimals = false;
            TrackPoint_StartOfAdditionalLongitudeDecimals = -1;
            TrackPoint_EndOfAdditionalLongitudeDecimals = -1;
        }

        private static bool ParseSourceEvent(string line, DateTime date, out DateTime timeStamp, out bool isPrimarySource, out bool isBallonLiveSensor, out string blsSerialNumber)
        {
            isPrimarySource = false;
            isBallonLiveSensor = false;
            blsSerialNumber = string.Empty;

            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                Logger?.LogError("Failed to parse position source event: Cannot parse timestamp from '{line}'", line);
                return false;
            }
            if (line.Contains("XS0"))
            {
                isPrimarySource = true;
            }
            else if (line.Contains("XS1"))
            {
                isPrimarySource = false;
            }
            else
            {
                Logger?.LogWarning("Unknown source event '{sourceEvent}' in line '{line}'", line[7..10], line);
            }

            if (line.Contains("INT"))
            {
                isBallonLiveSensor = false;
            }
            else if (line.Contains("BLS"))
            {
                isBallonLiveSensor = true;
                blsSerialNumber = line.Split(',').Last();
            }
            else
            {
                Logger?.LogWarning("Unknown position source '{sourceEvent}' in line '{line}'", line[10..13], line);
            }

            return true;

        }
        #endregion

    }
}
