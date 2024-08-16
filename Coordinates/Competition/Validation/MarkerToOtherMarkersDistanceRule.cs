using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Competition;

public class MarkerToOtherMarkersDistanceRule : IMarkerValidationRule
{
    #region Properties
    private readonly ILogger<MarkerToOtherMarkersDistanceRule> Logger = LogConnector.LoggerFactory.CreateLogger<MarkerToOtherMarkersDistanceRule>();

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
    /// <para>needs preprocessing of the tracks</para>
    /// <see cref="MarkerNumbers"/>
    /// </summary>
    public List<MarkerDrop> MarkerDrops
    {
        get; set;
    } = [];

    /// <summary>
    /// List of the marker numbers to be considered and fed into after preprocessing
    /// <para>optional; use empty list to consider all markers</para>
    /// </summary>
    public List<int> MarkerNumbers { get; set; } = [];


    #endregion

    #region API
    /// <summary>
    /// Check if the marker is conform to the distance rules to the other markers
    /// <para>will not check against itself</para>
    /// </summary>
    /// <param name="marker">the marker to be checked</param>
    /// <returns>true: is conform; false: is not conform</returns>
    public bool IsComplaintToRule(MarkerDrop marker)
    {
        bool isConform = true;
        foreach (MarkerDrop markerDrop in MarkerDrops)
        {
            if (marker.Equals(markerDrop))
                continue;
            if (MarkerNumbers?.Count > 0)
            {
                if(!MarkerNumbers.Contains(markerDrop.MarkerNumber))
                {
                    continue;
                }
            }
            double distanceToOtherMarker = CoordinateHelpers.Calculate2DDistanceHavercos(marker.MarkerLocation, markerDrop.MarkerLocation);
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
    /// Set all properties of the rule
    /// </summary>
    /// <param name="minimumDistance">Minimum distance to other markers in meter (optional; use double.NaN to omit)</param>
    /// <param name="maximumDistance">Maximum distance to other markers in meter (optional; use double.NaN to omit)</param>
    /// <param name="markerNumbers">List of marker numbers to be checked against [will not check against itself] (optional; use empty list to consider all markers). The actuals marker objects will need to be fed in after track-preprocessing</param>
    public void SetupRule(double minimumDistance, double maximumDistance, List<int> markerNumbers)
    {
        MinimumDistance = minimumDistance;
        MaximumDistance = maximumDistance;
        MarkerNumbers = markerNumbers;
    }

    public override string ToString()
    {
        return "Marker to other Markers Distance Rule";
    }
    #endregion

    #region Private methods

    #endregion
}
