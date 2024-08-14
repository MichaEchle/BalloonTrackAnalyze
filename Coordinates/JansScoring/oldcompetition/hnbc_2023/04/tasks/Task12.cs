using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._04.tasks;

public class Task12 : Task
{
    public Task12(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 12;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            return new[] { "No Result", "No Marker drops | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 1 | " };
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

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 475170, 5366311,
                CoordinateHelpers.ConvertToMeter(1774)),
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 11, 08, 15, 00);
    }
}