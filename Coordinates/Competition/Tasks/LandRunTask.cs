using Competition.Validation;
using Coordinates;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Competition
{
    public class LandRunTask : ICompetitionTask
    {
        #region Properties
        [JsonIgnore()]
        private readonly ILogger<LandRunTask> Logger;
        /// <summary>
        /// The task number
        /// <para>mandatory</para>
        /// </summary>
        public int TaskNumber
        {
            get;
            set;
        } = -1;

        /// <summary>
        /// The marker number of the first marker
        /// <para>mandatory</para>
        /// </summary>
        public int FirstMarkerNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// The marker number of the second marker
        /// <para>mandatory</para>
        /// </summary>
        public int SecondMarkerNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// The marker number of the third marker
        /// <para>mandatory</para>
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
        /// Calculate the area of the triangle specified by the three markers in square meter
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
        /// <param name="result">the area of the triangle in square meter</param>
        /// <returns>true:success;false:error</returns>
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;

            if (!ValidationHelper.IsMarkerValid(track, FirstMarkerNumber, MarkerValidationRules))
            {
                Logger.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Marker '{markerNumber}' is invalid or doesn't exists", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), FirstMarkerNumber);
                return false;
            }

            if (!ValidationHelper.IsMarkerValid(track, SecondMarkerNumber, MarkerValidationRules))
            {
                Logger.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Marker '{markerNumber}' is invalid or doesn't exists", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), SecondMarkerNumber);
                return false;
            }

            if (!ValidationHelper.IsMarkerValid(track, ThirdMarkerNumber, MarkerValidationRules))
            {
                Logger.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Marker '{markerNumber}' is invalid or doesn't exists", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), ThirdMarkerNumber);
                return false;
            }
            MarkerDrop firstMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == FirstMarkerNumber);
            MarkerDrop secondMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == SecondMarkerNumber);
            MarkerDrop thirdMarker = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == ThirdMarkerNumber);
            result = CoordinateHelpers.CalculateArea(firstMarker.MarkerLocation, secondMarker.MarkerLocation, thirdMarker.MarkerLocation);
            return true;
        }

        /// <summary>
        /// Set all properties for a land run
        /// </summary>
        /// <param name="taskNumber">The task number (mandatory)</param>
        /// <param name="firstMarkerNumber">The marker number of the first marker (mandatory)</param>
        /// <param name="secondMarkerNumber">The marker number of the second marker (mandatory)</param>
        /// <param name="thirdMarkerNumber">The marker number of the third marker (mandatory)</param>
        /// <param name="markerValidationRules">List of rules for marker validation (optional; leave list empty to omit)</param>
        public void SetupLandRun(int taskNumber, int firstMarkerNumber, int secondMarkerNumber, int thirdMarkerNumber, List<IMarkerValidationRules> markerValidationRules)
        {
            TaskNumber = taskNumber;
            FirstMarkerNumber = firstMarkerNumber;
            SecondMarkerNumber = secondMarkerNumber;
            ThirdMarkerNumber = thirdMarkerNumber;
            MarkerValidationRules = markerValidationRules;
        }

        public override string ToString()
        {
            return $"Task#{TaskNumber} (Land Run)";
        }
        #endregion

        #region Private methods
        #endregion
    }
}
