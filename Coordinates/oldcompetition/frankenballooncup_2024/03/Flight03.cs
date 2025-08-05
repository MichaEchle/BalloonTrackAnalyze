using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._03.tasks;
using System;

namespace JansScoring.flights.impl._03;

public class Flight03 : Flight
{
    public override int getFlightNumber()
    {
        return 03;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2024, 03, 02, 15, 00, 00);
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
        return 2000;
    }

    public override string getTracksPath()
    {
        return @"D:\onedrive\Ballonveranstaltungen\2024 - Fiestas\2024 Frankenballon\scoring_rl\flight03\wmftracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task09(this), new Task10(this), new Task11(this) };
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
        return 1005;
    }
}