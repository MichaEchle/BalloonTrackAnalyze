using Coordinates;
using System;
using System.Collections.Generic;
using System.IO;

namespace JansScoring.plt;

public class PLTParser
{
    public static List<Coordinate> parse(string filePath)
    {
        List<Coordinate> points = new List<Coordinate>();

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("  "))
                    {
                        continue;
                    }

                    line = line.Replace(" ", "");
                    string[] parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 4 &&
                        double.TryParse(parts[0].Replace(".",","), out double latitude) &&
                        double.TryParse(parts[1].Replace(".",","), out double longitude) &&
                        double.TryParse(parts[3].Replace(".",","), out double altitude))
                    {
                        (string utmZone, int easting, int northing) =
                            CoordinateHelpers.ConvertLatitudeLongitudeToUTM(latitude, longitude);
                        Coordinate coordinate = new Coordinate(latitude, longitude, altitude, altitude, new DateTime());
                        coordinate.utmZone = utmZone;
                        coordinate.easting = easting;
                        coordinate.northing = northing;
                        points.Add(coordinate);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading PLT file: {ex.Message}");
        }

        return points;
    }
}