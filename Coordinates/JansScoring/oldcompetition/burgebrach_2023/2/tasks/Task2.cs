using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._2.tasks;

public class Task2 : Task
{
    public Task2(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 2;
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

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);

        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 1 | " };
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
        return new[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 624709, 5521262, 267) };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 06, 09, 05, 00, 00);
    }
}