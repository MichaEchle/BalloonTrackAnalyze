using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._3.tasks;

public class Task11 : Task
{
    public Task11(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 11;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            return new[] { "No Result", "No Marker drops | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 5);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 5 | " };
        }
        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot5" };
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
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622053, 5519020,
                CoordinateHelpers.ConvertToMeter(991)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 622432, 5517105,
                CoordinateHelpers.ConvertToMeter(1392)),
        };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 06, 10, 06, 00, 00);
    }
}