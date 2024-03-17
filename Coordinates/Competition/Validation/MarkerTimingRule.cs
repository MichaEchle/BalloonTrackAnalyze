using Coordinates;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Competition
{
    public class MarkerTimingRule : IMarkerValidationRules
    {
        #region Properties

        private readonly ILogger<MarkerTimingRule> Logger;
        /// <summary>
        /// The list of timing definitions
        /// <para>each entry consists of two values</para>
        /// <para>first value: The first minute at which marking is valid</para>
        /// <para>second value: The first minute at which marking is no longer valid</para>
        /// </summary>
        public List<(int openAtMinute, int closeAtMinute)> TimingDefinitions
        {
            get; set;
        }
        #endregion

        #region API
        /// <summary>
        /// Check if the marker is conform to at least of the timing rules
        /// </summary>
        /// <param name="marker">the marker to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool IsComplaintToRule(MarkerDrop marker)
        {
            bool isConform = false;
            foreach ((int openAtMinute, int closeAtMinute) in TimingDefinitions)
            {
                bool confirmToCurrentTimingDefinition = true;
                if (openAtMinute < closeAtMinute)
                {
                    if (marker.MarkerLocation.TimeStamp.Minute < openAtMinute)
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute > closeAtMinute)
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute == closeAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                        confirmToCurrentTimingDefinition = false;
                }
                else if (openAtMinute > closeAtMinute)
                {
                    if ((marker.MarkerLocation.TimeStamp.Minute < openAtMinute) && (marker.MarkerLocation.TimeStamp.Minute > closeAtMinute))
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute == closeAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                        confirmToCurrentTimingDefinition = false;
                }
                isConform |= confirmToCurrentTimingDefinition;
            }

            return isConform;
        }


        /// <summary>
        /// Setup all properties of the rule
        /// </summary>
        ///<param name="timingDefinitions">List of timing definitions. Marker are considered valid if the conform with at least one timing definition</param>
        /// <para>each entry consists of two values</para>
        /// <para>first value: The first minute at which marking is valid</para>
        /// <para>second value: The first minute at which marking is no longer valid</para>
        public void SetupRule(List<(int openAtMinute, int closeAtMinute)> timingDefinitions)
        {
            TimingDefinitions = timingDefinitions;
        }

        public override string ToString()
        {
            return "Marker Timing Rule";
        }
        #endregion

        #region Private methods
        #endregion
    }
}
