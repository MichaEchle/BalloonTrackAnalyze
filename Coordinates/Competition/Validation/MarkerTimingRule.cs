using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace Competition
{
    public class MarkerTimingRule : IMarkerValidationRule
    {
        #region Properties

        private readonly ILogger<MarkerTimingRule> Logger = LogConnector.LoggerFactory.CreateLogger<MarkerTimingRule>();
        private int openAtMinute;
        private int closeAtMinute;

        /// <summary>
        /// The first minute at which the marker is valid
        /// </summary>
        public int OpenAtMinute
        {
            get => openAtMinute;
            set
            {
                if (value < 0 || value > 59)
                {
                    Logger.LogError("The minute value must be between 0 and 59");
                    return;
                }

                openAtMinute = value;

            }
        }
        /// <summary>
        /// The first minute at which the marker is no longer valid
        /// </summary>
        public int CloseAtMinute
        {
            get => closeAtMinute;
            set
            {
                if (value < 0 || value > 59)
                {
                    Logger.LogError("The minute value must be between 0 and 59");
                    return;
                }
                closeAtMinute = value;
            }
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

            if (OpenAtMinute < CloseAtMinute)
            {
                if (marker.MarkerLocation.TimeStamp.Minute < OpenAtMinute)
                    isConform = false;
                if (marker.MarkerLocation.TimeStamp.Minute > CloseAtMinute)
                    isConform = false;
                if (marker.MarkerLocation.TimeStamp.Minute == CloseAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                    isConform = false;
            }
            else if (OpenAtMinute > CloseAtMinute)
            {
                if ((marker.MarkerLocation.TimeStamp.Minute < OpenAtMinute) && (marker.MarkerLocation.TimeStamp.Minute > CloseAtMinute))
                    isConform = false;
                if (marker.MarkerLocation.TimeStamp.Minute == CloseAtMinute && marker.MarkerLocation.TimeStamp.Second > 0)
                    isConform = false;
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

        #region Private methods
        #endregion
    }
}
