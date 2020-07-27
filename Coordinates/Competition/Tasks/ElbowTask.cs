using Competition.Validation;
using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Competition
{
    public class ElbowTask : ICompetitionTask
    {
        #region Properties

        /// <summary>
        /// The task number
        /// <para>mandotory</para>
        /// </summary>
        public int TaskNumber
        {
            get;
            set;
        } = -1;

        /// <summary>
        /// The marker number of the first marker
        /// <para>mandotory</para>
        /// </summary>
        public int FirstMarkerNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// The marker number of the second marker
        /// <para>mandotory</para>
        /// </summary>
        public int SecondMarkerNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// The marker number of the third marker
        /// <para>mandotory</para>
        /// </summary>
        public int ThirdMarkerNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// List of rules for marker validation
        /// <para>optional; leave list empty to omit</para>
        /// </summary>
        public List<IMarkerValidationRules> MarkerValidationRules
        {
            get; set;
        } = new List<IMarkerValidationRules>();
        #endregion

        #region API

        /// <summary>
        /// Calculate the angle at the second marker number (when connect the first with second marker and the second with the third marker) in degree
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
        /// <param name="result">the angle at the second marker in degree</param>
        /// <returns>true:success;false:error</returns>
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;
            MarkerDrop firstMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == FirstMarkerNumber);
            if (firstMarker == null)
            {
                Console.WriteLine($"No Marker '{FirstMarkerNumber}' found");
                return false;
            }
            if (!ValidationHelper.IsMarkerValid(firstMarker, MarkerValidationRules))
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
            if (!ValidationHelper.IsMarkerValid(secondMarker, MarkerValidationRules))
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
            if (!ValidationHelper.IsMarkerValid(thirdMarker, MarkerValidationRules))
            {
                Console.WriteLine($"Marker '{ThirdMarkerNumber}' is not valid");
                return false;
            }
            result = CoordinateHelpers.CalculateInteriorAngle(firstMarker.MarkerLocation, secondMarker.MarkerLocation, thirdMarker.MarkerLocation);
            return true;
        }

        /// <summary>
        /// Set all properties for a elbow
        /// </summary>
        /// <param name="taskNumber">The task number (mandotory)</param>
        /// <param name="firstMarkerNumber">The marker number of the first marker (mandotory)</param>
        /// <param name="secondMarkerNumber">The marker number of the second marker (mandotory)</param>
        /// <param name="thirdMarkerNumber">The marker number of the third marker (mandotory)</param>
        /// <param name="markerValidationRules">List of rules for marker validation (optional; leave list empty to omit)</param>
        public void SetupElbow(int taskNumber, int firstMarkerNumber, int secondMarkerNumber, int thirdMarkerNumber, List<IMarkerValidationRules> markerValidationRules)
        {
            TaskNumber = taskNumber;
            FirstMarkerNumber = firstMarkerNumber;
            SecondMarkerNumber = secondMarkerNumber;
            ThirdMarkerNumber = thirdMarkerNumber;
            MarkerValidationRules = markerValidationRules;
        }

        public override string ToString()
        {
            return $"Elbow (Task #{TaskNumber})";

        }
        #endregion
    }
}
