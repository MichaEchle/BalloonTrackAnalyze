using JansScoring.calculation;
using JansScoring.flights.impl._4.tasks;
using System;

namespace JansScoring.flights.impl._4;

public class Flight4 : Flight
{
    public override int getFlightNumber()
    {
        return 4;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 06, 10, 18, 20, 00);
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
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 Bayr. Meisterschaft\scoring\flights\flight4\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[] { new Task12(this), new Task13(this) };
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