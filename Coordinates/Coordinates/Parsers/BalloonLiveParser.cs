﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using LoggerComponent;


namespace Coordinates.Parsers
{
    public static class BalloonLiveParser
    {
        #region API

        /// <summary>
        /// Parses a .igc file and pass back a track object
        /// </summary>
        /// <param name="fileNameAndPath">the file path and name of the .igc file</param>
        /// <param name="track">output parameter. the parsed track from the file</param>
        /// <param name="defaultGoalAlitude">the default declaration altitude in meters to be used when the pilot didn't declared one</param>
        /// <returns>true:success; false:error</returns>
        public static bool ParseFile(string fileNameAndPath, out Track track, double defaultGoalAlitude=0.0)
        {
            string functionErrorMessage = $"Failed to parse file '{fileNameAndPath}':";
            track = null;
            try
            {

                FileInfo fileInfo = new FileInfo(fileNameAndPath);
                if (!fileInfo.Exists)
                {
                    //Debug.WriteLine(functionErrorMessage + $"The file '{fileNameAndPath}' does not exists");
                    Log(LogSeverityType.Error, functionErrorMessage + $"The file '{fileNameAndPath}' does not exists");
                    return false;
                }

                if (!fileInfo.Extension.EndsWith("igc"))
                {
                    //Debug.WriteLine(functionErrorMessage + $"The file extension '{fileInfo.Extension}' is not supported");
                    Log(LogSeverityType.Error, functionErrorMessage + $"The file extension '{fileInfo.Extension}' is not supported");
                    return false;
                }
                track = new Track();
                int pilotNumber = -1;
                string pilotIdentifier = "";
                DateTime date = new DateTime();
                //int goalNortingDigits = -1;
                //int goalEastingDigits = -1;
                bool declaredAltitudeIsInFeet = true;
                List<string> lines = new List<string>();
                using (StreamReader reader = new StreamReader(fileNameAndPath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        lines.Add(line);
                    }
                }
                //string[] identifierLine = lines.Where(x => x.StartsWith("AXXX")).ToArray();
                string[] identifierLine = lines.Where(x => x.StartsWith("AXBL")).ToArray();
                if (identifierLine.Length == 0)
                {
                    identifierLine = lines.Where(x => x.StartsWith("AXXX")).ToArray();
                    if (identifierLine.Length == 0)
                    {
                        Log(LogSeverityType.Error, "Line with Pilot Identifier 'AXBL' or 'AXXX' is missing");
                        return false;
                    }
                    if (identifierLine.Length > 1)
                    {
                        //Debug.WriteLine("More the one line with Pilot Identifier is found. First occurrence will be used.");
                        Log(LogSeverityType.Warning, "More the one line with Pilot Identifier ('AXXX') is found. First occurrence will be used.");
                    }
                    else
                    {
                        pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("BalloonLive", "");
                    }
                }
                if (identifierLine.Length > 1)
                {
                    //Debug.WriteLine("More the one line with Pilot Identifier is found. First occurrence will be used.");
                    Log(LogSeverityType.Warning, "More the one line with Pilot Identifier ('AXBL') is found. First occurrence will be used.");
                }
                else
                {
                    pilotIdentifier = identifierLine[0][4..12];
                }
                //pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("BalloonLive", "");


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
                    Log(LogSeverityType.Error, functionErrorMessage);
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
                        if (!track.AdditionalPropertiesFromIGCFile.ContainsKey("SensBoxSerialNumber"))
                            track.AdditionalPropertiesFromIGCFile.Add("SensBoxSerialNumber", sensBoxSerialNumber);
                    }
                    if (configLine.StartsWith("LXXX loggerinterval"))
                    {
                        string loggerInterval = configLine.Split("=").Last();
                        if (!track.AdditionalPropertiesFromIGCFile.ContainsKey("LoggerInterval"))
                            track.AdditionalPropertiesFromIGCFile.Add("LoggerInterval", loggerInterval.Replace("s",""));
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
                    int numberOfAdditions;
                    if (!int.TryParse(iRecordLine[1..3], out numberOfAdditions))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + " Failed to parse number of additions from I-record");
                        return false;
                    }

                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        offset = index * 7 + 3;
                        int startPosition;
                        if (!int.TryParse(iRecordLine[offset..(offset + 2)], out startPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse start position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        int stopPosition;
                        if (!int.TryParse(iRecordLine[(offset)..(offset + 2)], out stopPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse stop position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = iRecordLine[(offset)..(offset + 3)];

                        if (additionIdentifier.ToUpper() == "LAD")
                        {
                            trackPoint_HasAdditionalLatitudeDecimals = true;
                            trackPoint_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            trackPoint_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.ToUpper() == "LOD")
                        {
                            trackPoint_HasAdditionalLongitudeDecimals = true;
                            trackPoint_StartOfAdditionalLongitudeDecimals = startPosition - 1;//adjust to zero based index
                            trackPoint_EndOfAdditionalLongitudeDecimals = stopPosition;
                        }
                    }
                }
                string[] positionSourceEvents = lines.Where(x => x.StartsWith('E') && x.Contains("XS")).ToArray();
                bool isFirstSourceEvent = true;
                DateTime timeStamp;
                bool isPrimarySource;
                bool isBallonLiveSensor;
                string blsSerialNumber;
                foreach (string positionSourceEvent in positionSourceEvents)
                {
                    if (!ParseSourceEvent(positionSourceEvent, date, out timeStamp, out isPrimarySource, out isBallonLiveSensor, out blsSerialNumber))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse position source event");
                        return false;
                    }
                    if (isFirstSourceEvent)
                    {
                        isFirstSourceEvent = false;
                        Log(LogSeverityType.Info, $"Position source at track start is: '{(isPrimarySource ? "primary" : "fallback")}' position source '{(isBallonLiveSensor ? "Ballon Live Sensor" : "Phone Internal")}' {(isBallonLiveSensor ? $"with serial number '{blsSerialNumber}'" : "")}");
                    }
                    else
                    {
                        Log(LogSeverityType.Warning, $"Caution: Change of position source detected at '{timeStamp:dd-MMM-yyyy HH\\:mm\\:ss}' :  '{(isPrimarySource ? "primary" : "fallback")}' source '{(isBallonLiveSensor ? "Ballon Live Sensor" : "Phone Internal")}' {(isBallonLiveSensor ? $"with serial number '{blsSerialNumber}'" : "")}");
                        if (!track.AdditionalPropertiesFromIGCFile.ContainsKey("Change of position source"))
                            track.AdditionalPropertiesFromIGCFile.Add("Change of position source", "yes");
                    }
                }
                if (!track.AdditionalPropertiesFromIGCFile.ContainsKey("Change of position source"))
                    track.AdditionalPropertiesFromIGCFile.Add("Change of position source", "no");

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
                        //Debug.WriteLine(functionErrorMessage + "Failed to parse trackpoint");
                        Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse trackpoint");
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
                        //Debug.WriteLine(functionErrorMessage + "Failed to parse marker drop");
                        Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse marker drop");
                        return false;
                    }
                    track.MarkerDrops.Add(markerDrop);
                }

                Coordinate referenceCoordinate = null;
                if (track.MarkerDrops.Count > 0)
                {
                    MarkerDrop markerDrop = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
                    if (markerDrop != null)
                        referenceCoordinate = markerDrop.MarkerLocation;
                    else
                    {
                        referenceCoordinate = track.MarkerDrops[0].MarkerLocation;
                        //Debug.WriteLine($"No marker 1 dropped. Marker '{track.MarkerDrops[0].MarkerNumber}' will be used as goal declaration reference");
                        Log(LogSeverityType.Warning, $"No marker 1 dropped. Marker '{track.MarkerDrops[0].MarkerNumber}' will be used as goal declaration reference");
                    }
                }
                else
                {
                    //Debug.WriteLine("No marker drops found. Position at declaration will be used as reference instead");
                    Log(LogSeverityType.Warning, "No marker drops found. Position at declaration will be used as reference instead");
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
                        //Debug.WriteLine(functionErrorMessage + "Failed to parse goal declaration");
                        Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse goal declaration");
                        return false;
                    }
                    if (declaration != null)
                        track.Declarations.Add(declaration);
                }

                Pilot pilot = new Pilot(pilotNumber, pilotIdentifier);
                track.Pilot = pilot;
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $" {ex.Message}");
                return false;
            }
            Log(LogSeverityType.Info, $"Successfully parsed file '{fileNameAndPath}'");
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
                    int day;
                    if (!int.TryParse(line[0..2], out day))
                    {
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse day portion of date '{line[0..2]}' in '{line}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse day portion of date '{line[0..2]}' in '{line}'");
                        return false;
                    }
                    int month;
                    if (!int.TryParse(line[2..4], out month))
                    {
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
                        return false;
                    }
                    int year;
                    if (!int.TryParse(line[4..6], out year))
                    {
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse year portion of date '{line[4..6]}' in '{line}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse year portion of date '{line[4..6]}' in '{line}'");
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
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse the pilot number '{pilotNumberText}' in '{headerLine}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse the pilot number '{pilotNumberText}' in '{headerLine}'");
                        return false;
                    }
                }
                if (headerLine.StartsWith("HFXII:XX0:"))
                {
                    int numberOfAdditions;
                    if (!int.TryParse(headerLine[10..12], out numberOfAdditions))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + " Failed to parse number of additions from I-record");
                        return false;
                    }
                    int offset = 12;
                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        offset = index * 7 + 12;
                        int startPosition;
                        if (!int.TryParse(headerLine[offset..(offset + 2)], out startPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse start position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        int stopPosition;
                        if (!int.TryParse(headerLine[(offset)..(offset + 2)], out stopPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse stop position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = headerLine[(offset)..(offset + 3)];

                        if (additionIdentifier.ToUpper() == "LAD")
                        {
                            markerDrop_HasAdditionalLatitudeDecimals = true;
                            markerDrop_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            markerDrop_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.ToUpper() == "LOD")
                        {
                            markerDrop_HasAdditionalLongitudeDecimals = true;
                            markerDrop_StartOfAdditionalLongitudeDecimals = startPosition - 1;//adjust to zero based index
                            markerDrop_EndOfAdditionalLongitudeDecimals = stopPosition;
                        }
                    }

                }
                if (headerLine.StartsWith("HFXII:XL1:"))
                {
                    int numberOfAdditions;
                    if (!int.TryParse(headerLine[10..12], out numberOfAdditions))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + " Failed to parse number of additions from I-record");
                        return false;
                    }
                    int offset = 12;
                    for (int index = 0; index < numberOfAdditions; index++)
                    {
                        offset = index * 7 + 12;
                        int startPosition;
                        if (!int.TryParse(headerLine[offset..(offset + 2)], out startPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse start position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        int stopPosition;
                        if (!int.TryParse(headerLine[(offset)..(offset + 2)], out stopPosition))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse stop position of addition no {index + 1}");
                            return false;
                        }
                        offset += 2;
                        string additionIdentifier = headerLine[(offset)..(offset + 3)];

                        if (additionIdentifier.ToUpper() == "LAD")
                        {
                            declaration_HasAdditionalLatitudeDecimals = true;
                            declaration_StartOfAdditionalLatitudeDecimals = startPosition - 1;//adjust to zero based index
                            declaration_EndOfAdditionalLatitudeDecimals = stopPosition;
                        }
                        if (additionIdentifier.ToUpper() == "LOD")
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
            string functionErrorMessage = $"Failed to parse track point:";
            coordinate = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }

            double latitude;
            if (trackPoint_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[7..15], line[trackPoint_StartOfAdditionalLatitudeDecimals..trackPoint_EndOfAdditionalLatitudeDecimals], out latitude))
                {
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[7..15], out latitude))
                {
                    //Debug.WriteLine(functionErrorMessage);
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return false;
                }
            }
            double longitude;
            if (trackPoint_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[15..24], line[trackPoint_StartOfAdditionalLongitudeDecimals..trackPoint_EndOfAdditionalLongitudeDecimals], out longitude))
                {
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[15..24], out longitude))
                {
                    //Debug.WriteLine(functionErrorMessage);
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return false;
                }
            }




            double altitudeBarometric;
            if (!double.TryParse(line[25..30], out altitudeBarometric))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude '{line[25..30]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[25..30]}' in '{line}'");
                return false;
            }

            double altitudeGPS;
            if (!double.TryParse(line[30..35], out altitudeGPS))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude '{line[30..35]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse GPS altitude '{line[30..35]}' in '{line}'");
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
            string functionErrorMessage = $"Failed to parse goal declaration:";
            declaration = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            int goalNumber;
            if (!int.TryParse(line[10..12], out goalNumber))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal number '{line[10..12]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal number '{line[10..12]}' in '{line}'");
                return false;
            }

            double declarationLatitude;
            if (declaration_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[12..20], line[declaration_StartOfAdditionalLatitudeDecimals..declaration_EndOfAdditionalLatitudeDecimals], out declarationLatitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse latitude at declaration position in '{line}'");
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[12..20], out declarationLatitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse latitude at declaration position  in '{line}'");
                    return false;
                }
            }
            double declarationLongitude;
            if (declaration_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[20..29], line[declaration_StartOfAdditionalLongitudeDecimals..declaration_EndOfAdditionalLongitudeDecimals], out declarationLongitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse longitude at declaration position  in '{line}'");
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[20..29], out declarationLongitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse longitude at declaration position  in '{line}'");
                    return false;
                }
            }

            double declarationPositonAltitudeBarometric;
            if (!double.TryParse(line[30..35], out declarationPositonAltitudeBarometric))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude at declaration position '{parts[2][18..23]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude at declaration position in '{line}'");
                return false;
            }

            double declarationPositionAltitudeGPS;
            if (!double.TryParse(line[35..40], out declarationPositionAltitudeGPS))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse GPS altitude at declaration position '{parts[2][23..28]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse GPS altitude at declaration position in '{line}'");
                return false;
            }
            string declarationText = line[43..^0];
            if (declarationText.Length > 2)
            {

                string[] parts = declarationText.Split(',');
                string[] locations = parts[0].Split('/');
                int eastingDigits = -1;
                int eastingUTM = -1;
                int northingDigits = -1;
                int northingUTM = -1;
                if (locations.Length == 2)
                {
                    if (!string.IsNullOrWhiteSpace(locations[0]))
                    {
                        eastingDigits = locations[0].Length;
                        if (!int.TryParse(locations[0], out eastingUTM))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse easting of declared goal in '{declarationText}'");
                            return false;
                        }
                    }
                    else
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse easting of declared goal in '{declarationText}'");
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(locations[1]))
                    {
                        northingDigits = locations[1].Length;
                        if (!int.TryParse(locations[1], out northingUTM))
                        {
                            Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse northing of declared goal in '{declarationText}'");
                            return false;
                        }
                    }
                    else
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse northing of declared goal in '{declarationText}'");
                        return false;
                    }

                }
                else
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $" Failed to parse easting and northing of declared goal in '{declarationText}'");
                    return false;
                }

                bool hasPilotDelaredGoalAltitude = true;
                double declaredAltitudeInMeter;
                if (parts.Length == 2)
                {
                    if (!string.IsNullOrWhiteSpace(parts[1]))
                    {
                        string altitudePart = parts[1].Replace("ft", "").Replace("m", "");
                        int declaredAltitude;
                        if (!int.TryParse(altitudePart, out declaredAltitude))
                        {
                            //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration altitude portion '{parts[1]}' in '{line}'");
                            Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration altitude portion '{altitudePart}' in '{line}'");
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
                        Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Default goal altitude of '{defaultGoalAltitude}' will be assumed");
                    }
                }
                else
                {
                    hasPilotDelaredGoalAltitude = false;
                    declaredAltitudeInMeter = defaultGoalAltitude;
                    Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Default goal altitude of '{defaultGoalAltitude}' will be assumed");
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
                        northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
                    }

                    if (eastingDigits != 6)
                    {
                        goalEastingUTM = eastingUTM * 10;
                        goalEastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));

                    }

                    CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator(utmGridZone, goalEastingUTM, goalNorthingUTM);

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
                        northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
                    }

                    if (eastingDigits != 6)
                    {
                        goalEastingUTM = eastingUTM * 10;
                        goalEastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));

                    }

                    CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator(utmGridZone, goalEastingUTM, goalNorthingUTM);

                    CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

                    declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);
                    positionAtDeclaration = new Coordinate(declarationLatitude, declarationLongitude, declarationPositionAltitudeGPS, declarationPositonAltitudeBarometric, timeStamp);
                    double distance = CoordinateHelpers.Calculate2DDistanceHavercos(declaredGoal, positionAtDeclaration);
                    if (distance > 70e3)
                    {

                        Log(LogSeverityType.Warning, $"Suspicious declaration of gaol {goalNumber}: {locations[0]}/{locations[1]}");
                    }
                }

                declaration = new Declaration(goalNumber, declaredGoal, positionAtDeclaration);
            }
            else
            {
                Log(LogSeverityType.Warning, "Declaration contained no coordinates and therefore has not been added to the track");
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
            string functionErrorMessage = $"Failed to parse marker drop:";
            markerDrop = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            int markerNumber;
            if (!int.TryParse(line[10..12], out markerNumber))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker number '{line[10..12]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker number '{line[10..12]}' in '{line}'");
                return false;
            }

            double latitude;
            if (markerDrop_HasAdditionalLatitudeDecimals)
            {
                if (!ParseLatitudeWithAdditionalDecimals(line[12..20], line[markerDrop_StartOfAdditionalLatitudeDecimals..markerDrop_EndOfAdditionalLatitudeDecimals], out latitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                    return false;
                }
            }
            else
            {
                if (!ParseLatitude(line[12..20], out latitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                    return false;
                }
            }
            double longitude;
            if (markerDrop_HasAdditionalLongitudeDecimals)
            {
                if (!ParseLongitudeWithAdditionalDecimals(line[20..29], line[markerDrop_StartOfAdditionalLongitudeDecimals..markerDrop_EndOfAdditionalLongitudeDecimals], out longitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                    return false;
                }
            }
            else
            {
                if (!ParseLongitude(line[20..29], out longitude))
                {
                    //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                    return false;
                }
            }

            double altitudeBarometric;
            if (!double.TryParse(line[30..35], out altitudeBarometric))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude '{line[30..35]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[30..35]}' in '{line}'");
                return false;
            }

            double altitudeGPS;
            if (!double.TryParse(line[35..40], out altitudeGPS))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude '{line[35..40]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[35..40]}' in '{line}'");
                return false;
            }
            Coordinate coordinate = new Coordinate(latitude, longitude, altitudeGPS, altitudeBarometric, timeStamp);
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
                factor = double.NaN;
                //Debug.WriteLine($"Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(latitudeText[0..2], out fullAngle))
            {
                //Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{latitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(latitudeText[2..7], out decimalAngle))
            {
                //Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{latitudeText}'");
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
                factor = double.NaN;
                //Debug.WriteLine($"Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text. Unexpected suffix '{standardLatitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(standardLatitudeText[0..2], out fullAngle))
            {
                //Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{standardLatitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(standardLatitudeText[2..7] + additionalDecimals, out decimalAngle))
            {
                //Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{standardLatitudeText}'");
                return false;
            }
            if (decimalAngle > 0.0)
            {
                //decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal angles
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
                factor = double.NaN;
                //Debug.WriteLine($"Failed to parse longitude text. Unexpected suffix '{longitudeText[^1]}'");
                Log(LogSeverityType.Error, $"Failed to parse longitude text. Unexpected suffix '{longitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(longitudeText[0..3], out fullAngle))
            {
                //Debug.WriteLine($"Failed to parse longitude text '{longitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse longitude text '{longitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(longitudeText[3..8], out decimalAngle))
            {
                //Debug.WriteLine($"Failed to parse longitude text '{longitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse longitude text '{longitudeText}'");
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
                factor = double.NaN;
                //Debug.WriteLine($"Failed to parse standardLongitude text. Unexpected suffix '{standardLongitudeText[^1]}'");
                Log(LogSeverityType.Error, $"Failed to parse standardLongitude text. Unexpected suffix '{standardLongitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(standardLongitudeText[0..3], out fullAngle))
            {
                //Debug.WriteLine($"Failed to parse standardLongitude text '{standardLongitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse standardLongitude text '{standardLongitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(standardLongitudeText[3..8] + additinalDecimals, out decimalAngle))
            {
                //Debug.WriteLine($"Failed to parse standardLongitude text '{standardLongitudeText}'");
                Log(LogSeverityType.Error, $"Failed to parse standardLongitude text '{standardLongitudeText}'");
                return false;
            }
            //decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal angles
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
            string functionErrorMessage = "Failed to parse time:";
            string time = line[1..7];
            int hours;
            timeStamp = date;
            if (!int.TryParse(time[0..2], out hours))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse hour portion '{time[0..2]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse hour portion '{time[0..2]}' in '{line}'");
                return false;
            }
            int minutes;
            if (!int.TryParse(time[2..4], out minutes))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse minute portion '{time[2..4]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse minute portion '{time[2..4]}' in '{line}'");
                return false;
            }
            int seconds;
            if (!int.TryParse(time[4..6], out seconds))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse second portion '{time[4..6]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse second portion '{time[4..6]}' in '{line}'");
                return false;
            }
            timeStamp = date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return true;
        }

        private static void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log((object)"Balloon Live Parser", logSeverity, text);
        }


        private static bool ParseSourceEvent(string line, DateTime date, out DateTime timeStamp, out bool isPrimarySource, out bool isBallonLiveSensor, out string blsSerialNumber)
        {
            string functionErrorMessage = "Failed to parse position source event";
            timeStamp = DateTime.MinValue;
            isPrimarySource = false;
            isBallonLiveSensor = false;
            blsSerialNumber = string.Empty;

            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
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
                Log(LogSeverityType.Warning, $"Unknown source event '{line[7..10]}' in line '{line}'");
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
                Log(LogSeverityType.Warning, $"Unknown position source '{line[10..13]}' in line '{line}'");
            }

            return true;

        }


        ///// <summary>
        ///// Parses a line with a goal declaration (EttttttXL1ddññññ*/ëëëë*,hhhh#,nnnnnnnNeeeeeeeeEbbbbbgggggaaassddd0000)
        ///// <para>where t:timestamp d:number of declared goal ñ:goal northing in utm ë:goal easting in utm (*:the exact format in specified in the header at 'LXXX declaration digits') h:declared height (#: the unit is specified in the header)
        ///// <para>n:northing e:easting b:barometric altitude g:gps altitude</para>
        ///// <para>a:accuracy s:number of satellites d: engine noise level and rpm 0:carrier return and line feed</para>
        ///// <para>e.g E105850XL101123456/987654,1500,4839658N00858176EA0000000537000224940000</para>
        ///// </summary>
        ///// <param name="line">the line in the file</param>
        ///// <param name="date">the date form the header</param>
        ///// <param name="declaredAltitudeIsInFeet">true: declared height is in feet; false: declared height is in meter</param>
        ///// <param name="northingDigits">expected number of digits for goal northing</param>
        ///// <param name="eastingDigits">expected number of digits for goal easting</param>
        ///// <param name="referenceCoordinate">a reference coordinate to fill up the missing info from utm goal declaration. If the reference is null, the position of declaration will be used instead</param>
        ///// <param name="declaredGoal">output parameter. the declared goal</param>
        ///// <returns>true:success; false:error</returns>
        //private static bool ParseGoalDeclaration(string line, DateTime date, bool declaredAltitudeIsInFeet, int northingDigits, int eastingDigits, Coordinate referenceCoordinate, out DeclaredGoal declaredGoal)
        //{
        //    string functionErrorMessage = $"Failed to parse goal declaration:";
        //    declaredGoal = null;

        //    DateTime timeStamp;
        //    if (!ParseTimeStamp(line, date, out timeStamp))
        //    {
        //        //Debug.WriteLine(functionErrorMessage);
        //        Log(LogSeverityType.Error, functionErrorMessage);
        //        return false;
        //    }
        //    int goalNumber;
        //    if (!int.TryParse(line[10..12], out goalNumber))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal number '{line[10..12]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal number '{line[10..12]}' in '{line}'");
        //        return false;
        //    }
        //    int eastingUTM;
        //    //int eastingStartCharacter = northingStartCharacter + northingDigits + 1;
        //    int eastingStartCharacter = 12;
        //    if (!int.TryParse(line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)], out eastingUTM))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration easting portion '{line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration easting portion '{line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)]}' in '{line}'");
        //        return false;
        //    }
        //    int northingUTM;
        //    //int northingStartCharacter = 12;
        //    int northingStartCharacter = eastingStartCharacter + eastingDigits + 1;
        //    if (!int.TryParse(line[northingStartCharacter..(northingStartCharacter + northingDigits)], out northingUTM))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration northing portion '{line[northingStartCharacter..(northingStartCharacter + northingDigits)]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration northing portion '{line[northingStartCharacter..(northingStartCharacter + northingDigits)]}' in '{line}'");
        //        return false;
        //    }
        //    string[] parts = line.Split(',');
        //    int declarationPartIndex = 2;
        //    double declaredAltitudeInMeter;
        //    if (parts.Length == 2)
        //    {
        //        if (!string.IsNullOrWhiteSpace(parts[1]))
        //        {
        //            string altitudePart = parts[1].Replace("ft", "").Replace("m", "");
        //            int declaredAltitude;
        //            if (!int.TryParse(altitudePart, out declaredAltitude))
        //            {
        //                //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration altitude portion '{parts[1]}' in '{line}'");
        //                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration altitude portion '{altitudePart}' in '{line}'");
        //                return false;
        //            }
        //            if (declaredAltitudeIsInFeet)
        //                declaredAltitudeInMeter = CoordinateHelpers.ConvertToMeter((double)declaredAltitude);
        //            else
        //                declaredAltitudeInMeter = (double)declaredAltitude;
        //        }
        //        else
        //        {
        //            declaredAltitudeInMeter = 0.0;
        //            Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
        //        }
        //    }
        //    else
        //    {
        //        declaredAltitudeInMeter = 0.0;
        //        Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
        //        declarationPartIndex = 1;
        //    }
        //    double declarationLatitude;
        //    if (!ParseLatitude(parts[declarationPartIndex][0..8], out declarationLatitude))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
        //        return false;
        //    }
        //    double declarationLongitude;
        //    if (!ParseLongitude(parts[declarationPartIndex][8..17], out declarationLongitude))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
        //        return false;
        //    }

        //    double declarationPositonAltitudeBarometric;
        //    if (!double.TryParse(parts[declarationPartIndex][18..23], out declarationPositonAltitudeBarometric))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude at declaration position '{parts[2][18..23]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude at declaration position '{parts[2][18..23]}' in '{line}'");
        //        return false;
        //    }

        //    double declarationPositionAltitudeGPS;
        //    if (!double.TryParse(parts[declarationPartIndex][23..28], out declarationPositionAltitudeGPS))
        //    {
        //        //Debug.WriteLine(functionErrorMessage + $"Failed to parse GPS altitude at declaration position '{parts[2][23..28]}' in '{line}'");
        //        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse GPS altitude at declaration position '{parts[2][23..28]}' in '{line}'");
        //        return false;
        //    }

        //    CoordinateSharp.Coordinate coordinateSharp;
        //    if (referenceCoordinate == null)
        //        coordinateSharp = new CoordinateSharp.Coordinate(declarationLatitude, declarationLongitude);
        //    else
        //        coordinateSharp = new CoordinateSharp.Coordinate(referenceCoordinate.Latitude, referenceCoordinate.Longitude);

        //    string utmGridZone = coordinateSharp.UTM.LatZone + coordinateSharp.UTM.LongZone;
        //    if (northingDigits < 6)
        //    {
        //        northingUTM *= 10;
        //        northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits + 1)) * Math.Pow(10, northingDigits + 1));
        //    }
        //    if (northingDigits == 6)
        //    {
        //        northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
        //    }

        //    if (eastingDigits != 6)
        //    {
        //        eastingUTM *= 10;
        //        eastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));
        //    }

        //    CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator(utmGridZone, eastingUTM, northingUTM);

        //    CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

        //    Coordinate goalDeclared = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);
        //    Coordinate positionAtDeclaration = new Coordinate(declarationLatitude, declarationLongitude, declarationPositionAltitudeGPS, declarationPositonAltitudeBarometric, timeStamp);

        //    declaredGoal = new DeclaredGoal(goalNumber, goalDeclared, positionAtDeclaration);

        //    return true;
        //}
        #endregion

    }
}
