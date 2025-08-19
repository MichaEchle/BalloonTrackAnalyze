using Competition;
using Competition.Penalty;
using Coordinates;
using CoordinateSharp.Formatters;
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

    public void Score(bool practiseFlight, int flightNo, int qnh)
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = flightNo;

        var flightPath = Path.Combine(root_path,
            "flight_" + (practiseFlight ? "practise_" : "") + flightNo.ToString("D2"));

        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flightPath, @"tracks\scoring")), true,
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

                foreach (Declaration decleration in track.Declarations)
                {
                    decleration.PositionAtDeclaration.AltitudeBarometric =
                        CoordinateHelpers.ConvertAltitudeQNH(decleration.PositionAtDeclaration.AltitudeBarometric, qnh);
                    ;
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

    private readonly DateTime task14EndOfScoringPeriode = new DateTime(2025, 08, 19, 8, 00, 00);
    private readonly DateTime task15EndOfScoringPeriode = new DateTime(2025, 08, 19, 8, 00, 00);

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
                Console.WriteLine($"{track.Pilot.PilotNumber}: Logger Mark #{task14Marker} and #{task15Marker} are at the same goal #{goalMarker1}");
            }
        }
    }
    
    private Dictionary<string, Coordinate> Flight4HWZTargets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["1"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0507149, 5326599, CoordinateHelpers.ConvertToMeter(933)),
            ["2"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0507569, 5326096, 283),
            ["3"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0506201, 5325996, CoordinateHelpers.ConvertToMeter(960)),
            ["4"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 0504874, 5327058, CoordinateHelpers.ConvertToMeter(1100))
        };
    }
    #endregion
}