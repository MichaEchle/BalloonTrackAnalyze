using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class TaskHNH : Task
{
    protected TaskHNH(Flight flight) : base(flight)
    {
    }

    protected abstract int markerDropNumber();
    protected abstract int mma();

    public override string[] score(Track track)
    {
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerDropNumber());

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Markerdrop " + markerDropNumber() };
        }


        double result;
        String comment = "";;
        if (flight.useGPSAltitude()
                ? markerDrop.MarkerLocation.AltitudeGPS <= flight.getSeperationAltitudeMeters()
                : markerDrop.MarkerLocation.AltitudeBarometric <= flight.getSeperationAltitudeMeters())
        {
            List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation,
                goals(),
                flight.getCalculationType());

            result = distanceToAllGoals.Min();
            comment += "Calculated with 2D | ";
        }
        else
        {
            List<Coordinate> heightGoals = new List<Coordinate>();
            foreach (Coordinate coordinate in goals())
            {
                Coordinate goal = coordinate.Clone();
                goal.AltitudeGPS = flight.getSeperationAltitudeMeters();
                goal.AltitudeBarometric = flight.getSeperationAltitudeMeters();
                heightGoals.Add(goal);
            }

            List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation,
                heightGoals.ToArray(),
                flight.useGPSAltitude(), flight.getCalculationType());


            result = distanceToAllGoals.Min();
            comment += "Calculated with 3D | ";
        }
        if (result < mma())
        {
            comment +=
                $"The distance is less than the MMA, the result must be {mma()}m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = mma();
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }
}