using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl;

public class Task05 : Task
{
    public Task05(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 5;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";

        if (!track.GetAllGoalNumbers().Contains(2))
        {
            return new[] { "No Result", "No Declaration in 2" };
        }

        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 2);

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            return new[] { "No Result", "No valid Declaration in 2" };
        }

        MarkerDrop logger6 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);

        if (logger6 != null)
        {
            if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, logger6.MarkerLocation,
                    flight.getCalculationType()) < 2000)
            {
                comment += "Declaration to close to LoggerMarker #6 | ";
            }
        }
        else
        {
            comment += "Could not check for distance. No LoggerMarker #6 | ";
        }


        Coordinate center = declaration.DeclaredGoal;
        Coordinate entered = null;
        Coordinate lastTrackpoint = null;

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 7);
        if (markerDrop == null)
        {
            comment += "No MarkerDrop #7 for finish | ";
        }


        List<double> distances = new();

        for (var i = 1; i <= track.TrackPoints.Count; i++)
        {
            Coordinate tp = track.TrackPoints[i - 1];

            if (lastTrackpoint != null && entered != null && (markerDrop != null && tp.TimeStamp > markerDrop.MarkerTime))
            {
                comment += $"MarkerDrop-Out: {i} | ";
                break;
            }
            if (lastTrackpoint != null && tp.TimeStamp > getScoringPeriodUntil())
            {
                comment += $"SP-Out: {i} | ";
                break;
            }

            if (CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) > 1500 && CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) <2000)
            {
                if (entered == null) entered = tp;

                if (lastTrackpoint != null)
                {
                    distances.Add(CalculationHelper.Calculate2DDistance(lastTrackpoint, tp,
                        flight.getCalculationType())
                    );
                }
                else
                {
                    comment += $"In: {i} | ";
                }

                lastTrackpoint = tp;
            }
            else
            {
                if (lastTrackpoint != null)
                {
                    comment += $"Out: {i} | ";
                    lastTrackpoint = null;
                }
            }
        }


        if (entered != null && declaration.DeclaredGoal.TimeStamp > entered.TimeStamp)
        {
            comment += "Declaration after entering | ";
        }

        foreach (double distance in distances)
        {
            result += distance;
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 01, 13, 00, 00);
    }
}