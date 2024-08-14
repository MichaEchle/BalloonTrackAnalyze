using Coordinates;
using System;

namespace JansScoring.flights.impl._01.tasks;

public class Task01 : TaskHNH
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 01;
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[]
        {

        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024,08,14,18,00,00);
    }

    protected override int markerDropNumber()
    {
        return 01;
    }

    protected override int mma()
    {
        return 30;
    }
}