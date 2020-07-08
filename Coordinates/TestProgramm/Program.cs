using System;
using Coordinates;
namespace TestProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            Track trackOriginal;
            if (!IGCParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\E94BC98E-001-20611105838 - Original.igc", out trackOriginal))
            {
                Console.WriteLine("Error parsing track");
            }

            double distance2D = CoordinateHelpers.CalculateDistance2D(trackOriginal.TrackPoints[100], trackOriginal.TrackPoints[101]);
            Console.WriteLine("2D Distance:"+distance2D);
            double distance3D = CoordinateHelpers.CalculateDistance3D(trackOriginal.TrackPoints[100], trackOriginal.TrackPoints[101],true);
            Console.WriteLine("3D Distance:" + distance3D);

            //Track track4x4;
            //if (!IGCParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\E94BC98E-001-20611105838 - 4x4.igc", out track4x4))
            //{
            //    Console.WriteLine("Error parsing track");
            //}

            //double latitude = trackOriginal.DeclaredGoals[1].GoalDeclared.Latitude;
            //double longitude = trackOriginal.DeclaredGoals[1].GoalDeclared.Longitude;
            //CoordinateSharp.Coordinate coordinate = new CoordinateSharp.Coordinate(latitude,longitude);
            //string gridZone = coordinate.UTM.LongZone + coordinate.UTM.LatZone;
            //Console.WriteLine("Long/Lat (min):" + coordinate);
            //Console.WriteLine("Long/Lat (degree):" + latitude + "N " + longitude + "E");
            //Console.WriteLine("UTM:" + gridZone + coordinate.UTM.Northing + "N " + coordinate.UTM.Easting + "E");

            Console.ReadLine();
        }
    }
}
