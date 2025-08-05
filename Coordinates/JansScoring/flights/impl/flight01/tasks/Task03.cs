using Coordinates;
using JansScoring.calculation;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Windows.Globalization.NumberFormatting;

namespace JansScoring.flights.impl.flight01.tasks;

public class Task03 : TaskFON
{
    public Task03(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 3;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        Declaration declaration =
            track.Declarations.FindLast(declaration1 => declaration1.GoalNumber == DeclarationNumber());

        if (declaration == null)
        {
            comment += " No declaration found. | ";
            return true;
        }
        if (track.Declarations.Count(declaration1 => declaration1.GoalNumber == DeclarationNumber()) > 3)
        {
            comment += " Track has more than 3 declarations. | ";
            declaration = track.Declarations.Where(declaration1 => declaration1.GoalNumber == DeclarationNumber())
                .ToList()[2];
        }


        foreach (Task task in Flight.getTasks())
        {
            if (task.TaskNumber() == TaskNumber())
            {
                continue;
            }

            if (task.Goals().Length == 0)
            {
                continue;
            }
            var distanceToGoal = CalculationHelper.Calculate2DDistance(task.Goals().First(), declaration.DeclaredGoal,
                Flight.getCalculationType());
            if (distanceToGoal < 1000)
            {
                comment += " Goal " + task.TaskNumber() + $" is within 1000m of declaration. ({NumberHelper.formatDoubleToStringAndRound(distanceToGoal)}m) | ";
            }

            if (task.TaskNumber() != 2)
            {
                continue;
            }

            foreach (Coordinate goal in Flight.getTasks().First(task1 => task1.TaskNumber() == 2).Goals())
            {
                var distanceToTask2 = CalculationHelper.Calculate2DDistance(goal, declaration.DeclaredGoal,
                    Flight.getCalculationType());
                if (distanceToTask2 > 4000)
                {
                    comment += $" Declaration is too far away from goal of task 2. ({NumberHelper.formatDoubleToStringAndRound(distanceToTask2)}m) |";
                }
            }
        }

        var distance =
            CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                Flight.getCalculationType());
        if (distance < 1000)
        {
            comment += $" Declaration is too close to the goal. ({NumberHelper.formatDoubleToStringAndRound(distance)}m) |";
        }

        if (distance > 5000)
        {
            comment += $" Declaration is too far from the goal. ({NumberHelper.formatDoubleToStringAndRound(distance)}m) |";
        }

        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2025, 04, 04, 18, 00, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 3;
    }
}