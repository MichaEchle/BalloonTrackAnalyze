using Coordinates;
using JansScoring.calculation;
using JansScoring.check;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.tasks;

public abstract class TaskHWZPair : TaskHWZ
{
    private readonly List<TaskHWZPair> pairs = new();

    protected TaskHWZPair(Flight flight) : base(flight)
    {
    }

    public void AddPair(params TaskHWZPair[] pair)
    {
        foreach (TaskHWZPair taskHWZPair in pair)
        {
            if (taskHWZPair == this || pairs.Contains(taskHWZPair))
            {
                continue;
            }

            pairs.Add(taskHWZPair);
        }
    }

    public abstract bool InOrder();
    
    private List<TaskHWZPair> FindPredecessors(Track track)
    {
        List<TaskHWZPair> predecessors = new();

        if (InOrder())
        {
            foreach (TaskHWZPair pair in pairs)
            {
                if (pair.TaskNumber() < TaskNumber())
                {
                    predecessors.Add(pair);
                }
            }

            predecessors.Sort((firstHwz, secondHwz) => firstHwz.TaskNumber().CompareTo(secondHwz.TaskNumber()));
            return predecessors;
        }

        MarkerChecks.LoadMarkerInternal(track, MarkerNumber(), out MarkerDrop currentMarkerDrop);
        if (currentMarkerDrop == null)
        {
            return predecessors;
        }

        foreach (TaskHWZPair pair in pairs)
        {
            MarkerChecks.LoadMarkerInternal(track, pair.MarkerNumber(), out MarkerDrop markerDrop);
            if (markerDrop == null)
            {
                continue;
            }

            int timeCompare = markerDrop.MarkerTime.CompareTo(currentMarkerDrop.MarkerTime);
            if (timeCompare < 0 || (timeCompare == 0 && pair.TaskNumber() < TaskNumber()))
            {
                predecessors.Add(pair);
            }
        }

        predecessors.Sort((firstHwz, secondHwz) =>
        {
            MarkerChecks.LoadMarkerInternal(track, firstHwz.MarkerNumber(), out MarkerDrop firstMarkerDrop);
            MarkerChecks.LoadMarkerInternal(track, secondHwz.MarkerNumber(), out MarkerDrop secondMarkerDrop);

            int timeCompare = firstMarkerDrop.MarkerTime.CompareTo(secondMarkerDrop.MarkerTime);
            return timeCompare != 0 ? timeCompare : firstHwz.TaskNumber().CompareTo(secondHwz.TaskNumber());
        });

        return predecessors;
    }

    private Coordinate[] FindAvailableGoals(Track track)
    {
        List<Coordinate> goalPool = Goals(track.Pilot.PilotNumber).ToList();

        if (pairs.Count == 0)
        {
            return goalPool.ToArray();
        }

        foreach (TaskHWZPair pair in FindPredecessors(track))
        {
            Coordinate usedGoal = pair.UsedGoal(track, goalPool);
            if (usedGoal != null)
            {
                goalPool.Remove(usedGoal);
            }
        }

        return goalPool.ToArray();
    }

    public Coordinate UsedGoal(Track track)
    {
        return UsedGoal(track, null);
    }
    
    public Coordinate UsedGoal(Track track, IReadOnlyList<Coordinate> availableGoals)
    {
        MarkerChecks.LoadMarkerInternal(track, MarkerNumber(), out MarkerDrop markerDrop);
        if (markerDrop == null)
        {
            return null;
        }

        IReadOnlyList<Coordinate> goals = availableGoals ?? Goals(track.Pilot.PilotNumber);
        if (goals.Count == 0)
        {
            return null;
        }

        if (GoalChecks.Use3DScoringInternal(Flight, markerDrop))
        {
            return goals.MinBy(coordinate =>
            {
                Coordinate goal = coordinate.Clone();
                goal.AltitudeBarometric = Flight.SeperationAltitudeMeters();
                goal.AltitudeGPS = Flight.SeperationAltitudeMeters();

                return CoordinateHelpers.Calculate3DDistance(goal, markerDrop.MarkerLocation,
                    Flight.UseGPSAltitude(), Flight.CalculationType());
            });
        }

        return goals.MinBy(coordinate => CalculationHelper.Calculate2DDistance(coordinate,
            markerDrop.MarkerLocation, Flight.CalculationType()));
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        MarkerChecks.LoadMarker(track, MarkerNumber(), out MarkerDrop markerDrop, ref comment);
        if (markerDrop == null)
        {
            result = double.MinValue;
            return;
        }

        Coordinate[] goals = FindAvailableGoals(track);
        if (goals.Length <= 0)
        {
            result = double.MinValue;
            comment += "No goal available to score.";
            return;
        }

        MarkerChecks.CheckScoringPeriode(this, markerDrop, ref comment);

        if (GoalChecks.Use3DScoring(Flight, markerDrop, ref comment))
        {
            List<Coordinate> calcGoals = new();
            foreach (Coordinate coordinate in goals)
            {
                Coordinate goal = coordinate.Clone();
                goal.AltitudeBarometric = Flight.SeperationAltitudeMeters();
                goal.AltitudeGPS = Flight.SeperationAltitudeMeters();
                calcGoals.Add(goal);
            }

            List<double> distanceToAllGoals = CalculationHelper.calculate3DDistanceToAllGoals(markerDrop.MarkerLocation,
                calcGoals.ToArray(), Flight.UseGPSAltitude(),
                Flight.CalculationType());

            result = distanceToAllGoals.Min();
        }
        else
        {
            result = CalculationHelper.calculate2DDistanceToAllGoals(markerDrop.MarkerLocation,
                goals, Flight.CalculationType()).Min();
        }

        GoalChecks.CorrectMMAResult(MMA(), ref result, ref comment);
    }
}