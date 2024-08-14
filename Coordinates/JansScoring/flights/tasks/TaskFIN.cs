using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class TaskFIN : Task
{
    protected TaskFIN(Flight flight) : base(flight)
    {
    }

    protected abstract int markerDropNumber();
    protected abstract int mma();


    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";

        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            return new[] { "No Result", "No Marker drops | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerDropNumber());

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot " + markerDropNumber() + " | " };
        }

        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerDropNumber() + " outside SP | ";
        }

        if (markerDrop.MarkerLocation.AltitudeGPS > flight.getSeperationAltitudeMeters())
        {
            result = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, goals()[0],
                flight.useGPSAltitude(),
                flight.getCalculationType());
            comment += "Calculated via 3D | ";
        }
        else
        {
            result = CalculationHelper.Calculate2DDistance(markerDrop.MarkerLocation, goals()[0],
                flight.getCalculationType());
            comment += "Calculated via 2D | ";
        }

        if (result < mma())
        {
            comment +=
                $"The distance is less than the MMA, the result must be " + mma() +
                "m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = mma();
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }
}