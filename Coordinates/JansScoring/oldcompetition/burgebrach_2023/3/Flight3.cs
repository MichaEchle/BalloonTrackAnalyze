using JansScoring.calculation;
using JansScoring.flights.impl._3.tasks;
using System;

namespace JansScoring.flights.impl._3;

public class Flight3 : Flight
{
    public override int getFlightNumber()
    {
        return 3;
    }

    public override DateTime getStartOfLaunchPeriode()
    {
        return new DateTime(2023, 06, 10, 02, 00, 00);
    }

    public override int launchPeriode()
    {
        return 150;
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
        return @"C:\Users\Jan M\OneDrive\Ballonveranstaltungen\2023 Bayr. Meisterschaft\scoring\flights\flight3\tracks";
    }

    public override Task[] getTasks()
    {
        return new Task[]
        {
            new Task6(this), new Task7(this), new Task8(this), new Task9(this), new Task10(this), new Task11(this),
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