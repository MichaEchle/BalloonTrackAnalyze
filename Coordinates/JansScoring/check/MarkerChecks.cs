using Coordinates;
using JansScoring.calculation;
using JansScoring.flights;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.check;

public class MarkerChecks
{
    public static void LoadMarker(Track track, int markerNumber, out MarkerDrop markerDrop, ref string comment)
    {
        List<int> allMarkerNumbers = track.GetAllGoalNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            comment += "No marker found. | ";
            markerDrop = null;
            return;
        }

        markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == markerNumber);

        if (markerDrop == null)
        {
            comment += $"No markers at slot {markerNumber}. | ";
            markerDrop = null;
            return;
        }

        if (markerDrop.MarkerLocation == null)
        {
            comment += $"The marker in {markerNumber} is not valid. | ";
            return;
        }
    }

    public static void CheckScoringPeriode(Task task, MarkerDrop markerDrop, ref string comment)
    {
        if (markerDrop.MarkerTime > task.GetScoringPeriodUntil())
        {
            comment += $"Markerdrop  {markerDrop.MarkerNumber} outside of ScoringPeriode | ";
        }
    }

    public static void CheckMin2DDistanceBetweenMarkers(Flight flight, MarkerDrop markerDropA, MarkerDrop markerDropB,
        int minDistanceInMeters, ref string comment)
    {
        double distance = CalculationHelper.Calculate2DDistance(markerDropA.MarkerLocation,
            markerDropB.MarkerLocation,
            flight.getCalculationType());
        if (distance < minDistanceInMeters)
        {
            comment +=
                $"The distance between marker {markerDropA.MarkerNumber} and Marker {markerDropB.MarkerNumber} is too short. [{NumberHelper.formatDoubleToStringAndRound(distance)}m / {NumberHelper.formatDoubleToStringAndRound(minDistanceInMeters)}m] [{DistanceViolationPenalties.CalculateAndFormatPenalty(distance, minDistanceInMeters, "TP")}] | ";
        }
    }
    public static void CheckMax2DDistanceBetweenMarkers(Flight flight, MarkerDrop markerDropA, MarkerDrop markerDropB,
        int maxDistanceInMeters, ref string comment)
    {
        double distance = CalculationHelper.Calculate2DDistance(markerDropA.MarkerLocation,
            markerDropB.MarkerLocation,
            flight.getCalculationType());
        if (distance > maxDistanceInMeters)
        {
            comment +=//TODO:
                $"The distance between marker {markerDropA.MarkerNumber} and Marker {markerDropB.MarkerNumber} is too long. [{NumberHelper.formatDoubleToStringAndRound(distance)}m / {NumberHelper.formatDoubleToStringAndRound(maxDistanceInMeters)}m] [{DistanceViolationPenalties.CalculateAndFormatPenalty(distance, maxDistanceInMeters, "TP")}] | ";
        }
    }
}