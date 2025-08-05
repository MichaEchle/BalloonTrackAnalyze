using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._03.tasks;

public class Task11 : Task
{
    public Task11(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 11;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";



        Coordinate center = goals()[0];
        Coordinate entered = null;
        Coordinate lastTrackpoint = null;


        List<double> distances = new();

        for (var i = 1; i <= track.TrackPoints.Count; i++)
        {
            Coordinate tp = track.TrackPoints[i - 1];

            if (lastTrackpoint != null && tp.TimeStamp > getScoringPeriodUntil())
            {
                comment += $"SP-Out: {i} | ";
                break;
            }

            if (CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) > 1000 && CalculationHelper.Calculate2DDistance(center, tp, flight.getCalculationType()) <4000)
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


        foreach (double distance in distances)
        {
            result += distance;
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U",649000, 5521000)};
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 02, 16, 30, 00);
    }
}