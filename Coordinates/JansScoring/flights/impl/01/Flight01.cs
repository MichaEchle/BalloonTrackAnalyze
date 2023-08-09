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
        return new DateTime(2023, 08, 09, 18, 05, 00);
    }

    public override int launchPeriode()
    {
        return 20;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 3000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan\OneDrive\Ballonveranstaltungen\2023 HNBC\Tracks\flight_01";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task01(this),new Task02(this) };
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 2400;
    }
}