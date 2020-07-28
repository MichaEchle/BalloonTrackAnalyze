using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerTimingRule : IMarkerValidationRules
    {
        #region Properties

        /// <summary>
        /// The first minute at which markering is valid
        /// <para>mandotory</para>
        /// </summary>
        public int OpenAtMinute
        {
            get; set;
        } = -1;

        /// <summary>
        /// The first minute at which markering is no longer valid
        /// <para>mandotory</para>
        /// </summary>
        public int CloseAtMinute
        {
            get; set;
        } = -1;
        #endregion

        #region API
        /// <summary>
        /// Check if the marker is conform to the timing rules
        /// </summary>
        /// <param name="marker">the marker to be checked</param>
        /// <returns>true: is conform; false: is not confrom</returns>
        public bool CheckConformance(MarkerDrop marker)
        {
            bool isConform = true;
            if (marker.MarkerLocation.TimeStamp.Minute < OpenAtMinute)
                isConform = false;
            if (marker.MarkerLocation.TimeStamp.Minute > CloseAtMinute)
                isConform = false;
            if (marker.MarkerLocation.TimeStamp.Minute == CloseAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                isConform = false;

            return isConform;
        }


        /// <summary>
        /// Setup all properties of the rule
        /// </summary>
        /// <param name="openAtMinute">The first minute at which markering is valid (mandotory)</param>
        /// <param name="closeAtMinute">The first minute at which markering is no longer valid (mandotory)</param>
        public void SetupRule(int openAtMinute, int closeAtMinute)
        {
            OpenAtMinute = openAtMinute;
            CloseAtMinute = closeAtMinute;
        }

        public override string ToString()
        {
            return "Marker Timing Rule";
        }
        #endregion
    }
}
