using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition
{
    public class HesitationWaltzTask : ICompetitionTask
    {
        #region Properties
        [JsonIgnore()]
        private readonly ILogger<HesitationWaltzTask> Logger = LogConnector.LoggerFactory.CreateLogger<HesitationWaltzTask>();

        public int TaskNumber
        {
            get;
            set;
        } = -1;

        public List<Coordinate> Goals
        {
            get; set;
        } = [];

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




        #endregion

        #region API
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            string functionErrorMessage = $"Failed to calculate result for {this} and Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}': ";
            result = double.NaN;
            if (!ValidationHelper.IsMarkerValid(track, MarkerNumber, MarkerValidationRules))
            {
                Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Marker '{markerNumber}' is invalid or doesn't exists", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), MarkerNumber);
                return false;
            }

            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == MarkerNumber);
            if (markerDrop != null)
            {
                List<double> distances = [];
                result = 0.0;

                if (Goals.Count == 0 && CalculateGoals != null)
                {
                    try
                    {
                        Goals = CalculateGoals(track);
                        if (Goals.Count == 0)
                        {
                            Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': No goals could be calculated", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex,"Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}'", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                        return false;
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
                        distances.Add(CoordinateHelpers.Calculate2DDistanceHavercos(goal, markerDrop.MarkerLocation));
                    }
                }

                result = distances.Min();
            }
            return true;
        }
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
        #endregion
    }
}
