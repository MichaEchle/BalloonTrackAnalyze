using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinates
{
    public class MarkerDrop
    {
        /// <summary>
        /// The number of the marker
        /// </summary>
        public int MarkerNumber { get; private set; }

        /// <summary>
        /// The position of the marker
        /// </summary>
        public Coordinate MarkerLocation { get; private set; }

        /// <summary>
        /// Create a marker drop
        /// </summary>
        /// <param name="markerNumber">The number of the marker</param>
        /// <param name="markerLocation">The position of the marker</param>
        public MarkerDrop(int markerNumber, Coordinate markerLocation)
        {
            MarkerNumber = markerNumber;
            MarkerLocation = markerLocation;
        }
    }
}
