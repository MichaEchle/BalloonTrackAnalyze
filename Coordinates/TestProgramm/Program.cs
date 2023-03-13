using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using Competition;
using Competition.Penalties;
using Coordinates;
using Coordinates.Parsers;
using CoordinateSharp;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace TestProgramm
{
    class Program
    {


        static void Main(string[] args)
        {


            //AccuracyEvaluation_GeodTest.CalculateDistances();
            DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\temp\E[HORB21]F[3]");
            FileInfo[] files = directoryInfo.GetFiles("*.igc");
            Track track;
            List<Track> tracks = new List<Track>();
            foreach (FileInfo fileInfo in files)
            {
                if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out track))
                {
                    Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
                    continue;
                }
                tracks.Add(track);
            }
            tracks = tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
            tracks.RemoveAt(0);
            PenaltyCalculation.CheckForCloseProximityAndCalculatePenaltyPoints(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), true, tracks);
            Console.WriteLine("Nothing found");
            //DonutTask donutTask = new DonutTask();
            //DeclarationToGoalDistanceRule declarationToGoalDistanceRule = new DeclarationToGoalDistanceRule();
            //declarationToGoalDistanceRule.SetupRule(2500, double.NaN);
            //donutTask.SetupDonut(12, 1, 3, 1000, 2000, 0, 10000, true, new List<IDeclarationValidationRules>() { declarationToGoalDistanceRule });

            ////using (StreamWriter writer2 = new StreamWriter(@"C:\Temp\Donut\F[3]_Task12_Results.csv"))
            ////{
            ////    writer2.WriteLine($"PilotNumber,Distance, Reason for no result");
            //foreach (Track currentTrack in tracks)
            //{
            //    double result = double.NaN;
            //    if (currentTrack.Declarations.Where(x => x.GoalNumber == 1).Count() <= 3)
            //    {
            //        if (!donutTask.CalculateResults(currentTrack, true, out result))
            //        {
            //            Console.WriteLine("Failed to calculate result");
            //        }
            //        //writer2.WriteLine($"{currentTrack.Pilot.PilotNumber},{result},Failed to calculate result for pilot {currentTrack.Pilot.PilotNumber}");
            //        else
            //        {
            //            Console.WriteLine($"{currentTrack.Pilot.PilotNumber},{result}");

            //            //result umt (round to int)=5392m
            //            //result vincenty (WSG84) = 5170m
            //        }

            //    }
            //    else
            //    {
            //        Console.WriteLine($"{currentTrack.Pilot.PilotNumber},{result},More than 3 declarations");
            //    }
            //}
            //Montgolfiade_DM2022.CalculateFlight5();
            Console.ReadLine();
        }
    



    private static string ToProperText(CoordinateSharp.CoordinatePart part)
    {
        string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
        return text;
    }

}
}
