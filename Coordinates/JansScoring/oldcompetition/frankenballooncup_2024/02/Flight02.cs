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
        return new DateTime(2024,03,02,06,42,00);
    }

    public override int launchPeriode()
    {
        return 30;
    }

    public override bool useGPSAltitude()
    {
        return false;
    }

    public override int distanceToAllGoals()
    {
        return 0;
    }

    public override string getTracksPath()
    {
        return @"D:\onedrive\Ballonveranstaltungen\2024 - Fiestas\2024 Frankenballon\scoring_rl\flight02\wmftracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task07(this), new Task08(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2000;
    }

    public override Coordinate getBackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 657353, 5515860);
    }

    public override int getQNH()
    {
        return 1006;
    }
}