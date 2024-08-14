using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._3.tasks;

public class Task8 : Task
{
    public Task8(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 8;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = null;

        if (!track.GetAllMarkerNumbers().Contains(2))
        {
            return new[] { "No Result", "No Logger Marker 3 | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 3 | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 3" };
        }

        if (markerDrop.MarkerLocation.AltitudeGPS > flight.getSeperationAltitudeMeters())
        {
            Coordinate coordinate = new Coordinate(goals()[0].Latitude, goals()[0].Longitude, flight.getSeperationAltitudeMeters(),
                flight.getSeperationAltitudeMeters(), goals()[0].TimeStamp);
            result = CoordinateHelpers.Calculate3DDistance(markerDrop.MarkerLocation, coordinate,
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
        return new[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 626394, 5519092, 322) };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 06, 10, 05, 30, 00);
    }
}