using Competition.Penalties;
using Coordinates;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._01.tasks;

public class Task02 : TaskFON
{
    public Task02(Flight flight) : base(flight)
    {
    }


    public override int TaskNumber()
    {
        return 02;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        TrackHelpers.EstimateLaunchAndLandingTime(track, Flight.UseGPSAltitude(), out Coordinate launchpoint, out _);

        List<Declaration> declarations = track.Declarations.FindAll(declaration => declaration.GoalNumber == DeclarationNumber());

        if (declarations.Count == 0)
        {
            comment += $"Pilot has no declaration in goal number {DeclarationNumber()} | ";
            return true;
        }

        var marker = track.MarkerDrops.FindLast(markerDrop => markerDrop.MarkerNumber == MarkerNumber());
        if (marker == null)
        {
            comment += $"Pilot has no marker in marker number {MarkerNumber()} | ";
            return true;
        }
        
        int declarationsAfterEstimated = 0;
        declarations.ForEach(declaration =>
        {
            if (declaration.PositionAtDeclaration.TimeStamp > launchpoint.TimeStamp)
            {
                declarationsAfterEstimated++;
            }
        });
        if (declarationsAfterEstimated > 1)
        {
            comment += "Pilot has maybe more than one declaration in the air | ";
        }

        var lastDeclaration = track.GetLatestDeclaration(DeclarationNumber());
        
        if (lastDeclaration.DeclaredGoal.AltitudeBarometric < CoordinateHelpers.ConvertToMeter(2000))
        {
            comment += "Pilot has declared bellow 2000ft | ";
            return true;
        }

        var heightAltitudeDifference = lastDeclaration.DeclaredGoal.AltitudeBarometric -
                                      lastDeclaration.PositionAtDeclaration.AltitudeBarometric;

        if (heightAltitudeDifference < CoordinateHelpers.ConvertToMeter(500))
        {
            string penalty = DistanceViolationPenalties.CalculateAndFormatPenalty(heightAltitudeDifference,
                CoordinateHelpers.ConvertToMeter(500), "TP");
            comment += $"Pilot declared with less than 500ft difference. {penalty} |";
        }

        var scoringPeriodBeginning = lastDeclaration.PositionAtDeclaration.TimeStamp.AddMinutes(15);
        if (scoringPeriodBeginning < Flight.StartOfLaunchPeriode().AddMinutes(Flight.LaunchPeriode()))
        {
            scoringPeriodBeginning = Flight.StartOfLaunchPeriode().AddMinutes(Flight.LaunchPeriode());
            comment += "Scoring Period would begin before end of starting period. | ";
        }

        if (marker.MarkerLocation.TimeStamp < scoringPeriodBeginning)
        {
            comment += $"Marker before start of start-period [was: {marker.MarkerLocation.TimeStamp} | Should be: {scoringPeriodBeginning}] | ";
            return true;
        }
        return false;
    }

    public override DateTime ScoringPeriodUntil()
    {
        return new DateTime(2026, 05, 15, 05, 30, 00);
    }

    protected override int DeclarationNumber()
    {
        return 1;
    }

    protected override int MarkerNumber()
    {
        return 2;
    }
}