using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Competition;
using Coordinates;
using Coordinates.Parsers;

namespace TestProgramm
{
    class Program
    {
        static void Main(string[] args)
        {
            //Track trackOriginal;
            //if (!BallonLiveParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\E94BC98E-001-20611105838 - Original.igc", out trackOriginal))
            //{
            //    Console.WriteLine("Error parsing track");
            //}

            //double distance2D = CoordinateHelpers.CalculateDistance2D(trackOriginal.TrackPoints[100], trackOriginal.TrackPoints[101]);
            //Console.WriteLine("2D Distance:"+distance2D);
            //double distance3D = CoordinateHelpers.CalculateDistance3D(trackOriginal.TrackPoints[100], trackOriginal.TrackPoints[101],true);
            //Console.WriteLine("3D Distance:" + distance3D);


            Track faiLogger;
            if (!FAILoggerParser.ParseFile(@"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\5AD_f003_p002_l0.igc", out faiLogger))
            {
                Console.WriteLine("Error parsing FAI logger track");
            }
            CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator("32U", 566700, 5488900);
            CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            Coordinate declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, double.NaN, double.NaN, DateTime.Now);
            faiLogger.DeclaredGoals.Add(new DeclaredGoal(2, declaredGoal, declaredGoal));

            DonutTask donut = new DonutTask();
            donut.GoalNumber = 2;
            donut.InnerRadius = 250;
            donut.OuterRadius = 750;
            donut.UpperBoundary = double.NaN;
            donut.IsReentranceAllowed = false;
            donut.LowerBoundary = CoordinateHelpers.ConvertToMeter(3200);
            donut.DeclarationValidationRules = new List<IDeclarationValidationRules>();

            double result;
            if (!donut.CalculateResults(faiLogger, true, out result))
            {
                Console.WriteLine("Failed to calculate result");
            }
            else
            {
                Console.WriteLine("Result:"+result);
            }

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
