using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class TaskJDG : Task, ISingleMarkerTask
{
    protected TaskJDG(Flight flight) : base(flight)
    {
    }

    public override string[] score(Track track)
    {
        string comment = "";

        HandleMarker(track, out MarkerDrop markerDrop, out comment);

        List<double> distances = null;

        if (markerDrop.MarkerLocation.AltitudeGPS > flight.getSeperationAltitudeMeters())
        {
            Coordinate[] coordinates = new Coordinate[goals().Length];
            Coordinate[] goals1 = goals();
            for (int index = 0; index < goals1.Length; index++)
            {
                Coordinate coordinate = goals1[index];
                coordinates[index] = new Coordinate(coordinate.Latitude, coordinate.Longitude,
                    flight.getSeperationAltitudeMeters(), flight.getSeperationAltitudeMeters(), coordinate.TimeStamp);
            }

            distances = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, coordinates,
                flight.useGPSAltitude(),
                flight.getCalculationType());
            comment += "Calculated via 3D | ";
        }
        else
        {
            distances = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                flight.getCalculationType());
            comment += "Calculated via 2D | ";
        }

        double result = Double.MaxValue;
        foreach (double distance in distances)
        {
            if (distance < result)
            {
                result = distance;
            }
        }


        if (result < 50)
        {
            comment +=
                $"The distance is less than the MMA, the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = 50;
        }

        if (result == Double.MaxValue)
            return new[] { "No Result", "There was no distances to goals calculated  | " };

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public abstract int MarkerNumber();
}