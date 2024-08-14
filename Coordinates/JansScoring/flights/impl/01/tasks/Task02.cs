using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._01.tasks;

public class Task02 : TaskFON
{
    public Task02(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 02;
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
       return new DateTime(2024,08,14,18,00,00);
    }

    protected override int declerationNumber()
    {
        return 1;
    }

    protected override int markerDropNumber()
    {
        return 2;
    }

    protected override int maxDeclerations()
    {
        return 3;
    }

    protected override int minDistanceToOtherGoals()
    {
        return 0;
    }

    protected override int minTimeInSecondsToDeclarationPoint()
    {
        return 300;
    }

    protected override int minDistanceToDeclerationPoint()
    {
        return 0;
    }

    protected override double minHeightDifferenceInMetersToDelcearedPoint()
    {
        return CoordinateHelpers.ConvertToMeter(1000);
    }
}