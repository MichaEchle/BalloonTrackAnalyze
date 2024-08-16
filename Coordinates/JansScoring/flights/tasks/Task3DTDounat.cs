using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.tasks;

public abstract class Task3DTDounat : Task
{
    protected Task3DTDounat(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        Coordinate center;
        if (PilotDeclared())
        {
            DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

            if (declaration == null)
            {
                result = Double.MaxValue;
                return;
            }

            center = declaration.DeclaredGoal;
        }
        else
        {
            if (Goals(track.Pilot.PilotNumber).Length < 1)
            {
                comment += "No Goal available -> no Scoring. | ";
                result = Double.MaxValue;
                return;
            }

            center = Goals(track.Pilot.PilotNumber)[0];
        }

        List<double> distances = new();
        Coordinate entered = null;
        Coordinate lastTrackpoint = null;

        for (var i = 1; i <= track.TrackPoints.Count; i++)
        {
            Coordinate tp = track.TrackPoints[i - 1];

            if (lastTrackpoint != null && tp.TimeStamp > GetScoringPeriodUntil())
            {
                comment += $"SP-Out: {i} | ";
                break;
            }

            if (CalculationHelper.Calculate2DDistance(center, tp, Flight.getCalculationType()) >
                InnerRadiusInMeters() &&
                CalculationHelper.Calculate2DDistance(center, tp, Flight.getCalculationType()) < OuterRadiusMeters() &&
                (Flight.useGPSAltitude() ? tp.AltitudeGPS : tp.AltitudeBarometric) > MinHeightInMeters() &&
                (Flight.useGPSAltitude() ? tp.AltitudeGPS : tp.AltitudeBarometric) < MaxHeightInMeters()
               )
            {
                if (entered == null) entered = tp;

                if (lastTrackpoint != null)
                {
                    distances.Add(CalculationHelper.Calculate2DDistance(lastTrackpoint, tp,
                        Flight.getCalculationType())
                    );
                }
                else
                {
                    comment += $"In: {i} | ";
                }

                lastTrackpoint = tp;
            }
            else
            {
                if (lastTrackpoint != null)
                {
                    comment += $"Out: {i} | ";
                    lastTrackpoint = null;
                    if (!ReEnter())
                    {
                        break;
                    }
                }
            }
        }

        result = 0;
        foreach (double distance in distances)
        {
            result += distance;
        }
    }


    /*
     * Only needed if center was declared by Competition.
     */
    public override Coordinate[] Goals(int pilot)
    {
        return Array.Empty<Coordinate>();
    }

    /*
     * Only needed if center should be declared by Competitor.
     */
    public abstract int DeclarationNumber();
    public abstract bool PilotDeclared();

    public abstract double OuterRadiusMeters();
    public abstract double InnerRadiusInMeters();
    public abstract double MinHeightInMeters();
    public abstract double MaxHeightInMeters();
    public abstract bool ReEnter();
}