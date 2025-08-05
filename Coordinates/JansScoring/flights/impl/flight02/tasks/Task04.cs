using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using OfficeOpenXml;
using System;
using System.Windows.Forms;

namespace JansScoring.flights.impl.flight02.tasks;

public class Task04 : TaskPDG
{
    public Task04(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 04;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == DeclarationNumber());
        if (declaration == null || declaration.DeclaredGoal == null)
        {
            comment += " No declaration found. | ";
            return true;
        }
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(markerDrop => markerDrop.MarkerNumber == MarkerNumber());
        if (markerDrop == null)
        {
            comment += " No marker drop found. | ";
            return true;
        }

        if (declaration.PositionAtDeclaration.TimeStamp > markerDrop.MarkerTime)
        {
            comment += " Marker drop is before declaration. | ";
            return true;
        }
        var distanceBetweenDelcAndDecPoint = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration, Flight.getCalculationType());
        if (distanceBetweenDelcAndDecPoint < 500)
        {
            comment += $" Declaration is within 500m of marker drop. ({NumberHelper.formatDoubleToStringAndRound(distanceBetweenDelcAndDecPoint)}m) | ";
        }

        foreach (Task task in Flight.getTasks())
        {
            if (task.TaskNumber() == TaskNumber())
                continue;

            foreach (Coordinate goal in task.Goals())
            {
                var distance = CalculationHelper.Calculate2DDistance(goal,declaration.DeclaredGoal, Flight.getCalculationType());
                
                if (distance < 500)
                {
                    comment += $" Goal {task.TaskNumber()} is within 500m of declaration. ({NumberHelper.formatDoubleToStringAndRound(distance)}m) | ";
                }
            }
            
        }

        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 06, 30, 00);
    }

    public override int MarkerNumber()
    {
        return 1;
    }

    public override int DeclarationNumber()
    {
        return 1;
    }
}