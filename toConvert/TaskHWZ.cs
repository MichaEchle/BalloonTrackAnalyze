using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class TaskHWZ : Task
{
    protected TaskHWZ(Flight flight) : base(flight)
    {
    }

    protected abstract int markerDropNumber();
    protected abstract int mma();

    public override string[] Score(Track track)
    {
        String comment = "";
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerDropNumber());

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Markerdrop " + markerDropNumber() };
        }

        if (markerDrop.MarkerTime > GetScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerDropNumber() + " outside SP | ";
        }

        double result;


        List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation,
            Goals(), Flight.useGPSAltitude(), Flight.getCalculationType());

        result = distanceToAllGoals.Min();

        if (result < mma())
        {
            comment +=
                $"The distance is less than the MMA, the result must be {mma()}m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = mma();
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }
}