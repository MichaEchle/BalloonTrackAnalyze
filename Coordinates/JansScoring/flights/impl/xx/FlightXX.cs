using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl.xx;

public class FlightXX : Flight
{
    public override int FlightNumber()
    {
        throw new NotImplementedException();
    }

    public override DateTime StartOfLaunchPeriode()
    {
        throw new NotImplementedException();
    }

    public override int LaunchPeriode()
    {
        throw new NotImplementedException();
    }

    public override bool UseGPSAltitude()
    {
        return true;
    }

    public override int DistanceToAllGoals()
    {
        throw new NotImplementedException();
    }

    public override string TracksPath()
    {
        return @"C:\Users\Jan\";;
    }

    public override Task[] Tasks()
    {
        return new Task[] { };
    }

    public override CalculationType CalculationType()
    {
        return calculation.CalculationType.Haversin;
    }

    public override double SeperationAltitudeFeet()
    {
        return 1800;
    }

    public override Coordinate BackupCoordinates()
    {
        return CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 625598, 5521249);
    }

    public override int QNH()
    {
        throw new NotImplementedException();
    }
}