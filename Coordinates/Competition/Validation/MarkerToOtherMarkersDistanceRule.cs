using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerToOtherMarkersDistanceRule : IMarkerValidationRules
    {
        public double MinimumDistance
        {
            get;set;
        }

        public double MaximumDistance
        {
            get;set;
        }

        public List<MarkerDrop> otherMarkers
        {
            get;set;
        }
        public bool CheckConformance(MarkerDrop marker)
        {
            bool isConform = true;
            foreach (MarkerDrop otherMarker in otherMarkers)
            {
                double distanceToOtherMarker = CoordinateHelpers.Calculate2DDistance(marker.MarkerLocation, otherMarker.MarkerLocation);
                if (!double.IsNaN(MinimumDistance))
                {
                    if (distanceToOtherMarker < MinimumDistance)
                        isConform = false;
                }
                if (!double.IsNaN(MaximumDistance))
                {
                    if (distanceToOtherMarker > MaximumDistance)
                        isConform = false;
                }
            }
            return isConform;
        }
    }
}
