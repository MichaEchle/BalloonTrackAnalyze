using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerTimingRule : IMarkerValidationRules
    {
        public int OpenAtMinute
        {
            get;set;
        }

        public int CloseAtMinute
        {
            get;set;
        }

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
    }
}
