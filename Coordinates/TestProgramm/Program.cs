using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using Competition;
using Coordinates;
using Coordinates.Parsers;
using Newtonsoft.Json;

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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //string trackFileName = @"C:\Users\Micha\Source\repos\BalloonTrackAnalyze\TestTrack\5AD_f003_p002_l0.igc";
            string trackFileName = @"E:\BLC2021\SynologyDrive\tracks\Flight 1\work\E[BLC21]F[1]P[20]-6DD95BC6-020.igc";
            Track track;
            if (!BalloonLiveParser.ParseFile(trackFileName, out track))
            {
                Console.WriteLine("Error parsing logger track");
            }

            string reportFileName = Path.Combine(Path.GetDirectoryName(trackFileName) , Path.GetFileNameWithoutExtension(trackFileName)+".xlsx");

            TrackReport.GenerateTrackReport(reportFileName, track,true);
            stopwatch.Stop();

            Console.WriteLine($"Time for parsing track file and generate report: {stopwatch.Elapsed:mm\\:ss}");
            //DonutTask donutTask = new DonutTask();

            //donutTask.SetupDonut(-1, 2, 1, 300, 500, 50, double.NaN, true, null);

            //string donut=JsonConvert.SerializeObject(donutTask,Formatting.Indented);

            //using (StreamWriter writer = new StreamWriter(@"./donutConfig.json"))
            //{
            //    writer.Write(donut);
            //}

            CoordinateSharp.UniversalTransverseMercator utm = new CoordinateSharp.UniversalTransverseMercator("32U", 567000, 5489000);
            CoordinateSharp.Coordinate coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);

            Console.WriteLine(coordinate.Latitude);
            Console.WriteLine(ToProperText(coordinate.Latitude));

            //int[] array =new int[6]{ 1,2,3,4,5,6};

            //for (int index = 0; index < array.Length; index++)
            //{
            //    for (int iterator = index+1; iterator < array.Length; iterator++)
            //    {
            //        Console.WriteLine($"{array[index]}->{array[iterator]}");
            //    }
            //}
            //Coordinate declaredGoal = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, double.NaN, double.NaN, DateTime.Now);
            //faiLogger.DeclaredGoals.Add(new DeclaredGoal(2, declaredGoal, declaredGoal));

            //DonutTask donut = new DonutTask();
            //donut.GoalNumber = 2;
            //donut.InnerRadius = 250;
            //donut.OuterRadius = 750;
            //donut.UpperBoundary = double.NaN;
            //donut.IsReentranceAllowed = true;
            //donut.LowerBoundary = CoordinateHelpers.ConvertToMeter(3200);
            //donut.DeclarationValidationRules = new List<IDeclarationValidationRules>();

            //double donutResult;
            //if (!donut.CalculateResults(faiLogger, true, out donutResult))
            //{
            //    Console.WriteLine("Failed to calculate donut result");
            //}
            //else
            //{
            //    Console.WriteLine("Result Donut:"+donutResult);
            //}

            //PieTask pie = new PieTask();
            //pie.Tiers = new List<PieTask.PieTier>();

            //PieTask.PieTier tier1 = new PieTask.PieTier();
            //tier1.DeclarationValidationRules = new List<IDeclarationValidationRules>();
            //tier1.IsReentranceAllowed = true;
            //tier1.Multiplier = 1.0;
            //tier1.LowerBoundary = CoordinateHelpers.ConvertToMeter(3500);
            //tier1.UpperBoundary = double.NaN;
            //tier1.Radius = 500;
            //tier1.GoalNumber = 3;
            //pie.Tiers.Add(tier1);

            //utm= new CoordinateSharp.UniversalTransverseMercator("32U", 567950, 5489300);
            //coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            //Coordinate centerTier1 = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, double.NaN, double.NaN, DateTime.Now);
            //faiLogger.DeclaredGoals.Add(new DeclaredGoal(3, centerTier1, centerTier1));

            //PieTask.PieTier tier2 = new PieTask.PieTier();
            //tier2.DeclarationValidationRules = new List<IDeclarationValidationRules>();
            //tier2.IsReentranceAllowed = true;
            //tier2.Multiplier = 1.0;
            //tier2.LowerBoundary = CoordinateHelpers.ConvertToMeter(3500);
            //tier2.UpperBoundary = double.NaN;
            //tier2.Radius = 500;
            //tier2.GoalNumber = 4;
            //pie.Tiers.Add(tier2);

            //utm = new CoordinateSharp.UniversalTransverseMercator("32U", 567950, 5488700);
            //coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            //Coordinate centerTier2 = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, double.NaN, double.NaN, DateTime.Now);
            //faiLogger.DeclaredGoals.Add(new DeclaredGoal(4, centerTier2, centerTier2));

            //PieTask.PieTier tier3 = new PieTask.PieTier();
            //tier3.DeclarationValidationRules = new List<IDeclarationValidationRules>();
            //tier3.IsReentranceAllowed = true;
            //tier3.Multiplier = 3.0;
            //tier3.LowerBoundary = CoordinateHelpers.ConvertToMeter(3800);
            //tier3.UpperBoundary = double.NaN;
            //tier3.Radius = 250;
            //tier3.GoalNumber = 5;
            //pie.Tiers.Add(tier3);

            //utm = new CoordinateSharp.UniversalTransverseMercator("32U", 568550, 5489000);
            //coordinate = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utm);
            //Coordinate centerTier3 = new Coordinate(coordinate.Latitude.DecimalDegree, coordinate.Longitude.DecimalDegree, double.NaN, double.NaN, DateTime.Now);
            //faiLogger.DeclaredGoals.Add(new DeclaredGoal(5, centerTier3, centerTier3));

            //double pieResult;
            //if (!pie.CalculateResults(faiLogger, true, out pieResult))
            //{
            //    Console.WriteLine("Failed to calculate pie result");
            //}
            //else
            //{
            //    Console.WriteLine("Result pie:"+pieResult);
            //}
            //CoordinateSharp.Coordinate coordinate1 = new CoordinateSharp.Coordinate(49.550316666666667, 9.92055);
            //Console.WriteLine(coordinate1.UTM.Easting);
            //Console.WriteLine(coordinate1.UTM.Northing);

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

        private static string ToProperText(CoordinateSharp.CoordinatePart part)
        {
            string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds,2,MidpointRounding.AwayFromZero) + "ʺ";
            return text;
        }

    }
}
