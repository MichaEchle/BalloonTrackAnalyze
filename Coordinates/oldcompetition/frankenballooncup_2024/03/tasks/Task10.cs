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
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Markerdrop 2" };
        }

        double result;

        if (markerDrop.MarkerLocation.AltitudeBarometric <= flight.getSeperationAltitudeMeters())
        {

            List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }
        else
        {
            List<Coordinate> heightGoals = new();
            foreach (Coordinate coordinate in goals())
            {
                Coordinate newCoordinate = coordinate.Clone();
                newCoordinate.AltitudeBarometric = flight.getSeperationAltitudeMeters();
                heightGoals.Add(newCoordinate);
            }

            List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation, heightGoals.ToArray(),
                flight.useGPSAltitude(), flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), "" };
    }

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 653978, 5517918, CoordinateHelpers.ConvertToMeter(1620)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 654407, 5518754, 488)
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 02, 16, 30, 00);
    }
}