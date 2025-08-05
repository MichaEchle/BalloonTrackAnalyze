using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._04.tasks;

public class Task15 : Task
{
    public Task15(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 15;
    }

    public override string[] score(Track track)
    {
        String comment = "";

        MarkerDrop mark4 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);
        if (mark4 == null)
        {
            return new[] { "No Result", "No Marker 4 found." };
        }

        MarkerDrop mark5 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
        if (mark5 == null)
        {
            return new[] { "No Result", "No Marker 5 found." };
        }

        TimeSpan timeDifferenz = mark5.MarkerTime - mark4.MarkerTime;

        if (timeDifferenz.TotalMilliseconds < 0)
        {
            return new[] { "No Result", "No Marker 5 before Marker 4." };
        }

        if (timeDifferenz.Minutes > 20)
        {
            return new[] { "No Result", "Marker out of Time" };
        }

        comment += $"Time-different: {timeDifferenz.Minutes} min.";

        double result = CalculationHelper.Calculate2DDistance(mark4.MarkerLocation, mark5.MarkerLocation,
            flight.getCalculationType());

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