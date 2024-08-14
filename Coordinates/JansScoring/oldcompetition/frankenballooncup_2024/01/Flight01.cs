using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl;

public class Flight01 : Flight
{
    public override int getFlightNumber()
    {
        return 01;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2024, 03, 01, 9, 45, 00);
    }

    public override int launchPeriode()
    {
        return 45;
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
        return @"D:\onedrive\Ballonveranstaltungen\2024 - Fiestas\2024 Frankenballon\scoring_rl\flight01\wmfTracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task01(this), new Task03(this), new Task04(this), new Task05(this), new Task06(this) };
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