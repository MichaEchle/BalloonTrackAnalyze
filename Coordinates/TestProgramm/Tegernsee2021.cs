//using Coordinates;
//using Coordinates.Parsers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestProgramm
//{
//    public static class Tegernsee2021
//    {
//        private const double SEPARATIONALTFT = 3000.0;

//        public static void CalcFlight3()
//        {
//            DirectoryInfo directoryInfo = new(@"\\esdmob1\TGS2021\Tracks\E[DM2021]F[3]");
//            FileInfo[] files = directoryInfo.GetFiles("*.igc");
//            Coordinates.Coordinate goalTask7 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 705906, 5295258);
//            Coordinates.Coordinate goalTask8 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32T", 704000, 5295000);
//            List<Track> tracks = [];
//            foreach (FileInfo fileInfo in files)
//            {
//                if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out Track track))
//                {
//                    Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
//                    continue;
//                }
//                tracks.Add(track);
//            }
//            tracks = [.. tracks.OrderBy(x => x.Pilot.PilotNumber)];
//            using (StreamWriter writer = new(@"C:\Temp\F[3]_ILPChecks.csv"))
//            {
//                writer.WriteLine("PilotNumber,LaunchPoint,TOBeforeEoSP,Dist T7,Dist T7 Ok,Dist T8,Dist T8 OK,Dist T9,Dist T9 OK");
//                foreach (Track currentTrack in tracks)
//                {
//                    writer.WriteLine(CheckILP(currentTrack, goalTask7, goalTask8));
//                }
//            }
//            using (StreamWriter writer = new(@"C:\Temp\F[3]_ResultsT7.csv"))
//            {
//                writer.WriteLine("PilotNumber,Result,MarkerTime,InScoringPeriod");
//                foreach (Track currentTrack in tracks)
//                {
//                    writer.WriteLine(CalculateResultsTask7(currentTrack, goalTask7));
//                }
//            }
//            using (StreamWriter writer = new(@"C:\Temp\F[3]_ResultsT8.csv"))
//            {
//                writer.WriteLine("PilotNumber,Result,Dist M2 G8,Dist M2 G8 Ok,Dist M3 G8,Dist M3 G8 ok,Angle,Angle comment,Marker2Time,Marker3Time,InScoringPeriod");
//                foreach (Track currentTrack in tracks)
//                {
//                    writer.WriteLine(CalculateResultsTask8(currentTrack, goalTask8));
//                }

//            }
//            using (StreamWriter writer = new(@"C:\Temp\F[3]_ResultsT9.csv"))
//            {
//                writer.WriteLine("PilotNumber,Result,No Dec, Dist Dec To G1, Dist Dec to G1 Ok,Dec to G1 infring,Dec to G1 penalty,Dist G1 to T7,Dist G1 to T7 Ok, G1 to T7 infring,G1 to T7 penalty,Dist G1 to T8,Dist G1 to T8 Ok, G1 to T8 infring,G1 to T8 penalty,Marker Time,In Scoring Period");
//                foreach (Track currentTrack in tracks)
//                {
//                    writer.WriteLine(CalculateResultsTask9(currentTrack, goalTask7, goalTask8));
//                }

//            }

//        }

//        private static string CheckILP(Track track, Coordinates.Coordinate goalTask7, Coordinates.Coordinate goalTask8)
//        {
//            //Coordinate landingPoint;
//            TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out _);

//            DateTime endOfStartPeriod = new(2021, 10, 8, 16, 0, 0);
//            bool launchedBeforeStartPeriod = launchPoint.TimeStamp < endOfStartPeriod;
//            double distanceT7 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, goalTask7);
//            bool distanceT7Ok = distanceT7 >= 1000.0;
//            double distanceT8 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, goalTask8);
//            bool distanceT8Ok = distanceT8 >= 1000.0;
//            Declaration declaration = track.Declarations.FindLast(x => x.GoalNumber == 1);
//            double distanceT9 = CoordinateHelpers.Calculate2DDistanceHavercos(launchPoint, declaration.DeclaredGoal);
//            bool distanceT9OK = distanceT9 >= 1000.0;
//            return $"{track.Pilot.PilotNumber},{launchPoint:HH:mm:ss},{launchedBeforeStartPeriod},{distanceT7},{distanceT7Ok},{distanceT8},{distanceT8Ok},{distanceT9},{distanceT9OK}";
//        }

//        private static string CalculateResultsTask7(Track track, Coordinates.Coordinate goalTask7)
//        {
//            MarkerDrop marker = track.MarkerDrops.Find(x => x.MarkerNumber == 1);
//            double result;
//            if (marker == null || marker == default)
//            {
//                result = double.NaN;
//                return $"{track.Pilot.PilotNumber},{result}";
//            }
//            else
//            {
//                double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goalTask7, marker.MarkerLocation, SEPARATIONALTFT, true);
//                result = Math.Max(50.0, Math.Round(distance, 0, MidpointRounding.AwayFromZero));
//                return $"{track.Pilot.PilotNumber},{result},{marker.MarkerLocation.TimeStamp:HH:mm:ss},{marker.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";
//            }
//        }

//        private static string CalculateResultsTask8(Track track, Coordinates.Coordinate goalTask8)
//        {

//            MarkerDrop marker2 = track.MarkerDrops.Find(x => x.MarkerNumber == 2);
//            MarkerDrop marker3 = track.MarkerDrops.Find(x => x.MarkerNumber == 3);
//            double result;
//            if (marker2 == null || marker2 == default || marker3 == null || marker3 == default)
//            {
//                result = double.NaN;
//                return $"{track.Pilot.PilotNumber},{result}";
//            }
//            else
//            {
//                double distanceM2G8 = CoordinateHelpers.Calculate2DDistanceHavercos(goalTask8, marker2.MarkerLocation);
//                bool distanceM2G8Ok = distanceM2G8 >= 500.0;
//                double distanceM3G8 = CoordinateHelpers.Calculate2DDistanceHavercos(goalTask8, marker3.MarkerLocation);
//                bool distanceM3G8Ok = distanceM3G8 >= 500.0;
//                double angle = CoordinateHelpers.CalculateInteriorAngle(marker2.MarkerLocation, goalTask8, marker3.MarkerLocation);
//                double distance = CoordinateHelpers.Calculate2DDistanceHavercos(marker2.MarkerLocation, marker3.MarkerLocation);
//                result = Math.Round(distance, 0, MidpointRounding.AwayFromZero);
//                string angleComment;
//                if (angle < 45.0)
//                {
//                    result = double.NaN;
//                    angleComment = "Angle to small";
//                }
//                else if (angle >= 45.0 || angle <= 90.0)
//                {
//                    angleComment = "need visual check";
//                }
//                else
//                {
//                    angleComment = "Angle is large enough";
//                }
//                return $"{track.Pilot.PilotNumber},{result},{distanceM2G8},{distanceM2G8Ok},{distanceM3G8},{distanceM3G8Ok},{angle},{angleComment},{marker2.MarkerLocation.TimeStamp:HH:mm:ss},{marker3.MarkerLocation.TimeStamp:HH:mm:ss},{marker3.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";

//            }

//        }

//        private static string CalculateResultsTask9(Track track, Coordinates.Coordinate goalTask7, Coordinates.Coordinate goalTask8)
//        {
//            int numberOfDeclarations = track.Declarations.Where(x => x.GoalNumber == 1).Count();
//            Declaration declaration;
//            if (numberOfDeclarations <= 3)
//                declaration = track.Declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).First(x => x.GoalNumber == 1);
//            else
//                declaration = track.Declarations.OrderBy(x => x.PositionAtDeclaration.TimeStamp).ToList()[2];
//            MarkerDrop marker = track.MarkerDrops.Find(x => x.MarkerNumber == 4);
//            double result;
//            if (declaration == null || declaration == default || marker == null || marker == default)
//            {
//                result = double.NaN;
//                return $"{track.Pilot.PilotNumber},{result},{numberOfDeclarations}";
//            }
//            else
//            {
//                double distanceDecToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
//                bool distanceDecToGoalOk = distanceDecToGoal >= 2000.0;
//                double infringementDec = 0.0;
//                double penaltyDec = 0.0;
//                if (!distanceDecToGoalOk)
//                {
//                    infringementDec = 2000.0 - distanceDecToGoal;
//                    penaltyDec = infringementDec / 2000.0 * 100 * 20;
//                }

//                double distanceG1T7 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, goalTask7);
//                bool distanceG1T7Ok = distanceG1T7 >= 1000.0;
//                double infringementT7 = 0.0;
//                double penaltyT7 = 0.0;
//                if (!distanceG1T7Ok)
//                {
//                    infringementT7 = 1000.0 - distanceG1T7;
//                    penaltyT7 = infringementT7 / 1000.0 * 100 * 20;
//                }
//                double distanceG1T8 = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, goalTask8);
//                bool distanceG1T8Ok = distanceG1T8 >= 1000.0;
//                double infringementT8 = 0.0;
//                double penaltyT8 = 0.0;
//                if (!distanceG1T8Ok)
//                {
//                    infringementT8 = 1000.0 - distanceG1T8;
//                    penaltyT8 = infringementT8 / 1000.0 * 100 * 20;
//                }

//                Coordinates.Coordinate declaredGaolT9 = declaration.DeclaredGoal;
//                if (declaration.DeclaredGoal.AltitudeGPS <= 0)
//                {
//                    declaredGaolT9 = new Coordinates.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude, CoordinateHelpers.ConvertToMeter(SEPARATIONALTFT), CoordinateHelpers.ConvertToMeter(SEPARATIONALTFT), declaration.DeclaredGoal.TimeStamp);
//                }
//                double distance = CoordinateHelpers.Calculate3DDistance(declaredGaolT9, marker.MarkerLocation, true);
//                result = Math.Round(distance, 0, MidpointRounding.AwayFromZero);

//                return $"{track.Pilot.PilotNumber},{result},{numberOfDeclarations},{distanceDecToGoal},{distanceDecToGoalOk},{infringementDec},{penaltyDec},{distanceG1T7},{distanceG1T7Ok},{infringementT7},{penaltyT7},{distanceG1T8},{distanceG1T8Ok},{infringementT8},{penaltyT8},{marker.MarkerLocation.TimeStamp:HH:mm:ss},{marker.MarkerLocation.TimeStamp < new DateTime(2021, 10, 8, 16, 30, 0)}";
//            }

//        }
//    }

//}
