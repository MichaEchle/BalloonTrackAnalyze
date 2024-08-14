using Coordinates.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coordinates.Parsers
{
    public static class FAILoggerParser
    {
        private static ILogger Logger
        {
            get
            {
                return ServiceConfiguration.LoggerFactory.CreateLogger(nameof(FAILoggerParser));
            }
        }

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
            track = null;

            FileInfo fileInfo = new($@"{fileNameAndPath}");
            if (!fileInfo.Exists)
            {
                Logger?.LogError("Failed to parse file '{fileNameAndPath}': the file does not exists", fileNameAndPath);
                return false;
            }

            if (!fileInfo.Extension.EndsWith("igc"))
            {
                Logger?.LogError("Failed to parse file '{fileNameAndPath}': the file extension '{fileInfo.Extension}' is not supported", fileNameAndPath, fileInfo.Extension);
                return false;
            }

            track = new Track();
            int pilotNumber = -1;
            string pilotIdentifier = "";
            DateTime date = new();
            int goalNortingDigits = -1;
            int goalEastingDigits = -1;
            bool declaredAltitudeIsInFeet = true;
            List<string> lines = [];
            using (StreamReader reader = new($@"{fileNameAndPath}"))
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
                Logger?.LogError("Line with Pilot Identifier 'AXXX' is missing");
                return false;
            }
            if (identifierLine.Length > 1)
            {
                Logger?.LogError("More the one line with Pilot Identifier were found. First occurrence will be used");
            }
            pilotIdentifier = identifierLine[0].Replace("AXXX", "").Replace("Balloon Competition Logger", "");

            string[] headerLines = lines.Where(x => x.StartsWith('H')).ToArray();
            foreach (string headerLine in headerLines)
            {
                if (headerLine.StartsWith("HFDTE"))
                {
                    string line = headerLine.Replace("HFDTE", "");
                    if (!int.TryParse(line[0..2], out int day))
                    {
                        Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the day portion for date form '{portion}' in line '{line}'", fileNameAndPath, line[0..2], line);
                        return false;
                    }
                    if (!int.TryParse(line[2..4], out int month))
                    {
                        Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the month portion for date form '{portion}' in line '{line}'", fileNameAndPath, line[2..4], line);
                        return false;
                    }
                    if (!int.TryParse(line[4..6], out int year))
                    {
                        Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the year portion for date form '{portion}' in line '{line}'", fileNameAndPath, line[4..6], line);
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
                        Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the pilot number '{pilotNumberText}' in line '{headerLine}'", fileNameAndPath, pilotNumberText, headerLine);
                        return false;
                    }
                }
            }

            string[] configLines = lines.Where(x => x.StartsWith("LXXX")).ToArray();
            foreach (string configLine in configLines)
            {
                if (configLine.StartsWith("LXXX declaration digits"))
                {
                    if (!int.TryParse(configLine[^1..^0], out int digits))
                    {

                        Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the number of declaration digits '{declarationDigits}' in line '{configLine}'", fileNameAndPath, configLine[^1..^0], configLine);
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
                if (!ParseTrackPoint(trackPointLine, date, out Coordinate coordinate))
                {
                    Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the trackpoint '{trackPointLine}'", fileNameAndPath, trackPointLine);
                    return false;
                }
                track.TrackPoints.Add(coordinate);
            }

            string[] markerDropLines = lines.Where(x => x.StartsWith('E') && x.Contains("XX0")).ToArray();
            foreach (string markerDropLine in markerDropLines)
            {
                if (!ParseMarkerDrop(markerDropLine, date, out MarkerDrop markerDrop))
                {
                    Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the marker drop '{markerDropLine}'", fileNameAndPath, markerDropLine);
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
                        Logger?.LogWarning("No marker 1 dropped. Marker '{track.MarkerDrops[0].MarkerNumber}' will be used as goal declaration reference", track.MarkerDrops[0].MarkerNumber);
                    }
                }
                else
                {
                    Logger?.LogWarning("No marker drops found. Position at declaration will be used as reference instead");
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
                                Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the trackpoint as position of declaration '{lines[index]}'", fileNameAndPath, lines[index]);
                                return false;
                            }
                            index = 0;
                            break;
                        }
                    }
                    while (index > 0);
                }
                if (positionAtDeclaration is null)
                {
                    Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot find a trackpoint as position of declaration", fileNameAndPath);
                    return false;
                }
                if (!ParseGoalDeclaration(goalDeclarationLine, date, declaredAltitudeIsInFeet, goalNortingDigits, goalEastingDigits, referenceCoordinate, positionAtDeclaration, out Declaration declaration))
                {
                    Logger?.LogError("Failed to parse file '{fileNameAndPath}': cannot parse the goal declaration '{goalDeclarationLine}'", fileNameAndPath, goalDeclarationLine);
                    return false;
                }
                track.Declarations.Add(declaration);
            }
            Pilot pilot = new(pilotNumber, pilotIdentifier);
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
            coordinate = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse track point: cannot parse the time stamp in line '{line}'", line);
                return false;
            }

            if (!ParseLatitude(line[7..15], out double latitude))
            {
                Logger?.LogError("Failed to parse track point: cannot parse the latitude portion '{portion}' in line '{line}'", line[7..15], line);
                return false;
            }
            if (!ParseLongitude(line[15..24], out double longitude))
            {
                Logger?.LogError("Failed to parse track point: cannot parse the longitude portion '{portion}' in line '{line}'", line[15..24], line);
                return false;
            }


            if (!double.TryParse(line[25..30], out double altitudeBarometric))
            {
                Logger?.LogError("Failed to parse track point: cannot parse the barometric altitude portion '{portion}' in line '{line}'", line[25..30], line);
                return false;
            }

            if (!double.TryParse(line[30..35], out double altitudeGPS))
            {
                Logger?.LogError("Failed to parse track point: cannot parse the GPS altitude portion '{portion}' in line '{line}'", line[30..35], line);
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
            declaration = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse goal declaration: cannot parse the time stamp in line '{line}'", line);
                return false;
            }
            if (!int.TryParse(line[10..12], out int goalNumber))
            {
                Logger?.LogError("Failed to parse goal declaration: cannot parse the goal number portion '{portion}' in line '{line}'", line[10..12], line);
                return false;
            }
            //int eastingStartCharacter = northingStartCharacter + northingDigits + 1;
            int eastingStartCharacter = 12;
            if (!int.TryParse(line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)], out int originalEastingDeclarationUTM))
            {
                Logger?.LogError("Failed to parse goal declaration: cannot parse the easting portion '{portion}' in line '{line}'", line[eastingStartCharacter..(eastingStartCharacter + eastingDigits)], line);
                return false;
            }
            //int northingStartCharacter = 12;
            int northingStartCharacter = eastingStartCharacter + eastingDigits + 1;
            if (!int.TryParse(line[northingStartCharacter..(northingStartCharacter + northingDigits)], out int originalNorthingDeclarationUTM))
            {
                Logger?.LogError("Failed to parse goal declaration: cannot parse the northing portion '{portion}' in line '{line}'", line[northingStartCharacter..(northingStartCharacter + northingDigits)], line);
                return false;
            }
            string[] parts = line.Split(',');
            double declaredAltitudeInMeter;
            bool hasPilotDeclaredGoalAltitude = true;
            if (parts.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(parts[1]))
                {
                    parts[1] = parts[1].Replace("ft", "").Replace("m", "");

                    if (!int.TryParse(parts[1], out int declaredAltitude))
                    {
                        Logger?.LogError("Failed to parse goal declaration: cannot parse the altitude portion '{portion}' in line '{line}'", parts[1], line);
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
                    Logger?.LogWarning("No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed", goalNumber);
                }
            }
            else
            {
                hasPilotDeclaredGoalAltitude = false;
                declaredAltitudeInMeter = 0.0;
                Logger?.LogWarning("No altitude declared for Goal No. '{goalNumber}'. Altitude of 0 will be assumed", goalNumber);
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

            CoordinateSharp.UniversalTransverseMercator utm = new(utmGridZone, eastingUTM, northingUTM);

            CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

            Coordinate declaredGoal = new(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, declaredAltitudeInMeter, declaredAltitudeInMeter, timeStamp);

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
            markerDrop = null;

            if (!ParseTimeStamp(line, date, out DateTime timeStamp))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the time stamp in line '{line}'", line);
                return false;
            }
            if (!int.TryParse(line[10..12], out int markerNumber))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the marker number portion '{portion}' in line '{line}'", line[10..12], line);
                return false;
            }

            if (!ParseLatitude(line[12..20], out double latitude))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the latitude portion '{portion}' in line '{line}'", line[12..20], line);
                return false;
            }
            if (!ParseLongitude(line[20..29], out double longitude))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the longitude portion '{portion}' in line '{line}'", line[20..29], line);
                return false;
            }

            if (!double.TryParse(line[30..35], out double altitudeBarometric))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the barometric altitude portion '{portion}' in line '{line}'", line[30..35], line);
                return false;
            }

            if (!double.TryParse(line[35..40], out double altitudeGPS))
            {
                Logger?.LogError("Failed to parse marker drop: cannot parse the GPS altitude portion '{portion}' in line '{line}'", line[35..40], line);
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
                Logger?.LogError("Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'", latitudeText[^1]);
                return false;
            }
            if (!double.TryParse(latitudeText[0..2], out double fullAngle))
            {
                Logger?.LogError("Failed to parse latitude full angle portion '{portion}' from '{latitudeText}'", latitudeText[0..2], latitudeText);
                return false;
            }
            if (!double.TryParse(latitudeText[2..7], out double decimalAngle))
            {
                Logger?.LogError("Failed to parse latitude decimal angle portion '{portion}' from '{latitudeText}'", latitudeText[2..7], latitudeText);
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
                Logger?.LogError("Failed to parse longitude text. Unexpected suffix '{longitudeText[^1]}'", longitudeText[^1]);
                return false;
            }
            if (!double.TryParse(longitudeText[0..3], out double fullAngle))
            {
                Logger?.LogError("Failed to parse longitude full angle portion '{portion}' from '{longitudeText}'", longitudeText[0..3], longitudeText);
                return false;
            }
            if (!double.TryParse(longitudeText[3..8], out double decimalAngle))
            {
                Logger?.LogError("Failed to parse longitude decimal angle portion '{portion}' from '{longitudeText}'", longitudeText[3..8], longitudeText);
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
            string time = line[1..7];
            timeStamp = date;
            if (!int.TryParse(time[0..2], out int hours))
            {
                Logger?.LogError("Failed to parse hour portion '{portion}' in line '{line}'", time[0..2], line);
                return false;
            }
            if (!int.TryParse(time[2..4], out int minutes))
            {
                Logger?.LogError("Failed to parse minute portion '{portion}' in line '{line}'", time[2..4], line);
                return false;
            }
            if (!int.TryParse(time[4..6], out int seconds))
            {
                Logger?.LogError("Failed to parse second portion '{portion}' in line '{line}'", time[4..6], line);
                return false;
            }
            timeStamp = date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return true;
        }
        #endregion
    }
}
