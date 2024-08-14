using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._04.tasks;

public class Task16 : Task
{
    public Task16(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 16;
    }

    public override string[] score(Track track)
    {
        String comment = "";

        List<int> allGoalDeclerationNumber = track.GetAllGoalNumbers();
        if (allGoalDeclerationNumber == null || !allGoalDeclerationNumber.Any())
        {
            return new[] { "No Result", "No Decleration | " };
        }

        Declaration declaration = track.Declarations.FindLast(drop => drop.GoalNumber == 1);

        if (declaration == null)
        {
            String data = "Data:";
            foreach (Declaration trackDeclaration in track.Declarations)
            {
                data += " || " + trackDeclaration.ToString();
            }

            return new[]
            {
                "No Result",
                $"No Declaration at slot 1. Available ({StringHelper.IntListToString(track.GetAllGoalNumbers())}) | " +
                data + " | "
            };
        }

        if (declaration.PositionAtDeclaration == null || declaration.DeclaredGoal == null)
        {
            return new[] { "No Result", $"No Valid-Declaration at slot 1. | " };
        }


        double result = -1;


        if (CalculationHelper.Calculate2DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal,
                flight.getCalculationType()) < 3000)
        {
            comment +=
                $"Declared inside 3km radius [{NumberHelper.formatDoubleToStringAndRound(CalculationHelper.Calculate2DDistance(declaration.PositionAtDeclaration, declaration.DeclaredGoal, flight.getCalculationType()))}] | ";
        }


        Coordinate lastTrackPoint = null;
        int calculatedTrackPoints = 0;

        foreach (Coordinate trackTrackPoint in track.TrackPoints)
        {
            double distance = CalculationHelper.Calculate2DDistance(trackTrackPoint, declaration.DeclaredGoal,
                flight.getCalculationType());
            if (distance is > 1000 and < 2000)
            {
                if (lastTrackPoint != null && trackTrackPoint != null)
                {
                    result += CalculationHelper.Calculate2DDistance(trackTrackPoint, lastTrackPoint,
                        flight.getCalculationType());
                    calculatedTrackPoints++;
                }
                else
                {
                    comment += $"In: {track.TrackPoints.IndexOf(trackTrackPoint) + 1} | ";
                }

                lastTrackPoint = trackTrackPoint;
            }
            else
            {
                if (lastTrackPoint != null)
                {
                    comment += $"Out: {track.TrackPoints.IndexOf(trackTrackPoint) + 1} | ";
                    lastTrackPoint = null;
                }
            }
        }

        comment += $"Calculated with {calculatedTrackPoints} TrackPoints.";


        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 11, 09, 00, 00);
    }
}