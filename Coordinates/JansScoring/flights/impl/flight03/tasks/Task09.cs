using Coordinates;
using JansScoring.calculation;
using OfficeOpenXml;
using System;

namespace JansScoring.flights.impl.flight03.tasks;

public class Task09 : TaskFON
{
    public Task09(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 9;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == DeclarationNumber());

        if (declaration == null || declaration.DeclaredGoal == null)
        {
            comment += "No declaration found | ";
            return NO_RESULT;
        }
        
        var decDistance = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration, Flight.getCalculationType());
        if(decDistance < 1000)
            comment += $"Declaration is within 1000m of the declaration point. ({NumberHelper.formatDoubleToStringAndRound(decDistance)}m) | ";
        
        
        foreach (Task task in Flight.getTasks())
        {
            if(task.TaskNumber() == TaskNumber())
                continue;

            Coordinate[] goals = task.Goals();
            for (int index = 1; index <= goals.Length; index++)
            {
                Coordinate goal = goals[index -1 ];
                var distance =
                    CalculationHelper.Calculate2DDistance(goal, declaration.DeclaredGoal, Flight.getCalculationType());

                if (distance < 1000)
                {
                    comment += $"Goal {task.TaskNumber()} is within 1000m of the declaration. ({NumberHelper.formatDoubleToStringAndRound(distance)}m) | ";
                }
            }
        }

        return NORMAL_CALCULATION;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 05, 18, 00, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 2;
    }
}