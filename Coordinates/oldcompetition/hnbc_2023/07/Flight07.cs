using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._07;

public class Flight07 : Flight
{
    public override int getFlightNumber()
    {
        return 7;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 08, 10, 17, 15, 00);
    }

    public override int launchPeriode()
    {
        return 20;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 750;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight07\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2400;
    }

    public override Coordinate getBackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 483641, 5370325,
            CoordinateHelpers.ConvertToMeter(1640));
    }
}