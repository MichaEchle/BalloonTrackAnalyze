using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._02.tasks;
using JansScoring.flights.impl._03.tasks;
using System;

namespace JansScoring.flights.impl._03;

public class Flight03 : Flight
{
    public override int getFlightNumber()
    {
        return 3;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 08, 10, 17, 35, 00);
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
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight03\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task09(this), new Task10(this), new Task11(this), };
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
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 496140, 5376850,
            CoordinateHelpers.ConvertToMeter(1453));
    }
}