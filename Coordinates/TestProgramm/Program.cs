using Coordinates;
using Coordinates.Parsers;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging.Console;

namespace TestProgramm;

class Program
{


    static void Main(string[] args)
    {
        LogConnector.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        //C:\Users\micechle\source\repos\MichaEchle\BalloonTrackAnalyze\TestTrack\5AD_f003_p002_l0.igc
        //\..\..\..\..\..\TestTrack\5AD_f003_p002_l0.igc

        FileInfo fileInfo = new(@"..\..\..\..\..\TestTrack\Archive-E[germannationals2022]F[3]P[18]-BFUrwYGs212.igc");
        Coordinates.Coordinate referenceCoordiante = new(15, 20, 10, 10, DateTime.Now); // define a reference coordinate (the values are random)
        if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out Track track, referenceCoordiante)) // provide the reference coordinate to the parser
        {
            Console.WriteLine("Failed to parse track");
        }

        var declaration = track.Declarations.First(x => x.GoalNumber == 1); // get the declaration for goal 1

        var newDeclaredGoal = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 670000 + declaration.OrignalEastingDeclarationUTM, 59000 + declaration.OrignalNorhtingDeclarationUTM, declaration.DeclaredGoal.AltitudeGPS); // create a new coordinate using the original declarations

        track.Declarations.Remove(declaration); // remove the old declaration
        track.Declarations.Add(new Declaration(declaration.GoalNumber, newDeclaredGoal, declaration.PositionAtDeclaration, true, declaration.OrignalEastingDeclarationUTM, declaration.OrignalNorhtingDeclarationUTM)); // add a new one with correct declared goal.

        Console.ReadLine();
        //AccuracyEvaluation_GeodTest.CalculateDistances();
        //DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\temp\Donut_DM2022");
        //FileInfo[] files = directoryInfo.GetFiles("*.igc");
        //Track track;
        //List<Track> tracks = new List<Track>();
        //foreach (FileInfo fileInfo in files)
        //{
        //    if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out track))
        //    {
        //        Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
        //        continue;
        //    }
        //    tracks.Add(track);
        //}
        //tracks = tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
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
    }




    private static string ToProperText(CoordinateSharp.CoordinatePart part)
    {
        string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
        return text;
    }

}
