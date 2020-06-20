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

            Track track4x4;
            if (!IGCParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\E94BC98E-001-20611105838 - 4x4.igc", out track4x4))
            {
                Console.WriteLine("Error parsing track");
            }

            double latitude = trackOriginal.DeclaredGoals[1].GoalDeclared.Latitude;
            double longitude = trackOriginal.DeclaredGoals[1].GoalDeclared.Longitude;
            CoordinateSharp.Coordinate coordinate = new CoordinateSharp.Coordinate(latitude,longitude);
            string gridZone = coordinate.UTM.LongZone + coordinate.UTM.LatZone;
            Console.WriteLine("Long/Lat (min):" + coordinate);
            Console.WriteLine("Long/Lat (degree):" + latitude + "N " + longitude + "E");
            Console.WriteLine("UTM:" + gridZone + coordinate.UTM.Northing + "N " + coordinate.UTM.Easting + "E");

            Console.ReadLine();
        }
    }
}
