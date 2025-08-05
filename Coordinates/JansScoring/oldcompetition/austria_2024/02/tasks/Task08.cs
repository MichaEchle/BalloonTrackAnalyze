using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;
using Windows.ApplicationModel.Email;

namespace JansScoring.flights.impl._02.tasks;

public class Task08 : TaskELB
{
    public Task08(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 8;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        MarkerChecks.LoadMarker(track, MarkerNumberA(), out MarkerDrop markerDropA, ref comment);
        MarkerChecks.LoadMarker(track, MarkerNumberB(), out MarkerDrop markerDropB, ref comment);
        MarkerChecks.LoadMarker(track, MarkerNumberC(), out MarkerDrop markerDropC, ref comment);
        if (markerDropA == null || markerDropB == null || markerDropC == null)
        {
            return true;
        }

        MarkerChecks.CheckMin2DDistanceBetweenMarkers(Flight, markerDropA, markerDropB, 3000, ref comment);
        MarkerChecks.CheckMax2DDistanceBetweenMarkers(Flight, markerDropA, markerDropB, 6000, ref comment);

        MarkerChecks.CheckMin2DDistanceBetweenMarkers(Flight, markerDropB, markerDropC, 3000, ref comment);
        MarkerChecks.CheckMax2DDistanceBetweenMarkers(Flight, markerDropB, markerDropC, 6000, ref comment);

        return false;
    }


    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 15, 06, 15, 00);
    }

    protected override int MarkerNumberA()
    {
        return 5;
    }

    protected override int MarkerNumberB()
    {
        return 6;
    }

    protected override int MarkerNumberC()
    {
        return 7;
    }
}