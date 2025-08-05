using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl.flight01.tasks;
using System;

namespace JansScoring.flights.impl.flight01;

public class Flight01 : Flight
{
    public Flight01()
    {
        task01 = new Task01(this);
        task02 = new Task02(this);
        task03 = new Task03(this);
    }

    public override int getFlightNumber()
    {
        return 01;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2025, 04, 04, 15, 45, 00);
    }

    public override int launchPeriode()
    {
        return 45;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 0;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Sonnenbühl\scoring\tracks\flight01\igc";
    }

    private Task01 task01;
    private Task02 task02;
    private Task03 task03;

    public override Task[] getTasks()
    {
        return new Task[] { task01, task02, task03 };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 3300;
    }

    public override Coordinate getBackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 513571, 5357273,
            CoordinateHelpers.ConvertToMeter(2556));
    }

    public override int getQNH()
    {
        return 0;
    }
}