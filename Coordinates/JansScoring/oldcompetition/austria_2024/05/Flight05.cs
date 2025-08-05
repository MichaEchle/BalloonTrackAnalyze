using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._05;

public class Flight05 : Flight
{
    public override int getFlightNumber()
    {
        return 5;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return DateTime.Now;
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
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight05\tracks";
    }

    public override Task[] getTasks()
    {
        return new[] { new Task22(this) };
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
        return 1015;
    }
}