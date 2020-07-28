using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerToOtherMarkersDistanceRule : IMarkerValidationRules
    {
        #region Properties

        /// <summary>
        /// Minimum distance to other markers in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MinimumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Maximum distance to other markers in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MaximumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// List of markers to be checked against (will not check against itself)
        /// <para>mandotory</para>
        /// </summary>
        public List<MarkerDrop> MarkerDrops
        {
            get;set;
        }

        #endregion

        #region API
        /// <summary>
        /// Check if the marker is conform to the distance rules to the other markers
        /// <para>will not check against itself</para>
        /// </summary>
        /// <param name="marker">the marker to be checked</param>
        /// <returns>true: is conform; false: is not confrom</returns>
        public bool CheckConformance(MarkerDrop marker)
        {
            bool isConform = true;
            foreach (MarkerDrop markerDrop in MarkerDrops)
            {
                if (marker.Equals(markerDrop))
                    continue;
                double distanceToOtherMarker = CoordinateHelpers.Calculate2DDistance(marker.MarkerLocation, markerDrop.MarkerLocation);
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

        /// <summary>
        /// Set all propteries of the rule
        /// </summary>
        /// <param name="minimumDistance">Minimum distance to other markers in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumDistance">Maximum distance to other markers in meter (optional; use double.NaN to omit)</param>
        /// <param name="markerDrops">List of markers to be checked against [will not check against itself] (mandotory)</param>
        public void SetupRule(double minimumDistance, double maximumDistance, List<MarkerDrop> markerDrops)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            MarkerDrops = markerDrops;
        }
        #endregion
    }
}
