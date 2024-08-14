using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.check;

public class DeclarationChecks
{
    public static void LoadDeclaration(Track track, int goalNumber, out Declaration declaration, ref string comment)
    {
        List<int> allGoalNumbers = track.GetAllGoalNumbers();
        if (allGoalNumbers == null || !allGoalNumbers.Any())
        {
            comment += "No declarations found. | ";
            declaration = null;
            return;
        }

        declaration = track.Declarations.FindLast(drop => drop.GoalNumber == goalNumber);

        if (declaration == null)
        {
            comment += $"No declarations at slot {goalNumber}. | ";
            declaration = null;
            return;
        }

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            comment += $"The declaration in {goalNumber} is not valid. | ";
            return;
        }
    }

    public static void CheckMaxReDeclaration(Track track, int goalNumber, int maxRedeclaration, ref string comment)
    {
        if (track.Declarations.FindAll(declaration1 => declaration1.GoalNumber == goalNumber).Count > maxRedeclaration)
        {
            comment += $"To many Declarations {track.Declarations.Count} | ";
        }
    }

    /*
     * Distance in meters
     */
    public static void CheckHeightBetweenDeclarationPointAndDeclaredGoal(Flight flight, Declaration declaration,
        double minHeightDifferenceInMetersToDeclaredPoint, ref string comment)
    {
        double heightDifference = flight.useGPSAltitude()
            ? Math.Abs(declaration.PositionAtDeclaration.AltitudeGPS - declaration.DeclaredGoal.AltitudeGPS)
            : Math.Abs(declaration.PositionAtDeclaration.AltitudeBarometric -
                       declaration.DeclaredGoal.AltitudeBarometric);

        if (heightDifference < minHeightDifferenceInMetersToDeclaredPoint)
        {
            comment +=
                $"Declared with to less height difference. {NumberHelper.formatDoubleToStringAndRound(heightDifference)}m difference  | Need: {NumberHelper.formatDoubleToStringAndRound(minHeightDifferenceInMetersToDeclaredPoint)}m| ";
        }
    }

    /*
     * Distance in meters
     */
    public static void CheckDistanceFromDeclarationPointToDelcaredGoal(Flight flight, Declaration declaration,
        int minDistance,
        ref string comment)
    {
        double distanceToDeclarationPoint = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal,
            declaration.PositionAtDeclaration,
            flight.getCalculationType());
        if (distanceToDeclarationPoint < minDistance)
        {
            comment +=
                $"Declared goal is to close to declaration point {NumberHelper.formatDoubleToStringAndRound(distanceToDeclarationPoint)}m | ";
        }
    }

    /*
     * Distance in meters
     */
    public static void CheckDistanceFromDelcaredGoalToAllGoals(Flight flight, Declaration declaration, int minDistance,
        ref string comment)
    {
        Dictionary<Coordinate, Task> goals = new();
        foreach (Task currentTask in flight.getTasks())
        {
            foreach (Coordinate coordinate in currentTask.Goals())
            {
                goals.Add(coordinate, currentTask);
            }
        }


        foreach (Coordinate goal in goals.Keys)
        {
            double distance = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal
                , goal, flight.getCalculationType());
            if (distance < minDistance)
            {
                comment +=
                    $"Declared goal is to close to another fixed goal [Task {goals[goal].TaskNumber()} ~ {NumberHelper.formatDoubleToStringAndRound(distance)}m] | ";
            }
        }
    }

    /*
     * Distance in meters
     */
    public static void CheckDistanceFromDelcaredGoalToOtherDeclarations(Flight flight, Track track,
        Declaration declaration,
        int minDistance, ref string comment)
    {
        Dictionary<Coordinate, Declaration> declarations = new();
        foreach (Declaration trackDeclaration in track.Declarations)
        {
            if (trackDeclaration.GoalNumber == declaration.GoalNumber)
            {
                continue;
            }

            declarations.Add(trackDeclaration.DeclaredGoal, trackDeclaration);
        }


        foreach (Coordinate currentDeclaration in declarations.Keys)
        {
            if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal
                    , currentDeclaration, flight.getCalculationType()) < minDistance)
            {
                comment +=
                    $"Declared goal is to close to another fixed goal [Goal {declarations[currentDeclaration].GoalNumber}] | ";
            }
        }
    }

    public static void CheckIfDeclarationWasBeforeMarkerDrop(Declaration declaration, MarkerDrop markerDrop,
        ref string comment)
    {
        if (declaration.PositionAtDeclaration.TimeStamp >
            markerDrop.MarkerTime)
        {
            TimeSpan timeSpan =
                declaration.PositionAtDeclaration.TimeStamp -
                markerDrop.MarkerTime;
            comment += $"Goal was declared after the marker drop. [{timeSpan.TotalSeconds}s after drop] | ";
        }
    }

    public static void CheckIfDeclarationWasWithDelayBeforeMarkerDrop(Declaration declaration, MarkerDrop markerDrop,
        int secondsInBetween,
        ref string comment)
    {
        if (declaration.PositionAtDeclaration.TimeStamp.AddSeconds(secondsInBetween) > markerDrop.MarkerTime)
        {
            TimeSpan timeSpan =
                declaration.PositionAtDeclaration.TimeStamp.AddSeconds(secondsInBetween) - markerDrop.MarkerTime;
            comment += $"Goal was declared to late for marker drop. [{timeSpan.TotalSeconds}s to soon] | ";
        }
    }
}