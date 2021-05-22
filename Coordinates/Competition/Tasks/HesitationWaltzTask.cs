using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Competition.Validation;

namespace Competition
{
    public class HesitationWaltzTask : ICompetitionTask
    {
        #region Properties
        public int TaskNumber
        {
            get;
            set;
        } = -1;

        public List<Coordinate> Goals
        {
            get; set;
        } = new List<Coordinate>();

        public int MarkerNumber
        {
            get; set;
        } = -1;

        public bool Use3DDistance
        {
            get; set;
        } = true;

        public Func<Track, List<Coordinate>> CalculateGoals
        {
            get; set;
        }

        public List<IMarkerValidationRules> MarkerValidationRules
        {
            get; set;
        }

        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            string functionErrorMessage = $"Failed to calculate result for {this} and Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}': ";
            result = 0.0;
            if (!ValidationHelper.IsMarkerValid(track, MarkerNumber, MarkerValidationRules))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Marker '{MarkerNumber}' is invalid or doesn't exists");
                return false;
            }

            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == MarkerNumber);

            List<double> distances = new List<double>();

            if (Goals.Count == 0 && CalculateGoals != null)
            {
                try
                {
                    Goals= CalculateGoals(track);
                }
                catch (Exception ex)
                {
                    Log(LogSeverityType.Error, $"Failed to calculate goals: {ex.Message}");
                    throw;
                }
            }

            foreach (Coordinate goal in Goals)
            {
                if (Use3DDistance)
                {
                    distances.Add(CoordinateHelpers.Calculate3DDistance(goal, markerDrop.MarkerLocation, useGPSAltitude));
                }
                else
                {
                    distances.Add(CoordinateHelpers.Calculate2DDistance(goal, markerDrop.MarkerLocation));
                }
            }

            result = distances.Min();
            return true;
        }


        #endregion

        #region API
        public void SetupHWZ(int taskNumber, List<Coordinate> goals, int markerNumber, bool use3DDistance, List<IMarkerValidationRules> markerValidationRules)
        {
            TaskNumber = taskNumber;
            Goals = goals;
            MarkerNumber = markerNumber;
            Use3DDistance = use3DDistance;
            MarkerValidationRules = markerValidationRules;

        }

        public void SetupHWZ(int taskNumber, Func<Track, List<Coordinate>> calculateGoals, int markerNumber, bool use3DDistance, List<IMarkerValidationRules> markerValidationRules)
        {
            TaskNumber = taskNumber;
            CalculateGoals = calculateGoals;
            MarkerNumber = markerNumber;
            Use3DDistance = use3DDistance;
            MarkerValidationRules = markerValidationRules;
        }

        public override string ToString()
        {
            return $"Task#{TaskNumber} (HWZ)";
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
