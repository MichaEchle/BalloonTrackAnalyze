using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public interface ISingleDeclarationTask
{
    protected int DeclarationNumber();
    protected int MaxRedeclaration();
    double MinHeightDifferenceInMetersToDeclaredPoint();
    double MinDistanceInMetersFromDeclarationPointToDeclaredPoint();
    double MinDistanceInMetersFromDeclaredGoalToOtherGoals();



    public void HandleDecleration(Task task, Track track, out Declaration declaration, out String comment)
    {
        comment = "";
        List<int> allMarkerNumbers = track.GetAllGoalNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            comment = "No declarations found. | ";
            declaration = null;
            return;
        }

        declaration = track.Declarations.FindLast(drop => drop.GoalNumber == DeclarationNumber());

        if (declaration == null)
        {
            comment = $"No Declarations at slot {DeclarationNumber()}. | ";
            declaration = null;
            return;
        }

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            comment = $"No valid Declaration in {DeclarationNumber()} | ";
            return;
        }

        if (track.Declarations.Count > MaxRedeclaration())
        {
            comment += $"To many Declarations {track.Declarations.Count} | ";
        }

        double distanceToDeclarationPoint = CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal,
            declaration.PositionAtDeclaration,
            task.flight.getCalculationType());
        if (distanceToDeclarationPoint < MinDistanceInMetersFromDeclarationPointToDeclaredPoint())
        {
            comment += $"Declared goal is to close to declaration point {distanceToDeclarationPoint}m | ";
        }

        double heightDifference = task.flight.useGPSAltitude()
            ? Math.Abs(declaration.PositionAtDeclaration.AltitudeGPS - declaration.DeclaredGoal.AltitudeGPS)
            : Math.Abs(declaration.PositionAtDeclaration.AltitudeBarometric -
                       declaration.DeclaredGoal.AltitudeBarometric);

        if (heightDifference < MinHeightDifferenceInMetersToDeclaredPoint())
        {
            comment += $"Declared with to less height difference. {heightDifference}m | ";
        }

        if (MinDistanceInMetersFromDeclaredGoalToOtherGoals() != 0)
        {
            Dictionary<Coordinate, Task> goals = new();
            Dictionary<Coordinate, Declaration> declarations = new();
            foreach (Task currentTask in task.flight.getTasks())
            {
                foreach (Coordinate coordinate in currentTask.goals())
                {
                    goals.Add(coordinate, currentTask);
                }
            }


            foreach (Coordinate goal in goals.Keys)
            {
                if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal
                        , goal, task.flight.getCalculationType()) < MinDistanceInMetersFromDeclaredGoalToOtherGoals())
                {
                    comment += $"Declared goal is to close to another fixed goal [Task {goals[goal].number()}] | ";
                }
            }

            foreach (Declaration trackDeclaration in track.Declarations)
            {
                if (trackDeclaration.GoalNumber == DeclarationNumber())
                {
                    continue;
                }

                declarations.Add(trackDeclaration.DeclaredGoal, trackDeclaration);
            }

            foreach (Coordinate currentDeclaration in declarations.Keys)
            {
                if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal
                        , currentDeclaration, task.flight.getCalculationType()) <
                    MinDistanceInMetersFromDeclaredGoalToOtherGoals())
                {
                    comment +=
                        $"Declared goal is to close to another declared goal [Goal {declarations[currentDeclaration].GoalNumber}] | ";
                }
            }
        }


        comment = null;
    }
}