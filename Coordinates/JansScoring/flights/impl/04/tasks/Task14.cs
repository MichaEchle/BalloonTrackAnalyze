using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using JansScoring.flights.tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._04.tasks;

public class Task14 : TaskHWZ
{
    public Task14(Flight flight) : base(flight)
    {
    }

    public override int TaskNumber()
    {
        return 14;
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

            _usedGoals.Add(track.Pilot.PilotNumber, prevCoordinate);
            comment += "Used goal " + Array.FindIndex(Goals(track.Pilot.PilotNumber),
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

            _usedGoals.Add(track.Pilot.PilotNumber, prevCoordinate);
            comment += "Used goal " + Array.FindIndex(Goals(track.Pilot.PilotNumber),
                coordinate => coordinate.Longitude.Equals(prevCoordinate.Longitude)  && coordinate.Latitude.Equals(prevCoordinate.Latitude))  + " | ";
        }

        return false;
    }

    public override Coordinate[] Goals(int pilot)
    {
        return new Coordinate[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U",0511858,5328360, 338),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U",0512640,5328528, CoordinateHelpers.ConvertToMeter(1049)),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("33U",0512589,5327730, CoordinateHelpers.ConvertToMeter(1251))
        };
    }

    public Coordinate[] GetUnusedGoals(int pilot)
    {
        if (!_usedGoals.ContainsKey(pilot))
            return Goals(pilot);
        List<Coordinate> coordinates = new List<Coordinate>();
        foreach (Coordinate coordinate in Goals(pilot))
        {
            if (_usedGoals[pilot] != coordinate)
            {
                coordinates.Add(coordinate);
            }
        }

        return coordinates.ToArray();
    }

    private Dictionary<int, Coordinate> _usedGoals = new();

    public override DateTime GetScoringPeriodUntil()
    {
        return new DateTime(2024, 08, 16, 06, 00, 00);
    }

    protected override int MarkerNumber()
    {
        return 1;
    }

    protected override int MMA()
    {
        return 50;
    }
}