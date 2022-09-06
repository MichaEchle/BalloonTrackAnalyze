using Competition.Validation;
using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Competition
{
    public class DonutTask : ICompetitionTask
    {
        #region Properties
        /// <summary>
        /// The task number
        /// <para>mandatory</para>
        /// </summary>
        [JsonProperty("Task number")]
        public int TaskNumber
        {
            get;
            set;
        } = -1;

        /// <summary>
        /// The target goal number
        /// <para>mandatory</para>
        /// </summary>
        [JsonProperty("Goal number")]
        public int GoalNumber
        {
            get; set;
        } = -1;

        /// <summary>
        /// Number of allowed declarations
        /// <para>currently not used</para>
        /// </summary>
        [JsonProperty("Number of declarations")]
        public int NumberOfDeclarations
        {
            get; set;
        } = 1;

        /// <summary>
        /// The radius of the inner circle in meter
        /// <para>mandatory</para>
        /// </summary>
        [JsonProperty("Inner radius [m]")]
        public double InnerRadius
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// The radius of the outer circle in meter
        /// <para>mandatory</para>
        /// </summary>
        [JsonProperty("Outer radius [m]")]
        public double OuterRadius
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Lower boundary of the donut in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        [JsonProperty("Lower boundary [m]")]
        public double LowerBoundary
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Upper boundary of the donut in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        [JsonProperty("Upper boundary [m]")]
        public double UpperBoundary
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Specify whether or not re-entrance in the donut is allowed
        /// <para>mandatory</para>
        /// </summary>
        [JsonProperty("Is reentrance allowed")]
        public bool IsReentranceAllowed
        {
            get; set;
        } = true;

        /// <summary>
        /// List of rules for declaration validation
        /// <para>optional; leave list empty to omit</para>
        /// </summary>
        [JsonIgnore]
        public List<IDeclarationValidationRules> DeclarationValidationRules
        {
            get; set;
        } = new List<IDeclarationValidationRules>();
        #endregion

        #region API
        /// <summary>
        /// Calculate the 2D distance traveled in the donut in meter
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="useGPSAltitude">true: use GPS altitude;false: use barometric altitude</param>
        /// <param name="result">the 2D distance in the donut in meter</param>
        /// <returns>true:success;false:error</returns>
        public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            string functionErrorMessage = $"Failed to calculate result for {this} and Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}': ";
            result = 0.0;
            List<(int trackPointNumber, Coordinate coordinate)> trackPointsInDonut = new List<(int trackPointNumber, Coordinate coordinate)>();

            Declaration targetDeclaration = ValidationHelper.GetValidDeclaration(track, GoalNumber, DeclarationValidationRules);
            if (targetDeclaration == null)
            {
                //Debug.WriteLine("No valid goal found");
                Log(LogSeverityType.Error, functionErrorMessage + $"No valid goal found for goal '#{GoalNumber}'");
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
                if (distanceToGoal <= OuterRadius && distanceToGoal >= InnerRadius)//save all trackpoints between outer and inner radius
                    trackPointsInDonut.Add((track.TrackPoints.FindIndex(x => x == coordinates[index]), coordinates[index]));

            }
            List<List<Coordinate>> chunksInDonut = new List<List<Coordinate>>();
            int addIndex = 0;
            chunksInDonut.Add(new List<Coordinate>());
            for (int index = 0; index < trackPointsInDonut.Count - 1; index++)
            {
                if (trackPointsInDonut[index + 1].trackPointNumber - trackPointsInDonut[index].trackPointNumber == 1)//trackpoints are successive
                {
                    if (chunksInDonut[addIndex].Count == 0)
                    {
                        chunksInDonut[addIndex].Add(trackPointsInDonut[index].coordinate);
                        chunksInDonut[addIndex].Add(trackPointsInDonut[index + 1].coordinate);
                    }
                    else
                    {
                        chunksInDonut[addIndex].Add(trackPointsInDonut[index + 1].coordinate);
                    }
                }
                else//trackpoints are not successive -> create new chunk
                {
                    chunksInDonut.Add(new List<Coordinate>());
                    addIndex++;
                }
            }


            if (!IsReentranceAllowed)//evaluate first chunk only
            {
                if (chunksInDonut[0].Count >= 2)
                {
                    //for (int index = 0; index < chunksInDonut[0].Count - 1; index++)
                    //{
                    //    double tempResult = CoordinateHelpers.Calculate2DDistance(chunksInDonut[0][index], chunksInDonut[0][index + 1]);
                    //    result += tempResult;
                    //}
                    result += CoordinateHelpers.Calculate2DDistanceBetweenPoints(chunksInDonut[0]);
                }
            }
            else//evaluate all chunks
            {
                for (int chuckIndex = 0; chuckIndex < chunksInDonut.Count; chuckIndex++)
                {
                    if (chunksInDonut[chuckIndex].Count >= 2)
                    {
                        //for (int index = 0; index < chunksInDonut[chuckIndex].Count - 1; index++)
                        //{
                        //    double tempResult = CoordinateHelpers.Calculate2DDistance(chunksInDonut[chuckIndex][index], chunksInDonut[chuckIndex][index + 1]);
                        //    result += tempResult;
                        //}
                        result += CoordinateHelpers.Calculate2DDistanceBetweenPoints(chunksInDonut[chuckIndex]);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Set all properties for a donut
        /// </summary>
        /// <param name="taskNumber">The task number (mandatory)</param>
        /// <param name="goalNumber">The target goal number (mandatory)</param>
        /// <param name="numberOfDeclarations">Number of allowed declarations (not used yet)</param>
        /// <param name="innerRadius">The radius of the inner circle in meter (mandatory)</param>
        /// <param name="outerRadius">The radius of the outer circle in meter (mandatory)</param>
        /// <param name="lowerBoundary">Lower boundary of the donut in meter (optional; use double.NaN to omit)</param>
        /// <param name="upperBoundary">Upper boundary of the donut in meter (optional; use double.NaN to omit)</param>
        /// <param name="isReentranceAllowed">Specify whether or not re-entrance in the donut is allowed (mandatory)</param>
        /// <param name="declarationValidationRules">List of rules for declaration validation (optional; leave list empty to omit)</param>
        public void SetupDonut(int taskNumber, int goalNumber, int numberOfDeclarations, double innerRadius, double outerRadius, double lowerBoundary, double upperBoundary, bool isReentranceAllowed, List<IDeclarationValidationRules> declarationValidationRules)
        {
            TaskNumber = taskNumber;
            GoalNumber = goalNumber;
            NumberOfDeclarations = numberOfDeclarations;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
            IsReentranceAllowed = isReentranceAllowed;
            DeclarationValidationRules = declarationValidationRules;
        }

        public override string ToString()
        {
            return $"Task#{TaskNumber} (Donut)";
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
