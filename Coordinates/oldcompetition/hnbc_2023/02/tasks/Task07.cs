using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System;

namespace JansScoring.oldcompetition.hnbc_2023._02.tasks;

public class Task07 : Task
{
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 07;
    }

    public override string[] score(Track track)
    {
        string result = "";
        string comment = "";

        MarkerDrop a = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);
        MarkerDrop b = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);
        MarkerDrop c = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 7);

        if (a == null)
        {
            return new[] { "No Result", "No Markerdrop A" };
        }

        if (b == null)
        {
            return new[] { "No Result", "No Markerdrop B" };
        }

        if (c == null)
        {
            return new[] { "No Result", "No Markerdrop C" };
        }

        if (a.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop A outside SP | ";
        }

        if (b.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop B outside SP | ";
        }

        if (c.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop C outside SP | ";
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

        comment += $" a: {a.MarkerTime} | c: {c.MarkerTime}";

        if (a.MarkerTime.AddMinutes(30) < c.MarkerTime)
        {
            comment += $"Logged more than 30min after point 'A' {c.MarkerTime - a.MarkerTime.AddMinutes(30)} | ";
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
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 10, 09, 30, 00);
    }
}