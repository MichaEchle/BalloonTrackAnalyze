using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._01.tasks;

public class Task03 : TaskFON
{
    public Task03(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 03;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        List<Declaration> declarations =
            track.Declarations.FindAll(declaration => declaration.GoalNumber == DeclarationNumber());
        if (declarations.Count == 0)
        {
            comment += "No declarations found. | ";
            return true;
        }

        List<MarkerDrop> markerDrops =
            track.MarkerDrops.FindAll(markerDrop => markerDrop.MarkerNumber == MarkerNumber());
        if (markerDrops.Count == 0)
        {
            comment += "No marker drops found. | ";
            return true;
        }

        if (markerDrops.Count > 1)
        {
            comment += "More than one marker drop found. | ";
            return true;
        }

        Declaration declaration = declarations.Last();

        double distanceBetweenDeclarationPointAndDeclaredGoal = CalculationHelper.Calculate2DDistance(
            declaration.PositionAtDeclaration, markerDrops.Last().MarkerLocation, Flight.CalculationType());
        if (distanceBetweenDeclarationPointAndDeclaredGoal < 1000)
        {
            string penalty =
                DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenDeclarationPointAndDeclaredGoal,
                    1000, "TP");
            comment +=
                $"Distance between declaration point and declared goal is to short. [{distanceBetweenDeclarationPointAndDeclaredGoal}m / {penalty}] | ";
        }

        Task[] tasks = Flight.Tasks();
        List<(string, Coordinate)> coordinates = new();
        foreach (Task task in tasks)
        {
            if (task == this)
                continue;
            Coordinate[] goals = task.Goals(track.Pilot.PilotNumber);
            for (int index = 1; index <= goals.Length; index++)
            {
                Coordinate coordinate = goals[index];
                coordinates.Add(($"T{task.TaskNumber()}:G{index}", coordinate));
            }
        }
        foreach (var goals in coordinates)
        {
            double distanceBetweenGoalAndDeclaredGoal = CalculationHelper.Calculate2DDistance(goals.Item2, declaration.DeclaredGoal, Flight.CalculationType());
            if (distanceBetweenGoalAndDeclaredGoal < 1000)
            {
                string penalty = DistanceViolationPenalties.CalculateAndFormatPenalty(distanceBetweenGoalAndDeclaredGoal, 1000, "TP");
                comment += $"Distance between goal {goals.Item1} and declared goal is to short. [{distanceBetweenGoalAndDeclaredGoal}m / {penalty}] | ";
            }
        }

        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 04, 10, 18, 00, 00);
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