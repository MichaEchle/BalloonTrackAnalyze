using Coordinates;
using JansScoring.flights;
using System.Collections.Generic;

namespace JansScoring.check;

public class GoalChecks
{
    public static bool Use3DScoring(Flight flight, MarkerDrop markerDrop, ref string comment)
    {
        bool use3DScoringInternal = Use3DScoringInternal(flight, markerDrop);
        if (use3DScoringInternal)
        {
            comment += "Calculated via 3D | ";
            return true;
        }

        comment += "Calculated via 2D | ";
        return false;
    }
    public static bool Use3DScoringInternal(Flight flight, MarkerDrop markerDrop)
    {
        return (flight.UseGPSAltitude() ? markerDrop.MarkerLocation.AltitudeGPS : markerDrop.MarkerLocation.AltitudeBarometric) > flight.SeperationAltitudeMeters();
    }

    public static void CorrectMMAResult(int mma, ref double result, ref string comment)
    {
        if (result < mma)
        {
            comment +=
                $"The distance is less than the MMA, the result must be {mma}m ({NumberHelper.formatDoubleToStringAndRound(result)}m)  | ";
            result = mma;
        }
    }

    public static void MoveGoalHeightToSeperationAltitude(Flight flight, ref Coordinate[] goals) {
        List<Coordinate> heightGoals = new List<Coordinate>();
        foreach (Coordinate coordinate in goals)
        {
            Coordinate goal = coordinate.Clone();
            goal.AltitudeGPS = flight.SeperationAltitudeMeters();
            goal.AltitudeBarometric = flight.SeperationAltitudeMeters();
            heightGoals.Add(goal);
        }
        goals = heightGoals.ToArray();
    }
}