using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Accessibility;
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

            //string trackFileName = @"C:\temp\2907p025.igc";
            ////string trackFileName = @"C:\Users\Micha\SynologyDrive\tracks\Flight 3\work\E[BLC21]F[3]P[19]-80BE388D-003-2192655248.igc";
            //Track track;
            //if (!FAILoggerParser.ParseFile(trackFileName, out track))
            //{
            //    Console.WriteLine("Error parsing logger track");
            //}
            //bool isDangerousFlyingDetected;
            //List<Coordinate> relatedCoordinates;
            //TimeSpan totalDuration;
            //TrackHelpers.CheckForDangerousFlying(track, true,out isDangerousFlyingDetected, out relatedCoordinates,out _, out _,out totalDuration,out _);

            //string trackFileName = @"\\esdmob1\TGS2021\Tracks\E[DM2021]F[1]\E[DM2021]F[1]P[9]-87A53D33-009.igc";
            //string trackFileName = @"\\esdmob1\TGS2021\Tracks\E[DM2021]F[1]\E[DM2021]F[1]P[18]-DE7EECC0-018.igc";
            // string trackFileName = @"\\esdmob1\TGS2021\Tracks\E[DM2021]F[1]\E[DM2021]F[1]P[21]-28B482B2-021.igc";
            // //32T 0702443 5300449
            // Track track;
            // Coordinate goal1 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 702443, 5300449);
            // if (!BalloonLiveParser.ParseFile(trackFileName, out track))
            // {
            //     Console.WriteLine("Error parsing logger track");
            // }
            //double distance=CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goal1, track.MarkerDrops.First(x => x.MarkerNumber == 3).MarkerLocation, CoordinateHelpers.ConvertToMeter(3000.0), true);
            // Console.WriteLine(Math.Round(distance,0,MidpointRounding.AwayFromZero));
            // Console.ReadLine();
            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\esdmob1\TGS2021\Tracks\E[DM2021]F[2]");
            FileInfo[] files = directoryInfo.GetFiles("*.igc");
            Track track;
            Coordinate goalTask5 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 698351, 5293750);
            PieTask pieTask = new PieTask();
            PieTask.PieTier pieTier = new PieTask.PieTier();
            pieTier.SetupPieTier(2, 1000, true, 1.0, double.NaN, CoordinateHelpers.ConvertToMeter(4000.0), null);
            pieTask.SetupPie(6, new List<PieTask.PieTier>() { pieTier });

            //using (StreamWriter writer = new StreamWriter(@"c:\temp\ResultT4_NewSpec.csv"))
            //{
            //    writer.WriteLine("Pilot,Result,DecOk");

            foreach (FileInfo fileInfo in files)
            {
                if (!fileInfo.Name.Contains("P[28]"))
                    continue;
                if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out track))
                {
                    Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
                    continue;
                }
                //Coordinate firstPoint = track.TrackPoints.First(x => x.Latitude>0);
                //Coordinate launch;
                //TrackHelpers.EstimateLaunchAndLandingTime(track, true, out launch, out _);
                //Declaration declaration1 = track.Declarations.FindLast(x => x.GoalNumber == 1);
                //if (declaration1 == null || declaration1 == default)
                //    continue;
                //Declaration declaration2 = track.Declarations.Find(x => x.GoalNumber == 2);
                //if (declaration2 == null || declaration2 == default)
                //    continue;
                //double ilpToG1 = CoordinateHelpers.Calculate2DDistance(launch, declaration1.DeclaredGoal);
                //double ilpToG2 = CoordinateHelpers.Calculate2DDistance(launch, declaration2.DeclaredGoal);
                //double ilpToT5 = CoordinateHelpers.Calculate2DDistance(launch, goalTask5);
                //writer.WriteLine($"{track.Pilot.PilotNumber},{firstPoint.TimeStamp},{launch.TimeStamp},{ilpToG1},{ilpToT5},{ilpToG2}");
                MarkerDrop markerDrop = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
                if (markerDrop == null || markerDrop == default)
                    continue;
                //Declaration declaration = track.Declarations.Find(x => x.GoalNumber == 2);
                //if (declaration == null || declaration == default)
                //    continue;
                //bool decBeforeMark = declaration.PositionAtDeclaration.TimeStamp < markerDrop.MarkerLocation.TimeStamp;
                if (pieTask.CalculateResults(track, true, out double result))
                {
                    result = Math.Round(result, 0, MidpointRounding.AwayFromZero);
                    Console.WriteLine($"{track.Pilot.PilotNumber},{result}");
                    //writer.WriteLine($"{track.Pilot.PilotNumber},{result},{decBeforeMark}");
                }
                //Declaration declaration = track.Declarations.Find(x => x.GoalNumber == 1);
                //MarkerDrop markerDrop = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
                //if (markerDrop == null || markerDrop == default)
                //    continue;

                //if (declaration == null || declaration == default)
                //    continue;
                //double distanceDecToGoal = CoordinateHelpers.Calculate2DDistance(declaration1.PositionAtDeclaration, declaration1.DeclaredGoal);
                //bool decOk = distanceDecToGoal > 1000.0;
                //if (declaration1.DeclaredGoal.AltitudeGPS > 0)
                //{
                //    //Coordinate coordinate = new Coordinate(declaration1.DeclaredGoal.Latitude, declaration1.DeclaredGoal.Longitude,
                //    //   CoordinateHelpers.ConvertToMeter(3000), CoordinateHelpers.ConvertToMeter(3000), declaration1.DeclaredGoal.TimeStamp);
                //    double result = Math.Round(CoordinateHelpers.Calculate3DDistance(declaration1.DeclaredGoal, markerDrop.MarkerLocation, true), 0, MidpointRounding.AwayFromZero);
                //    Console.WriteLine($"{track.Pilot.PilotNumber},{result},{decOk}");
                    //writer.WriteLine($"{track.Pilot.PilotNumber},{result},{decOk}");
                //}
            }
        }



        private static string ToProperText(CoordinateSharp.CoordinatePart part)
        {
            string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
            return text;
        }

    }
}
