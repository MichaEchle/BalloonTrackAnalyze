using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights;

public class Flight1 : Flight
{
    public override int getFlightNumber()
    {
        return 1;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023,06,08,18,03,30);
    }

    public override int launchPeriode()
    {
        return 30;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 0;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 Bayr. Meisterschaft\scoring\flights\flight1\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]{new Task1(this)};
    }

    public override CalculationType getCalculationType()
    {
        return CalculationType.Haversin;
    }

    public override double getSeperationAltitudeFeet()
    {
        return 1800;
    }
}