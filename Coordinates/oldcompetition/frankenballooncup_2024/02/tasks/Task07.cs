using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._02.tasks;

public class Task07 : Task
{
    public Task07(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 07;
    }

    public override string[] score(Track track)
    {
        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Markerdrop 1" };
        }

        //TODO: If Seperation Altitude

        double result;

        if (true)
        {
            List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }
        else
        {
            List<Coordinate> heightGoals = new List<Coordinate>();
            foreach (Coordinate coordinate in goals())
            {
                coordinate.Clone().AltitudeBarometric = flight.getSeperationAltitudeFeet();
            }

            List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation, goals(),
                flight.getCalculationType());

            result = distanceToAllGoals.Min();
        }

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), "" };
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 653876, 5527048, 384), //D-ONBG
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 654075, 5528314,
                CoordinateHelpers.ConvertToMeter(1554)), //D-OEEJ
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 02, 08, 00, 00);
    }
}