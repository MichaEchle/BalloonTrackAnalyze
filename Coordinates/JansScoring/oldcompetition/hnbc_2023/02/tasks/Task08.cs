using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.oldcompetition.hnbc_2023._02.tasks;

public class Task08 : Task
{
    public Task08(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 08;
    }

    public override string[] score(Track track)
    {
        String comment = "";

        List<int> allGoalDeclerationNumber = track.GetAllGoalNumbers();
        if (allGoalDeclerationNumber == null || !allGoalDeclerationNumber.Any())
        {
            return new[] { "No Result", "No Decleration | " };
        }

        Declaration declaration = track.Declarations.FindLast(drop => drop.GoalNumber == 2);


        if (track.Declarations.FindAll(drop => drop.GoalNumber == 2).Count > 1)
        {
            comment += "Found multiple declarations. Check for Re-Declaration after Enter. | ";
        }


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
                $"No Declaration at slot 2. Available ({StringHelper.IntListToString(track.GetAllGoalNumbers())}) | " +
                data + " | "
            };
        }

        if (declaration.PositionAtDeclaration == null || declaration.DeclaredGoal == null)
        {
            return new[] { "No Result", $"No Valid-Declaration at slot 2. | " };
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
        return new DateTime(2023, 08, 10, 07, 30, 00);
    }
}