using Competition;
using Competition.Penalty;
using Competition.Tasks;
using Coordinates;
using CoordinateSharp.Formatters;
using System.Data;
using System.Drawing.Printing;

namespace TestProgramm;

public class EuropeansWieselburg2025
{
    private readonly bool useGPSAltitude = false;
    private readonly int separationAltitude = 1500;

    private readonly string root_path =
        @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Wieselburg\scoring\flights";

    private readonly Coordinate referencePoint =
        CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 507272, 5326702,
            CoordinateHelpers.ConvertToMeter(873));

    public void Score(bool practiseFlight, int flightNo, int qnh, string? path = null)
    {
        Thread.Sleep(1000);
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = flightNo;

        var flightPath = Path.Combine(root_path,
            "flight_" + (practiseFlight ? "practise_" : "") + flightNo.ToString("D2"));

        string trackPath = Path.Combine(root_path, Path.Combine(flightPath, @"tracks\scoring"));

        if (path is not null)
            trackPath = path;

        if (!flight.ParseTrackFiles(trackPath, true,
                referencePoint))
        {
            Console.WriteLine("Failed to parse track files for " + (practiseFlight ? "trainings " : "") + "flight " +
                              flightNo);
        }


        if (!flight.MapPilotNamesToTracks(@".\PilotsMapping.csv"))
        {
            Console.WriteLine("Failed to map pilot names to tracks");
        }

        if (!useGPSAltitude)
        {
            foreach (Track track in flight.Tracks)
            {
                foreach (Coordinate trackPoint in track.TrackPoints)
                {
                    trackPoint.AltitudeBarometric =
                        CoordinateHelpers.ConvertAltitudeQNH(trackPoint.AltitudeBarometric, qnh);
                }

                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    markerDrop.MarkerLocation.AltitudeBarometric =
                        CoordinateHelpers.ConvertAltitudeQNH(markerDrop.MarkerLocation.AltitudeBarometric, qnh);
                }

                foreach (Declaration declaration in track.Declarations)
                {
                    double original = declaration.PositionAtDeclaration.AltitudeBarometric;
                    double converted = declaration.PositionAtDeclaration.AltitudeBarometric =
                        CoordinateHelpers.ConvertAltitudeQNH(original, qnh);
                    if (track.Pilot.PilotNumber == 15)
                    {
                        Console.WriteLine($"Converted height from {original}m to {converted}m");
                    }
                }
            }
        }

        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1500));

        if (practiseFlight)
        {
            switch (flightNo)
            {
                case 2:
                    CheckPractiseFlight2(flight);
                    CheckPractise2FlightTask7(flight);
                    break;
            }

            return;
        }


        switch (flightNo)
        {
            case 1:
                CheckTask01(flight);
                break;
            case 2:
                Console.WriteLine("Flight 2");
                Console.WriteLine(
                    "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Task 03");
                CheckTask03(flight);
                Console.WriteLine(
                    "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Task 07");
                CheckAndResultTask07(flight);
                break;
            case 3:
                Console.WriteLine("Flight 3");
                CheckAndResultTask11(flight);
                break;
            case 4:
                Console.WriteLine("Flight 4");
                CheckTask14And15(flight);
                break;
            case 5:
                Console.WriteLine("Flight 5");
                CheckAndResultTask19(flight);
                break;
            case 6:
                //Console.WriteLine("Flight 6");
                //Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                //Console.WriteLine("Task 21 & 22");
                //CheckTask21And22(flight);
                //Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("Task 24");
                CheckTask24(flight);
                //Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                //Console.WriteLine("Task 25");
                //CheckAndResultTask25(flight);
                break;

            case 7:
                Console.WriteLine("Flight 7");
                CheckTask28(flight);
                break;

            case 8:
                Console.WriteLine("Flight 8");
                CheckTask33(flight);
                break;
            case 9:
                Console.WriteLine("Flight 9");
                CheckTask36(flight);
                break;
        }
    }


    #region PractiseFlight2

    private readonly int practiseFlight2Task7GoalNumber = 1;

    private void CheckPractiseFlight2(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (!TrackHelpers.CheckLaunchConstraints(track, useGPSAltitude, new DateTime(2025, 08, 16, 04, 02, 00),
                    new DateTime(2025, 08, 16, 04, 30, 00),
                    PractiseFlight2Targets().Values.ToList(), 2000, double.NaN, out Coordinate launchPoint,
                    out bool launchedInStartPeriod, out List<double> distanceToGoals,
                    out List<bool> distancesToGoalsOk))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to check launch constraints");
            }
            else
            {
                if (!launchedInStartPeriod)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: not launched in start period. Launch point: " +
                                      launchPoint.TimeStamp.ToString("HH:mm:ss"));
                }

                if (distancesToGoalsOk.Any())
                {
                    for (int i = 0; i < distancesToGoalsOk.Count; i++)
                    {
                        if (!distancesToGoalsOk[i])
                        {
                            Console.WriteLine(
                                $"{track.Pilot.PilotNumber}: distance violation. distance between launch and goal {i + 1} is {distanceToGoals[i].Round(2)}m");
                        }
                    }
                }
            }
        }
    }

    private void CheckPractise2FlightTask7(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? latestDeclaration = track.GetLatestDeclaration(practiseFlight2Task7GoalNumber);

            if (latestDeclaration is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: no declaration in #{practiseFlight2Task7GoalNumber}");
                continue;
            }

            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    latestDeclaration.PositionAtDeclaration, latestDeclaration.DeclaredGoal, 4500, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}:declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }
        }
    }

    private Dictionary<string, Coordinate> PractiseFlight2Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T4"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 504551, 5328694,
                    CoordinateHelpers.ConvertToMeter(945)),
            ["T5"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508931, 5328679,
                    CoordinateHelpers.ConvertToMeter(873)),
            ["T6"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 517811, 5327020,
                CoordinateHelpers.ConvertToMeter(889)),
        };
    }

    #endregion


    #region Flight1

    private readonly int task1GoalNumber = 1;
    private readonly DateTime task1EndOfScoringPeriode = new DateTime(2025, 08, 17, 18, 00, 00);

    private void CheckTask01(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task1GoalNumber);

            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task1GoalNumber}");
                continue;
            }

            if (markerDrop.MarkerLocation.TimeStamp > task1EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            string goal = "N/A";
            double distance = Double.MaxValue;
            foreach (var keyValuePair in Flight1Targets().Where(pair => pair.Key.StartsWith("1")))
            {
                double goalDistance =
                    CoordinateHelpers.CalculateDistanceWithSeparationAltitude(markerDrop.MarkerLocation,
                        keyValuePair.Value, separationAltitude, useGPSAltitude);
                if (distance < goalDistance)
                {
                    continue;
                }

                goal = keyValuePair.Key;
                distance = goalDistance;
            }

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker drop calculated to goal #{goal} with distance {distance.Round(2)}m");
        }
    }

    private Dictionary<string, Coordinate> Flight1Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["1a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0513580, 5328611,
                    CoordinateHelpers.ConvertToMeter(1102)),
            ["1b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0513864, 5327238,
                CoordinateHelpers.ConvertToMeter(1033)),
        };
    }

    #endregion

    #region Flight2

    private readonly int task03GoalNumber = 1;
    private readonly int task03MarkerNumber = 1;
    private readonly DateTime task03EndOfScoringPeriode = new DateTime(2025, 08, 18, 06, 30, 00);


    private readonly int task07GoalNumber = 2;
    private readonly int task07MarkerNumber = 4;
    private readonly DateTime task07EndOfScoringPeriode = new DateTime(2025, 08, 18, 07, 00, 00);

    private void CheckTask03(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? declaration = track.GetLatestDeclaration(task03GoalNumber);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no declaration in goal #{task03GoalNumber}");
                continue;
            }


            bool status = TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchCoordinate,
                out Coordinate landingCoordinate);

            if (!status)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Could not calculate launch coordinate. could not check if declaration is before takeoff.");
            }
            else
            {
                if (launchCoordinate.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Declaration from pilot {track.Pilot.PilotNumber} is after takeoff.");
                    IOrderedEnumerable<Declaration> declarations = track.Declarations
                        .Where(dec => dec.GoalNumber == task03GoalNumber)
                        .OrderBy(dec => dec.PositionAtDeclaration.TimeStamp);
                    if (declarations.Count() > 1)
                    {
                        declaration = declarations.ElementAt(declarations.Count() - 2);
                        Console.WriteLine(
                            $"{track.Pilot.PilotNumber}: Switched declaration to previous declaration in goal #{task03GoalNumber}.");
                    }
                }
            }

            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.PositionAtDeclaration, declaration.DeclaredGoal, 2000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }


            foreach (var target in Flight2Targets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        target.Value, declaration.DeclaredGoal, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                        out penaltyAtPositionAtDeclarationAndDeclaredGoal,
                        out distanceBetweenPositionAtDeclarationAndDeclaredGoal);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
                }
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task03MarkerNumber);

            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task03MarkerNumber}");
                continue;
            }

            if (markerDrop.MarkerLocation.TimeStamp > task03EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            if (markerDrop.MarkerLocation.AltitudeBarometric < 400)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop altitude below 400m. Marker drop altitude: {markerDrop.MarkerLocation.AltitudeBarometric}m");
            }
        }
    }

    private void CheckAndResultTask07(Flight flight)
    {
        CSVOutput output = new(Path.Combine(root_path, @"flight_02\results\result_t07.csv"), true);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? declaration = track.GetLatestDeclaration(task07GoalNumber);

            if (declaration is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: No declaration in goal #{task07GoalNumber}.");
                output.AppendResult(track.Pilot.PilotNumber, "NR", comment: "No declaration.");
                continue;
            }

            if (track.Declarations.Where(declaration => declaration.GoalNumber == task07GoalNumber).Count() > 2)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has more than 2 declaration in goal #{task07GoalNumber}.");
                declaration = track.Declarations.Where(declaration => declaration.GoalNumber == task07GoalNumber)
                    .ElementAt(1);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Switched declaration to declaration in goal #{task07GoalNumber}.");
            }


            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task07MarkerNumber);

            if (markerDrop is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: No marker drop for marker #{task07MarkerNumber}.");
                output.AppendResult(track.Pilot.PilotNumber, "NR", comment: "No marker drop.");
                continue;
            }

            if (markerDrop.MarkerLocation.TimeStamp > task07EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
            }

            if (declaration.DeclaredGoal.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(2000))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude below 2000ft. Declaration altitude: {declaration.DeclaredGoal.AltitudeBarometric}");
            }


            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               declaration.PositionAtDeclaration.AltitudeBarometric);
            if (heightDifference < CoordinateHelpers.ConvertToMeter(1000))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < 1000ft ). Diff {CoordinateHelpers.ConvertToFeet(heightDifference)}f Declaration altitude: {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric).Round(2)}ft, declaration point altitude: {CoordinateHelpers.ConvertToFeet(declaration.PositionAtDeclaration.AltitudeBarometric).Round(2)}ft");
            }

            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.PositionAtDeclaration, declaration.DeclaredGoal, 1500, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }

            foreach (var target in Flight2Targets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        target.Value, declaration.DeclaredGoal, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                        out penaltyAtPositionAtDeclarationAndDeclaredGoal,
                        out distanceBetweenPositionAtDeclarationAndDeclaredGoal);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
                }
            }

            foreach (Declaration declarationBeforeTask in track.Declarations.Where(dec =>
                         dec.PositionAtDeclaration.TimeStamp < declaration.PositionAtDeclaration.TimeStamp &&
                         dec.GoalNumber != task07GoalNumber))
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declarationBeforeTask.DeclaredGoal, declaration.PositionAtDeclaration, 1500, Double.NaN,
                        out hasInfringement,
                        out distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                        out penaltyAtPositionAtDeclarationAndDeclaredGoal,
                        out distanceBetweenPositionAtDeclarationAndDeclaredGoal);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal.Round(2)}m between declaration point and prior goal #{declarationBeforeTask.GoalNumber}. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal.Round(2)}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
                }
            }


            double distanceBetweenMarkAndDeclaredGoal = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation,
                declaration.DeclaredGoal, useGPSAltitude);

            double distanceBetweenDeclarationPointAndDeclaredGoal =
                CoordinateHelpers.Calculate3DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal,
                    useGPSAltitude);

            double distanceBetweenDeclarationPointAndDeclaredGoalInKM =
                distanceBetweenDeclarationPointAndDeclaredGoal / 1000;

            double ratio = distanceBetweenMarkAndDeclaredGoal / distanceBetweenDeclarationPointAndDeclaredGoalInKM;
            output.AppendResult(track.Pilot.PilotNumber, Math.Round(ratio, 2).ToString(),
                comment:
                $"Distance between marker and declared goal: {Math.Round(distanceBetweenMarkAndDeclaredGoal, 2)}m and Distance between declaration point and declared goal: {Math.Round(distanceBetweenDeclarationPointAndDeclaredGoal, 2)}m");
        }

        output.Save();
    }

    private Dictionary<string, Coordinate> Flight2Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            //TODO
            ["4"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 512470, 5329809,
                    303),
            ["5"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508931, 5328681,
                    260),
            ["6"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 506200, 5325995,
                CoordinateHelpers.ConvertToMeter(976)),
        };
    }

    #endregion

    #region Flight3

    private readonly DateTime task11EndOfScoringPeriode = new DateTime(2025, 08, 18, 18, 00, 00);
    private readonly int task11MarkerNumber1 = 3;
    private readonly int task11MarkerNumber2 = 4;

    public void CheckAndResultTask11(Flight flight)
    {
        CSVOutput output = new(Path.Combine(root_path, @"flight_03\results\result_t11.csv"), true);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop1 = track.GetFirstMarkerDrop(task11MarkerNumber1);
            MarkerDrop? markerDrop2 = track.GetFirstMarkerDrop(task11MarkerNumber2);

            if (markerDrop1 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task11MarkerNumber1}");
                continue;
            }

            if (markerDrop1.MarkerLocation.TimeStamp > task11EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop1.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            if (markerDrop2 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task11MarkerNumber2}");
                continue;
            }

            if (markerDrop2.MarkerLocation.TimeStamp > task11EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop2.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            string goalMarker1 = "N/A";
            string goalMarker2 = "N/A";

            foreach (var flight3XddTarget in Flight3XDDTargets())
            {
                if (CoordinateHelpers.Calculate2DDistanceHavercos(flight3XddTarget.Value, markerDrop1.MarkerLocation) <
                    flight3XddTarget.Value.AltitudeBarometric)
                {
                    goalMarker1 = flight3XddTarget.Key;
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Logger Mark #{task11MarkerNumber1} is at goal #{goalMarker1}");
                }

                if (CoordinateHelpers.Calculate2DDistanceHavercos(flight3XddTarget.Value, markerDrop2.MarkerLocation) <
                    flight3XddTarget.Value.AltitudeBarometric)
                {
                    goalMarker2 = flight3XddTarget.Key;
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Logger Mark #{task11MarkerNumber2} is at goal #{goalMarker1}");
                }
            }

            if (goalMarker1.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task11MarkerNumber1} is not at any goal.");
            }

            if (goalMarker2.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task11MarkerNumber2} is not at any goal.");
            }

            if (goalMarker1.Equals(goalMarker2))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Logger Mark #{task11MarkerNumber1} and #{task11MarkerNumber2} are at the same goal #{goalMarker1}");
            }

            double distance =
                CoordinateHelpers.Calculate2DDistanceHavercos(markerDrop1.MarkerLocation, markerDrop2.MarkerLocation);

            output.AppendResult(track.Pilot.PilotNumber, distance.Round(2).ToString(),
                goalMarker1 + " | " + goalMarker2);
        }

        output.Save();
    }

    private Dictionary<string, Coordinate> Flight3XDDTargets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["1"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 502600, 5329151, 150),
            ["2"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 499900, 5329400, 75),
            ["3"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 500100, 5327350, 75),
            ["4"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 500900, 5325850, 75),
            ["5"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 505500, 5325800, 75),
        };
    }

    #endregion

    #region Flight4

    private readonly DateTime task14EndOfScoringPeriode = new(2025, 08, 19, 8, 00, 00);
    private readonly DateTime task15EndOfScoringPeriode = new(2025, 08, 19, 8, 00, 00);

    private readonly int task14Marker = 3;
    private readonly int task15Marker = 4;

    public void CheckTask14And15(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop1 = track.GetFirstMarkerDrop(task14Marker);
            MarkerDrop? markerDrop2 = track.GetFirstMarkerDrop(task15Marker);

            if (markerDrop1 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task14Marker}");
                continue;
            }

            if (markerDrop1.MarkerLocation.TimeStamp > task14EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop1.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            if (markerDrop2 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task15Marker}");
                continue;
            }

            if (markerDrop2.MarkerLocation.TimeStamp > task15EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop2.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            string goalMarker1 = "N/A";
            double distanceMarker1 = Double.MaxValue;
            string goalMarker2 = "N/A";
            double distanceMarker2 = Double.MaxValue;

            double distance;
            foreach (var targets in Flight4HWZTargets())
            {
                distance = CoordinateHelpers.Calculate2DDistanceHavercos(targets.Value, markerDrop1.MarkerLocation);
                if (distance < distanceMarker1)
                {
                    goalMarker1 = targets.Key;
                    distanceMarker1 = distance;
                }

                distance = CoordinateHelpers.Calculate2DDistanceHavercos(targets.Value, markerDrop2.MarkerLocation);
                if (distance < distanceMarker2)
                {
                    goalMarker2 = targets.Key;
                    distanceMarker2 = distance;
                }
            }

            if (goalMarker1.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task14Marker} is not at any goal.");
            }

            if (goalMarker2.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task15Marker} is not at any goal.");
            }

            if (goalMarker1.Equals(goalMarker2))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Logger Mark #{task14Marker} and #{task15Marker} are at the same goal #{goalMarker1}");
            }
        }
    }

    private Dictionary<string, Coordinate> Flight4HWZTargets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["1"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0507149, 5326599,
                    CoordinateHelpers.ConvertToMeter(933)),
            ["2"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0507569, 5326096, 283),
            ["3"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0506201, 5325996,
                    CoordinateHelpers.ConvertToMeter(960)),
            ["4"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0504874, 5327058,
                CoordinateHelpers.ConvertToMeter(1100))
        };
    }

    #endregion

    #region Flight5

    public void CheckAndResultTask19(Flight flight)
    {
        CSVOutput output = new(Path.Combine(root_path, @"flight_05\results\result_t19.csv"), true);

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Coordinate firstTrackpointAfterGridlineCrossing = null;
            foreach (Coordinate trackPoint in track.TrackPoints)
            {
                (string _, int easting, int _) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(trackPoint);
                if (easting < 485000) //Remove 0|0 spike
                {
                    continue;
                }

                if (easting < gridline)
                {
                    firstTrackpointAfterGridlineCrossing = trackPoint;
                    break;
                }
            }

            if (firstTrackpointAfterGridlineCrossing is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: No trackpoint after gridline crossing.");
                output.AppendResult(track.Pilot.PilotNumber, "NR");
                continue;
            }

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: First trackpoint after gridline crossing: {firstTrackpointAfterGridlineCrossing.ToString()}");

            DateTime endOffMDT = firstTrackpointAfterGridlineCrossing.TimeStamp.AddMinutes(25);
            Coordinate firstTrackpointAfter25Minutes = null;
            foreach (Coordinate trackPoint in track.TrackPoints)
            {
                if (trackPoint.TimeStamp > endOffMDT)
                {
                    firstTrackpointAfter25Minutes = trackPoint;
                    break;
                }
            }

            if (firstTrackpointAfter25Minutes is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: No trackpoint after 25 minutes.");
                output.AppendResult(track.Pilot.PilotNumber, "NR");
                continue;
            }

            TrackHelpers.EstimateLaunchAndLandingTime(track, useGPSAltitude, out Coordinate? _,
                out Coordinate? landing);
            Console.WriteLine($"{track.Pilot.PilotNumber}: Landing: " + landing.ToString());
            ;
            if (firstTrackpointAfter25Minutes.TimeStamp > landing.TimeStamp)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: First trackpoint after 25 minutes is after landing.");
            }

            if (firstTrackpointAfter25Minutes.TimeStamp > task19EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Trackpoint after 25 minutes is after end of scoring period: {firstTrackpointAfter25Minutes.ToString()}");
                output.AppendResult(track.Pilot.PilotNumber, "NR");
                continue;
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(1);
            if (markerDrop is not null &&
                markerDrop.MarkerLocation.TimeStamp < firstTrackpointAfter25Minutes.TimeStamp &&
                markerDrop.MarkerLocation.TimeStamp > firstTrackpointAfterGridlineCrossing.TimeStamp)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Markerdrop #1 in 25 min. " + markerDrop.MarkerLocation);
                ;
            }

            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: First trackpoint after 25 minutes: {firstTrackpointAfter25Minutes.ToString()}");

            double distance =
                CoordinateHelpers.Calculate2DDistanceHavercos(task19ReferencePoint, firstTrackpointAfter25Minutes);
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Distance: {distance.Round(2)}m");
            output.AppendResult(track.Pilot.PilotNumber, distance.Round(2).ToString());
        }

        output.Save();
    }

    private double gridline = 504000;
    private readonly DateTime task19EndOfScoringPeriode = new DateTime(2025, 08, 19, 18, 00, 00);

    private readonly Coordinate task19ReferencePoint =
        CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508615, 5328338, 0);

    #endregion

    #region Flight6

    private readonly DateTime task21EndOfScoringPeriode = new(2025, 08, 20, 06, 00, 00);
    private readonly DateTime task22EndOfScoringPeriode = new(2025, 08, 20, 06, 00, 00);

    private readonly int task21Marker = 2;
    private readonly int task22Marker = 3;

    public void CheckTask21And22(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? markerDrop1 = track.GetFirstMarkerDrop(task21Marker);
            MarkerDrop? markerDrop2 = track.GetFirstMarkerDrop(task22Marker);

            if (markerDrop1 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task21Marker}");
                continue;
            }

            if (markerDrop1.MarkerLocation.TimeStamp > task21EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop1.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            Console.WriteLine($"{track.Pilot.PilotNumber}: Marker drop 1: " +
                              markerDrop1.MarkerLocation.TimeStamp.AddHours(2));

            if (markerDrop2 is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: no marker drop in goal #{task22Marker}");
                continue;
            }

            if (markerDrop2.MarkerLocation.TimeStamp > task22EndOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop after end of scoring period. Marker drop: {markerDrop2.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
                continue;
            }

            Console.WriteLine($"{track.Pilot.PilotNumber}: Marker drop 2: " +
                              markerDrop2.MarkerLocation.TimeStamp.AddHours(2));

            if (markerDrop2.MarkerLocation.TimeStamp < markerDrop1.MarkerLocation.TimeStamp)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: marker drop 2 is before marker drop 1. Marker drop 2: {markerDrop2.MarkerLocation.TimeStamp.ToString("HH:mm:ss")} Marker drop 1: {markerDrop1.MarkerLocation.TimeStamp.ToString("HH:mm:ss")}");
            }


            string goalMarker1 = "N/A";
            double distanceMarker1 = Double.MaxValue;
            string goalMarker2 = "N/A";
            double distanceMarker2 = Double.MaxValue;

            double distance;
            foreach (var targets in Flight6HWZTargets())
            {
                distance = CoordinateHelpers.Calculate2DDistanceHavercos(targets.Value, markerDrop1.MarkerLocation);
                if (distance < distanceMarker1)
                {
                    goalMarker1 = targets.Key;
                    distanceMarker1 = distance;
                }

                distance = CoordinateHelpers.Calculate2DDistanceHavercos(targets.Value, markerDrop2.MarkerLocation);
                if (distance < distanceMarker2)
                {
                    goalMarker2 = targets.Key;
                    distanceMarker2 = distance;
                }
            }

            if (goalMarker1.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task21Marker} is not at any goal.");
            }

            if (goalMarker2.Equals("N/A"))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task22Marker} is not at any goal.");
            }

            if (goalMarker1.Equals(goalMarker2))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Logger Mark #{task21Marker} and #{task22Marker} are at the same goal #{goalMarker1}");
            }
        }
    }

    private Dictionary<string, Coordinate> Flight6HWZTargets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 509228, 5331981,
                    CoordinateHelpers.ConvertToMeter(1064)),
            ["b"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508080, 5332013,
                    CoordinateHelpers.ConvertToMeter(936)),
            ["c"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508037, 5330889,
                    CoordinateHelpers.ConvertToMeter(986)),
            ["d"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 511331, 5330551, 263),
            ["e"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 512760, 5330859,
                    CoordinateHelpers.ConvertToMeter(892)),
            ["f"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 513321, 5330762,
                CoordinateHelpers.ConvertToMeter(995))
        };
    }

    private readonly int task24Goal = 2;
    private readonly int task24Marker = 6;

    private void CheckTask24(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (track.Declarations.Count(dec => dec.GoalNumber == task24Goal) > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Multiple declarations for goal #{task24Goal}. {track.Declarations.Count(dec => dec.GoalNumber == task24Goal)}");
            }

            Declaration? declaration = track.GetDeclarationWithMaxRedeclaration(task24Goal, 1);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal #{task24Goal}");
                continue;
            }

            bool successfullyEstimated = TrackHelpers.EstimateLaunchAndLandingTime(track, useGPSAltitude,
                out Coordinate? launchPoint, out Coordinate? _);

            Coordinate positionAtDeclaration = declaration.PositionAtDeclaration;
            if (successfullyEstimated)
            {
                //Console.WriteLine($"{track.Pilot.PilotNumber}: Launch: " + launchPoint.ToString());
                if (positionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                {
                    positionAtDeclaration = launchPoint;
                }
            }


            if (declaration.DeclaredGoal.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(3999))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declared altitude is below 4000ft. {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)}ft");
            }

            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               positionAtDeclaration.AltitudeBarometric);
            if (heightDifference < CoordinateHelpers.ConvertToMeter(1000))
            {
                PenaltyCalculation.CalculatePenaltyPoints(CoordinateHelpers.ConvertToMeter(1000), heightDifference,
                    out double infringement, out double calculatedPenalty);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < 1000ft ). Diff {CoordinateHelpers.ConvertToFeet(heightDifference)}f Declaration altitude: {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric).Round(2)}ft, declaration point altitude: {CoordinateHelpers.ConvertToFeet(positionAtDeclaration.AltitudeBarometric).Round(2)}ft ({infringement.Round(2)}% -> {calculatedPenalty} pts)");
            }


            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, positionAtDeclaration, 3000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringement,
                    out double penalty,
                    out double distance);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and declaration goal #{task24Goal}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
            }

            foreach (var target in Flight6HWZTargets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declaration.DeclaredGoal, target.Value, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringement,
                        out penalty,
                        out distance);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
                }
            }
        }
    }


    public void CheckAndResultTask25(Flight flight)
    {
        var innerBand = new AltitudeProfileTask.AltitudeBand
        {
            Type = AltitudeProfileTask.BandType.Inner, Multiplier = 2.0, Profile = CreateInnerBandProfile()
        };

        var outerBand = new AltitudeProfileTask.AltitudeBand
        {
            Type = AltitudeProfileTask.BandType.Outer, Multiplier = 1.0, Profile = CreateOuterBandProfile()
        };


        var task = new AltitudeProfileTask();
        task.SetupTimeBasedAPT(
            taskNumber: 25,
            startCondition: new AltitudeProfileTask.StartCondition
            {
                Type = AltitudeProfileTask.StartConditionType.MarkerDrop, MarkerNumber = 7
            },
            timeIntervalSeconds: 60,
            bands:
            [
                innerBand, outerBand
            ],
            maxDurationSeconds: 660
        );

        CSVOutput output = new(Path.Combine(root_path, @"flight_06\results\result_t25.csv"), true);

        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            bool success = task.CalculateResults(track, useGPSAltitude, out double result);

            if (!success)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Task 25 calc failed.");
                output.AppendResult(track.Pilot.PilotNumber, "NR");
                continue;
            }

            output.AppendResult(track.Pilot.PilotNumber, result.Round(0).ToString());
        }

        output.Save();
    }

    private static List<AltitudeProfileTask.AltitudePoint> CreateInnerBandProfile()
    {
        return new List<AltitudeProfileTask.AltitudePoint>
        {
            new()
            {
                Step = 0,
                MinAltitude = CoordinateHelpers.ConvertToMeter(4950),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5050)
            },
            new()
            {
                Step = 1,
                MinAltitude = CoordinateHelpers.ConvertToMeter(4950),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5050)
            },
            new()
            {
                Step = 2,
                MinAltitude = CoordinateHelpers.ConvertToMeter(5450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5550)
            },
            new()
            {
                Step = 3,
                MinAltitude = CoordinateHelpers.ConvertToMeter(2450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5550)
            },
            new()
            {
                Step = 5,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6550)
            },
            new()
            {
                Step = 6,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6700),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6800)
            },
            new()
            {
                Step = 7,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6700),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6800)
            },
            new()
            {
                Step = 8,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6550)
            },
            new()
            {
                Step = 9,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6550)
            },
            new()
            {
                Step = 11,
                MinAltitude = CoordinateHelpers.ConvertToMeter(5450),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5550)
            },
        };
    }

    private static List<AltitudeProfileTask.AltitudePoint> CreateOuterBandProfile()
    {
        return new List<AltitudeProfileTask.AltitudePoint>
        {
            new()
            {
                Step = 0,
                MinAltitude = CoordinateHelpers.ConvertToMeter(4900),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5100)
            },
            new()
            {
                Step = 1,
                MinAltitude = CoordinateHelpers.ConvertToMeter(4900),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5100)
            },
            new()
            {
                Step = 2,
                MinAltitude = CoordinateHelpers.ConvertToMeter(5400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5600)
            },
            new()
            {
                Step = 3,
                MinAltitude = CoordinateHelpers.ConvertToMeter(2400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5600)
            },
            new()
            {
                Step = 5,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6600)
            },
            new()
            {
                Step = 6,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6650),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6850)
            },
            new()
            {
                Step = 7,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6650),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6850)
            },
            new()
            {
                Step = 8,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6600)
            },
            new()
            {
                Step = 9,
                MinAltitude = CoordinateHelpers.ConvertToMeter(6400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(6600)
            },
            new()
            {
                Step = 11,
                MinAltitude = CoordinateHelpers.ConvertToMeter(5400),
                MaxAltitude = CoordinateHelpers.ConvertToMeter(5600)
            },
        };
    }

    #endregion

    #region Flight7

    private readonly int task28Goal = 1;

    private void CheckTask28(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (track.Declarations.Count(dec => dec.GoalNumber == task28Goal) > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Multiple declarations for goal #{task28Goal}. {track.Declarations.Count(dec => dec.GoalNumber == task28Goal)}");
            }

            Declaration? declaration = track.GetDeclarationWithMaxRedeclaration(task28Goal, 1);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal #{task28Goal}");
                continue;
            }

            bool successfullyEstimated = TrackHelpers.EstimateLaunchAndLandingTime(track, useGPSAltitude,
                out Coordinate? launchPoint, out Coordinate? _);

            Coordinate positionAtDeclaration = declaration.PositionAtDeclaration;
            if (successfullyEstimated)
            {
                //Console.WriteLine($"{track.Pilot.PilotNumber}: Launch: " + launchPoint.ToString());
                if (positionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                {
                    positionAtDeclaration = launchPoint;
                }
            }


            if (declaration.DeclaredGoal.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(1999))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declared altitude is below 4000ft. {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)}ft");
            }

            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               positionAtDeclaration.AltitudeBarometric);
            if (heightDifference < CoordinateHelpers.ConvertToMeter(1000))
            {
                PenaltyCalculation.CalculatePenaltyPoints(CoordinateHelpers.ConvertToMeter(1000), heightDifference,
                    out double infringement, out double calculatedPenalty);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < 1000ft ). Diff {CoordinateHelpers.ConvertToFeet(heightDifference)}f Declaration altitude: {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric).Round(2)}ft, declaration point altitude: {CoordinateHelpers.ConvertToFeet(positionAtDeclaration.AltitudeBarometric).Round(2)}ft ({infringement.Round(2)}% -> {calculatedPenalty} pts)");
            }


            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, positionAtDeclaration, 3000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringement,
                    out double penalty,
                    out double distance);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and declaration goal #{task28Goal}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
            }
        }
    }

    #endregion

    #region Flight8

    private readonly int task33Goal = 1;
    private readonly int task33Marker = 7;
    private readonly DateTime task33EndOfScoringPeriode = new(2025, 08, 21, 07, 0, 0);

    private void CheckTask33(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (track.Declarations.Count(dec => dec.GoalNumber == task33Goal) > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Multiple declarations for goal #{task33Goal}. {track.Declarations.Count(dec => dec.GoalNumber == task33Goal)}");
            }

            Declaration? declaration = track.GetDeclarationWithMaxRedeclaration(task33Goal, 1);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal #{task33Goal}");
                continue;
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task33Marker);
            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker #{task33Marker}");
            }

            if (markerDrop is not null &&
                markerDrop.MarkerLocation.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has declared after marker drop.");
            }

            if (markerDrop is not null && markerDrop.MarkerLocation.TimeStamp > task33EndOfScoringPeriode)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has declared after end of scoring periode.");
            }

            bool successfullyEstimated = TrackHelpers.EstimateLaunchAndLandingTime(track, useGPSAltitude,
                out Coordinate? launchPoint, out Coordinate? _);

            Coordinate positionAtDeclaration = declaration.PositionAtDeclaration;
            if (successfullyEstimated)
            {
                //Console.WriteLine($"{track.Pilot.PilotNumber}: Launch: " + launchPoint.ToString());
                if (positionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                {
                    positionAtDeclaration = launchPoint;
                    Console.WriteLine($"{track.Pilot.PilotNumber}: Launch: " + launchPoint.ToString());
                }
            }

            if (track.Pilot.PilotNumber == 15)
            {
                foreach (Declaration currentDeclaration in track.Declarations)
                {
                    Console.WriteLine(
                        $"{currentDeclaration.GoalNumber}: {currentDeclaration.PositionAtDeclaration.AltitudeBarometric}");
                }
            }

            if (declaration.DeclaredGoal.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(1999))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declared altitude is below 1999ft. {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)}ft");
            }


            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               positionAtDeclaration.AltitudeBarometric);
            if (heightDifference < CoordinateHelpers.ConvertToMeter(1500))
            {
                PenaltyCalculation.CalculatePenaltyPoints(CoordinateHelpers.ConvertToMeter(1500), heightDifference,
                    out double infringement, out double calculatedPenalty);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < {CoordinateHelpers.ConvertToMeter(1500)}m ). Diff {heightDifference}m Declaration altitude: {declaration.DeclaredGoal.AltitudeBarometric.Round(2)}m, declaration point altitude: {positionAtDeclaration.AltitudeBarometric.Round(2)}m ({infringement.Round(2)}% -> {calculatedPenalty} pts)");
            }


            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, positionAtDeclaration, 4000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringement,
                    out double penalty,
                    out double distance);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and declaration goal #{task33Goal}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
            }


            foreach (var target in Flight8Targets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declaration.DeclaredGoal, target.Value, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringement,
                        out penalty,
                        out distance);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
                }
            }
        }
    }

    private Dictionary<string, Coordinate> Flight8Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["29"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 510520, 5330681,
                    CoordinateHelpers.ConvertToMeter(833)),
            ["30"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 506643, 5330482,
                    CoordinateHelpers.ConvertToMeter(1004)),
            ["31a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 503162, 5329630,
                    CoordinateHelpers.ConvertToMeter(1013)),
            ["31b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 503330, 5329810,
                CoordinateHelpers.ConvertToMeter(996)),
        };
    }

    #endregion

    #region Flight9

    private readonly int task36Goal = 1;
    private readonly int task36Marker = 2;
    private readonly DateTime task36EndOfScoringPeriode = new(2025, 08, 22, 07, 0, 0);

    private void CheckTask36(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (track.Declarations.Count(dec => dec.GoalNumber == task36Goal) > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Multiple declarations for goal #{task36Goal}. {track.Declarations.Count(dec => dec.GoalNumber == task36Goal)}");
            }

            Declaration? declaration = track.GetDeclarationWithMaxRedeclaration(task36Goal, 1);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal #{task36Goal}");
                continue;
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task36Marker);
            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker #{task36Marker}");
            }

            if (markerDrop is not null &&
                markerDrop.MarkerLocation.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has declared after marker drop.");
            }

            if (markerDrop is not null && markerDrop.MarkerLocation.TimeStamp > task36EndOfScoringPeriode)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has declared after end of scoring periode.");
            }

            Coordinate positionAtDeclaration = declaration.PositionAtDeclaration;

            if (positionAtDeclaration.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(1000))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Position at Declaration altitude is below 1000ft. {CoordinateHelpers.ConvertToFeet(positionAtDeclaration.AltitudeBarometric)}ft");
            }

            if (declaration.DeclaredGoal.AltitudeBarometric < CoordinateHelpers.ConvertToMeter(1500))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declared altitude is below 1500ft. {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)}ft");
            }

            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               positionAtDeclaration.AltitudeBarometric);

            if (heightDifference < CoordinateHelpers.ConvertToMeter(500))
            {
                PenaltyCalculation.CalculatePenaltyPoints(CoordinateHelpers.ConvertToMeter(500), heightDifference,
                    out double infringement, out double calculatedPenalty);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < {CoordinateHelpers.ConvertToMeter(500)}m ). Diff {heightDifference}m Declaration altitude: {declaration.DeclaredGoal.AltitudeBarometric.Round(2)}m, declaration point altitude: {positionAtDeclaration.AltitudeBarometric.Round(2)}m ({infringement.Round(2)}% -> {calculatedPenalty} pts)");
            }


            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, positionAtDeclaration, 2000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringement,
                    out double penalty,
                    out double distance);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and declaration goal #{task36Goal}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
            }


            foreach (var target in Flight9Targets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declaration.DeclaredGoal, target.Value, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringement,
                        out penalty,
                        out distance);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
                }
            }
        }
    }

    private Dictionary<string, Coordinate> Flight9Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["35a"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("36U", 513240, 5329430,
                    CoordinateHelpers.ConvertToMeter(1067)),
            ["35b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("36U", 513380, 5328941, 352),
            ["35c"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("36U", 513860, 5328530, 338),
        };
    }

    #endregion


    #region Flight10

    private readonly int task40Goal = 1;
    private readonly int task40Marker = 3;
    private readonly DateTime task40EndOfScoringPeriode = new(2025, 08, 22, 18, 00,00);

    private void CheckTask40(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            if (track.Declarations.Count(dec => dec.GoalNumber == task40Goal) > 1)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Multiple declarations for goal #{task40Goal}. {track.Declarations.Count(dec => dec.GoalNumber == task40Goal)}");
            }

            Declaration? declaration = track.GetDeclarationWithMaxRedeclaration(task40Goal, 1);

            if (declaration is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No declaration for goal #{task40Goal}");
                continue;
            }

            MarkerDrop? markerDrop = track.GetFirstMarkerDrop(task40Marker);
            if (markerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: No marker drop for marker #{task40Marker}");
            }

            if (markerDrop is not null &&
                markerDrop.MarkerLocation.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has declared after marker drop.");
            }

            if (markerDrop is not null && markerDrop.MarkerLocation.TimeStamp > task40EndOfScoringPeriode)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Pilot has markered after end of scoring periode.");
            }

            Coordinate positionAtDeclaration = declaration.PositionAtDeclaration;

            if (positionAtDeclaration.AltitudeBarometric <= CoordinateHelpers.ConvertToMeter(1000))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Position at Declaration altitude is below 1000ft. {CoordinateHelpers.ConvertToFeet(positionAtDeclaration.AltitudeBarometric)}ft");
            }

            if (declaration.DeclaredGoal.AltitudeBarometric < CoordinateHelpers.ConvertToMeter(1500))
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declared altitude is below 1500ft. {CoordinateHelpers.ConvertToFeet(declaration.DeclaredGoal.AltitudeBarometric)}ft");
            }

            double heightDifference = Math.Abs(declaration.DeclaredGoal.AltitudeBarometric -
                                               positionAtDeclaration.AltitudeBarometric);

            if (heightDifference < CoordinateHelpers.ConvertToMeter(500))
            {
                PenaltyCalculation.CalculatePenaltyPoints(CoordinateHelpers.ConvertToMeter(500), heightDifference,
                    out double infringement, out double calculatedPenalty);
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration altitude close to declaration point altitude ( < {CoordinateHelpers.ConvertToMeter(500).Round(2)}m ). Diff {heightDifference}m Declaration altitude: {declaration.DeclaredGoal.AltitudeBarometric.Round(2)}m, declaration point altitude: {positionAtDeclaration.AltitudeBarometric.Round(2)}m ({infringement.Round(2)}% -> {calculatedPenalty} pts)");
            }


            bool successfullyChecked =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, positionAtDeclaration, 3000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringement,
                    out double penalty,
                    out double distance);

            if (successfullyChecked && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and declaration goal #{task40Goal}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
            }


            foreach (var target in Flight10Targets())
            {
                successfullyChecked =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        declaration.DeclaredGoal, target.Value, 1000, Double.NaN,
                        out hasInfringement,
                        out distanceInfringement,
                        out penalty,
                        out distance);

                if (successfullyChecked && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distance.Round(2)}m between declaration point and director goal #{target.Key}. ({distanceInfringement.Round(2)}% -> {penalty} pts)");
                }
            }
        }
    }

    private Dictionary<string, Coordinate> Flight10Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            //TODO:
            ["38"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 513240, 5329430, 0),
            
            ["39a"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 512934,5331283, CoordinateHelpers.ConvertToMeter(916)),
            ["39b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 512854, 5330853, CoordinateHelpers.ConvertToMeter(888)),
        };
    }

    #endregion
}