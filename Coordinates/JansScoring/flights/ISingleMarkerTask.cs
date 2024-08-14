using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public interface ISingleMarkerTask
{

    protected int MarkerNumber();

    public void HandleMarker(Task task, Track track, out MarkerDrop markerDrop, out String noResultMessage) {
        List<int> allMarkerNumbers = track.GetAllMarkerNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            noResultMessage = "No marker numbers found. | ";
            markerDrop = null;
            return;
        }

        markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == MarkerNumber());

        if (markerDrop == null)
        {
            noResultMessage = $"No Marker drops at slot {MarkerNumber()}. | ";
            markerDrop = null;
            return;
        }

        noResultMessage = null;
    }
}