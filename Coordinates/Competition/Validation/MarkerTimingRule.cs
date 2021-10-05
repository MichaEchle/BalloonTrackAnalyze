using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerTimingRule : IMarkerValidationRules
    {
        #region Properties

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
            foreach ((int openAtMinute, int closeAtMinute) timingDefinition in TimingDefinitions)
            {
                bool confirmToCurrentTimingDefinition = true;
                if (timingDefinition.openAtMinute < timingDefinition.closeAtMinute)
                {
                    if (marker.MarkerLocation.TimeStamp.Minute < timingDefinition.openAtMinute)
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute > timingDefinition.closeAtMinute)
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute == timingDefinition.closeAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                        confirmToCurrentTimingDefinition = false;
                }
                else if (timingDefinition.openAtMinute > timingDefinition.closeAtMinute)
                {
                    if ((marker.MarkerLocation.TimeStamp.Minute < timingDefinition.openAtMinute) && (marker.MarkerLocation.TimeStamp.Minute > timingDefinition.closeAtMinute))
                        confirmToCurrentTimingDefinition = false;
                    if (marker.MarkerLocation.TimeStamp.Minute == timingDefinition.closeAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
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
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
        #endregion
    }
}
