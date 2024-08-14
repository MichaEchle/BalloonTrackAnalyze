using Coordinates;
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

}