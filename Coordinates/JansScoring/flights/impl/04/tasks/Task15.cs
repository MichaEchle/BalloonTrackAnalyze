using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._04.tasks;

public class Task15 : TaskHWZ
{
    private Task14 _task14;

    public Task15(Flight flight, Task14 task14) : base(flight)
    {
        _task14 = task14;
    }

    public override int TaskNumber()
    {
        return 15;
    }

    public override bool ScoringChecks(Track track, ref string comment)
    {
        if (Goals(track.Pilot.PilotNumber).Length <= 0)
        {
            comment = "No goal available to score.";
            return true;
        }

        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            return true;
        }

        if (GoalChecks.Use3DScoring(Flight, markerDrop, ref comment))
        {
            List<Coordinate> goals = new();
            foreach (Coordinate coordinate in Goals(track.Pilot.PilotNumber))
            {
                Coordinate goal = coordinate.Clone();
                goal.AltitudeBarometric = Flight.getSeperationAltitudeMeters();
                goal.AltitudeGPS = Flight.getSeperationAltitudeMeters();
                goals.Add(goal);
            }

            double prevDistance = Double.MaxValue;
            Coordinate prevCoordinate = null;
            foreach (Coordinate coordinate in goals)
            {
                double distance = CoordinateHelpers.Calculate3DDistance(coordinate, markerDrop.MarkerLocation,
                    Flight.useGPSAltitude(), Flight.getCalculationType());
                if (prevCoordinate == null || distance < prevDistance)
                {
                    prevCoordinate = coordinate;
                    prevDistance = distance;
                }
            }

            if (prevCoordinate == null)
            {
                return false;
            }

            comment += "Used gaol " + Array.FindIndex(_task14.Goals(track.Pilot.PilotNumber),
                coordinate => coordinate.Longitude.Equals(prevCoordinate.Longitude)  && coordinate.Latitude.Equals(prevCoordinate.Latitude))  + " | ";
        }
        else
        {
            double prevDistance = Double.MaxValue;
            Coordinate prevCoordinate = null;
            foreach (Coordinate coordinate in Goals(track.Pilot.PilotNumber))
            {
                double distance = CalculationHelper.Calculate2DDistance(coordinate, markerDrop.MarkerLocation,
                    Flight.getCalculationType());
                if (prevCoordinate == null || distance < prevDistance)
                {
                    prevCoordinate = coordinate;
                    prevDistance = distance;
                }
            }

            if (prevCoordinate == null)
            {
                return false;
            }

            comment += "Used gaol " + Array.FindIndex(_task14.Goals(track.Pilot.PilotNumber),
                coordinate => coordinate.Longitude.Equals(prevCoordinate.Longitude)  && coordinate.Latitude.Equals(prevCoordinate.Latitude))  + " | ";
        }

        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return _task14.GetUnusedGoals(pilot);
    }

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 16, 06, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 2;
    }

    protected override int MMA()
    {
        return 50;
    }
}