using Coordinates;
using JansScoring.calculation;
using JansScoring.flights.tasks;
using System;

namespace JansScoring.flights.impl.xx.tasks;

public class Task01 : TaskPDG
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 1;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {

        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchCoordinate,
            out Coordinate landingCoordinate);
        
        
        throw new NotImplementedException();
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2025, 08, 06, 05, 30, 00);
    }

    public override int MarkerNumber()
    {
        return 1;
    }

    public override int DeclarationNumber()
    {
        return 1;
    }
}