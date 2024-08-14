using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights;

public abstract class TaskLRN : Task
{
    protected TaskLRN(Flight flight) : base(flight)
    {
    }

    protected abstract int markerNumber1();
    protected abstract int markerNumber2();
    protected abstract int markerNumber3();

    protected abstract int maxHeightMeters();
    protected abstract int minHeightMeters();

    public override string[] Score(Track track)
    {
        string result = "";
        string comment = "";

        MarkerDrop a = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerNumber1());
        MarkerDrop b = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerNumber2());
        MarkerDrop c = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerNumber3());

        if (a == null)
        {
            return new[] { "No Result", "No Markerdrop " + markerNumber1() };
        }

        if (b == null)
        {
            return new[] { "No Result", "No Markerdrop " + markerNumber2() };
        }

        if (c == null)
        {
            return new[] { "No Result", "No Markerdrop " + markerNumber3() };
        }

        if (a.MarkerTime > GetScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerNumber1() + " outside SP | ";
        }

        if (b.MarkerTime > GetScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerNumber2() + " outside SP | ";
        }

        if (c.MarkerTime > GetScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerNumber3() + " outside SP | ";
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
            a.MarkerLocation.AltitudeBarometric > maxHeightMeters() ||
            b.MarkerLocation.AltitudeBarometric > maxHeightMeters() ||
            c.MarkerLocation.AltitudeBarometric > maxHeightMeters()
        )
        {
            comment +=
                $"Min one Markerdrop out of bounds (To high). [a: {a.MarkerLocation.AltitudeBarometric}m,b: {b.MarkerLocation.AltitudeBarometric}m,c: {c.MarkerLocation.AltitudeBarometric}m] ->  No Result | ";
        }

        if (
            a.MarkerLocation.AltitudeBarometric < minHeightMeters() ||
            b.MarkerLocation.AltitudeBarometric < minHeightMeters() ||
            c.MarkerLocation.AltitudeBarometric < minHeightMeters()
        )
        {
            comment +=
                $"Min one Markerdrop out of bounds (To low). [a: {a.MarkerLocation.AltitudeBarometric}m,b: {b.MarkerLocation.AltitudeBarometric}m,c: {c.MarkerLocation.AltitudeBarometric}m] ->  No Result | ";
        }

        double lengthAB =
            CalculationHelper.Calculate2DDistance(a.MarkerLocation, b.MarkerLocation, Flight.getCalculationType());
        double lengthBC =
            CalculationHelper.Calculate2DDistance(b.MarkerLocation, c.MarkerLocation, Flight.getCalculationType());
        double lengthCA =
            CalculationHelper.Calculate2DDistance(c.MarkerLocation, a.MarkerLocation, Flight.getCalculationType());


        double s = (lengthAB + lengthBC + lengthCA) / 2;
        result = NumberHelper.formatDoubleToStringAndRound(Math.Sqrt(s * (s - lengthAB) * (s - lengthBC) *
                                                                     (s - lengthCA)));

        return new[] { result, comment };
    }
}