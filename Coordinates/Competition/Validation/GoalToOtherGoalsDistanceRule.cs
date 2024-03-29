﻿using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Competition
{
    public class GoalToOtherGoalsDistanceRule : IDeclarationValidationRules
    {
        #region Properties

        /// <summary>
        /// Minimum distance to other declared goals in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MinimumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Maximum distance to other declared goals in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MaximumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// List of goals to be checked against (will not check against itself)
        /// <para>needs preprocessing of the tracks</para>
        /// <see cref="GoalNumbers"/>
        /// </summary>
        public List<Declaration> Declarations
        {
            get; set;
        } = new List<Declaration>();

        /// <summary>
        /// List of the goals numbers to be considered and fed into after preprocessing (the last valid goal with that number will be used)
        /// <para>optional; use empty list to consider the last valid goal of each goal number </para>
        /// </summary>
        public List<int> GoalNumbers { get; set; } = new List<int>();
        #endregion

        public GoalToOtherGoalsDistanceRule()
        {

        }
        #region API

        /// <summary>
        /// Check if the declared goal is conform to the distance rules to other goals
        /// <para>will not check against itself</para>
        /// </summary>
        /// <param name="declaration">the declared goal to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool IsComplaintToRule(Declaration declaration)
        {
            bool isConform = true;
            foreach (Declaration otherGoal in Declarations)
            {
                if (declaration.Equals(otherGoal))
                    continue;
                double distanceBetweenGoals = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.DeclaredGoal, otherGoal.DeclaredGoal);

                if (!double.IsNaN(MinimumDistance))
                    if (distanceBetweenGoals < MinimumDistance)
                    {
                        double absoluteInfringement = MinimumDistance - distanceBetweenGoals;
                        double relativeInfringement = absoluteInfringement / MinimumDistance;
                        Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {MinimumDistance:0.#}m - {distanceBetweenGoals:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}%) [minimum - actual = absolute (relative)] ");
                        isConform = false;
                        break;
                    }
                if (!double.IsNaN(MaximumDistance))
                    if (distanceBetweenGoals > MaximumDistance)
                    {
                        double absoluteInfringement = distanceBetweenGoals - MaximumDistance;
                        double relativeInfringement = absoluteInfringement / MaximumDistance;
                        Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {distanceBetweenGoals:0.#}m - {MaximumDistance:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}%) [actual - minimum = absolute (relative)] ");
                        isConform = false;
                        break;
                    }
            }
            return isConform;
        }

        /// <summary>
        /// Set all properties of the rule
        /// </summary>
        /// <param name="minimumDistance">Minimum distance between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumDistance">Maximum distance between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="goalNumbers">List of the goals numbers to be considered (the last valid goal with that number will be used) [will not check against itself] (optional; use empty list to consider the last valid goal of each goal number). The actuals declaration objects will be fed in after track-preprocessing</param>
        public void SetupRule(double minimumDistance, double maximumDistance, List<int> goalNumbers)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            GoalNumbers = goalNumbers;
        }

        public override string ToString()
        {
            return "Goal to other Goals Distance Rules";
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
