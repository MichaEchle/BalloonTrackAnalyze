using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._03.tasks;
using JansScoring.flights.impl._04.tasks;
using System;

namespace JansScoring.flights.impl._04;

public class Flight04 : Flight
{
    public override int getFlightNumber()
    {
        return 4;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2024, 08, 16, 04, 00, 00);
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
        return 750;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\Documents\scoring\wieselburg_2024\flight04\tracks";
    }

    public override Task[] getTasks()
    {
        Task14 task14 = new Task14(this);
        return new Task[]
        {
            task14,
            new Task15(this, task14),
           /*
            *  new Task16(this),
            *  new Task17(this),
            */
            new Task18(this)

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
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U", 508660, 5328360);
    }

    public override int getQNH()
    {
        return 1018;
    }
}