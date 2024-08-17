using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._04.tasks;
using JansScoring.flights.impl._05.tasks;
using System;

namespace JansScoring.flights.impl._05;

public class Flight05 : Flight
{
    public override int getFlightNumber()
    {
        return 4;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2024, 08, 17, 04, 00, 00);
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
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight05\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task22(this) };
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
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508660, 5328360);
    }

    public override int getQNH()
    {
        return 1017;
    }
}