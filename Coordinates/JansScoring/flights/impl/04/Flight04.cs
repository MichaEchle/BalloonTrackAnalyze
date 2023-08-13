using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._03.tasks;
using JansScoring.flights.impl._04.tasks;
using System;

namespace JansScoring.flights.impl._04;

public class Flight04 : Flight
{
    public override int getFlightNumber()
    {
        return 4;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 08, 11, 04, 11, 00);
    }

    public override int launchPeriode()
    {
        return 69;
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
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight04\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task12(this), new Task13(this), new Task14(this), new Task15(this), new Task16(this), };
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