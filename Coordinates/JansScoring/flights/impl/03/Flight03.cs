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
        return new DateTime(2024, 08, 15, 16, 20, 00);
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
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight03\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task10(this),
            new Task11(this),
            new Task12(this)
        };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 1500;
    }
    public override Coordinate getBackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U",508660,5328360);
    }

    public override int getQNH()
    {
        return 1016;
    }
}