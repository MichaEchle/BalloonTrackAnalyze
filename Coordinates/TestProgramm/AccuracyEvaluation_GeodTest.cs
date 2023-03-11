using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace TestProgramm
{
    internal static class AccuracyEvaluation_GeodTest
    {
        internal record GeodTestRecord
        {

            internal double Latitude1
            {
                get; init;
            }

            internal double Longitude1
            {
                get; init;
            }

            internal double Azimuth1
            {
                get; init;
            }

            internal double Latitude2
            {
                get; init;
            }

            internal double Longitude2
            {
                get; init;
            }

            internal double Azimuth2
            {
                get; init;
            }

            internal double Geodesic_Distance
            {
                get; init;
            }
            internal double Arc_Distance
            {
                get; init;
            }
            internal double Reduced_Length
            {
                get; init;
            }
            internal double Area_Geodesic_Equator
            {
                get; init;
            }

        }

        internal static bool ParseGeodTestData(out List<GeodTestRecord> geodTestRecords)
        {
            geodTestRecords = new List<GeodTestRecord>();
            using (StreamReader reader = new StreamReader(@".\GeodTest\GeodTest.dat"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(' ');

                    if (parts.Length != 10)
                    {
                        Console.WriteLine($"Failed to parse line {line} in GeodTest.dat");
                        reader.Close();
                        return false;
                    }
                    double latitude1;//degrees, exact
                    if (!double.TryParse(parts[0], out latitude1))
                    {
                        Console.WriteLine($"Failed to parse latitude 1 from {parts[0]}");
                        return false;
                    }
                    double longitude1;//degrees, always 0
                    if (!double.TryParse(parts[1], out longitude1))
                    {
                        Console.WriteLine($"Failed to parse longitude from {parts[1]}");
                        return false;
                    }
                    double azimuth1;//degrees, clockwise from north, exact
                    if (!double.TryParse(parts[2], out azimuth1))
                    {
                        Console.WriteLine($"Failed to parse azimuth 1 from {parts[2]}");
                        return false;
                    }
                    double latitude2; //degrees, accurate to 1e-18 deg
                    if (!double.TryParse(parts[3], out latitude2))
                    {
                        Console.WriteLine($"Failed to parse latitude 2 from {parts[3]}");
                        return false;
                    }
                    double longitude2;//degrees, accurate to 1e-18 deg
                    if (!double.TryParse(parts[4], out longitude2))
                    {
                        Console.WriteLine($"Failed to parse longitude 2 from {parts[4]}");
                        return false;
                    }
                    double azimuth2;//degrees, accurate to 1e-18 deg
                    if (!double.TryParse(parts[5], out azimuth2))
                    {
                        Console.WriteLine($"Failed to parse azimuth 2 from {parts[5]}");
                        return false;
                    }
                    double geodesic_distance;//meters, exact
                    if (!double.TryParse(parts[6], out geodesic_distance))
                    {
                        Console.WriteLine($"Failed to parse geodesic distance from {parts[6]}");
                        return false;
                    }
                    double arc_distance;//degrees,accurate to 1e-18 deg
                    if (!double.TryParse(parts[7], out arc_distance))
                    {
                        Console.WriteLine($"Failed to parse arc distance from {parts[7]}");
                        return false;
                    }
                    double reduced_length;//meters, accurate to 1e-13 m
                    if (!double.TryParse(parts[8], out reduced_length))
                    {
                        Console.WriteLine($"Failed to parse reduced length from {parts[8]}");
                        return false;
                    }
                    double area_geodesic_equator;//meters², accurate to 1e-3 m²
                    if (!double.TryParse(parts[9], out area_geodesic_equator))
                    {
                        Console.WriteLine($"Failed to parse area between geodesic and equator from {parts[9]}");
                        return false;
                    }
                    GeodTestRecord geodTestData = new()
                    {
                        Latitude1 = latitude1,
                        Longitude1 = longitude1,
                        Azimuth1 = azimuth1,
                        Latitude2 = latitude2,
                        Longitude2 = longitude2,
                        Azimuth2 = azimuth2,
                        Geodesic_Distance = geodesic_distance,
                        Arc_Distance = arc_distance,
                        Reduced_Length = reduced_length,
                        Area_Geodesic_Equator = area_geodesic_equator
                    };
                    geodTestRecords.Add(geodTestData);
                }
            }
            return true;
        }

        internal static void CalculateDistances()
        {
            if (ParseGeodTestData(out List<GeodTestRecord> geodTestRecords))
            {
                geodTestRecords = geodTestRecords.OrderBy(x => x.Geodesic_Distance).ToList();
                Stopwatch stopwatch = Stopwatch.StartNew();
                //using (StreamWriter writer = new StreamWriter(@"C:\Temp\DistAlgoAcc.txt"))
                //{
                //    writer.WriteLine("Lat1\tLong1\tLat2\tLong2\tRef Dist\t Dist Havercos\t Error Havercos\t Error Havercos rel\t Dist Haversin\tError Haversin\tError Haversin rel\tDist Vincenty\tError Vincenty\tError Vincenty rel\tDist UTM\tError UTM\tError UTM rel\tDist UTM prec.\tError UTM prec.\tError UTM prec. rel");
                    for (int index = 0; index < 10000; index++)
                    {
                        GeodTestRecord record = geodTestRecords[index];
                        Coordinates.Coordinate coordinate1 = new Coordinates.Coordinate(record.Latitude1, record.Longitude1, 0, 0, DateTime.MinValue);
                        Coordinates.Coordinate coordinate2 = new Coordinates.Coordinate(record.Latitude2, record.Longitude2, 0, 0, DateTime.MinValue);

                        //double distance_havercos = Math.Round(Coordinates.CoordinateHelpers.Calculate2DDistanceHavercos(coordinate1, coordinate2), 3, MidpointRounding.AwayFromZero);
                        //double error_havercos = Math.Round(record.Geodesic_Distance - distance_havercos, 3, MidpointRounding.AwayFromZero);
                        //double error_havercos_rel = Math.Round(error_havercos / record.Geodesic_Distance * 100, 2, MidpointRounding.AwayFromZero);
                        //double distance_haversin = Math.Round(Coordinates.CoordinateHelpers.Calculate2DDistanceHaversin(coordinate1, coordinate2), 3, MidpointRounding.AwayFromZero);
                        //double error_haversin = Math.Round(record.Geodesic_Distance - distance_haversin, 3, MidpointRounding.AwayFromZero);
                        //double error_haversin_rel = Math.Round(error_haversin / record.Geodesic_Distance * 100, 2, MidpointRounding.AwayFromZero);
                        //double distance_vincenty = Math.Round(Coordinates.CoordinateHelpers.Calculate2DDistanceVincentyWSG84(coordinate1, coordinate2), 3, MidpointRounding.AwayFromZero);
                        //double error_vincenty = Math.Round(record.Geodesic_Distance - distance_vincenty, 3, MidpointRounding.AwayFromZero);
                        //double error_vincenty_rel = Math.Round(error_vincenty / record.Geodesic_Distance * 100, 2, MidpointRounding.AwayFromZero);
                        //double distance_UTM = Math.Round(Coordinates.CoordinateHelpers.Calculate2DDistanceUTM(coordinate1, coordinate2), 3, MidpointRounding.AwayFromZero);
                        //double error_UTM = Math.Round(record.Geodesic_Distance - distance_UTM, 3, MidpointRounding.AwayFromZero);
                        //double error_UTM_rel = Math.Round(error_UTM / record.Geodesic_Distance * 100, 2, MidpointRounding.AwayFromZero);
                        double distance_UTM_precise = Math.Round(Coordinates.CoordinateHelpers.Calculate2DDistanceUTM_Precise(coordinate1, coordinate2), 3, MidpointRounding.AwayFromZero);
                        //double error_UTM_precise = Math.Round(record.Geodesic_Distance - distance_UTM_precise, 3, MidpointRounding.AwayFromZero);
                        //double error_UTM_precise_rel = Math.Round(error_UTM_precise / record.Geodesic_Distance * 100, 2, MidpointRounding.AwayFromZero);

                        //writer.WriteLine($"{record.Latitude1}\t{record.Longitude1}\t{record.Latitude2}\t{record.Longitude2}\t{Math.Round(record.Geodesic_Distance, 3, MidpointRounding.AwayFromZero)}\t{distance_havercos}\t{error_havercos}\t{error_havercos_rel}\t{distance_haversin}\t{error_haversin}\t{error_haversin_rel}\t{distance_vincenty}\t{error_vincenty}\t{error_vincenty_rel}\t{distance_UTM}\t{error_UTM}\t{error_UTM_rel}\t{distance_UTM_precise}\t{error_UTM_precise}\t{error_UTM_precise_rel}");

                        //Console.WriteLine($"ref distance {Math.Round(record.Geodesic_Distance, 6, MidpointRounding.AwayFromZero)}\t\t  havercos {Math.Round(distance_havercos, 6, MidpointRounding.AwayFromZero)} (error {Math.Round(record.Geodesic_Distance - distance_havercos, 6, MidpointRounding.AwayFromZero)}) \t\t vincenty {Math.Round(distance_vincenty, 6, MidpointRounding.AwayFromZero)} (error {Math.Round(record.Geodesic_Distance - distance_vincenty, 6, MidpointRounding.AwayFromZero)})");
                    }
                    stopwatch.Stop();
                Console.WriteLine($"Haversin: {stopwatch.Elapsed:mm\\:ss\\.fff}");
                //}
            }
        }

    }
}
