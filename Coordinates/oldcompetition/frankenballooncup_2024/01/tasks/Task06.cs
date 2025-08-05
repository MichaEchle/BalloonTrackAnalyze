using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl;

public class Task06 : Task
{
    public Task06(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 06;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        MarkerDrop marker7 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 7);

        if (marker7 == null)
        {
            return new[] { "No Result", "No MarkerDrop in #7" };
        }
        if (marker7.MarkerTime > getScoringPeriodUntil())
        {
            comment += "MarkerDrop #7 outside SP | ";
        }

        MarkerDrop marker8 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 8);
        if (marker8 == null)
        {
            return new[] { "No Result", "No MarkerDrop in #8" };
        }

        if (marker8.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop #8 outside SP | ";
        }

        if (marker7.MarkerTime.CompareTo(marker8.MarkerTime) >= 0)
        {
            comment += "Marker #7 and Marker #8 messed up.";
            (marker7, marker8) = (marker8, marker7);
        }

        double timeDiffrence = marker8.MarkerTime.Subtract(marker7.MarkerTime).TotalSeconds;
        if (timeDiffrence <= 1200D)
        {
            comment += $"Marker time to small. [Is {timeDiffrence}s, Should 1200s] | ";
        }

        double distance = CalculationHelper.Calculate2DDistance(marker7.MarkerLocation, marker8.MarkerLocation,
            flight.getCalculationType());


        return new[] { NumberHelper.formatDoubleToStringAndRound(distance), comment };
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