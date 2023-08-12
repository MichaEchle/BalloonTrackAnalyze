using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System;
using System.Linq;

namespace JansScoring.pz_rework.type;

public class YellowPZ : PZ
{

    private Coordinate center;

    public YellowPZ(int id, Coordinate center, int radius) : base(id)
    {
        this.center = center;
        this.radius = radius;
    }

    private int radius;

    public override bool IsInsidePz(Flight flight, Track track, Coordinate coordinate, out String comment)
    {
        comment = "";
        bool isInsite = false;

        Coordinate launchPoint;
        if (!TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out launchPoint,
                out _))

        {
            comment = "Cannot calculate Start and Landing Time";
            return false;
        }
        double distanceBetweenStartAndYellowPZ =
            CalculationHelper.Calculate2DDistance(launchPoint, center, flight.getCalculationType());
        if (distanceBetweenStartAndYellowPZ <= radius)
        {
            comment =
                "Pilot has a Yellow-PZ infringement for starting. ";
            isInsite = true;
        }

        double distanceBetweenLandingAndYellowPZ =
            CalculationHelper.Calculate2DDistance(track.TrackPoints.Last(), center,
                flight.getCalculationType());
        if (distanceBetweenLandingAndYellowPZ <= radius)
        {
            comment =
                "Pilot has a Yellow-PZ infringement for landing.";
            isInsite = true;
        }

        return isInsite;
    }
}