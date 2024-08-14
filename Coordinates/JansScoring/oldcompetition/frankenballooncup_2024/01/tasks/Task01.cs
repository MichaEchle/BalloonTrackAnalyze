using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl;

public class Task01 : Task
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 1;
    }

    public override string[] score(Track track)
    {
         string result = "";
        string comment = "";

        MarkerDrop a = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
        MarkerDrop b = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);
        MarkerDrop c = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);

        if (a == null)
        {
            return new[] { "No Result", "No Markerdrop 1" };
        }

        if (b == null)
        {
            return new[] { "No Result", "No Markerdrop 2" };
        }

        if (c == null)
        {
            return new[] { "No Result", "No Markerdrop 3" };
        }

        if (a.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop 1 outside SP | ";
        }

        if (b.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop 2 outside SP | ";
        }

        if (c.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop 3 outside SP | ";
        }

        if (a.MarkerTime > b.MarkerTime || a.MarkerTime > c.MarkerTime || b.MarkerTime > c.MarkerTime)
        {
            comment += "Wrong marker order | ";
            MarkerDrop tempA = null;
            MarkerDrop tempB = null;
            MarkerDrop tempC = null;
            if (a.MarkerTime > b.MarkerTime)
            {
                if (a.MarkerTime > c.MarkerTime)
                {
                    tempC = a;
                }
                else
                {
                    tempB = a;
                }
            }
            else
            {
                tempA = a;
            }

            if (b.MarkerTime > a.MarkerTime)
            {
                if (b.MarkerTime > c.MarkerTime)
                {
                    tempC = b;
                }
                else
                {
                    tempB = b;
                }
            }
            else
            {
                tempA = b;
            }


            if (c.MarkerTime > a.MarkerTime)
            {
                if (c.MarkerTime > b.MarkerTime)
                {
                    tempC = c;
                }
                else
                {
                    tempB = c;
                }
            }
            else
            {
                tempA = c;
            }

            a = tempA;
            b = tempB;
            c = tempC;
        }

        if (
            a.MarkerLocation.AltitudeBarometric > CoordinateHelpers.ConvertToMeter(4500)||
            b.MarkerLocation.AltitudeBarometric > CoordinateHelpers.ConvertToMeter(4500)||
            c.MarkerLocation.AltitudeBarometric > CoordinateHelpers.ConvertToMeter(4500)
            )
        {
            comment += $"Markerdrop out of bounds. [a: {a.MarkerLocation.AltitudeBarometric}m,b: {b.MarkerLocation.AltitudeBarometric}m,c: {c.MarkerLocation.AltitudeBarometric}m] ->  No Result | ";
        }

        double lengthAB =
            CalculationHelper.Calculate2DDistance(a.MarkerLocation, b.MarkerLocation, flight.getCalculationType());
        double lengthBC =
            CalculationHelper.Calculate2DDistance(b.MarkerLocation, c.MarkerLocation, flight.getCalculationType());
        double lengthCA =
            CalculationHelper.Calculate2DDistance(c.MarkerLocation, a.MarkerLocation, flight.getCalculationType());


        double s = (lengthAB + lengthBC + lengthCA) / 2;
        result = NumberHelper.formatDoubleToStringAndRound(Math.Sqrt(s * (s - lengthAB) * (s - lengthBC) *
                                                                     (s - lengthCA)));

        return new[] { result, comment };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }


    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 01, 11, 30, 00);
    }
}