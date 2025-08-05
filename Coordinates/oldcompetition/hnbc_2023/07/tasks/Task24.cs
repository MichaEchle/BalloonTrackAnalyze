using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._07.tasks;

public class Task24 : Task
{
    public Task24(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 24;
    }

    public override string[] score(Track track)
    {
        string comment = "";

        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            return new[] { "No Result", "No Marker drops | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 1 | " };
        }


        if (!track.GetAllGoalNumbers().Contains(1))
        {
            return new[] { "No Result", "No Declaration A in 1" };
        }

        List<Coordinate> goals = new();

        Declaration declarationA = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);

        if (declarationA.DeclaredGoal != null && declarationA.PositionAtDeclaration != null)
        {
            double distance = CalculationHelper.Calculate2DDistance(declarationA.DeclaredGoal,
                declarationA.PositionAtDeclaration, flight.getCalculationType());
            if (distance > 2000 && distance < 4000)
            {
                goals.Add(declarationA.DeclaredGoal);
            }
            else
            {
                comment += $"Declaration A out of range ({distance}m) | ";
            }
        }
        else
        {
            comment += "No valid Declaration A in 1 | ";
        }


        if (!track.GetAllGoalNumbers().Contains(2))
        {
            return new[] { "No Result", "No Declaration A in 2" };
        }

        Declaration declarationB = track.Declarations.FindLast(declaration => declaration.GoalNumber == 2);

        if (declarationB.DeclaredGoal != null && declarationB.PositionAtDeclaration != null)
        {
            double distance = CalculationHelper.Calculate2DDistance(declarationB.DeclaredGoal,
                declarationB.PositionAtDeclaration, flight.getCalculationType());
            if (distance > 2000 && distance < 4000)
            {
                goals.Add(declarationB.DeclaredGoal);
            }
            else
            {
                comment += $"Declaration B out of range ({distance}m) | ";
            }
        }
        else
        {
            comment += "No valid Declaration B in 2 | ";
        }


        List<double> distances = null;


        distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, goals.ToArray(),
            flight.useGPSAltitude(),
            flight.getCalculationType());


        double result = Double.MaxValue;
        foreach (double distance in distances)
        {
            if (distance < result)
            {
                result = distance;
            }
        }


        if (result < 50)
        {
            comment +=
                $"The distance is less than the MMA, the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = 50;
        }

        if (result == Double.MaxValue)
            return new[] { "No Result", "There was no distances to goals calculated  | " };

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 12, 20, 48, 00);
    }
}