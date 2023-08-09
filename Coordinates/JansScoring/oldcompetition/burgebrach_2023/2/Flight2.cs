using JansScoring.calculation;
using JansScoring.flights.impl._2.tasks;
using System;

namespace JansScoring.flights.impl._2;

public class Flight2 : Flight
{
    public override int getFlightNumber()
    {
        return 2;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 06, 09, 02, 30, 00);
    }

    public override int launchPeriode()
    {
        return 120;
    }

    public override bool useGPSAltitude()
    {
        return true;
    }

    public override int distanceToAllGoals()
    {
        return 1000;
    }

    public override string getTracksPath()
    {
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 Bayr. Meisterschaft\scoring\flights\flight2\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task2(this),
            new Task3(this),
            new Task4(this),
            new Task5(this),
        };
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