using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._3.tasks;

public class Task9 : Task
{
    public Task9(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 9;
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

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < 2000)
        {
            comment += "Declaration to close to Dec. Point";
        }

        Coordinate center = declaration.DeclaredGoal;
        Coordinate entered = null;
        Coordinate lastTrackpoint = null;

        List<double> distances = new();

        for (var i = 1; i <= track.TrackPoints.Count; i++)
        {
            Coordinate tp = track.TrackPoints[i - 1];

            if (isInLayer3(center, tp))
            {
                if (entered == null) entered = tp;

                if (lastTrackpoint != null)
                {
                    distances.Add(CalculationHelper.Calculate2DDistance(lastTrackpoint, tp,
                            flight.getCalculationType()) * 3
                    );
                }
                else
                {
                    comment += $"In (3): {i} | ";
                }

                lastTrackpoint = tp;
            }
            else if (isInLayer2(center, tp))
            {
                if (entered == null) entered = tp;

                if (lastTrackpoint != null)
                {
                    distances.Add(CalculationHelper.Calculate2DDistance(lastTrackpoint, tp,
                            flight.getCalculationType()) * 2
                    );
                }
                else
                {
                    comment += $"In (2): {i} | ";
                }

                lastTrackpoint = tp;
            }
            else if (isInLayer1(center, tp))
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
                    comment += $"In (1): {i} | ";
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


    private bool isInLayer1(Coordinate center, Coordinate tp)
    {
        if (CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) > 1500)
        {
            return false;
        }


        if (CoordinateHelpers.ConvertToFeet(tp.AltitudeGPS) is > 2101 and < 2600)
        {
            return true;
        }

        return false;
    }

    private bool isInLayer2(Coordinate center, Coordinate tp)
    {
        if (CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) > 1000)
        {
            return false;
        }

        if (CoordinateHelpers.ConvertToFeet(tp.AltitudeGPS) is > 1601 and < 2100)
        {
            return true;
        }

        return false;
    }

    private bool isInLayer3(Coordinate center, Coordinate tp)
    {
        if (CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) > 500)
        {
            return false;
        }

        if (CoordinateHelpers.ConvertToFeet(tp.AltitudeGPS) is > 1100 and < 1600)
        {
            return true;
        }

        return false;
    }


    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 06, 10, 06, 00, 00);
    }
}