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
        return new DateTime(2024, 08, 15, 04, 00, 00);
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
        return 3000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight02\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task03(this),
            new Task04(this),
            new Task06(this),
            new Task07(this),
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