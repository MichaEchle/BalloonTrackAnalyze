using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Competition.Validation
{
    public class MarkerToMarkerTimingRule : IMarkerValidationRules
    {
        #region Properties
        /// <summary>
        /// Earliest time to drop second marker
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public TimeSpan Earliest
        {
            get; set;
        } = TimeSpan.Zero;

        /// <summary>
        /// Latest time to drop second marker
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public TimeSpan Latest
        {
            get; set;
        } = TimeSpan.MaxValue;

        /// <summary>
        /// List of markers to be checked against (will not check against itself)
        /// <para>needs preprocessing of the tracks</para>
        /// <see cref="MarkerNumbers"/>
        /// </summary>
        public List<MarkerDrop> MarkerDrops
        {
            get; set;
        } = new List<MarkerDrop>();

        /// <summary>
        /// List of the marker numbers to be considered and fed into after preprocessing
        /// <para>optional; use empty list to consider all markers</para>
        /// </summary>
        public List<int> MarkerNumbers { get; set; } = new List<int>();
        #endregion Properties

        #region API
        public bool IsComplaintToRule(MarkerDrop marker)
        {
            bool isConform = true;
            foreach (MarkerDrop markerDrop in MarkerDrops)
            {
                if (marker.Equals(markerDrop))
                    continue;
                if (!MarkerNumbers.Contains(markerDrop.MarkerNumber))
                    continue;
                if (markerDrop.MarkerLocation.TimeStamp.Subtract(marker.MarkerLocation.TimeStamp) < TimeSpan.Zero)
                {
                    if (marker.MarkerLocation.TimeStamp.Subtract(markerDrop.MarkerLocation.TimeStamp) < Earliest)
                        isConform = false;
                    if (marker.MarkerLocation.TimeStamp.Subtract(markerDrop.MarkerLocation.TimeStamp) > Latest)
                        isConform = false;


                }
                else
                {
                    if (markerDrop.MarkerLocation.TimeStamp.Subtract(marker.MarkerLocation.TimeStamp) < Earliest)
                        isConform = false;
                    if (markerDrop.MarkerLocation.TimeStamp.Subtract(marker.MarkerLocation.TimeStamp) > Latest)
                        isConform = false;
                }
            }
            return isConform;
        }
        public void SetupRule(TimeSpan earliest, TimeSpan latest, List<int> markerNumbers)
        {
            Earliest = earliest;
            Latest = latest;
            MarkerNumbers = markerNumbers;
        }

        public override string ToString()
        {
            return "Marker to Markers Timing Rule";
        }
        #endregion
    }
}
