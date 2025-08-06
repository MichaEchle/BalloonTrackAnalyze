using Competition;
using Competition.Penalty;
using Coordinates;
using OfficeOpenXml;
using System.Drawing;

namespace TestProgramm;

internal class German_and_British_National_Hot_Air_Balloon_Championships_2025
{
    internal void Flight1()
    {
        Flight flight = Flight.GetInstance();
        flight.FlightNumber = 1;
        _ = flight.MapPilotNamesToTracks(@".\PilotsMapping.csv");
        //TODO parse tracks
        _ = flight.SetDefaultGoalAltitude(CoordinateHelpers.ConvertToFeet(1800));

        ChecksTask1(flight);
    }

    private Coordinate[] _directorGoals =
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


    internal void ChecksTask1(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? lastDeclaration = track.GetLatestDeclaration(1);
            if (lastDeclaration == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has no declaration in goal 1.");

                continue;
            }

            MarkerDrop? lastMarker = track.MarkerDrops.FindLast(markerDrop => markerDrop.MarkerNumber == 1);
            if (lastMarker == null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has no marker drop in goal 1.");
                continue;
            }

            //TODO Check if declaration is before takeoff 
            bool status = TrackHelpers.EstimateLaunchAndLandingTime(track, true, out Coordinate? launchCoordinate,
                out Coordinate? landingCoordinate);

            if (!status)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}\t|\tCould not calculate launch coordinate. could not check if declaration is before takeoff.");
            }
            else
            {
                if (launchCoordinate.TimeStamp < lastDeclaration.PositionAtDeclaration.TimeStamp)
                {
                    Console.WriteLine($"{track.Pilot.PilotNumber}\t|\tDeclaration from pilot {track.Pilot.PilotNumber} is after takeoff.");
                }
            }

            //TODO Check min distance from dec point to dec

            bool hasInfringementAtPositionAtDeclarationAndDeclaredGoal =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    lastDeclaration.PositionAtDeclaration, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (hasInfringementAtPositionAtDeclarationAndDeclaredGoal)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }


            //TODO Check minimum distance from any goals set by director


            for (int index = 0; index < _directorGoals.Length; index++)
            {
                Coordinate directorGoal = _directorGoals[index];

                bool hasInfringementBetweenDeclarationAndDirectorGoal =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        lastDeclaration.DeclaredGoal, directorGoal, 1000, Double.NaN,
                        out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                        out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                        out double distanceBetweenDeclarationAndDirectorGoal
                    );

                if (hasInfringementBetweenDeclarationAndDirectorGoal)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and declared goal {(index + 1)}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }
        }
    }

    internal void ChecksTask5(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? lastDeclaration;
            if (track.Declarations.FindAll(declaration => declaration.GoalNumber == 2).Count > 3)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has more than 3 declaration in goal 2.");

                lastDeclaration = track.Declarations.FindAll(declaration => declaration.GoalNumber == 2)[2];
            }
            else
            {
                lastDeclaration = track.GetLatestDeclaration(2);
            }


            if (lastDeclaration == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has no declaration in goal 2.");
                continue;
            }

            MarkerDrop? markerDrop = track.MarkerDrops.FindLast(markerDrop => markerDrop.MarkerNumber == 5);

            if (markerDrop == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has no marker drop in goal 2.");
                continue;
            }

            if (lastDeclaration.PositionAtDeclaration.TimeStamp > markerDrop.MarkerLocation.TimeStamp)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a declaration after marker drop.");
            }

            //TODO Check minimum distance between previous marker and declared goal

            MarkerDrop? previousMarker = track.MarkerDrops
                .FindAll(drop => drop.MarkerLocation.TimeStamp < markerDrop.MarkerLocation.TimeStamp)
                .OrderBy(drop => drop.MarkerLocation.TimeStamp).Last();
            if (previousMarker != null)
            {
                bool hasInfringementAtDistanceToPreviousMarker =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        previousMarker.MarkerLocation, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                        out double infringementAtDistanceToPreviousMarker, out double penaltyAtDistanceToPreviousMarker,
                        out double distanceBetweenDistanceToPreviousMarker);

                if (hasInfringementAtDistanceToPreviousMarker)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a distance of {distanceBetweenDistanceToPreviousMarker}m between previous marker and declared goal. ({infringementAtDistanceToPreviousMarker}% -> {penaltyAtDistanceToPreviousMarker} pts)");
                }
            }

            //TODO Check minimum distance between declaration point and declared goals

            bool hasInfringementAtPositionAtDeclarationAndDeclaredGoal =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    lastDeclaration.PositionAtDeclaration, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                    out double distanceInfringementAtPositionAtDeclarationAndDeclaredGoal,
                    out double penaltyAtPositionAtDeclarationAndDeclaredGoal,
                    out double distanceBetweenPositionAtDeclarationAndDeclaredGoal);

            if (hasInfringementAtPositionAtDeclarationAndDeclaredGoal)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }

            //TODO Check minimum distance between declared goals and director set goals

            for (int index = 0; index < _directorGoals.Length; index++)
            {
                Coordinate directorGoal = _directorGoals[index];

                bool hasInfringementBetweenDeclarationAndDirectorGoal =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        lastDeclaration.DeclaredGoal, directorGoal, 1000, Double.NaN,
                        out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                        out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                        out double distanceBetweenDeclarationAndDirectorGoal
                    );

                if (hasInfringementBetweenDeclarationAndDirectorGoal)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}\t|\t{track.Pilot.PilotNumber} has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and declared goal {(index + 1)}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }
        }
    }
}