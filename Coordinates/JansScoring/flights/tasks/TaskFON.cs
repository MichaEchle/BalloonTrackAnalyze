using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights;

public abstract class TaskFON : Task
{
    protected TaskFON(Flight flight) : base(flight)
    {
    }

    protected abstract int declerationNumber();
    protected abstract int markerDropNumber();
    protected abstract int maxDeclerations();
    protected abstract int minDistanceToOtherGoals();
    protected abstract int minTimeInSecondsToDeclarationPoint();
    protected abstract int minDistanceToDeclerationPoint();
    protected abstract double minHeightDifferenceInMetersToDelcearedPoint();

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";

        if (!track.GetAllGoalNumbers().Contains(declerationNumber()))
        {
            return new[] { "No Result", $"No declaration in {declerationNumber()}" };
        }

        Declaration declaration =
            track.Declarations.FindLast(declaration => declaration.GoalNumber == declerationNumber());

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            return new[] { "No Result", $"No valid declaration in {declerationNumber()}" };
        }

        if (!track.GetAllMarkerNumbers().Contains(markerDropNumber()))
        {
            return new[] { "No Result", $"No marker in {markerDropNumber()}" };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerDropNumber());
        if (markerDrop == null)
        {
            return new[] { "No Result", $"No marker drops at slot {markerDropNumber()} | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", $"No valid marker in slot {markerDropNumber()}" };
        }

        if (track.Declarations.Count > maxDeclerations())
        {
            comment += "To many Declarations | ";
        }

        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerDropNumber() + " outside SP | ";
        }

        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            goals.AddRange(task.goals());
        }

        foreach (Declaration trackDeclaration in track.Declarations)
        {
            if (trackDeclaration.GoalNumber == declerationNumber())
            {
                continue;
            }

            goals.Add(trackDeclaration.DeclaredGoal);
        }

        List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(declaration.DeclaredGoal,
            goals.ToArray(),
            flight.getCalculationType());
        double distanceToDeclarationPoint = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal,
            declaration.PositionAtDeclaration,
            flight.getCalculationType());

        if (distanceToDeclarationPoint < minDistanceToDeclerationPoint())
        {
            comment += $"Goal is to close to dec. point {distanceToDeclarationPoint}m | ";
        }

        foreach (double distanceToGoal in distanceToAllGoals)
        {
            if (distanceToGoal < minDistanceToOtherGoals())
            {
                comment += "Goal is to close to another goal | ";
            }
        }




        if (declaration.PositionAtDeclaration.TimeStamp.AddSeconds(minTimeInSecondsToDeclarationPoint()) >
            markerDrop.MarkerTime)
        {
            TimeSpan timeSpan =
                declaration.PositionAtDeclaration.TimeStamp.AddSeconds(minTimeInSecondsToDeclarationPoint()) -
                markerDrop.MarkerTime;
            comment += $"Goal was declared to late for marker drop. [{timeSpan.TotalSeconds}s to close] | ";
        }

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation,
            flight.useGPSAltitude(), flight.getCalculationType());

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }
}