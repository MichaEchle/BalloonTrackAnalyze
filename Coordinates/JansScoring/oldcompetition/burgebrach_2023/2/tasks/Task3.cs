﻿using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._2.tasks;

public class Task3 : Task
{
    public Task3(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 3;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";
        
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
        
        if (markerDrop.MarkerLocation.AltitudeGPS > flight.getSeperationAltitudeMeters())
        {
            Coordinate coordinate = goals()[0];
            result = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, new Coordinate(coordinate.Latitude,coordinate.Longitude, flight.getSeperationAltitudeMeters(),flight.getSeperationAltitudeMeters(),coordinate.TimeStamp),
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

        if (result < 50)
        {
            comment +=
                $"The distance is less than the MMA, the result must be 50m ({NumberHelper.formatDoubleToStringAndRound(result)})  | ";
            result = 50;
        }
        
        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 621192, 5520719,
                CoordinateHelpers.ConvertToMeter(974))
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 06, 09, 05, 30, 00);
    }
}