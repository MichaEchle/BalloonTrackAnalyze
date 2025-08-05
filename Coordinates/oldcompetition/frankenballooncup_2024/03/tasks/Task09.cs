using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._03.tasks;

public class Task09 : Task
{
    public Task09(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 9;
    }

    public override string[] score(Track track)
    {
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Markerdrop 1" };
        }

        double result;


        List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation,
            goals(), flight.useGPSAltitude(), flight.getCalculationType());

        result = distanceToAllGoals.Min();


        return new[] { NumberHelper.formatDoubleToStringAndRound(result), "" };
    }

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 656890, 5516590,
                CoordinateHelpers.ConvertToMeter(1500)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 657220, 5516420,
                CoordinateHelpers.ConvertToMeter(1500))
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 02, 16, 30, 00);
    }
}