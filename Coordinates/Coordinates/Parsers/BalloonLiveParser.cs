using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
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
        /// <returns>true:success; false:error</returns>
        public static bool ParseFile(string fileNameAndPath, out Track track)
        {
            //TODO make method async?

            string functionErrorMessage = $"Failed to parse file '{fileNameAndPath}':";
            track = null;

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
            int goalNortingDigits = -1;
            int goalEastingDigits = -1;
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
            string[] identifierLine = lines.Where(x => x.StartsWith("AXXX")).ToArray();
            if (identifierLine.Length == 0)
            {
                //Debug.WriteLine("Line with Pilot Identifier 'AXXX' is missing");
                Log(LogSeverityType.Error, "Line with Pilot Identifier 'AXXX' is missing");
                return false;
            }
            if (identifierLine.Length > 1)
            {
                //Debug.WriteLine("More the one line with Pilot Identifier is found. First occurrence will be used.");
                Log(LogSeverityType.Warning, "More the one line with Pilot Identifier is found. First occurrence will be used.");
            }
            pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("BalloonLive", "");

            string[] headerLines = lines.Where(x => x.StartsWith('H')).ToArray();
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
            }

            string[] configLines = lines.Where(x => x.StartsWith("LXXX")).ToArray();
            foreach (string configLine in configLines)
            {
                if (configLine.StartsWith("LXXX declaration digits"))
                {
                    if (!int.TryParse(configLine[^3..^2], out goalNortingDigits))
                    {
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration northing digits '{configLine[^3..^2]}' in '{configLine}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration northing digits '{configLine[^3..^2]}' in '{configLine}'");
                        return false;
                    }
                    if (!int.TryParse(configLine[^1..^0], out goalEastingDigits))
                    {
                        //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration easting digits '{configLine[^1..^0]}' in '{configLine}'");
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration easting digits '{configLine[^1..^0]}' in '{configLine}'");
                        return false;
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
                if (!ParseMarkerDrop(markerDropLine, date, out markerDrop))
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

                DeclaredGoal declaredGoal;
                if (!ParseGoalDeclaration(goalDeclarationLine, date, declaredAltitudeIsInFeet, goalNortingDigits, goalEastingDigits, referenceCoordinate, out declaredGoal))
                {
                    //Debug.WriteLine(functionErrorMessage + "Failed to parse goal declaration");
                    Log(LogSeverityType.Error, functionErrorMessage + "Failed to parse goal declaration");
                    return false;
                }
                track.DeclaredGoals.Add(declaredGoal);
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
            //                        return false;
            //                    }
            //                    int month;
            //                    if (!int.TryParse(line[2..4], out month))
            //                    {
            //                        Debug.WriteLine(functionErrorMessage + $"Failed to parse month portion of date '{line[2..4]}' in '{line}'");
            //                        return false;
            //                    }
            //                    int year;
            //                    if (!int.TryParse(line[4..6], out year))
            //                    {
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
            //                        return false;
            //                    }
            //                    if (!int.TryParse(line[^1..^0], out goalEastingDigits))
            //                    {
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
            Pilot pilot = new Pilot(pilotNumber,  pilotIdentifier);
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
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }

            double latitude;
            if (!ParseLatitude(line[7..15], out latitude))
            {
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
            }
            double longitude;
            if (!ParseLongitude(line[15..24], out longitude))
            {
                //Debug.WriteLine(functionErrorMessage);
                Log(LogSeverityType.Error, functionErrorMessage);
                return false;
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
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude '{line[30..35]}' in '{line}'");
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
        /// <param name="declaredGoal">output parameter. the declared goal</param>
        /// <returns>true:success; false:error</returns>
        private static bool ParseGoalDeclaration(string line, DateTime date, bool declaredAltitudeIsInFeet, int northingDigits, int eastingDigits, Coordinate referenceCoordinate, out DeclaredGoal declaredGoal)
        {
            string functionErrorMessage = $"Failed to parse goal declaration:";
            declaredGoal = null;

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
            int eastingUTM;
            //int eastingStartCharacter = northingStartCharacter + northingDigits + 1;
            int eastingStartCharacter = 12;
            if (!int.TryParse(line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)], out eastingUTM))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration easting portion '{line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration easting portion '{line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)]}' in '{line}'");
                return false;
            }
            int northingUTM;
            //int northingStartCharacter = 12;
            int northingStartCharacter = eastingStartCharacter + eastingDigits + 1;
            if (!int.TryParse(line[northingStartCharacter..(northingStartCharacter + northingDigits)], out northingUTM))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse goal declaration northing portion '{line[northingStartCharacter..(northingStartCharacter + northingDigits)]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse goal declaration northing portion '{line[northingStartCharacter..(northingStartCharacter + northingDigits)]}' in '{line}'");
                return false;
            }
            string[] parts = line.Split(',');
            int declarationPartIndex=2;
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
                    declaredAltitudeInMeter = 0.0;
                    Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
                }
            }
            else
            {
                declaredAltitudeInMeter = 0.0;
                Log(LogSeverityType.Warning, $"No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed");
                declarationPartIndex = 1;
            }
            double declarationLatitude;
            if (!ParseLatitude(parts[declarationPartIndex][0..8], out declarationLatitude))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse latitude at declaration position '{parts[2][0..8]}' in '{line}'");
                return false;
            }
            double declarationLongitude;
            if (!ParseLongitude(parts[declarationPartIndex][8..17], out declarationLongitude))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse longitude at declaration position '{parts[2][8..17]}' in '{line}'");
                return false;
            }

            double declarationPositonAltitudeBarometric;
            if (!double.TryParse(parts[declarationPartIndex][18..23], out declarationPositonAltitudeBarometric))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude at declaration position '{parts[2][18..23]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse barometric altitude at declaration position '{parts[2][18..23]}' in '{line}'");
                return false;
            }

            double declarationPositionAltitudeGPS;
            if (!double.TryParse(parts[declarationPartIndex][23..28], out declarationPositionAltitudeGPS))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse GPS altitude at declaration position '{parts[2][23..28]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse GPS altitude at declaration position '{parts[2][23..28]}' in '{line}'");
                return false;
            }

            CoordinateSharp.Coordinate coordinateSharp;
            if (referenceCoordinate == null)
                coordinateSharp = new CoordinateSharp.Coordinate(declarationLatitude, declarationLongitude);
            else
                coordinateSharp = new CoordinateSharp.Coordinate(referenceCoordinate.Latitude, referenceCoordinate.Longitude);

            string utmGridZone = coordinateSharp.UTM.LatZone + coordinateSharp.UTM.LongZone;
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

            CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator(utmGridZone, eastingUTM, northingUTM);

            CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

            Coordinate goalDeclared = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);
            Coordinate positionAtDeclaration = new Coordinate(declarationLatitude, declarationLongitude, declarationPositionAltitudeGPS, declarationPositonAltitudeBarometric, timeStamp);

            declaredGoal = new DeclaredGoal(goalNumber, goalDeclared, positionAtDeclaration);

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
            if (!ParseLatitude(line[12..20], out latitude))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop latitude '{line[12..20]}' in '{line}'");
                return false;
            }
            double longitude;
            if (!ParseLongitude(line[20..29], out longitude))
            {
                //Debug.WriteLine(functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse marker drop longitude '{line[20..29]}' in '{line}'");
                return false;
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

        #endregion

    }
}
