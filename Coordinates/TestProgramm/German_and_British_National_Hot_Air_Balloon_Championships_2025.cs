using Competition;
using Competition.Penalty;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;
using OfficeOpenXml;
using System.Drawing;

namespace TestProgramm;

internal class German_and_British_National_Hot_Air_Balloon_Championships_2025
{
    private readonly string root_path = @"C:\Users\micechle\Nextcloud\";
    private readonly string flight_path = @"2025 DM Burgebrach\scoring\flights\Flight_01\";
    internal void Flight1()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 1;
        _ = flight.MapPilotNamesToTracks(@".\PilotsMapping.csv");
        if (!flight.ParseTrackFiles(Path.Combine(root_path, Path.Combine(flight_path, @"tracks\scoring")), true))
        {
            Console.WriteLine("Failed to parse track files for flight 1");
        }
        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToMeter(1800));

        //ChecksAndResultsTask1(flight);
        //ResultsTask2(flight);
        //ResultsTask3(flight);
        //ResultsTask4(flight);
        //ChecksAndResultTask5(flight);
        ChecksAndResultsTask6(flight);
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

            //TODO Check if declaration is before takeoff 
            bool status = TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchCoordinate,
                out Coordinate landingCoordinate);

            if (!status)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}:Could not calculate launch coordinate. could not check if declaration is before takeoff.");
            }
            else
            {
                if (launchCoordinate.TimeStamp < lastDeclaration.PositionAtDeclaration.TimeStamp)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}:Declaration from pilot {track.Pilot.PilotNumber} is after takeoff.");
                }
            }

            //TODO Check min distance from dec point to dec

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


            //TODO Check minimum distance from any goals set by director


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


            double result = CoordinateHelpers.Calculate3DDistance(lastDeclaration.DeclaredGoal,lastMarker.MarkerLocation, true);
            csvOutput += $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero).ToString().Replace("," , ".")}\n";
        }
        File.WriteAllText(Path.Combine(root_path, Path.Combine(flight_path , "results")) + @$"\task01-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
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

            //TODO Check minimum distance between previous marker and declared goal

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

            //TODO Check minimum distance between declaration point and declared goals

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

            //TODO Check minimum distance between declared goals and director set goals

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
            Shapes.Shapes2D.Circle circle = new (decT6.DeclaredGoal, 3000);
            if (circle.IsWithin(lastDeclaration.DeclaredGoal))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Goal declared in {lastDeclaration.GoalNumber} in within the Donut");
            }
            
            double result = CoordinateHelpers.Calculate3DDistance(lastDeclaration.DeclaredGoal,markerDrop.MarkerLocation, true);
            csvOutput += $"{track.Pilot.PilotNumber},{Math.Round(result, 2, MidpointRounding.AwayFromZero).ToString().Replace("," , ".")}\n";

        }
        File.WriteAllText(Path.Combine(root_path, Path.Combine(flight_path , "results")) + @$"\task05-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.csv", csvOutput);
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
            if (!TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate launchPoint, out Coordinate landingPoint))
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
                Console.WriteLine($"{track.Pilot.PilotNumber}: More than one declaration for goal 3. Latest will be used");
            }
            Declaration declaration = track.GetLatestDeclaration(goalNumber);
            if (declaration.OrignalEastingDeclarationUTM == 2800)
            {

                string utmZone;
                int easting;
                int northing;
                if (declaration.PositionAtDeclaration.TimeStamp < launchPoint.TimeStamp)
                {
                    (utmZone, easting, northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(launchPoint);
                }
                else
                {
                    (utmZone, easting, northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(declaration.PositionAtDeclaration);
                }
                if (easting <= 623000)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: Declaration is valid: Declared goal before NS 2300 and on NS 2800");
                }
                else
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}: Declaration is invalid: Declared goal NOT BEFORE NS 2300 and on NS 2800");
                    continue;
                }
            }
            else
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Declaration is invalid: Declared goal NOT ON NS 2800");
                continue;
            }

            //remove all track points after 2025-08-06 06:30:00 (UTC)
            int removed=track.TrackPoints.RemoveAll(x => x.TimeStamp > new DateTime(2025, 08, 06, 06, 30, 0));

            if (!donutTask.CalculateResults(track, true, out double result))
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Failed to calculate results for track 6");
                continue;
            }
            Console.WriteLine($"{track.Pilot.PilotNumber}: Result for task 6: {Math.Round(result, 0, MidpointRounding.AwayFromZero)} [m]");
        }
    }

    private void ResultsTask2(Flight flight)
    {
        Coordinate targetCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 615641, 5526272, CoordinateHelpers.ConvertToMeter(942));

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
            double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(targetCoordinate, markerDrop.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);

            Console.WriteLine($"{track.Pilot.PilotNumber}: Electronic result Task2: {(distance < 50 ? $"50[m] ({Math.Round(distance, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(distance, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }

    private void ResultsTask3(Flight flight)
    {
        Coordinate targetCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 618949, 5524732, 271);

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
            double distance = CoordinateHelpers.CalculateDistanceWithSeparationAltitude(targetCoordinate, markerDrop.MarkerLocation, CoordinateHelpers.ConvertToMeter(1800), true);

            Console.WriteLine($"{track.Pilot.PilotNumber}: Electronic result Task3: {(distance < 50 ? $"50[m] ({Math.Round(distance, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(distance, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }

    private void ResultsTask4(Flight flight)
    {
        Coordinate targetCoordinateA = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623329, 5523086, CoordinateHelpers.ConvertToMeter(918));
        Coordinate targetCoordinateB = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624480, 5521727, 286);
        Coordinate targetCoordinateC = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624169, 5520741, CoordinateHelpers.ConvertToMeter(750));

        List<Coordinate> targetCoordinates =
        [
            targetCoordinateA,
            targetCoordinateB,
            targetCoordinateC
        ];
        HesitationWaltzTask hesitationWaltzTask = new();
        hesitationWaltzTask.SetupHWZ(4, targetCoordinates, 4, DistanceCalculationType.WithSeparationAlitude, null, Competition.Validation.ValidationStrictnessType.LatestValid);
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

            Console.WriteLine($"{track.Pilot.PilotNumber}:Electronic result Task 4: {(result < 50 ? $"50[m] ({Math.Round(result, 0, MidpointRounding.AwayFromZero)})" : $"{Math.Round(result, 0, MidpointRounding.AwayFromZero)}[m]")}");
        }
    }
}