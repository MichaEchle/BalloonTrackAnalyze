using Coordinates;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task10 : Task3DDounat
{
    public Task10(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 10;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 09, 06, 07, 00, 00);
    }

    public override int DeclarationNumber()
    {
        throw new NotImplementedException();
    }

    public override bool PilotDeclared()
    {
        return false;
    }

    public override double OuterRadiusMeters()
    {
        return 1500;
    }

    public override double InnerRadiusInMeters()
    {
        return 500;
    }

    public override double MinHeightInMeters()
    {
        return 0;
    }

    public override double MaxHeightInMeters()
    {
        return CoordinateHelpers.ConvertToMeter(4500);
    }

    public override bool ReEnter()
    {
        return true;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return
        [
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 543910, 5506962)
        ];
    }
}