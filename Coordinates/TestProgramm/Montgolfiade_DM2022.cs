using Competition;
using Coordinates;
using Coordinates.Parsers;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Security.Authentication.OnlineId;

namespace TestProgramm
{
    public static class Montgolfiade_DM2022
    {
        public static void CalculateFlight3()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\192.168.188.123\scoringdm2022\tracks\work\flight_03");
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

            List<Coordinate> goals = new List<Coordinate>();
            goals.Add(CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624019, 5570870));//Goal Task8
            goals.Add(CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622102, 5571266));//Goal Task9+10
            goals.Add(CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 619586, 5571709));//Goal Task11


            Coordinate centerPoint = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622103, 5571266);
            Coordinate referencePoint = CoordinateHelpers.CalculatePointWithDistanceAndBearing(centerPoint, 200, 0);

            using (StreamWriter writer = new StreamWriter(@"C:\Temp\F[3]_LaunchChecks.csv"))
            {
                writer.WriteLine("PilotNumber,LaunchTime,LauchPoint Lat, LaunchPoint Long,LaunchPointInStartPeriod, Distance8Goal,DistanceGoal8Ok,Distance9_10Goal,DistanceGoal9_10Ok,Distance11Goal,DistanceGoal11Ok");
                foreach (Track currentTrack in tracks)
                {
                    if (!TrackHelpers.CheckLaunchConstraints(currentTrack, true, new DateTime(2022, 8, 12, 4, 1, 0), new DateTime(2022, 8, 12, 5, 30, 0), goals, 1000, double.NaN, out Coordinate launchPoint, out bool launchInStartPeriod, out List<double> distanceToGoals, out List<bool> distanceToGoalsOk))
                    {
                        Console.WriteLine($"Failed to check launch constraints for {currentTrack.Pilot.PilotNumber}");
                        writer.WriteLine(currentTrack.Pilot.PilotNumber);
                    }
                    else
                    {
                        writer.WriteLine($"{currentTrack.Pilot.PilotNumber},{launchPoint.TimeStamp:HH:mm:ss},{launchPoint.Latitude},{launchPoint.Longitude},{launchInStartPeriod},{distanceToGoals[0]},{distanceToGoalsOk[0]},{distanceToGoals[1]},{distanceToGoalsOk[1]},{distanceToGoals[2]},{distanceToGoalsOk[2]}");
                    }
                }
            }

            using (StreamWriter writer1 = new StreamWriter(@"C:\Temp\F[3]_Task9_Results.csv"))
            {
                writer1.WriteLine($"PilotNumber,Distance, Reason for no result,Marker2 Slice, Marker3 Slice");
                foreach (Track currentTrack in tracks)
                {
                    writer1.WriteLine(CalculateTask9(currentTrack, centerPoint, referencePoint));
                }
            }

            DonutTask donutTask = new DonutTask();
            DeclarationToGoalDistanceRule declarationToGoalDistanceRule = new DeclarationToGoalDistanceRule();
            declarationToGoalDistanceRule.SetupRule(2500, double.NaN);
            donutTask.SetupDonut(12, 1, 3, 1000, 2000, 0, 10000, true, new List<IDeclarationValidationRules>() { declarationToGoalDistanceRule });

            using (StreamWriter writer2 = new StreamWriter(@"C:\Temp\F[3]_Task12_Results.csv"))
            {
                writer2.WriteLine($"PilotNumber,Distance, Reason for no result");
                foreach (Track currentTrack in tracks)
                {
                    double result = double.NaN;
                    if (currentTrack.Declarations.Where(x => x.GoalNumber == 1).Count() <= 3)
                    {
                        if (!donutTask.CalculateResults(currentTrack, true, out result))
                            writer2.WriteLine($"{currentTrack.Pilot.PilotNumber},{result},Failed to calculate result for pilot {currentTrack.Pilot.PilotNumber}");
                        else
                        {
                            writer2.WriteLine($"{currentTrack.Pilot.PilotNumber},{result}");
                        }

                    }
                    else
                    {
                        writer2.WriteLine($"{currentTrack.Pilot.PilotNumber},{result},More than 3 declarations");
                    }
                }
            }
        }

        private static string CalculateTask9(Track track, Coordinate centerPoint, Coordinate referencePoint)
        {
            int pilotNo = track.Pilot.PilotNumber;
            double distance = double.NaN;
            string reasonForNoResult = "";
            MarkerDrop marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            MarkerDrop marker3 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
            int marker2SliceNumber = -1;
            int marker3SliceNumber = -1;
            if ((marker2 is null) || (marker3 is null))
            {
                reasonForNoResult = "Marker 2 or Marker 3 not dropped";
            }
            else
            {
                double distanceMarker2 = CoordinateHelpers.Calculate2DDistance(marker2.MarkerLocation, centerPoint);
                double distanceMarker3 = CoordinateHelpers.Calculate2DDistance(marker3.MarkerLocation, centerPoint);

                if (distanceMarker2 < 200 || distanceMarker3 < 200)
                    reasonForNoResult = "Marker 2 or Marker 3 to close to center point";
                else
                {
                    double angleMarker2 = CoordinateHelpers.CalculateInteriorAngle(marker2.MarkerLocation, centerPoint, referencePoint);
                    if (marker2.MarkerLocation.Longitude < centerPoint.Longitude)
                        angleMarker2 = 360 - angleMarker2;
                    double angleMarker3 = CoordinateHelpers.CalculateInteriorAngle(marker3.MarkerLocation, centerPoint, referencePoint);
                    if (marker3.MarkerLocation.Longitude < centerPoint.Longitude)
                        angleMarker3 = 360 - angleMarker3;

                    for (int index = 0; index < 8; index++)
                    {
                        if (angleMarker2 >= (360 / 8 * index) && angleMarker2 < (360 / 8 * (index + 1)))
                            marker2SliceNumber = index + 1;
                        if (angleMarker3 >= (360 / 8 * index) && angleMarker3 < (360 / 8 * (index + 1)))
                            marker3SliceNumber = index + 1;
                    }

                    double angle = CoordinateHelpers.CalculateInteriorAngle(marker2.MarkerLocation, centerPoint, marker3.MarkerLocation);
                    if (angle < 45.0)
                        reasonForNoResult = "Angle between markers below 45 Degrees";
                    else if (angle >= 45.0 && angle < 90.0)
                    {
                        if (marker2SliceNumber == marker3SliceNumber)
                            reasonForNoResult = "Markers are in same slice";
                        else
                        {
                            if (marker3SliceNumber == ((marker2SliceNumber + 6) % 8) + 1)
                                reasonForNoResult = "Markers are in adjacent slices";
                            else if (marker3SliceNumber == ((marker2SliceNumber) % 8) + 1)
                                reasonForNoResult = "Markers are in adjacent slices";
                            else
                                distance = CoordinateHelpers.Calculate2DDistance(marker2.MarkerLocation, marker3.MarkerLocation);
                        }
                    }
                    else//angle >=90
                    {
                        distance = CoordinateHelpers.Calculate2DDistance(marker2.MarkerLocation, marker3.MarkerLocation);
                    }
                }
            }

            return $"{pilotNo},{distance},{reasonForNoResult},{marker2SliceNumber},{marker3SliceNumber}";
        }

        public static void CalculateFlight4()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\192.168.188.123\scoringdm2022\tracks\work\flight_04");
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

            using (StreamWriter writer = new StreamWriter(@"C:\temp\F[4]_Task14_Results.csv"))
            {
                writer.WriteLine("Pilot Number,Goal used,Result,G1 Easting, G1 Northing, G2 Easting, G2 Northing, G3 Easting, G3 Northing, ILP Easting, ILP Northing, ILP Time, ILP->G1, ILP->G2, ILP->G3, Marker Number,Marker Easting, Marker Northing, Marker Time,Dec1->G1, Dec2->G2, Dec3->G3,G1->M,G2->M,G3->M,Comment");
                foreach (Track currentTrack in tracks)
                {
                    string comment = "";
                    int pilotNo = currentTrack.Pilot.PilotNumber;
                    if (!TrackHelpers.EstimateLaunchAndLandingTime(currentTrack, true, out Coordinate launchPoint, out Coordinate landingPoint))
                    {
                        Console.WriteLine($"Failed to estimate launch and landing point for {pilotNo}");
                        comment += $"Failed to estimate launch and landing point for {pilotNo}";
                    }

                    List<double> distances = new List<double>();
                    List<Declaration> validGoals = new List<Declaration>();
                    List<double> distancesGoalToILP = new List<double>();
                    List<double> distancesDecToGoal = new List<double>();

                    MarkerDrop markerUsed = currentTrack.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
                    if (markerUsed is null)
                    {
                        markerUsed = currentTrack.MarkerDrops.FirstOrDefault();
                    }
                    for (int goalNumber = 1; goalNumber <= 3; goalNumber++)
                    {

                        List<Declaration> declarations = currentTrack.Declarations.Where(x => x.GoalNumber == goalNumber).ToList();

                        if (declarations.Count > 2)
                        {
                            Console.WriteLine($"Pilot {pilotNo} declared more than 2 goals for goal number {goalNumber}");
                            comment += $"Pilot {pilotNo} declared more than 2 goals for goal number {goalNumber}";
                            distances.Add(double.MaxValue);
                            validGoals.Add(null);
                            distancesDecToGoal.Add(double.NaN);
                            distancesGoalToILP.Add(double.NaN);
                            continue;
                        }
                        if (declarations.Count == 0)
                        {
                            Console.WriteLine($"Pilot {pilotNo} declared no goal for goal number {goalNumber}");
                            comment += $"Pilot {pilotNo} declared no goal for goal number {goalNumber}";
                            distances.Add(double.MaxValue);
                            validGoals.Add(null);
                            distancesDecToGoal.Add(double.NaN);
                            distancesGoalToILP.Add(double.NaN);
                            continue;
                        }

                        Declaration declarationToUse = declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList().First();

                        Declaration validGoal = null;

                        double distanceToILP = CoordinateHelpers.Calculate2DDistance(launchPoint, declarationToUse.DeclaredGoal);
                        distancesGoalToILP.Add(distanceToILP);
                        double distanceDeclarationPositionToDeclaredGoal = CoordinateHelpers.Calculate2DDistance(declarationToUse.PositionAtDeclaration, declarationToUse.DeclaredGoal);
                        distancesDecToGoal.Add(distanceDeclarationPositionToDeclaredGoal);
                        if (distanceToILP > 1000.0 && distanceDeclarationPositionToDeclaredGoal > 1000.0 && declarationToUse.PositionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                            validGoal = declarationToUse;


                        validGoals.Add(validGoal);
                        if (validGoal is null)
                        {
                            Console.WriteLine($"Pilot {pilotNo} declared no valid goal for goal number {goalNumber}");
                            comment += $"Pilot {pilotNo} declared no valid goal for goal number {goalNumber}";
                            distances.Add(double.MaxValue);
                            continue;
                        }
                        if (markerUsed is not null && markerUsed.MarkerNumber == 1)
                        {

                            if (validGoal.DeclaredGoal.AltitudeGPS != 0)
                            {
                                distances.Add(CoordinateHelpers.Calculate3DDistance(validGoal.DeclaredGoal, markerUsed.MarkerLocation, true));
                            }
                            else
                            {
                                Coordinate validGoalWithHeight = new Coordinate(validGoal.DeclaredGoal.Latitude, validGoal.DeclaredGoal.Longitude, CoordinateHelpers.ConvertToMeter(1500), CoordinateHelpers.ConvertToMeter(1500), validGoal.DeclaredGoal.TimeStamp);
                                distances.Add(CoordinateHelpers.Calculate3DDistance(validGoalWithHeight, markerUsed.MarkerLocation, true));

                            }
                        }
                        if (markerUsed is not null && markerUsed.MarkerNumber != 1)
                        {
                            if (validGoal.DeclaredGoal.AltitudeGPS != 0)
                            {
                                distances.Add(CoordinateHelpers.Calculate3DDistance(validGoal.DeclaredGoal, currentTrack.MarkerDrops[0].MarkerLocation, true));
                                Console.WriteLine($"******* Attention ********\r\n Pilot {pilotNo} dropped marker but didn't used marker 1");
                                comment += $"Pilot {pilotNo} dropped marker but didn't used marker 1";
                            }
                            else
                            {
                                Coordinate validGoalWithHeight = new Coordinate(validGoal.DeclaredGoal.Latitude, validGoal.DeclaredGoal.Longitude, CoordinateHelpers.ConvertToMeter(1500), CoordinateHelpers.ConvertToMeter(1500), validGoal.DeclaredGoal.TimeStamp);
                                distances.Add(CoordinateHelpers.Calculate3DDistance(validGoalWithHeight, markerUsed.MarkerLocation, true));
                                Console.WriteLine($"******* Attention ********\r\n Pilot {pilotNo} dropped marker but didn't used marker 1");
                                comment += $"Pilot {pilotNo} dropped marker but didn't used marker 1";
                            }
                        }
                        if (markerUsed is null)
                        {
                            Console.WriteLine($"Pilot {pilotNo} dropped no marker");
                            comment += $"Pilot {pilotNo} dropped no marker";
                            distances.Add(double.MaxValue);
                        }

                    }

                    double result = distances.Min();
                    int goalUsed = distances.FindIndex(x => x == result) + 1;
                    List<(string utmZone, int easting, int northing)> validGoalsUTM = new List<(string utmZone, int easting, int northing)>();
                    foreach (Declaration validGoal in validGoals)
                    {
                        if (validGoal is null)
                        {
                            validGoalsUTM.Add(("XX", -1, -1));
                        }
                        else
                        {
                            validGoalsUTM.Add(CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(validGoal.DeclaredGoal));
                        }

                    }

                    (string utmZone, int easting, int northing) launchPointUTM;
                    if (launchPoint is not null)
                        launchPointUTM = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(launchPoint);
                    else
                        launchPointUTM = ("XX", -1, -1);

                    (string utmZone, int easting, int northing) markerUsedUTM;
                    if (markerUsed is not null)
                        markerUsedUTM = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(markerUsed.MarkerLocation);
                    else
                        markerUsedUTM = ("XX", -1, -1);

                    writer.WriteLine($"{pilotNo},{goalUsed},{result},{validGoalsUTM[0].easting},{validGoalsUTM[0].northing},{validGoalsUTM[1].easting},{validGoalsUTM[1].northing},{validGoalsUTM[2].easting},{validGoalsUTM[2].northing},{launchPointUTM.easting},{launchPointUTM.northing},{launchPoint.TimeStamp:HH:mm:ss},{distancesGoalToILP[0]},{distancesGoalToILP[1]},{distancesGoalToILP[2]},{markerUsed?.MarkerNumber ?? -1},{markerUsedUTM.easting},{markerUsedUTM.northing},{markerUsed?.MarkerLocation.TimeStamp ?? DateTime.MinValue:HH:mm:ss},{distancesDecToGoal[0]},{distancesDecToGoal[1]},{distancesDecToGoal[2]},{distances[0]},{distances[1]},{distances[2]},{comment}");
                }
            }
        }

        public static void CalculateFlight5()
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(@"\\192.168.188.123\scoringdm2022\tracks\work\flight_05");
            //DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\temp\dm2022");

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

            Coordinate goalTask15 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623500, 5571282, CoordinateHelpers.ConvertToMeter(971));
            Coordinate goalTask16 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621491, 5571596, CoordinateHelpers.ConvertToMeter(1024));
            Coordinate goalTask17 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 618960, 5571878, CoordinateHelpers.ConvertToMeter(984));
            Coordinate goalTask18 = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 615538, 5572173, CoordinateHelpers.ConvertToMeter(1063));
            Coordinate goalTask19A = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 611872, 5573846, CoordinateHelpers.ConvertToMeter(1006));
            Coordinate goalTask19B = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 610683, 5571427, CoordinateHelpers.ConvertToMeter(1009));

            using (StreamWriter writer = new StreamWriter(@"C:\temp\Fight5_ILPChecks.csv"))
            {
                writer.WriteLine("Pilot Number,ILP Easting, ILP Northing, ILP Time,G15 easting, G15 northing, ILP->G15, ILP->G15 ok,G16 easting, G16 northing,ILP->G16, ILP->G16 ok, G17 easting, GT17 northing,ILP->G17,ILP->G17 ok , G18 easting, G18 northing,ILP->G18,ILP->G18 ok, G19A easting, G19A northing,ILP->G19,ILP->G19A ok, G19B easting, G19B northing,ILP->G19B,ILP->G19B ok");
                foreach (Track currentTrack in tracks)
                {
                    writer.WriteLine(Flight5ILPCheck(currentTrack, new List<Coordinate>() { goalTask15, goalTask16, goalTask17, goalTask18, goalTask19A, goalTask19B }));
                }

            }

            using (StreamWriter writer15 = new StreamWriter(@"C:\temp\F5_Taks15_results.csv"))
            {
                writer15.WriteLine("Pilot Number, Result, Comment");
                foreach (Track currentTrack in tracks)
                {
                    writer15.WriteLine(CalculateTask15(currentTrack, goalTask15));
                }
            }

            using (StreamWriter writer16 = new StreamWriter(@"C:\temp\F5_Taks16_results.csv"))
            {
                writer16.WriteLine("Pilot Number, Result, Comment");
                foreach (Track currentTrack in tracks)
                {
                    writer16.WriteLine(CalculateTask16(currentTrack, goalTask16));
                }
            }

            using (StreamWriter writer17 = new StreamWriter(@"C:\temp\F5_Taks17_results.csv"))
            {
                writer17.WriteLine("Pilot Number, Result, Comment");
                foreach (Track currentTrack in tracks)
                {
                    writer17.WriteLine(CalculateTask17(currentTrack, goalTask17));
                }
            }

            using (StreamWriter writer18 = new StreamWriter(@"C:\temp\F5_Taks18_results.csv"))
            {
                writer18.WriteLine("Pilot Number, Result, Comment");
                foreach (Track currentTrack in tracks)
                {
                    writer18.WriteLine(CalculateTask18(currentTrack, goalTask18));
                }
            }

            using (StreamWriter writer19 = new StreamWriter(@"C:\temp\F5_Taks19_results.csv"))
            {
                writer19.WriteLine("Pilot Number, Result, Comment");
                foreach (Track currentTrack in tracks)
                {
                    writer19.WriteLine(CalculateTask19(currentTrack, goalTask19A, goalTask19B));
                }
            }

            using (StreamWriter writer20 = new StreamWriter(@"C:\temp\F5_Taks20_results.csv"))
            {
                writer20.WriteLine("Pilot Number, Result, G easting,G northing,G alt [m], Dec->G,G->other Goals (min),G-> other Goals (max),M easting, M northing,M alt [m],comment");
                foreach (Track currentTrack in tracks)
                {
                    writer20.WriteLine(CalculateTask20(currentTrack, new List<Coordinate>() { goalTask15, goalTask16, goalTask17, goalTask18, goalTask19A, goalTask19B }));
                }
            }

            using (StreamWriter writer21 = new StreamWriter(@"C:\temp\F5_Taks21_results.csv"))
            {
                writer21.WriteLine("Pilot Number, Result, G easting,G northing,G alt [m], Dec->G,G->other Goals (min),G-> other Goals (max),M easting, M northing,M alt [m],comment");
                foreach (Track currentTrack in tracks)
                {
                    writer21.WriteLine(CalculateTask21(currentTrack, new List<Coordinate>() { goalTask15, goalTask16, goalTask17, goalTask18, goalTask19A, goalTask19B }));
                }
            }
        }

        private static string Flight5ILPCheck(Track track, List<Coordinate> goals)
        {
            if (TrackHelpers.CheckLaunchConstraints(track, true, new DateTime(2022, 8, 13, 3, 30, 0), new DateTime(2022, 8, 13, 5, 30, 0), goals, 1000, double.NaN, out Coordinate launchPoint, out bool launchedInStartPeriod, out List<double> distanceToGoals, out List<bool> distanceToGoalsOk))
            {
                (string utmZone, int easting, int northing) launchPointUTM = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(launchPoint);

                List<(string utmZone, int easting, int northing)> goalsUTM = new List<(string utmZone, int easting, int northing)>();
                string goalChecks = string.Empty;
                for (int index = 0; index < goals.Count; index++)
                {
                    goalsUTM.Add(CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(goals[index]));
                    goalChecks += string.Join(',', goalsUTM[index].easting, goalsUTM[index].northing, distanceToGoals[index], distanceToGoalsOk[index]);
                    goalChecks += ",";
                }


                return string.Join(',', track.Pilot.PilotNumber, launchPointUTM.easting, launchPointUTM.northing, launchPoint.TimeStamp.ToString("HH:mm:ss"), goalChecks);
            }
            else
            {
                return $"{track.Pilot.PilotNumber},failed to check ILP constraints: failed to estimate launch point";
            }
        }

        private static string CalculateTask15(Track track, Coordinate goalTask15)
        {
            (double result, string comment) = CalculateResultToGivenGoal(track, 1, goalTask15, CoordinateHelpers.ConvertToMeter(1500),75);
            return $"{track.Pilot.PilotNumber},{result:0.0#},{comment}";
        }

        private static string CalculateTask16(Track track, Coordinate goalTask16)
        {
            (double result, string comment) = CalculateResultToGivenGoal(track, 2, goalTask16, CoordinateHelpers.ConvertToMeter(1500),75);
            return $"{track.Pilot.PilotNumber},{result:0.0#},{comment}";
        }

        private static string CalculateTask17(Track track, Coordinate goalTask17)
        {
            (double result, string comment) = CalculateResultToGivenGoal(track, 3, goalTask17, CoordinateHelpers.ConvertToMeter(1500),75);
            return $"{track.Pilot.PilotNumber},{result:0.0#},{comment}";
        }

        private static string CalculateTask18(Track track, Coordinate goalTask18)
        {
            (double result, string comment) = CalculateResultToGivenGoal(track, 4, goalTask18, CoordinateHelpers.ConvertToMeter(1500),75);
            return $"{track.Pilot.PilotNumber},{result:0.0#},{comment}";
        }
        private static string CalculateTask19(Track track, Coordinate goalTask19A, Coordinate goalTask19B)
        {

            (double result19A, string comment19A) = CalculateResultToGivenGoal(track, 5, goalTask19A, CoordinateHelpers.ConvertToMeter(1500),75);
            (double result19B, string comment19B) = CalculateResultToGivenGoal(track, 5, goalTask19B, CoordinateHelpers.ConvertToMeter(1500),75);
            if (result19A < result19B)
                return $"{track.Pilot.PilotNumber},{result19A:0.0#},{comment19A}";
            else
                return $"{track.Pilot.PilotNumber},{result19B:0.0#},{comment19B}";
        }

        private static string CalculateTask20(Track track, List<Coordinate> goals)
        {
            return CalculateFlighOnResult(track, 1, 3, 6, 1000, double.NaN, goals, 1000, double.NaN);
        }

        private static string CalculateTask21(Track track, List<Coordinate> goals)
        {
            return CalculateFlighOnResult(track, 2, 3, 7, 1000, double.NaN, goals, 1000, double.NaN);
        }

        private static (double result, string comment) CalculateResultToGivenGoal(Track track, int markerNumber, Coordinate goal, double separationAltitude, double radiusMMA)
        {

            MarkerDrop marker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == markerNumber);
            double result = double.NaN;
            string comment = string.Empty;
            if (marker is not null)
            {
                result = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goal, marker.MarkerLocation, separationAltitude, true);
                if (!double.IsNaN(radiusMMA))
                {
                    if (result < radiusMMA)
                    {
                        comment = $"Electronic result {result:0.##} < MMA {radiusMMA:0.#}. Result will be set to MMA radius";
                        result = radiusMMA;
                    }
                }

            }
            else
            {
                comment = $"No marker {markerNumber} dropped by Pilot {track.Pilot.PilotNumber}";
            }

            return (result, comment);
        }

        private static string CalculateFlighOnResult(Track track, int goalNumber, int maxNumberOfDeclarations, int markerNumber, double minDistanceToPositionOfDeclaration, double maxDistanceToPositionOfDeclaration, List<Coordinate> otherGoals, double minDistanceToOtherGoals, double maxDistanceToOtherGoals)
        {
            List<Declaration> declarations = track.Declarations.Where(x => x.GoalNumber == goalNumber).OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList();

            string comment = string.Empty;
            double result = double.NaN;
            Declaration validDeclaration = null;
            double distanceBetweenPositionOfDeclarationAndDeclaredGoal = double.NaN;
            List<double> distanceToOtherGoals = new List<double>();
            MarkerDrop markerDrop = null;
            if (declarations.Count > maxNumberOfDeclarations)
            {
                comment = $"Pilot {track.Pilot.PilotNumber} declared more than 3 times goal number {goalNumber}";
            }
            else
            {

                for (int index = 0; index < declarations.Count; index++)
                {
                    bool isValid = true;
                    distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.Calculate2DDistance(declarations[index].PositionAtDeclaration, declarations[index].DeclaredGoal);
                    for (int goalIndex = 0; goalIndex < otherGoals.Count; goalIndex++)
                    {
                        distanceToOtherGoals.Add(CoordinateHelpers.Calculate2DDistance(declarations[index].DeclaredGoal, otherGoals[index]));
                    }

                    if (!double.IsNaN(minDistanceToPositionOfDeclaration))
                    {
                        if (distanceBetweenPositionOfDeclarationAndDeclaredGoal < minDistanceToPositionOfDeclaration)
                            isValid = false;
                    }
                    if (!double.IsNaN(maxDistanceToPositionOfDeclaration))
                    {
                        if (distanceBetweenPositionOfDeclarationAndDeclaredGoal > maxDistanceToPositionOfDeclaration)
                            isValid = false;
                    }
                    if (!double.IsNaN(minDistanceToOtherGoals))
                    {
                        if (distanceToOtherGoals.Min() < minDistanceToOtherGoals)
                            isValid = false;
                    }
                    if (!double.IsNaN(maxDistanceToOtherGoals))
                    {
                        if (distanceToOtherGoals.Max() > maxDistanceToOtherGoals)
                            isValid = false;
                    }


                    if (isValid)
                    {
                        validDeclaration = declarations[index];
                        break;
                    }
                }
                if (validDeclaration is null)
                {
                    comment = $"Pilot {track.Pilot.PilotNumber} declared no valid goal in goal number {goalNumber}";
                }
                else
                {
                    markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == markerNumber);
                    if (markerDrop is null)
                        comment += $"Pilot {track.Pilot.PilotNumber} didn't drop marker {markerNumber}";

                    else
                    {
                        result = CoordinateHelpers.Calculate3DDistance(validDeclaration.DeclaredGoal, markerDrop.MarkerLocation, true);
                    }
                }
            }

            (string utmZone, int easting, int northing) validDeclaredGoalUTM;
            if (validDeclaration is null)
                validDeclaredGoalUTM = ("XX", -1, -1);
            else
                validDeclaredGoalUTM = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(validDeclaration.DeclaredGoal);

            (string utmZone, int easting, int northing) markerDropUTM;
            if (markerDrop is null)
                markerDropUTM = ("XX", -1, -1);
            else
                markerDropUTM = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(markerDrop.MarkerLocation);

            return string.Join(',', track.Pilot.PilotNumber, result.ToString("0.0#"), validDeclaredGoalUTM.easting, validDeclaredGoalUTM.northing, validDeclaration?.DeclaredGoal.AltitudeGPS ?? double.NaN, distanceBetweenPositionOfDeclarationAndDeclaredGoal, (distanceToOtherGoals.Count > 0 ? distanceToOtherGoals.Min() : double.NaN), (distanceToOtherGoals.Count > 0 ? distanceToOtherGoals.Max() : double.NaN), markerDropUTM.easting, markerDropUTM.northing, markerDrop?.MarkerLocation.AltitudeGPS ?? double.NaN, comment);
        }

    }
}
