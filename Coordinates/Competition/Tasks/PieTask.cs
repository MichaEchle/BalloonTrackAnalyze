using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Competition
{
    public class PieTask : ICompetitionTask
    {
        #region Inner Class

        public class PieTier
        {
            #region Properties
            [JsonIgnore()]
            private readonly ILogger<PieTier> Logger = LogConnector.LoggerFactory.CreateLogger<PieTier>();

            /// <summary>
            /// The target goal number
            /// <para>mandatory</para>
            /// </summary>
            public int GoalNumber
            {
                get; set;
            } = -1;

            /// <summary>
            /// Specify whether or not re-entrance in the donut is allowed
            /// <para>mandatory</para>
            /// </summary>
            public bool IsReEntranceAllowed
            {
                get; set;
            } = true;

            /// <summary>
            /// The radius of the pie tier in meter
            /// <para>mandatory</para>
            /// </summary>
            public double Radius
            {
                get; set;
            } = double.NaN;

            /// <summary>
            /// Lower boundary of the pie tier in meter
            /// <para>optional; use double.NaN to omit</para>
            /// </summary>
            public double LowerBoundary
            {
                get; set;
            } = double.NaN;

            /// <summary>
            /// Upper boundary of the pie tier in meter
            /// <para>optional; use double.NaN to omit</para>
            /// </summary>
            public double UpperBoundary
            {
                get; set;
            } = double.NaN;

            /// <summary>
            /// Multiplier for the 2D distance traveled within the pie tier
            /// </summary>
            public double Multiplier
            {
                get; set;
            } = 1.0;

            /// <summary>
            /// The rule that defines if a declaration is valid
            /// <para>optional. use null to omit</para>
            /// <para>use <see cref="DeclarationAndRule"/> or <see cref="DeclarationOrRule"/> to chain multiple rules together</para>
            /// </summary>
            public IDeclarationValidationRule DeclarationValidationRule
            {
                get; set;
            } = null;

            /// <summary>
            /// The strictness by which validation rules are enforced
            /// </summary>
            public ValidationStrictnessType ValidationStrictness
            {
                get; set;
            } = ValidationStrictnessType.LatestValid;

            public PieTier()
            {

            }

            #endregion

            #region API

            /// <summary>
            /// Calculate the 2d distance traveled in the pie tier times the multiplier
            /// </summary>
            /// <param name="track">the track to be used</param>
            /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
            /// <param name="result">the 2D distance in the pie tier times the multiplier</param>
            /// <returns>true:success;false:error</returns>
            public bool CalculateTierResult(Track track, bool useGPSAltitude, out double result)
            {
                string functionErrorMessage = $"Failed to calculate result for {this} and Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}': ";
                result = 0.0;
                List<(int, Coordinate)> trackPointsInTier = [];

                Declaration targetDeclaration = ValidationHelper.GetValidDeclaration(track, GoalNumber, DeclarationValidationRule,ValidationStrictness);
                if (targetDeclaration is null)
                {
                    Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': No valid goal found for goal '#{goalNumber}'", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), GoalNumber);
                    return false;
                }
                List<Coordinate> coordinates = track.TrackPoints;
                if (!double.IsNaN(LowerBoundary))
                {
                    if (useGPSAltitude)
                        coordinates = coordinates.Where(x => x.AltitudeGPS >= LowerBoundary).ToList();//take all point above lower boundary
                    else
                        coordinates = coordinates.Where(x => x.AltitudeBarometric >= LowerBoundary).ToList();//take all point above lower boundary
                }
                if (!double.IsNaN(UpperBoundary))
                {
                    if (useGPSAltitude)
                        coordinates = coordinates.Where(x => x.AltitudeGPS <= UpperBoundary).ToList();//take all points below upper boundary
                    else
                        coordinates = coordinates.Where(x => x.AltitudeBarometric <= UpperBoundary).ToList();//take all points below upper boundary
                }

                for (int index = 0; index < coordinates.Count; index++)
                {
                    double distanceToGoal = CoordinateHelpers.Calculate2DDistanceHavercos(coordinates[index], targetDeclaration.DeclaredGoal);//calculate distance to goal
                    if (distanceToGoal <= Radius)//save all trackpoints within the radius
                        trackPointsInTier.Add((track.TrackPoints.FindIndex(x => x == coordinates[index]), coordinates[index]));

                }
                List<List<Coordinate>> chunksInTier = [];
                int addIndex = 0;
                chunksInTier.Add([]);
                for (int index = 0; index < trackPointsInTier.Count - 1; index++)
                {
                    if (trackPointsInTier[index + 1].Item1 - trackPointsInTier[index].Item1 == 1)//trackpoints are successive
                    {
                        if (chunksInTier[addIndex].Count == 0)
                        {
                            chunksInTier[addIndex].Add(trackPointsInTier[index].Item2);
                            chunksInTier[addIndex].Add(trackPointsInTier[index + 1].Item2);
                        }
                        else
                        {
                            chunksInTier[addIndex].Add(trackPointsInTier[index + 1].Item2);
                        }
                    }
                    else//trackpoints are not successive -> create new chunk
                    {
                        chunksInTier.Add([]);
                        addIndex++;
                    }
                }

                if (!IsReEntranceAllowed)//evaluate first chunk only
                {
                    if (chunksInTier[0].Count >= 2)
                    {
                        //for (int index = 0; index < chunksInDonut[0].Count - 1; index++)
                        //{
                        //    double tempResult = CoordinateHelpers.Calculate2DDistance(chunksInDonut[0][index], chunksInDonut[0][index + 1]);
                        //    result += tempResult;
                        //}
                        result += CoordinateHelpers.Calculate2DDistanceBetweenPoints(chunksInTier[0]);
                    }
                }
                else//evaluate all chunks
                {
                    for (int chuckIndex = 0; chuckIndex < chunksInTier.Count; chuckIndex++)
                    {
                        if (chunksInTier[chuckIndex].Count >= 2)
                        {
                            //for (int index = 0; index < chunksInDonut[chuckIndex].Count - 1; index++)
                            //{
                            //    double tempResult = CoordinateHelpers.Calculate2DDistance(chunksInDonut[chuckIndex][index], chunksInDonut[chuckIndex][index + 1]);
                            //    result += tempResult;
                            //}
                            result += CoordinateHelpers.Calculate2DDistanceBetweenPoints(chunksInTier[chuckIndex]);
                        }
                    }
                }
                result *= Multiplier;
                return true;
            }

            /// <summary>
            /// Set all properties for a pie tier
            /// </summary>
            /// <param name="goalNumber">The target goal number (mandatory)</param>
            /// <param name="radius">The radius of the pie tier in meter (mandatory)</param>
            /// <param name="lowerBoundary">Lower boundary of the donut in meter (optional; use double.NaN to omit)</param>
            /// <param name="upperBoundary">Upper boundary of the donut in meter (optional; use double.NaN to omit)</param>
            /// <param name="isReEntranceAllowed">Specify whether or not re-entrance in the donut is allowed (mandatory)</param>
            /// <param name="declarationValidationRules">List of rules for declaration validation (optional; leave list empty to omit)</param>
            public void SetupPieTier(int goalNumber, double radius, bool isReEntranceAllowed, double multiplier, double lowerBoundary, double upperBoundary, IDeclarationValidationRule declarationValidationRule, ValidationStrictnessType validationStrictness)
            {
                GoalNumber = goalNumber;
                Radius = radius;
                IsReEntranceAllowed = isReEntranceAllowed;
                Multiplier = multiplier;
                LowerBoundary = lowerBoundary;
                UpperBoundary = upperBoundary;
                DeclarationValidationRule = declarationValidationRule;
                ValidationStrictness = validationStrictness;
            }

            public override string ToString()
            {
                return "Pietier";
            }
            #endregion

            #region Private methods

            #endregion
        }
        #endregion

        #region Properties

        private readonly ILogger<PieTask> Logger = LogConnector.LoggerFactory.CreateLogger<PieTask>();
        /// <summary>
        /// The task number
        /// <para>mandatory</para>
        /// </summary>
        public int TaskNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// The list of pie tiers
        /// <para>mandatory</para>
        /// </summary>
        public List<PieTier> Tiers
        {
            get; set;
        } = [];
        #endregion

        public PieTask()
        {

        }

        #region API

        /// <summary>
        /// Calculates result in each pier tier and returns the sum
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
        /// <param name="result">the sum of all results in the pie tiers</param>
        /// <returns>true:success;false:error</returns>
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;
            foreach (PieTier tier in Tiers)
            {
                if (!tier.CalculateTierResult(track, useGPSAltitude, out double tempResult))
                {
                    Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Failed to calculate tier result", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                    return false;
                }
                result += tempResult;
            }
            return true;
        }
        /// <summary>
        /// Set all properties for a donut
        /// </summary>
        /// <param name="taskNumber">The task number (mandatory)</param>
        /// <param name="tiers">The list of pie tiers (mandatory)</param>
        public void SetupPie(int taskNumber, List<PieTier> tiers)
        {
            TaskNumber = taskNumber;
            Tiers = tiers;
        }

        public override string ToString()
        {
            return $"Task#{TaskNumber} (Pie)";
        }
        #endregion

        #region Private methods
        #endregion
    }


}
