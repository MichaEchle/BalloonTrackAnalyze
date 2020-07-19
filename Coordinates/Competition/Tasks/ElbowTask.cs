using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Competition
{
    public class ElbowTask : CompetitionTask
    {
        public int FirstMarkerNumber
        {
            get;set;
        }

        public int SecondMarkerNumber
        {
            get;set;
        }

        public int ThirdMarkerNumber
        {
            get;set;
        }

        public List<IMarkerValidationRules> MarkerValidationRules
        {
            get;set;
        }

        public override bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;
            MarkerDrop firstMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == FirstMarkerNumber);
            if (firstMarker == null)
            {
                Console.WriteLine($"No Marker '{FirstMarkerNumber}' found");
                return false;
            }
            if (!IsMarkerValid(firstMarker, MarkerValidationRules))
            {
                Console.WriteLine($"Marker '{FirstMarkerNumber}' is not valid");
                return false;
            }
            MarkerDrop secondMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == SecondMarkerNumber);
            if (secondMarker == null)
            {
                Console.WriteLine($"No Marker '{SecondMarkerNumber}' found");
                return false;
            }
            if (!IsMarkerValid(secondMarker, MarkerValidationRules))
            {
                Console.WriteLine($"Marker '{SecondMarkerNumber}' is not valid");
                return false;
            }
            MarkerDrop thirdMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == ThirdMarkerNumber);
            if (thirdMarker == null)
            {
                Console.WriteLine($"No Marker '{ThirdMarkerNumber}' found");
                return false;
            }
            if (!IsMarkerValid(thirdMarker, MarkerValidationRules))
            {
                Console.WriteLine($"Marker '{ThirdMarkerNumber}' is not valid");
                return false;
            }
            result = CoordinateHelpers.CalculateInteriorAngle(firstMarker.MarkerLocation, secondMarker.MarkerLocation, thirdMarker.MarkerLocation);
            return true;
        }
    }
}
