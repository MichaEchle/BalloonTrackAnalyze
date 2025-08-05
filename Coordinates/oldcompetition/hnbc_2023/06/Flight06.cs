using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.impl._06.tasks;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._06;

public class Flight06 : Flight
{
    public override int getFlightNumber()
    {
        return 6;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 08, 12, 04, 12, 00);
    }

    public override int launchPeriode()
    {
        return 68;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 750;
    }

    public override string getTracksPath()
    {
        //return @"C:\Users\Jan\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight06\tracks";
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 HNBC\Flights\flight06\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task19(this),
            //new Task21(this)// Finished but low performance
            new Task22(this),
            new Task23(this),
        };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2400;
    }

    public override Coordinate getBackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 483641, 5370325,
            CoordinateHelpers.ConvertToMeter(1640));
    }
}