﻿using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Coordinates.Parsers
{
    public static class FAILoggerParser
    {
        #region API

        /// <summary>
        /// Parses a .igc file and pass back a track object
        /// </summary>
        /// <param name="fileNameAndPath">the file path and name of the .igc file</param>
        /// <param name="track">output parameter. the parsed track from the file</param>
        /// <param name="referenceCoordinate">provide a reference coordinate to be used complete the missing information of a goal declaration
        /// <para>a goal declaration not in the zone 6/7 format is ambiguous</para>
        /// <para>this is an optional parameter, the parse will use either marker drop 1 or the position at declaration if not reference point has been provided</para></param>
        /// <returns>true:success; false:error</returns>
        public static bool ParseFile(string fileNameAndPath, out Track track, Coordinate referenceCoordinate = null)
        {

            string functionErrorMessage = $"Failed to parse file '{fileNameAndPath}':";
            track = null;

            FileInfo fileInfo = new FileInfo($@"{fileNameAndPath}");
            if (!fileInfo.Exists)
            {
                //Debug.WriteLine(functionErrorMessage + $"The file '{fileNameAndPath}' does not exists");
                Log(LogSeverityType.Error, functionErrorMessage + $"The file '{fileNameAndPath}' does not exists");
                return false;
            }

            if (!fileInfo.Extension.EndsWith("igc"))
            {
                
                Log(LogSeverityType.Error, functionErrorMessage + $"The file extension '{fileInfo.Extension}' is not supported");
                return false;
            }

            track = new Track();
            int pilotNumber = -1;
            string pilotIdentifier = "";
            DateTime date = new DateTime();
            int goalNortingDigits = -1;
            int goalEastingDigits = -1;
            bool declaredAltitudeIsInFeet = true;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader($@"{fileNameAndPath}"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lines.Add(line);
                }
            }
            string[] identifierLine = lines.Where(x => x.StartsWith("AXXX")).ToArray();
            if (identifierLine.Length == 0)
            {
                
                Log(LogSeverityType.Error, "Line with Pilot Identifier 'AXXX' is missing");
                return false;
            }
            if (identifierLine.Length > 1)
            {
                
                Log(LogSeverityType.Error, "More the one line with Pilot Identifier is found. First occurrence will be used.");
            }
            pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("Balloon Competition Logger", "");

            string[] headerLines = lines.Where(x => x.StartsWith('H')).ToArray();
            foreach (string headerLine in headerLines)
            {
                if (headerLine.StartsWith("HFDTE"))
                {
                    string line = headerLine.Replace("HFDTE", "");
                    int day;
                    if (!int.TryParse(line[0..2], out day))
                    {
                        
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse day portion of date '{line[0..2]}' in '{line}'");
                        return false;
                    }
                    int month;
                    if (!int.TryParse(line[2..4], out month))
                    {
                        
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
                        return false;
                    }
                    int year;
                    if (!int.TryParse(line[4..6], out year))
                    {
                       
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
                        
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse the pilot number '{pilotNumberText}' in '{headerLine}'");
                        return false;
                    }
                }
            }

            string[] configLines = lines.Where(x => x.StartsWith("LXXX")).ToArray();
            foreach (string configLine in configLines)
            {
                if (configLine.StartsWith("LXXX declaration digits"))
                {
                    int digits;
                    if (!int.TryParse(configLine[^1..^0], out digits))
                    {
                        
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse number of declaration digits '{configLine[^1..^0]}' in '{configLine}'");
                        return false;
                    }

                    goalNortingDigits = digits / 2;
                    goalEastingDigits = digits / 2;
                    if (digits % 2 == 1)//if digits are odd, expect that northing will declared with one digits more
                    {
                        goalNortingDigits++;
                    }
                }
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
            }

            string[] trackPointLines = lines.Where(x => x.StartsWith('B')).ToArray();
            foreach (string trackPointLine in trackPointLines)
            {
                Coordinate coordinate;
                if (!ParseTrackPoint(trackPointLine, date, out coordinate))
                {
                    
                    Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse trackpoint");
                    return false;
                }
                track.TrackPoints.Add(coordinate);
            }

            string[] markerDropLines = lines.Where(x => x.StartsWith('E') && x.Contains("XX0")).ToArray();
            foreach (string markerDropLine in markerDropLines)
            {
                MarkerDrop markerDrop;
                if (!ParseMarkerDrop(markerDropLine, date, out markerDrop))
                {
                    
                    Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse marker drop");
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
            }
            string[] goalDeclarationLines = lines.Where(x => x.StartsWith('E') && x.Contains("XX1")).ToArray();
            foreach (string goalDeclarationLine in goalDeclarationLines)
            {
                int index = lines.FindIndex(x => x == goalDeclarationLine);
                Coordinate positionAtDeclaration = null;
                if (index != -1)
                {
                    do
                    {
                        index--;
                        if (lines[index].StartsWith('B'))
                        {
                            if (!ParseTrackPoint(lines[index], date, out positionAtDeclaration))
                            {
                                
                                Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse track point as position of declaration");
                                return false;
                            }
                            index = 0;
                            break;
                        }
                    }
                    while (index > 0);
                }
                if (positionAtDeclaration == null)
                {
                    
                    Log(LogSeverityType.Error, functionErrorMessage + "Failed to find a trackpoint as position of declaration");
                    return false;
                }
                Declaration declaration;
                if (!ParseGoalDeclaration(goalDeclarationLine, date, declaredAltitudeIsInFeet, goalNortingDigits, goalEastingDigits, referenceCoordinate, positionAtDeclaration, out declaration))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse goal declaration");
                    return false;
                }
                track.Declarations.Add(declaration);
            }
            //using (StreamReader reader = new StreamReader(fileNameAndPath))
            //{
            //    while (!reader.EndOfStream)
            //    {
            //        string line = reader.ReadLine();
            //        if (string.IsNullOrEmpty(line))
            //            continue;
            //        char lineIdenticator = line[0];
            //        switch (lineIdenticator)
            //        {
            //            case 'A':
            //                pilotIdentifier = line.Replace("AXXX", "").Replace("BalloonLive", "");
            //                break;
            //            case 'H':
            //                if (line.StartsWith("HFDTE"))
            //                {
            //                    line = line.Replace("HFDTE", "");
            //                    int day;
            //                    if (!int.TryParse(line[0..2], out day))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse day portion of date '{line[0..2]}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse day portion of date '{line[0..2]}' in '{line}'");
            //                        return false;
            //                    }
            //                    int month;
            //                    if (!int.TryParse(line[2..4], out month))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
            //                        return false;
            //                    }
            //                    int year;
            //                    if (!int.TryParse(line[4..6], out year))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse year portion of date '{line[4..6]}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse year portion of date '{line[4..6]}' in '{line}'");
            //                        return false;
            //                    }
            //                    year += 2000;
            //                    date = new DateTime(year, month, day);
            //                }
            //                if (line.StartsWith("HFPID"))
            //                {
            //                    string pilotNumberText = line.Replace("HFPID", "");
            //                    if (!int.TryParse(pilotNumberText, out pilotNumber))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse the pilot number '{pilotNumberText}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse the pilot number '{pilotNumberText}' in '{line}'");
            //                        return false;
            //                    }
            //                }
            //                break;
            //            case 'L':
            //                if (line.StartsWith("LXXX declaration digits"))
            //                {
            //                    if (!int.TryParse(line[^3..^2], out goalNortingDigits))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration northing digits '{line[^3..^2]}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration northing digits '{line[^3..^2]}' in '{line}'");
            //                        return false;
            //                    }
            //                    if (!int.TryParse(line[^1..^0], out goalEastingDigits))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration easting digits '{line[^1..^0]}' in '{line}'");
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration easting digits '{line[^1..^0]}' in '{line}'");
            //                        return false;
            //                    }
            //                }
            //                if (line.StartsWith("LXXX alt unit"))
            //                {
            //                    if (line.Contains("feet"))
            //                    {
            //                        declaredAltitudeIsInFeet = true;
            //                    }
            //                    else
            //                    {
            //                        declaredAltitudeIsInFeet = false;
            //                    }
            //                }

            //                break;
            //            case 'B':
            //                Coordinate coordinate;
            //                if (!ParseTrackPoint(line, date, out coordinate))
            //                {
            //                    Debug.WriteLine(functionErrorMessage + "Failed to parse trackpoint");
            //                    Debug.WriteLine(functionErrorMessage + "Failed to parse trackpoint");
            //                    return false;
            //                }
            //                track.TrackPoints.Add(coordinate);
            //                break;
            //            case 'E':
            //                if (line.Contains("XL1"))
            //                {
            //                    DeclaredGoal declaredGoal;
            //                    if (!ParseGoalDeclaration(line, date,declaredAltitudeIsInFeet, goalNortingDigits, goalEastingDigits, out declaredGoal))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + "Failed to parse goal declaration");
            //                        Debug.WriteLine(functionErrorMessage + "Failed to parse goal declaration");
            //                        return false;
            //                    }
            //                    track.DeclaredGoals.Add(declaredGoal);
            //                }
            //                if (line.Contains("XX0"))
            //                {
            //                    MarkerDrop markerDrop;
            //                    if (!ParseMarkerDrop(line, date, out markerDrop))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + "Failed to parse marker drop");
            //                        Debug.WriteLine(functionErrorMessage + "Failed to parse marker drop");
            //                        return false;
            //                    }
            //                    track.MarkerDrops.Add(markerDrop);
            //                }
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}
            Pilot pilot = new Pilot(pilotNumber, pilotIdentifier);
            track.Pilot = pilot;
            return true;
        }
        #endregion

        #region Private methods


        /// <summary>
        /// Parses a line containing a track point (BttttttnnnnnnnNeeeeeeeeEAbbbbbgggggaaassddd0000)
        /// <para>where t:timestamp n:northing e:easting b:barometric altitude g:gps altitude</para>
        /// <para>a:accuracy s:number of satellites d: engine noise level and rpm 0:carrier return and line feed</para>
        /// <para> e.g. B1058394839658N00858176EA0000000537000225940000</para>
        /// </summary>
        /// <param name="line">the line in the file</param>
        /// <param name="date">the date from the header</param>
        /// <param name="coordinate">output parameter. a trackpoint as coordinate object</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseTrackPoint(string line, DateTime date, out Coordinate coordinate)
        {
            string functionErrorMessage = $"Failed to parse track point:";
            coordinate = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }

            double latitude;
            if (!ParseLatitude(line[7..15], out latitude))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            double longitude;
            if (!ParseLongitude(line[15..24], out longitude))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }


            double altitudeBarometric;
            if (!double.TryParse(line[25..30], out altitudeBarometric))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[25..30]}' in '{line}'");
                return false;
            }

            double altitudeGPS;
            if (!double.TryParse(line[30..35], out altitudeGPS))
            {
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
        /// <param name="northingDigits">expected number of digits for goal northing</param>
        /// <param name="eastingDigits">expected number of digits for goal easting</param>
        /// <param name="referenceCoordinate">a reference coordinate to fill up the missing info from utm goal declaration. If the reference is null, the position of declaration will be used instead</param>
        /// <param name="declaration">output parameter. the declared goal</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseGoalDeclaration(string line, DateTime date, bool declaredAltitudeIsInFeet, int northingDigits, int eastingDigits, Coordinate referenceCoordinate, Coordinate positionAtDeclaration, out Declaration declaration)
        {
            string functionErrorMessage = $"Failed to parse goal declaration:";
            declaration = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            int goalNumber;
            if (!int.TryParse(line[10..12], out goalNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal number '{line[10..12]}' in '{line}'");
                return false;
            }
            int originalEastingDeclarationUTM;
            //int eastingStartCharacter = northingStartCharacter + northingDigits + 1;
            int eastingStartCharacter = 12;
            if (!int.TryParse(line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)], out originalEastingDeclarationUTM))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration easting portion '{line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)]}' in '{line}'");
                return false;
            }
            int originalNorthingDeclarationUTM;
            //int northingStartCharacter = 12;
            int northingStartCharacter = eastingStartCharacter + eastingDigits + 1;
            if (!int.TryParse(line[northingStartCharacter..(northingStartCharacter + northingDigits)], out originalNorthingDeclarationUTM))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration northing portion '{line[northingStartCharacter..(northingStartCharacter + northingDigits)]}' in '{line}'");
                return false;
            }
            string[] parts = line.Split(',');
            int declaredAltitude;
            double declaredAltitudeInMeter;
            bool hasPilotDeclaredGoalAltitude = true;
            if (parts.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(parts[1]))
                {
                    parts[1] = parts[1].Replace("ft", "").Replace("m", "");

                    if (!int.TryParse(parts[1], out declaredAltitude))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration altitude portion '{parts[1]}' in '{line}'");
                        return false;
                    }
                    if (declaredAltitudeIsInFeet)
                        declaredAltitudeInMeter = CoordinateHelpers.ConvertToMeter((double)declaredAltitude);
                    else
                        declaredAltitudeInMeter = (double)declaredAltitude;
                }
                else
                {
                    hasPilotDeclaredGoalAltitude = false;
                    declaredAltitudeInMeter = 0.0;
                    Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
                }
            }
            else
            {
                hasPilotDeclaredGoalAltitude = false;
                declaredAltitudeInMeter = 0.0;
                Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
            }


            CoordinateSharp.Coordinate coordinateSharp;
            if (referenceCoordinate == null)
                coordinateSharp = new CoordinateSharp.Coordinate(positionAtDeclaration.Latitude, positionAtDeclaration.Longitude);
            else
                coordinateSharp = new CoordinateSharp.Coordinate(referenceCoordinate.Latitude, referenceCoordinate.Longitude);

            string utmGridZone = coordinateSharp.UTM.LatZone + coordinateSharp.UTM.LongZone;
            int northingUTM = originalNorthingDeclarationUTM;
            int eastingUTM = originalEastingDeclarationUTM;
            if (northingDigits < 6)
            {
                northingUTM *= 10;
                northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits + 1)) * Math.Pow(10, northingDigits + 1));
            }
            if (northingDigits == 6)
            {
                northingUTM += (int)(Math.Floor(coordinateSharp.UTM.Northing / Math.Pow(10, northingDigits)) * Math.Pow(10, northingDigits));
            }

            if (eastingDigits != 6)
            {
                eastingUTM *= 10;
                eastingUTM += (int)(Math.Floor(coordinateSharp.UTM.Easting / Math.Pow(10, eastingDigits + 1)) * Math.Pow(10, eastingDigits + 1));
            }

            CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator(utmGridZone, originalEastingDeclarationUTM, originalNorthingDeclarationUTM);

            CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

            Coordinate declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);

            declaration = new Declaration(goalNumber, declaredGoal, positionAtDeclaration, hasPilotDeclaredGoalAltitude, originalEastingDeclarationUTM, originalNorthingDeclarationUTM);

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
        private static bool ParseMarkerDrop(string line, DateTime date, out MarkerDrop markerDrop)
        {
            string functionErrorMessage = $"Failed to parse marker drop:";
            markerDrop = null;

            DateTime timeStamp;
            if (!ParseTimeStamp(line, date, out timeStamp))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            int markerNumber;
            if (!int.TryParse(line[10..12], out markerNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker number '{line[10..12]}' in '{line}'");
                return false;
            }

            double latitude;
            if (!ParseLatitude(line[12..20], out latitude))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                return false;
            }
            double longitude;
            if (!ParseLongitude(line[20..29], out longitude))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                return false;
            }

            double altitudeBarometric;
            if (!double.TryParse(line[30..35], out altitudeBarometric))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[30..35]}' in '{line}'");
                return false;
            }

            double altitudeGPS;
            if (!double.TryParse(line[35..40], out altitudeGPS))
            {
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
                Log(LogSeverityType.Error, $"Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(latitudeText[0..2], out fullAngle))
            {
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{latitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(latitudeText[2..7], out decimalAngle))
            {
                Log(LogSeverityType.Error, $"Failed to parse latitude text '{latitudeText}'");
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal angles

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
                Log(LogSeverityType.Error, $"Failed to parse longitude text. Unexpected suffix '{longitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(longitudeText[0..3], out fullAngle))
            {
                Log(LogSeverityType.Error, $"Failed to parse longitude text '{longitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(longitudeText[3..8], out decimalAngle))
            {
                Log(LogSeverityType.Error, $"Failed to parse longitude text '{longitudeText}'");
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decimal angles

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
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse hour portion '{time[0..2]}' in '{line}'");
                return false;
            }
            int minutes;
            if (!int.TryParse(time[2..4], out minutes))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse minute portion '{time[2..4]}' in '{line}'");
                return false;
            }
            int seconds;
            if (!int.TryParse(time[4..6], out seconds))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse second portion '{time[4..6]}' in '{line}'");
                return false;
            }
            timeStamp = date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return true;
        }

        private static void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log((object)"FAI Logger Parser", logSeverity, text);
        }
        #endregion
    }
}
