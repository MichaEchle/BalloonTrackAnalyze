using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl.flight03.tasks;
using System;

namespace JansScoring.flights.impl.flight03;

public class Flight03 : Flight
{
    public override int getFlightNumber()
    {
        return 3;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2025, 04, 05, 16, 35, 00);
    }

    public override int launchPeriode()
    {
        return 30;
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
        return @"C:\Users\Jan\Nextcloud2\shared\Ballonveranstaltungen\2025 Sonnenbühl\scoring\tracks\flight03\igc";

    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task08(this),
            new Task09(this),
        };
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