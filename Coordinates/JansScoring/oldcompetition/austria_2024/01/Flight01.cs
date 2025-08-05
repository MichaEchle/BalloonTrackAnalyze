using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._01.tasks;
using System;

namespace JansScoring.flights.impl._01;

public class Flight01 : Flight
{
    public override int getFlightNumber()
    {
        return 1;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2024,08,14,16,35,00);
    }

    public override int launchPeriode()
    {
        return 20;
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
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight01\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task01(this),
            new Task02(this)
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
       return 1010;
    }
}