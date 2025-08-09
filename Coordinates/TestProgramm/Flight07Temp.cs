using Competition;
using Competition.Penalty;
using Competition.Tasks;
using Competition.Validation;
using Coordinates;

namespace TestProgramm;

public class Flight07Temp
{
    internal void Flight07()
    {
    }

    public void CheckAndResultsTask24(Flight flight)
    {
        int markerNumber = 1;
        DateTime endOfScoringPeriode = new DateTime(2025, 08, 09, 06, 00, 00);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? firstMarkerDrop = track.GetFirstMarkerDrop(markerNumber);

            if (firstMarkerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker {markerNumber} was not dropped.");
                continue;
            }


            if (firstMarkerDrop.MarkerLocation.TimeStamp > endOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped after the end of scoring periode.");
                continue;
            }

            double distance =
                CoordinateHelpers.Calculate2DDistanceHavercos(firstMarkerDrop.MarkerLocation, Flight7Targets()["T24"]);
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped at {firstMarkerDrop.MarkerLocation} with a distance of {Math.Round(distance, 2, MidpointRounding.AwayFromZero)}m.");
        }
    }

    private void ChecksTask25(Flight flight)
    {
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            int goalNumber = 1;
            int markerNumber = 2;
            Declaration? lastDeclaration = track.GetLatestDeclaration(goalNumber);

            if (lastDeclaration == null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: has no declaration in goal {goalNumber}.");
                continue;
            }

            MarkerDrop? markerDrop =
                track.MarkerDrops.FirstOrDefault(markerDrop => markerDrop.MarkerNumber == markerNumber);

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
            //CHECK Check minimum distance between declared goal and previous goal

            if (track.GetFirstMarkerDrop(1) != null)
            {
                bool success =
                    PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                        track.GetFirstMarkerDrop(1)!.MarkerLocation, lastDeclaration.DeclaredGoal, 1000, Double.NaN,
                        out bool hasInfringement,
                        out double distanceInfringement,
                        out double penaltyAtPosition,
                        out double distanceBetweenPosition);

                if (success && hasInfringement)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPosition}m between declared goal and prev. goal. ({distanceInfringement}% -> {penaltyAtPosition} pts)");
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
                    $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenPositionAtDeclarationAndDeclaredGoal}m between declaration point and declared goal. ({distanceInfringementAtPositionAtDeclarationAndDeclaredGoal}% -> {penaltyAtPositionAtDeclarationAndDeclaredGoal} pts)");
            }

            //CHECK Check minimum distance between declared goals and director set goals

            foreach (var keyValuePair in Flight7Targets())
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
                        $"{track.Pilot.PilotNumber}: declaration has infringement. has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and set goal {keyValuePair.Key}. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
                }
            }
        }
    }


    public void CheckAndResultsTask26(Flight flight)
    {
        int markerNumber = 3;
        DateTime endOfScoringPeriode = new DateTime(2025, 08, 09, 06, 00, 00);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? firstMarkerDrop = track.GetFirstMarkerDrop(markerNumber);

            if (firstMarkerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker {markerNumber} was not dropped.");
                continue;
            }


            if (firstMarkerDrop.MarkerLocation.TimeStamp > endOfScoringPeriode)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped after the end of scoring periode.");
                continue;
            }

            double distance =
                CoordinateHelpers.Calculate2DDistanceHavercos(firstMarkerDrop.MarkerLocation, Flight7Targets()["T26"]);
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped at {firstMarkerDrop.MarkerLocation} with a distance of {Math.Round(distance, 2, MidpointRounding.AwayFromZero)}m.");
        }
    }

    public void CheckTask27(Flight flight)
    {
        int markerNumber = 4;
        DateTime endOfScoringPeriod = new DateTime(2025, 08, 09, 06, 00, 00);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? firstMarkerDrop = track.GetFirstMarkerDrop(markerNumber);

            if (firstMarkerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker {markerNumber} was not dropped.");
                continue;
            }


            if (firstMarkerDrop.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped after the end of scoring periode.");
                continue;
            }
        }
    }

    public void CheckTask28(Flight flight)
    {
        int markerNumber = 5;
        DateTime endOfScoringPeriod = new DateTime(2025, 08, 09, 06, 30, 00);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            MarkerDrop? firstMarkerDrop = track.GetFirstMarkerDrop(markerNumber);

            if (firstMarkerDrop is null)
            {
                Console.WriteLine($"{track.Pilot.PilotNumber}: Marker {markerNumber} was not dropped.");
                continue;
            }


            if (firstMarkerDrop.MarkerLocation.TimeStamp > endOfScoringPeriod)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Marker {markerNumber} was dropped after the end of scoring periode.");
                continue;
            }
        }
    }


    public void CheckAndResultsTask29(Flight flight)
    {
        int declarationNumber = 2;
        DonutTask donut = new DonutTask();
        donut.SetupDonut(29, 2, Int32.MaxValue, 1000, 2000, Double.MinValue, Double.MaxValue, true,
            new EmptyValidationRule(), ValidationStrictnessType.FirstValid);

        DateTime endOfScoringPeriod = new DateTime(2025, 08, 09, 06, 30, 00);
        foreach (Track? track in flight.Tracks.OrderBy(x => x.Pilot.PilotNumber))
        {
            if (track is null)
            {
                continue;
            }

            Declaration? declaration = track.Declarations.Where(currentDeclaration =>
            {
                if (currentDeclaration.GoalNumber != declarationNumber)
                {
                    return false;
                }

                if (currentDeclaration.OrignalNorhtingDeclarationUTM != 2500)
                {
                    Console.WriteLine(
                        $"{track.Pilot.PilotNumber}: Declaration {declarationNumber} was not declared at EW-Gridline 2500. Declared at {currentDeclaration.OrignalNorhtingDeclarationUTM}.");
                    return false;
                }

                return true;
            }).FirstOrDefault();

            if (declaration is null)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declaration {declarationNumber} was not declared (or only invalid declarations).");
                continue;
            }

            bool isPenaltyCheckSuccessfully =
                PenaltyCalculation.CheckForSingle2DDistanceInfringementAndCalculatePenaltyPoints(
                    declaration.DeclaredGoal, declaration.PositionAtDeclaration, 3000, Double.NaN,
                    out bool hasInfringement,
                    out double distanceInfringementBetweenDeclarationAndDirectorGoal,
                    out double penaltyAtDistanceBetweenDeclarationAndDirectorGoal,
                    out double distanceBetweenDeclarationAndDirectorGoal
                );

            if (isPenaltyCheckSuccessfully && hasInfringement)
            {
                Console.WriteLine(
                    $"{track.Pilot.PilotNumber}: Declaration has infringement. has a distance of {distanceBetweenDeclarationAndDirectorGoal}m between declaration point and declared goal. ({distanceInfringementBetweenDeclarationAndDirectorGoal}% -> {penaltyAtDistanceBetweenDeclarationAndDirectorGoal} pts)");
            }

            donut.CalculateResults(track, true, out double result);
            Console.WriteLine(
                $"{track.Pilot.PilotNumber}: The distance in the donut is {Math.Round(result, 2, MidpointRounding.AwayFromZero)}m.");
        }
    }


    private Dictionary<string, Coordinate> Flight7Targets()
    {
        return new Dictionary<string, Coordinate>
        {
            ["T24"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 617504, 5519868, 284),
            ["T26"] =
                CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621014, 5520814,
                    CoordinateHelpers.ConvertToMeter(994)),
            ["T27a"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621221, 5520641, 296),
            ["T27b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621221, 5520641, 296),
            ["T28a"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623771, 5521780, 285),
            ["T28b"] = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 623876, 5521351,
                CoordinateHelpers.ConvertToMeter(937)),
        };
    }
}