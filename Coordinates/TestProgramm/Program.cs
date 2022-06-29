using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accessibility;
using Competition;
using Coordinates;
using Coordinates.Parsers;
using CoordinateSharp;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace TestProgramm
{
    class Program
    {
        private const double SEPARATIONALTFT = 3000.0;

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
            CalcFlight3();
        }



        private static string ToProperText(CoordinateSharp.CoordinatePart part)
        {
            string text = part.Degrees + "° " + part.Minutes + "ʹ " + Math.Round(part.Seconds, 2, MidpointRounding.AwayFromZero) + "ʺ";
            return text;
        }

        private static void CalcFlight3()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\esdmob1\TGS2021\Tracks\E[DM2021]F[3]");
            FileInfo[] files = directoryInfo.GetFiles("*.igc");
            Track track;
            Coordinates.Coordinate goalTask7 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 705906, 5295258);
            Coordinates.Coordinate goalTask8 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 704000, 5295000);
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
            using (StreamWriter writer = new StreamWriter(@"C:\Temp\F[3]_ILPChecks.csv"))
            {
                writer.WriteLine("PilotNumber,LaunchPoint,TOBeforeEoSP,Dist T7,Dist T7 Ok,Dist T8,Dist T8 OK,Dist T9,Dist T9 OK");
                foreach (Track currentTrack in tracks)
                {
                    writer.WriteLine(CheckILP(currentTrack, goalTask7, goalTask8));
                }
            }
            using (StreamWriter writer = new StreamWriter(@"C:\Temp\F[3]_ResultsT7.csv"))
            {
                writer.WriteLine("PilotNumber,Result,MarkerTime,InScoringPeriod");
                foreach (Track currentTrack in tracks)
                {
                    writer.WriteLine(CalculateResultsTask7(currentTrack, goalTask7));
                }
            }
            using (StreamWriter writer = new StreamWriter(@"C:\Temp\F[3]_ResultsT8.csv"))
            {
                writer.WriteLine("PilotNumber,Result,Dist M2 G8,Dist M2 G8 Ok,Dist M3 G8,Dist M3 G8 ok,Angle,Angle comment,Marker2Time,Marker3Time,InScoringPeriod");
                foreach (Track currentTrack in tracks)
                {
                    writer.WriteLine(CalculateResultsTask8(currentTrack, goalTask8));
                }

            }
            using (StreamWriter writer = new StreamWriter(@"C:\Temp\F[3]_ResultsT9.csv"))
            {
                writer.WriteLine("PilotNumber,Result,No Dec, Dist Dec To G1, Dist Dec to G1 Ok,Dec to G1 infring,Dec to G1 penalty,Dist G1 to T7,Dist G1 to T7 Ok, G1 to T7 infring,G1 to T7 penalty,Dist G1 to T8,Dist G1 to T8 Ok, G1 to T8 infring,G1 to T8 penalty,Marker Time,In Scoring Period");
                foreach (Track currentTrack in tracks)
                {
                    writer.WriteLine(CalculateResultsTask9(currentTrack, goalTask7, goalTask8));
                }

            }

        }

        private static string CheckILP(Track track, Coordinates.Coordinate goalTask7, Coordinates.Coordinate goalTask8)
        {
            Coordinates.Coordinate launchPoint;
            //Coordinate landingPoint;
            TrackHelpers.EstimateLaunchAndLandingTime(track, true, out launchPoint, out _);

            DateTime endOfStartPeriod = new DateTime(2021, 10, 8, 16, 0, 0);
            bool launchedBeforeStartPeriod = launchPoint.TimeStamp < endOfStartPeriod;
            double distanceT7 = CoordinateHelpers.Calculate2DDistance(launchPoint, goalTask7);
            bool distanceT7Ok = distanceT7 >= 1000.0;
            double distanceT8 = CoordinateHelpers.Calculate2DDistance(launchPoint, goalTask8);
            bool distanceT8Ok = distanceT8 >= 1000.0;
            Declaration declaration = track.Declarations.FindLast(x => x.GoalNumber == 1);
            double distanceT9 = CoordinateHelpers.Calculate2DDistance(launchPoint, declaration.DeclaredGoal);
            bool distanceT9OK = distanceT9 >= 1000.0;
            return $"{track.Pilot.PilotNumber},{launchPoint:HH:mm:ss},{launchedBeforeStartPeriod},{distanceT7},{distanceT7Ok},{distanceT8},{distanceT8Ok},{distanceT9},{distanceT9OK}";
        }

        private static string CalculateResultsTask7(Track track, Coordinates.Coordinate goalTask7)
        {
            MarkerDrop marker = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
            double result;
            if (marker == null || marker == default)
            {
                result = double.NaN;
                return $"{track.Pilot.PilotNumber},{result}";
            }
            else
            {
                double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goalTask7, marker.MarkerLocation, SEPARATIONALTFT, true);
                result = Math.Max(50.0, Math.Round(distance, 0, MidpointRounding.AwayFromZero));
                return $"{track.Pilot.PilotNumber},{result},{marker.MarkerLocation.TimeStamp:HH:mm:ss},{marker.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";
            }
        }

        private static string CalculateResultsTask8(Track track, Coordinates.Coordinate goalTask8)
        {

            MarkerDrop marker2 = track.MarkerDrops.Find(x => x.MarkerNumber == 2);
            MarkerDrop marker3 = track.MarkerDrops.Find(x => x.MarkerNumber == 3);
            double result;
            if (marker2 == null || marker2 == default || marker3 == null || marker3 == default)
            {
                result = double.NaN;
                return $"{track.Pilot.PilotNumber},{result}";
            }
            else
            {
                double distanceM2G8 = CoordinateHelpers.Calculate2DDistance(goalTask8, marker2.MarkerLocation);
                bool distanceM2G8Ok = distanceM2G8 >= 500.0;
                double distanceM3G8 = CoordinateHelpers.Calculate2DDistance(goalTask8, marker3.MarkerLocation);
                bool distanceM3G8Ok = distanceM3G8 >= 500.0;
                double angle = CoordinateHelpers.CalculateInteriorAngle(marker2.MarkerLocation, goalTask8, marker3.MarkerLocation);
                double distance = CoordinateHelpers.Calculate2DDistance(marker2.MarkerLocation, marker3.MarkerLocation);
                result = Math.Round(distance, 0, MidpointRounding.AwayFromZero);
                string angleComment;
                if (angle < 45.0)
                {
                    result = double.NaN;
                    angleComment = "Angle to small";
                }
                else if (angle >= 45.0 || angle <= 90.0)
                {
                    angleComment = "need visual check";
                }
                else
                {
                    angleComment = "Angle is large enough";
                }
                return $"{track.Pilot.PilotNumber},{result},{distanceM2G8},{distanceM2G8Ok},{distanceM3G8},{distanceM3G8Ok},{angle},{angleComment},{marker2.MarkerLocation.TimeStamp:HH:mm:ss},{marker3.MarkerLocation.TimeStamp:HH:mm:ss},{marker3.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";

            }

        }

        private static string CalculateResultsTask9(Track track, Coordinates.Coordinate goalTask7, Coordinates.Coordinate goalTask8)
        {
            int numberOfDeclarations = track.Declarations.Where(x => x.GoalNumber == 1).Count();
            Declaration declaration;
            if (numberOfDeclarations <= 3)
                declaration = track.Declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).First(x => x.GoalNumber == 1);
            else
                declaration = track.Declarations.OrderBy(x => x.PositionAtDeclaration.TimeStamp).ToList()[2];
            MarkerDrop marker = track.MarkerDrops.Find(x => x.MarkerNumber == 4);
            double result;
            if (declaration == null || declaration == default || marker == null || marker == default)
            {
                result = double.NaN;
                return $"{track.Pilot.PilotNumber},{result},{numberOfDeclarations}";
            }
            else
            {
                double distanceDecToGoal = CoordinateHelpers.Calculate2DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
                bool distanceDecToGoalOk = distanceDecToGoal >= 2000.0;
                double infringementDec = 0.0;
                double penaltyDec = 0.0;
                if (!distanceDecToGoalOk)
                {
                    infringementDec = 2000.0 - distanceDecToGoal;
                    penaltyDec = infringementDec / 2000.0 * 100 * 20;
                }

                double distanceG1T7 = CoordinateHelpers.Calculate2DDistance(declaration.DeclaredGoal, goalTask7);
                bool distanceG1T7Ok = distanceG1T7 >= 1000.0;
                double infringementT7 = 0.0;
                double penaltyT7 = 0.0;
                if (!distanceG1T7Ok)
                {
                    infringementT7 = 1000.0 - distanceG1T7;
                    penaltyT7 = infringementT7 / 1000.0 * 100 * 20;
                }
                double distanceG1T8 = CoordinateHelpers.Calculate2DDistance(declaration.DeclaredGoal, goalTask8);
                bool distanceG1T8Ok = distanceG1T8 >= 1000.0;
                double infringementT8 = 0.0;
                double penaltyT8 = 0.0;
                if (!distanceG1T8Ok)
                {
                    infringementT8 = 1000.0 - distanceG1T8;
                    penaltyT8 = infringementT8 / 1000.0 * 100 * 20;
                }

                Coordinates.Coordinate declaredGaolT9 = declaration.DeclaredGoal;
                if (declaration.DeclaredGoal.AltitudeGPS <= 0)
                {
                    declaredGaolT9 = new Coordinates.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude, CoordinateHelpers.ConvertToMeter(SEPARATIONALTFT), CoordinateHelpers.ConvertToMeter(SEPARATIONALTFT), declaration.DeclaredGoal.TimeStamp);
                }
                double distance = CoordinateHelpers.Calculate3DDistance(declaredGaolT9, marker.MarkerLocation, true);
                result = Math.Round(distance, 0, MidpointRounding.AwayFromZero);

                return $"{track.Pilot.PilotNumber},{result},{numberOfDeclarations},{distanceDecToGoal},{distanceDecToGoalOk},{infringementDec},{penaltyDec},{distanceG1T7},{distanceG1T7Ok},{infringementT7},{penaltyT7},{distanceG1T8},{distanceG1T8Ok},{infringementT8},{penaltyT8},{marker.MarkerLocation.TimeStamp:HH:mm:ss},{marker.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";
            }

        }
    }
}
