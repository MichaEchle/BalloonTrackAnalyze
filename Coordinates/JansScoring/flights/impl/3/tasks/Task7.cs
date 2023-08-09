using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._3.tasks;

public class Task7 : Task
{
    public Task7(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 7;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = null;

        if (!track.GetAllMarkerNumbers().Contains(2))
        {
            return new[] { "No Result", "No Logger Marker 2 | " };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 2 | " };
        }
        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 2" };
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
        return new[] { CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 627860, 5520010, 300) };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 06, 10, 05, 00, 00);
    }
}