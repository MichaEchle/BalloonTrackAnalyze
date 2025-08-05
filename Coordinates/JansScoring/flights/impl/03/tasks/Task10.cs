using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using OfficeOpenXml;
using OfficeOpenXml.VBA;
using System;

namespace JansScoring.flights.impl._03.tasks;

public class Task10 : TaskRTA
{
    private double entryPoint;
    private double exitPoint;
    public Task10(Flight flight) : base(flight)
    {
        (double entryLatitude, double entryLongitude) = CoordinateHelpers.ConvertUTMToLatitudeLongitude("33U", 509000,5328360);
        entryPoint = entryLongitude;
        (double  exitLatitude, double exitLongitude) = CoordinateHelpers.ConvertUTMToLatitudeLongitude("33U", 510000,5328360);
        exitPoint = exitLongitude;
    }

    public override int TaskNumber()
    {
        return 10;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        return false;
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 18, 00, 00);
    }

    public override bool IsInsideScoringArea(Coordinate coordinate)
    {
        return coordinate.Longitude >entryPoint && coordinate.Longitude  < exitPoint;
    }

    public override int DeclarationNumber()
    {
        return 1;
    }
}