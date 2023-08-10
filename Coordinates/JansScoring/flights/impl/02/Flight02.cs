using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._02.tasks;
using System;

namespace JansScoring.flights.impl._02;

public class Flight02 : Flight
{
    public override int getFlightNumber()
    {
        return 2;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 08, 10, 04, 10, 00);
    }

    public override int launchPeriode()
    {
        return 60;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight02\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task03(this), new Task04(this), new Task05(this), new Task06(this), new Task07(this),
            new Task08(this),
        };
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
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 483390, 5370330,
            CoordinateHelpers.ConvertToMeter(1453));
    }
}