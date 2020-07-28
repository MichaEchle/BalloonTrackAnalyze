﻿using Competition.Validation;
using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Competition
{
    public class LandRunTask : ICompetitionTask
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
        /// Calculate the area of the triangle specified by the three markers in square meter
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
        /// <param name="result">the area of the triangle in square meter</param>
        /// <returns>true:success;false:error</returns>
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            string functionErrorMessage = $"Failed to caluclate result for {this} and Pilot '#{track.Pilot.PilotNumber}': ";
            result = 0.0;
            MarkerDrop firstMarker = track.MarkerDrops.First(x => x.MarkerNumber == FirstMarkerNumber);
            if (firstMarker == null)
            {
                //Console.WriteLine($"No Marker '{FirstMarkerNumber}' found");
                Log(LogSeverityType.Error, functionErrorMessage + $"No Marker '{FirstMarkerNumber}' found");
                return false;
            }
            if (!ValidationHelper.IsMarkerValid(firstMarker, MarkerValidationRules))
            {
                //Console.WriteLine($"Marker '{FirstMarkerNumber}' is not valid");
                Log(LogSeverityType.Error, functionErrorMessage + $"Marker '{FirstMarkerNumber}' is invalid");
                return false;
            }
            MarkerDrop secondMarker = track.MarkerDrops.First(x => x.MarkerNumber == SecondMarkerNumber);
            if (secondMarker == null)
            {
                //Console.WriteLine($"No Marker '{SecondMarkerNumber}' found");
                Log(LogSeverityType.Error, functionErrorMessage + $"No Marker '{SecondMarkerNumber}' found");
                return false;
            }
            if (!ValidationHelper.IsMarkerValid(secondMarker, MarkerValidationRules))
            {
                //Console.WriteLine($"Marker '{SecondMarkerNumber}' is not valid");
                Log(LogSeverityType.Error, functionErrorMessage + $"Marker '{SecondMarkerNumber}' is invalid");
                return false;
            }
            MarkerDrop thirdMarker = track.MarkerDrops.First(x => x.MarkerNumber == ThirdMarkerNumber);
            if (thirdMarker == null)
            {
                //Console.WriteLine($"No Marker '{ThirdMarkerNumber}' found");
                Log(LogSeverityType.Error, functionErrorMessage + $"No Marker '{ThirdMarkerNumber}' found");
                return false;
            }
            if (!ValidationHelper.IsMarkerValid(thirdMarker, MarkerValidationRules))
            {
                //Console.WriteLine($"Marker '{ThirdMarkerNumber}' is not valid");
                Log(LogSeverityType.Error, functionErrorMessage + $"Marker '{ThirdMarkerNumber}' is invalid");
                return false;
            }
            result = CoordinateHelpers.CalculateArea(firstMarker.MarkerLocation, secondMarker.MarkerLocation, thirdMarker.MarkerLocation);
            return true;
        }

        /// <summary>
        /// Set all properties for a land run
        /// </summary>
        /// <param name="taskNumber">The task number (mandotory)</param>
        /// <param name="firstMarkerNumber">The marker number of the first marker (mandotory)</param>
        /// <param name="secondMarkerNumber">The marker number of the second marker (mandotory)</param>
        /// <param name="thirdMarkerNumber">The marker number of the third marker (mandotory)</param>
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
            return $"Land Run (Task #{TaskNumber})";
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
