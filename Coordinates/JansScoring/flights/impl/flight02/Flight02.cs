using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl.flight02.tasks;
using System;

namespace JansScoring.flights.impl.flight02;

public class Flight02 : Flight
{
    public override int getFlightNumber()
    {
        return 2;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2025, 04, 05, 04, 00, 00);
    }

    public override int launchPeriode()
    {
        return 120;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 500;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Sonnenbühl\scoring\tracks\flight02\igc";
    }

    private Task04 task04;
    private Task05 task05;
    private Task06 task06;
    private Task07 task07;

    public Flight02()
    {
        task04 = new Task04(this);
        task05 = new Task05(this);
        task06 = new Task06(this);
        task07 = new Task07(this);
    }

    public override Task[] getTasks()
    {
        return new Task[] { task04, task05, task06, task07 };
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
        return 1011;
    }
}