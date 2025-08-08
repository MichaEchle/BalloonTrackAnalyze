using Competition;
using Competition.Penalty;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using Shapes.Shapes2D;
using System.Drawing.Drawing2D;

namespace TestProgramm;

internal class German_and_British_National_Hot_Air_Balloon_Championships_2025
{
    private readonly string root_path = @"C:\Users\micechle\Nextcloud\";
    private readonly string flight_path = @"2025 DM Burgebrach\scoring\flights\Flight_01\";


    #region Flight01

    internal void Flight1()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 1;
        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flight_path, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for flight 1");
        }

        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1800));

        //ChecksAndResultsTask1(flight);
        //ResultsTask2(flight);
        //ResultsTask3(flight);
        //ResultsTask4(flight);
        //ChecksAndResultTask5(flight);
        ChecksAndResultsTask6(flight);
    }

    private void ChecksAndResultsTask1(Flight flight)
    {
        String csvOutput = "pilot number, result\n";
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? lastDeclaration = track.GetLatestDeclaration(1);
            if (lastDeclaration == null)
            {
                Console.WriteLine($@"{track.Pilot.PilotNumber}: has no declaration in goal 1.");

                continue;
            }

            MarkerDrop? lastMarker = track.MarkerDrops.FindLast(markerDrop => markerDrop.MarkerNumber == 1);
            if (lastMarker == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: has no marker drop in goal 1.");
                continue;
            }

            //CHECK Check if declaration is before takeoff 

            bool status = TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchCoordinate,
                out Coordinate landingCoordinate);

            if (!status)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:Could not calculate launch coordinate. could not check if declaration is before takeoff.");
            }
            else
            {
                if (launchCoordinate.TimeStamp < lastDeclaration.PositionAtDeclaration.TimeStamp)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}:Declaration from pilot {track.Pilot.PilotNumber} is after takeoff.");
                }
            }

            //CHECK Check min distance from dec point to dec

            bool success =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    lastDeclaration.PositionAtDeclaration, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (success && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }


            //CHECK Check minimum distance from any goals set by director


            for (int index = 0; index < GetDirectorGoalsFlight1().Length; index++)
            {
                Coordinate directorGoal = GetDirectorGoalsFlight1()[index];

                success =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        lastDeclaration.DeclaredGoal, directorGoal, 1000, double.NaN,
                        out hasInfringement,
                        out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                        out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                        out double distanceBetweenDeclarationAndDirectorGoal
                    );

                if (success && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and declared goal {(index + 1)}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }


            double result =
                CoordinateHelpers.Calculate3DDistance(lastDeclaration.DeclaredGoal, lastMarker.MarkerLocation, true);
            csvOutput +=
                $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero).ToString().Replace(",", ".")}\n";
        }

        File.WriteAllText(
            Path.Combine(root_path, Path.Combine(flight_path, "results")) +
            @$"\task01-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
    }

    private void ChecksAndResultTask5(Flight flight)
    {
        String csvOutput = "pilot number, result\n";

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int goalNumber = 2;
            Declaration? lastDeclaration;
            if (track.Declarations.FindAll(declaration => declaration.GoalNumber == goalNumber).Count > 3)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has more than 3 declaration in goal {goalNumber}.");

                lastDeclaration = track.Declarations.FindAll(declaration => declaration.GoalNumber == goalNumber)[2];
            }
            else
            {
                lastDeclaration = track.GetLatestDeclaration(goalNumber);
            }


            if (lastDeclaration == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has no declaration in goal {goalNumber}.");
                continue;
            }

            MarkerDrop? markerDrop = track.MarkerDrops.FirstOrDefault(markerDrop => markerDrop.MarkerNumber == 5);

            if (markerDrop == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has no marker drop in goal {goalNumber}.");
                continue;
            }

            if (lastDeclaration.PositionAtDeclaration.TimeStamp > markerDrop.MarkerLocation.TimeStamp)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: invalid declaration. Declaration after marker drop.");
            }

            //CHECK Check minimum distance between previous marker and declared goal

            MarkerDrop? previousMarker = track.MarkerDrops
                .FindAll(drop => drop.MarkerLocation.TimeStamp < markerDrop.MarkerLocation.TimeStamp)
                .OrderBy(drop => drop.MarkerLocation.TimeStamp).Last();
            if (previousMarker != null)
            {
                bool success =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        previousMarker.MarkerLocation, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                        out bool hasInfringement,
                        out double infringementAtDistanceToPreviousMarker,
                        out double penaltyAtDistanceToPreviousMarker,
                        out double distanceBetweenDistanceToPreviousMarker);

                if (success && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenDistanceToPreviousMarker}m between previous marker and declared goal. ({infringementAtDistanceToPreviousMarker}% -> {penaltyAtDistanceToPreviousMarker} pts)");
                }
            }

            //CHECK Check minimum distance between declaration point and declared goals

            bool success2 =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    lastDeclaration.PositionAtDeclaration, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                    out bool hasInfringement2,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (success2 && hasInfringement2)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }

            //CHECK Check minimum distance between declared goals and director set goals

            for (int index = 0; index < GetDirectorGoalsFlight1().Length; index++)
            {
                Coordinate directorGoal = GetDirectorGoalsFlight1()[index];

                bool success3 =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        lastDeclaration.DeclaredGoal, directorGoal, 1000, Double.NaN,
                        out bool hasInfringement3,
                        out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                        out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                        out double distanceBetweenDeclarationAndDirectorGoal
                    );

                if (success3 && hasInfringement3)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and declared goal {(index + 1)}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }


            //check if in task 6 donut
            int goalNumberTask6 = 3;
            Declaration decT6 = track.GetLatestDeclaration(goalNumberTask6);
            if (decT6 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration in goal {goalNumberTask6}");
                continue;
            }

            Shapes.Shapes2D.Circle circle = new(decT6.DeclaredGoal, 3000);
            if (circle.IsWithin(lastDeclaration.DeclaredGoal))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Goal declared in {lastDeclaration.GoalNumber} in within the Donut");
            }

            double result =
                CoordinateHelpers.Calculate3DDistance(lastDeclaration.DeclaredGoal, markerDrop.MarkerLocation, true);
            csvOutput +=
                $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero).ToString().Replace(",", ".")}\n";
        }

        File.WriteAllText(
            Path.Combine(root_path, Path.Combine(flight_path, "results")) +
            @$"\task05-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
    }

    private void ChecksAndResultsTask6(Flight flight)
    {
        DonutTask donutTask = new();
        int goalNumber = 3;

        donutTask.SetupDonut(6, goalNumber, 1, 1000, 3000, 0, 10000, true, null, ValidationStrictnessType.LatestValid);

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint,
                    out Coordinate landingPoint))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to estimate launch and landing");
            }

            int goalCount = track.Declarations.Where(x => x.GoalNumber == goalNumber).Count();
            if (goalCount == 0)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal 3");
                continue;
            }

            if (goalCount > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: More than one declaration for goal 3. Latest will be used");
            }

            Declaration declaration = track.GetLatestDeclaration(goalNumber);
            if (declaration.OrignalEastingDeclarationUTM == 2800)
            {
                string utmZone;
                int easting;
                int northing;
                if (declaration.PositionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                {
                    (utmZone, easting, northing) =
                        CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(launchPoint);
                }
                else
                {
                    (utmZone, easting, northing) =
                        CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(declaration.PositionAtDeclaration);
                }

                if (easting <= 623000)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Declaration is valid: Declared goal before NS 2300 and on NS 2800");
                }
                else
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Declaration is invalid: Declared goal NOT BEFORE NS 2300 and on NS 2800");
                    continue;
                }
            }
            else
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Declaration is invalid: Declared goal NOT ON NS 2800");
                continue;
            }

            //remove all track points after 2025-08-06 06:30:00 (UTC)
            int removed = track.TrackPoints.RemoveAll(x => x.TimeStamp > new DateTime(2025, 08, 06, 06, 30, 0));

            if (!donutTask.CalculateResults(track, true, out double result))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to calculate results for track 6");
                continue;
            }

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Result for task 6: {Math.Round(result, 0, MidpointRounding.AwayFromZero)} [m]");
        }
    }

    private void ResultsTask2(Flight flight)
    {
        Coordinate targetCoordinate =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 615641, 5526272,
                CoordinateHelpers.ConvertToMeter(942));

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker 2");
                continue;
            }

            double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(targetCoordinate,
                markerDrop.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Electronic result Task2: {(distance < 50 ? $"50[m] ({Math.Round(distance, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(distance, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }

    private void ResultsTask3(Flight flight)
    {
        Coordinate targetCoordinate =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 618949, 5524732, 271);

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 3);
            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker 3");
                continue;
            }

            double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(targetCoordinate,
                markerDrop.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Electronic result Task3: {(distance < 50 ? $"50[m] ({Math.Round(distance, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(distance, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }

    private void ResultsTask4(Flight flight)
    {
        Coordinate targetCoordinateA =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623329, 5523086,
                CoordinateHelpers.ConvertToMeter(918));
        Coordinate targetCoordinateB =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624480, 5521727, 286);
        Coordinate targetCoordinateC =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624169, 5520741,
                CoordinateHelpers.ConvertToMeter(750));

        List<Coordinate> targetCoordinates =
        [
            targetCoordinateA,
            targetCoordinateB,
            targetCoordinateC
        ];
        HesitationWaltzTask hesitationWaltzTask = new();
        hesitationWaltzTask.SetupHWZ(4, targetCoordinates, 4, DistanceCalculationType.WithSeparationAlitude, null,
            Competition.Validation.ValidationStrictnessType.LatestValid);
        hesitationWaltzTask.SeparationAltitude = CoordinateHelpers.ConvertToMeter(1800);


        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!hesitationWaltzTask.CalculateResults(track, true, out double result))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to calculate results for task 4");
                continue;
            }

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}:Electronic result Task 4: {(result < 50 ? $"50[m] ({Math.Round(result, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(result, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }

    private Coordinate[] GetDirectorGoalsFlight1()
    {
        return
        [
            //Task 2: JDG
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 615641, 5526272,
                CoordinateHelpers.ConvertToMeter(942)),

            //Task 3: JDG
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 618949, 5524732,
                CoordinateHelpers.ConvertToMeter(889)),

            //Task 4: HWZ
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623329, 5523086,
                CoordinateHelpers.ConvertToMeter(918)), //Height may be incorrect
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624480, 5521727,
                CoordinateHelpers.ConvertToMeter(939)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624169, 5520741,
                CoordinateHelpers.ConvertToMeter(750)) //Height may be incorrect
        ];
    }

    #endregion


    #region Flight02

    private readonly string flight2_path = @"2025 DM Burgebrach\scoring\flights\Flight_02\";

    internal void Flight2()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 2;
        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flight2_path, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for flight 1");
        }

        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1800));

        Dictionary<string, Coordinate> goals = Flight2Targets();
        HesitationWaltzTask task7 = new();
        task7.SetupHWZ(7, [.. goals.Values], 1, DistanceCalculationType.WithSeparationAlitude, null,
            ValidationStrictnessType.LatestValid);
        task7.SeparationAltitude = CoordinateHelpers.ConvertToMeter(1800);

        HesitationWaltzTask task8 = new();
        task8.SetupHWZ(8, [.. goals.Values], 2, DistanceCalculationType.WithSeparationAlitude, null,
            ValidationStrictnessType.LatestValid);
        task8.SeparationAltitude = CoordinateHelpers.ConvertToMeter(1800);

        foreach (Track track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (!TrackHelpers.CheckLaunchConstraints(track, true, new DateTime(2025, 08, 06, 17, 00, 00),
                    new DateTime(2025, 08, 06, 18, 00, 00), Flight2Targets().Values.ToList(), 1000, double.NaN,
                    out Coordinate launchPoint, out bool launchedInStartPeriod, out List<double> distanceToGoals,
                    out List<bool> distancesToGoalsOk))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to check launch constraints");
            }
            else
            {
                if (!launchedInStartPeriod)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: not launched in start period");
                }

                if (!distancesToGoalsOk.Any())
                {
                    for (int i = 0; i < distancesToGoalsOk.Count; i++)
                    {
                        if (!distancesToGoalsOk[i])
                        {
                            Console.WriteLine(
                                $"{track.Pilot.PilotNumber}: distance violation. distance between launch and goal {i + 1} is {distanceToGoals[i]}");
                        }
                    }
                }
            }


            if (!task7.CalculateResults(track, true, out double resultT7))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: failed to calculate result for Task 7");
            }
            else
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Result T7 {Math.Round(Math.Max(50.0, resultT7), 0, MidpointRounding.AwayFromZero)}[m]");
            }

            foreach (var goal in goals)
            {
                MarkerDrop? marker1 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 1);
                if (marker1 is null)
                {
                    break;
                }

                double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goal.Value,
                    marker1.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);
                if (Math.Abs(distance - resultT7) < 1.0)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: Task 7 was scored against {goal.Key}");
                }
            }


            if (!task8.CalculateResults(track, true, out double resultT8))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: failed to calculate result for Task 8");
            }
            else
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Result T8 {Math.Round(Math.Max(50.0, resultT8), 0, MidpointRounding.AwayFromZero)}[m]");
            }

            foreach (var goal in goals)
            {
                MarkerDrop? marker2 = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == 2);
                if (marker2 is null)
                {
                    break;
                }

                double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goal.Value,
                    marker2.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);
                if (Math.Abs(distance - resultT8) < 1.0)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: Task 8 was scored against {goal.Key}");
                }
            }
        }
    }

    private Dictionary<string, Coordinate> Flight2Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T7/8A"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623741, 5520990,
                    CoordinateHelpers.ConvertToMeter(864)),
            ["T7/8B"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622470, 5520952, 292),
            ["T7/8C"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 620452, 5520241,
                    CoordinateHelpers.ConvertToMeter(908)),
            ["T7/8D"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622571, 5519901,
                    CoordinateHelpers.ConvertToMeter(925)),
            ["T7/8E"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621436, 5519536,
                    CoordinateHelpers.ConvertToMeter(949)),
            ["T7/8F"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621887, 5518926,
                CoordinateHelpers.ConvertToMeter(954)),
        };
    }

    #endregion


    #region Flight03

    private readonly string flight3_path = @"2025 DM Burgebrach\scoring\flights\Flight_03\";

    internal void Flight3()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 3;

        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flight3_path, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for flight 1");
        }

        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        List<(Pilot pilot, Declaration declaration)> correctedDeclarations =
            flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1800));
        foreach ((Pilot pilot, Declaration declaration) in correctedDeclarations)
        {
            if (declaration.GoalNumber == 1)
            {
                Console.WriteLine($"{pilot.PilotNumber}: Did not declared height for goal {declaration.GoalNumber}");
            }
        }

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!TrackHelpers.CheckLaunchConstraints(track, true, new DateTime(2025, 08, 07, 04, 00, 00),
                    new DateTime(2025, 08, 07, 05, 00, 00),
                    Flight3Targets().Values.ToList(), 1500, double.NaN, out Coordinate launchPoint,
                    out bool launchedInStartPeriod, out List<double> distanceToGoals,
                    out List<bool> distancesToGoalsOk))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to check launch constraints");
            }
            else
            {
                if (!launchedInStartPeriod)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: not launched in start period");
                }

                if (!distancesToGoalsOk.Any())
                {
                    for (int i = 0; i < distancesToGoalsOk.Count; i++)
                    {
                        if (!distancesToGoalsOk[i])
                        {
                            Console.WriteLine(
                                $"{track.Pilot.PilotNumber}: distance violation. distance between launch and goal {i + 1} is {distanceToGoals[i]}");
                        }
                    }
                }
            }
        }


        ChecksTask13(flight);
    }

    private void CheckTask09(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int declarationGoalNumber = 1;
            Declaration? declaration;
            //if (track.Declarations.FindAll(declaration => declaration.GoalNumber == declarationGoalNumber).Count > 3)
            //{
            //    Console.WriteLine(
            //        $"{track.Pilot.PilotNumber}: has more than 3 declaration in goal {declarationGoalNumber}.");

            //    declaration =
            //        track.Declarations.FindAll(declaration => declaration.GoalNumber == declarationGoalNumber)[2];
            //}
            //else
            //{
            declaration = track.GetLatestDeclaration(declarationGoalNumber);
            //}
            if (declaration == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: has no declaration in goal {declarationGoalNumber}.");
                continue;
            }

            //CHECK Check Minimum and maximum distances between declaration point and declared goal

            bool success2 =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.PositionAtDeclaration, declaration.DeclaredGoal, 1000, Double.NaN,
                    out bool hasInfringement2,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (success2 && hasInfringement2)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }


            foreach (var keyValuePair in Flight3Targets())
            {
                bool successCheckForDirectorGoals =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declaration.DeclaredGoal, keyValuePair.Value, 1000, Double.NaN,
                        out bool hasInfringementWithDirectorGoals,
                        out double distanceInfringementWithDirectorGoals,
                        out double penaltyAtPositionWithDirectorGoals,
                        out double distanceBetweenPositionWithDirectorGoals);

                if (successCheckForDirectorGoals && hasInfringementWithDirectorGoals)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceInfringementWithDirectorGoals}m between declared goal and director goal #{keyValuePair.Key}. ({distanceBetweenPositionWithDirectorGoals}% -> {penaltyAtPositionWithDirectorGoals} pts)");
                }
            }
        }
    }

    private void CheckAndResultTask12(Flight flight)
    {
        string csvOutput = "pilot number, result\n";
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int fristGoalNumber = 4;
            MarkerDrop? firstMarker = track.GetFirstMarkerDrop(fristGoalNumber);
            if (firstMarker is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker {fristGoalNumber}");
                continue;
            }

            int secondGoalNumber = 5;
            MarkerDrop? secondMarker = track.GetFirstMarkerDrop(secondGoalNumber);

            if (secondMarker is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker {secondGoalNumber}");
                continue;
            }

            //CHECK Add check if the markers are in the areas and if yes if there are in the same areas

            string? boxKey = GetBoxFromCoordinate(firstMarker.MarkerLocation);
            string? boxKey2 = GetBoxFromCoordinate(secondMarker.MarkerLocation);

            if (boxKey == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No box for marker 4");
                continue;
            }

            if (boxKey2 == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No box for marker 5");
                continue;
            }

            if (boxKey == boxKey2)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Markers 4 and 5 are in the same box");
                continue;
            }

            double result =
                CoordinateHelpers.Calculate2DDistanceHavercos(firstMarker.MarkerLocation, secondMarker.MarkerLocation);
            csvOutput +=
                $"{track.Pilot.PilotNumber},{Math.Round(result, 0, MidpointRounding.AwayFromZero)}\n";
        }

        File.WriteAllText(
            Path.Combine(root_path, Path.Combine(flight3_path, "results")) +
            @$"\task12-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
    }

    private string? GetBoxFromCoordinate(Coordinate coordinate)
    {
        foreach (var boxFromCoordinate in GetBoxCoordinates())
        {
            if (boxFromCoordinate.Value.IsWithin(coordinate))
            {
                return boxFromCoordinate.Key;
            }
        }

        return null;
    }

    private Dictionary<string, Rectangle> GetBoxCoordinates()
    {
        return new Dictionary<string, Rectangle>
        {
            ["Area1"] = new Rectangle(
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625000, 5518000, 0),
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 626000, 5517000, 0)),
            ["Area2"] =
                new Rectangle(
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625000, 5520000, 0),
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 626000, 5519000, 0)
                ),
            ["Area3"] =
                new Rectangle(
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627000, 5520000, 0),
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 628000, 5519000, 0)
                ),
            ["Area4"] =
                new Rectangle(
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624000, 5523000, 0),
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625000, 5522000, 0)
                ),
            ["Area5"] =
                new Rectangle(
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622000, 5524000, 0),
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623000, 5523000, 0)
                ),
            ["Area6"] =
                new Rectangle(
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 619000, 5525000, 0),
                    CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 620000, 5524000, 0)
                ),
        };
    }

    private void ChecksTask13(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int goalNumber = 2;
            Declaration? lastDeclaration;
            if (track.Declarations.FindAll(declaration => declaration.GoalNumber == goalNumber).Count > 3)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has more than 3 declaration in goal {goalNumber}.");

                lastDeclaration = track.Declarations.FindAll(declaration => declaration.GoalNumber == goalNumber)[2];
            }
            else
            {
                lastDeclaration = track.GetLatestDeclaration(goalNumber);
            }


            if (lastDeclaration == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has no declaration in goal {goalNumber}.");
                continue;
            }

            if (lastDeclaration.OrignalEastingDeclarationUTM < 1800)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: invalid declaration. OrignalEastingDeclarationUTM is west of 1800.");
                continue;
            }

            MarkerDrop? markerDrop = track.MarkerDrops.FirstOrDefault(markerDrop => markerDrop.MarkerNumber == 6);

            if (markerDrop == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has no marker drop in goal {goalNumber}.");
                continue;
            }

            if (lastDeclaration.PositionAtDeclaration.TimeStamp > markerDrop.MarkerLocation.TimeStamp)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: invalid declaration. Declaration after marker drop.");
            }

            //CHECK Check minimum distance between declaration point and declared goals

            bool success2 =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    lastDeclaration.PositionAtDeclaration, lastDeclaration.DeclaredGoal, 2000, Double.NaN,
                    out bool hasInfringement2,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (success2 && hasInfringement2)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }

            //CHECK Check minimum distance between declared goals and director set goals

            foreach (var keyValuePair in Flight3Targets())
            {
                Coordinate directorGoal = keyValuePair.Value;

                bool success3 =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        lastDeclaration.DeclaredGoal, directorGoal, 1000, Double.NaN,
                        out bool hasInfringement3,
                        out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                        out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                        out double distanceBetweenDeclarationAndDirectorGoal
                    );

                if (success3 && hasInfringement3)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and set goal {keyValuePair.Key}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }
        }
    }

    private void ChecksAndResultTask14(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int firstGoalNumber = 6;
            MarkerDrop? marker6 = track.GetFirstMarkerDrop(firstGoalNumber);
            if (marker6 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No goal {firstGoalNumber} declared");
                continue;
            }

            int secondGoalNumber = 7;
            MarkerDrop? marker7 = track.GetFirstMarkerDrop(secondGoalNumber);
            if (marker7 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No goal {secondGoalNumber} declared");
                continue;
            }

            if (!PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    marker6.MarkerLocation, marker7.MarkerLocation, 2000, 4000,
                    out bool hasInfringement,
                    out double distanceInfringementBetweenMarker6AndMarker7,
                    out double penaltyAtDistanceBetweenMarker6AndMarker7,
                    out double distanceBetweenMarker6AndMarker7
                ))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Failed to check distance infringement between {firstGoalNumber} and {secondGoalNumber}");
            }
            else
            {
                if (hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Distance infringement between {firstGoalNumber} and {secondGoalNumber}: Distance {Math.Round(distanceBetweenMarker6AndMarker7, 0, MidpointRounding.AwayFromZero)}[m] / Infringement {distanceInfringementBetweenMarker6AndMarker7}% / Penalty {penaltyAtDistanceBetweenMarker6AndMarker7}pts");
                }
            }

            DateTime endOfScoringPeriod = new(2025, 08, 07, 06, 30, 0);
            if (marker6.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker 6 is after the end of the scoring period");
                continue;
            }

            if (marker7.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker 7 is after the end of the scoring period");
                continue;
            }

            Coordinate referenceCoordinate = CoordinateHelpers.CalculatePointWithDistanceAndBearing(
                marker6.MarkerLocation, 2000, 20);

            double angle = CoordinateHelpers.CalculateInteriorAngle(
                referenceCoordinate, marker6.MarkerLocation, marker7.MarkerLocation);

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Angle in Task14 {Math.Round(angle, 2, MidpointRounding.AwayFromZero)}[°]");
        }
    }


    private Dictionary<string, Coordinate> Flight3Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T10"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 630738, 5515301,
                    CoordinateHelpers.ConvertToMeter(903)),
            ["T11a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 626625, 5516716,
                    CoordinateHelpers.ConvertToMeter(1074)),
            ["T11b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627024, 5517875, 317),
            ["T11c"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627270, 5518021, 315),
        };
    }

    #endregion

    #region Flight04

    private Dictionary<string, Coordinate> Flight4Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T15"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625959, 5520929,
                    CoordinateHelpers.ConvertToMeter(864)),
            ["T16a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627183, 5522659,
                    CoordinateHelpers.ConvertToMeter(924)),
            ["T16b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627629, 5522464, 286),
            ["T16c"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 628107, 5522117, 277),
            ["T16d"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 628519, 5521939,
                CoordinateHelpers.ConvertToMeter(904)),
        };
    }

    private readonly string flight4_path = @"2025 DM Burgebrach\scoring\flights\Flight_04\";

    internal void Flight4()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 4;

        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flight4_path, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for flight 1");
        }

        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        List<(Pilot pilot, Declaration declaration)> correctedDeclarations =
            flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1800));
        foreach ((Pilot pilot, Declaration declaration) in correctedDeclarations)
        {
            if (declaration.GoalNumber == 1)
            {
                Console.WriteLine($"{pilot.PilotNumber}: Did not declared height for goal {declaration.GoalNumber}");
            }
        }

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!TrackHelpers.CheckLaunchConstraints(track, true, new DateTime(2025, 08, 07, 17, 00, 00),
                    new DateTime(2025, 08, 07, 18, 00, 00),
                    Flight4Targets().Values.ToList(), 1500, double.NaN, out Coordinate launchPoint,
                    out bool launchedInStartPeriod, out List<double> distanceToGoals,
                    out List<bool> distancesToGoalsOk))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to check launch constraints");
            }
            else
            {
                if (!launchedInStartPeriod)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: not launched in start period");
                }

                if (!distancesToGoalsOk.Any())
                {
                    for (int i = 0; i < distancesToGoalsOk.Count; i++)
                    {
                        if (!distancesToGoalsOk[i])
                        {
                            Console.WriteLine(
                                $"{track.Pilot.PilotNumber}: distance violation. distance between launch and goal {i + 1} is {distanceToGoals[i]}");
                        }
                    }
                }
            }

            DateTime endOfScoringPeriod = new(2025, 08, 07, 18, 30, 0);
            MarkerDrop? markerDrop1 = track.GetFirstMarkerDrop(1);
            if (markerDrop1?.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker 1 dropped after scoring period {(markerDrop1?.MarkerLocation.TimeStamp)} ");
            }

            MarkerDrop? markerDrop2 = track.GetFirstMarkerDrop(2);
            if (markerDrop2?.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker 2 dropped after scoring period {(markerDrop2?.MarkerLocation.TimeStamp)} ");
            }
        }
    }

    #endregion

    #region Flight05


    private void ChecksAndResultTask17(Flight flight)
    {
        DateTime endOfScoringPeriod = new(2025, 08, 08, 06, 00, 0);

        String csvOutput = "pilot number, result\n";

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!CalculateLandRun(17, endOfScoringPeriod, 1,2,3, track, out double result))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to calculate land run");
                continue;
            }
            csvOutput +=
                $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero)}\n";
        }

        File.WriteAllText(
            Path.Combine(root_path, Path.Combine(flight_path, "results")) +
            @$"\task17-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
    }

    private void ChecksAndResultTask21(Flight flight)
    {
        DateTime endOfScoringPeriod = new(2025, 08, 08, 07, 00, 0);

        String csvOutput = "pilot number, result\n";

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!CalculateLandRun(21, endOfScoringPeriod, 7, 8, 9, track, out double result))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to calculate land run");
                continue;
            }
            csvOutput +=
                $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero)}\n";
        }

        File.WriteAllText(
            Path.Combine(root_path, Path.Combine(flight_path, "results")) +
            @$"\task21-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
    }


    private bool CalculateLandRun(int taskNumber, DateTime endOfScoringPeriod, int pointAMarkerNumber,
        int pointBMarkerNumber, int pointCMarkerNumber, Track track, out double result)
    {
        result = -1;
        MarkerDrop? pointA = track.GetFirstMarkerDrop(pointAMarkerNumber);
        MarkerDrop? pointB = track.GetFirstMarkerDrop(pointBMarkerNumber);
        MarkerDrop? pointC = track.GetFirstMarkerDrop(pointCMarkerNumber);

        if (pointA is null)
        {
            Console.WriteLine($"{track.Pilot.PilotNumber}: No marker {pointAMarkerNumber} (A) pressed.");
            return false;
        }

        if (pointB is null)
        {
            Console.WriteLine($"{track.Pilot.PilotNumber}: No marker {pointAMarkerNumber} (B) pressed.");
            return false;
        }

        if (pointC is null)
        {
            Console.WriteLine($"{track.Pilot.PilotNumber}: No marker {pointAMarkerNumber} (C) pressed.");
            return false;
        }

        if (pointA.MarkerLocation.TimeStamp > endOfScoringPeriod)
        {
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {pointAMarkerNumber} (A) dropped after scoring period {(pointA.MarkerLocation.TimeStamp)} ");
            return false;
        }

        if (pointB.MarkerLocation.TimeStamp > endOfScoringPeriod)
        {
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {pointAMarkerNumber} (B) dropped after scoring period {(pointB.MarkerLocation.TimeStamp)} ");
            return false;
        }

        if (pointC.MarkerLocation.TimeStamp > endOfScoringPeriod)
        {
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {pointAMarkerNumber} (C) dropped after scoring period {(pointC.MarkerLocation.TimeStamp)} ");
        }

        if (pointA.MarkerLocation.TimeStamp > pointB.MarkerLocation.TimeStamp)
        {
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {pointAMarkerNumber} (A) pressed after marker {pointBMarkerNumber} (B).");
            return false;
        }

        if (pointB.MarkerLocation.TimeStamp > pointC.MarkerLocation.TimeStamp)
        {
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {pointBMarkerNumber} (B) pressed after marker {pointCMarkerNumber} (C).");
            return false;
        }

        LandRunTask landRunTask = new();
        MarkerToMarkerTimingRule validationRule = new();
        validationRule.SetupRule(pointA.MarkerLocation.TimeStamp.TimeOfDay,
            pointA.MarkerLocation.TimeStamp.AddMinutes(20).TimeOfDay,
            new List<int> { pointBMarkerNumber, pointCMarkerNumber });
        landRunTask.SetupLandRun(taskNumber, pointAMarkerNumber, pointBMarkerNumber, pointCMarkerNumber, validationRule,
            ValidationStrictnessType.First);

        landRunTask.CalculateResults(track, true, out result);
        return true;
    }
    
   
    #endregion
}