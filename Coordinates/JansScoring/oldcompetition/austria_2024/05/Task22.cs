using Coordinates;
using System;

namespace JansScoring.flights.impl._05;

public class Task22 : TaskFON
{
    public Task22(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 22;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return DateTime.Now;
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 5;
    }
}