using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._03.tasks;

public class Task10 : Task
{
    public Task10(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 10;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            return new[] { "No Result", "No Marker drops | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 2 | " };
        }

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


        if (result < 30)
        {
            comment +=
                $"The distance is less than the MMA, the result must be 30m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = 30;
        }

        if (result == Double.MaxValue)
            return new[] { "No Result", "There was no distances to goals calculated  | " };

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 489161, 5373941,
                CoordinateHelpers.ConvertToMeter(1450)),
        };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 08, 10, 20, 49, 00);
    }
}