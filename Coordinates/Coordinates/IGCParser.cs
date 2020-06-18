using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;

namespace Coordinates
{
    public static class IGCParser
    {
        public static bool ParseFile(string fileNameAndPath, out Track track)
        {
            //TODO make method async?
            track = null;

            FileInfo fileInfo = new FileInfo(fileNameAndPath);
            if (!fileInfo.Exists)
            {
                Debug.WriteLine($"The file '{fileNameAndPath}' does not exsits");
                return false;
            }

            if (!fileInfo.Extension.EndsWith("igc"))
            {
                Debug.WriteLine($"The file extension '{fileInfo.Extension}' is not supported");
                return false;
            }

            track = new Track();
            int pilotNumber=-1;
            string pilotIdentifier="";
            DateTime date=new DateTime();
            using (StreamReader reader = new StreamReader(fileNameAndPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    char lineIdenticator = line[0];
                    switch (lineIdenticator)
                    {
                        case 'A':
                            pilotIdentifier = line.Replace("AXXX", "").Replace("BalloonLive", "");
                            break;
                        case 'H':
                            if (line.StartsWith("HFDTE"))
                            {
                                line = line.Replace("HFDTE", "");
                                int day;
                                if (!int.TryParse(line[0..2], out day))
                                {
                                    Debug.WriteLine("Failed to parse day portion of date. Please check the HFDTE part of the header");
                                    return false;
                                }
                                int month;
                                if (!int.TryParse(line[2..4], out month))
                                {
                                    Debug.WriteLine("Failed to parse month portion of date. Please check the HFDTE part of the header");
                                    return false;
                                }
                                int year;
                                if (!int.TryParse(line[4..6], out year))
                                {
                                    Debug.WriteLine("Failed to parse year portion of date. Please check the HFDTE part of the header");
                                    return false;
                                }
                                year += 2000;
                                date = new DateTime(year,month,day);
                            }
                            if (line.StartsWith("HFPID"))
                            {
                                line = line.Replace("HFPID", "");
                                if (!int.TryParse(line, out pilotNumber))
                                {
                                    Debug.WriteLine("Failed to parse the pilot number. Please check the HFPID part of the header");
                                    return false;
                                }
                            }
                            break;
                        case 'B':
                            Coordinate coordinate;
                            if (!ParseTrackPoint(line, date, out coordinate))
                            {
                                Debug.WriteLine("Failed to parse trackpoint");
                                return false;
                            }
                            track.TrackPoints.Add(coordinate);
                            break;
                        case 'E':
                            break;
                        default:
                            break;
                    }
                }
            }
            Pilot pilot = new Pilot(pilotNumber, new List<string> { pilotIdentifier });
            track.Pilot = pilot;
            return true;
        }

        private static bool ParseTrackPoint(string line, DateTime date,out Coordinate coordinate)
        {
            string functionErrorMessage = $"Failed to parse track point form 'line'";
            coordinate=null;

            string time = line[1..7];
            int hours;
            if (!int.TryParse(time[0..2], out hours))
            {
                Debug.WriteLine(functionErrorMessage+$"Failed to parse hour portion in '{line}'");
                return false;
            }
            int minutes;
            if (!int.TryParse(time[2..4], out minutes))
            {
                Debug.WriteLine(functionErrorMessage+$"Failed to parse minute portion in '{line}'");
                return false;
            }
            int seconds;
            if (!int.TryParse(time[4..6], out seconds))
            {
                Debug.WriteLine(functionErrorMessage+$"Failed to parse second portion in '{line}'");
                return false;
            }
            DateTime timeStamp = date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            double latitude;
            if (!ParseLatitude(line[7..15], out latitude))
            {
                Debug.WriteLine(functionErrorMessage);
                return false;
            }
            double longitude;
            if(!ParseLongitude(line[15..24], out longitude))
            {
                Debug.WriteLine(functionErrorMessage);
                return false;
            }

            //TODO check A/V (A=3D fix;V=2D fix)
            double altitudeBaro;
            if (!double.TryParse(line[26..31], out altitudeBaro))
            {
                Debug.WriteLine(functionErrorMessage+$"Failed to parse barometric altitude in '{line}'");
                return false;
            }

            double altitudeGPS;
            if (!double.TryParse(line[31..36], out altitudeGPS))
            {
                Debug.WriteLine(functionErrorMessage + $"Failed to parse barometric altitude in '{line}'");
                return false;
            }
            coordinate = new Coordinate(latitude, longitude, altitudeGPS, altitudeBaro, timeStamp);
            return true;
        }
        private static bool ParseLatitude(string latitudeText,out double latitude)
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
                Debug.WriteLine($"Failed to parse latitude text. Unexpected suffix '{latitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(latitudeText[0..2], out fullAngle))
            {
                Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(latitudeText[2..7], out decimalAngle))
            {
                Debug.WriteLine($"Failed to parse latitude text '{latitudeText}'");
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decial angles

            latitude = factor * (fullAngle + decimalAngle);
            return true;
        }

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
                Debug.WriteLine($"Failed to parse longitude text. Unexpected suffix '{longitudeText[^1]}'");
                return false;
            }
            double fullAngle = double.NaN;
            if (!double.TryParse(longitudeText[0..3], out fullAngle))
            {
                Debug.WriteLine($"Failed to parse longitude text '{longitudeText}'");
                return false;
            }
            double decimalAngle = double.NaN;
            if (!double.TryParse(longitudeText[3..8], out decimalAngle))
            {
                Debug.WriteLine($"Failed to parse longitude text '{longitudeText}'");
                return false;
            }
            decimalAngle /= 60000.0;//divided by 1000 to get decimal value, divided by 60 to get from angle minutes to decial angles

            longitude = factor * (fullAngle + decimalAngle);
            return true;
        }

    }
}
