using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._4.tasks;

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

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 1" };
        }

        Dictionary<double, int> distances = new();
        if (markerDrop.MarkerLocation.AltitudeGPS > flight.getSeperationAltitudeMeters())
        {
            Coordinate[] goals1 = goals();
            for (int index = 0; index < goals1.Length; index++)
            {
                Coordinate coordinate = new(goals()[index].Latitude, goals()[index].Longitude,
                    flight.getSeperationAltitudeMeters(), flight.getSeperationAltitudeMeters(),
                    goals()[index].TimeStamp);

                distances.Add(CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, coordinate,
                    flight.useGPSAltitude(),
                    flight.getCalculationType()), index);
            }


            comment += "Calculated via 3D | ";
        }
        else
        {
            for (var index = 0; index < goals().Length; index++)
            {
                distances.Add(
                    CalculationHelper.Calculate2DDistance(markerDrop.MarkerLocation, goals()[index],
                        flight.getCalculationType()), index);
            }

            comment += "Calculated via 2D | ";
        }

        double result = Double.MaxValue;

        foreach (double distance in distances.Keys)
        {
            if (distance < result)
            {
                result = distance;
            }
        }

        comment += $"Calculated to Goal '{(distances[result] + 1)}'";

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
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 0620424, 5520026,
                CoordinateHelpers.ConvertToMeter(1289)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 0622906, 5520767,
                CoordinateHelpers.ConvertToMeter(886)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 0618754, 5519816,
                CoordinateHelpers.ConvertToMeter(887)),
        };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 06, 10, 19, 20, 00);
    }
}