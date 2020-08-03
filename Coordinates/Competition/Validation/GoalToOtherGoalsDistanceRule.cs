using Coordinates;
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
        public List<DeclaredGoal> DeclaredGoals
        {
            get; set;
        } = new List<DeclaredGoal>();

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
        /// <param name="declaredGoal">the declared goal to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            foreach (DeclaredGoal otherGoal in DeclaredGoals)
            {
                if (declaredGoal.Equals(otherGoal))
                    continue;
                double distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.Calculate2DDistance(declaredGoal.GoalDeclared, otherGoal.GoalDeclared);

                if (!double.IsNaN(MinimumDistance))
                    if (distanceBetweenPositionOfDeclarationAndDeclaredGoal < MinimumDistance)
                    {
                        isConform = false;
                        break;
                    }
                if (!double.IsNaN(MaximumDistance))
                    if (distanceBetweenPositionOfDeclarationAndDeclaredGoal > MaximumDistance)
                    {
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
        /// <param name="declaredGoals">List of the goals numbers to be considered (the last valid goal with that number will be used) [will not check against itself] (optional; use empty list to consider the last valid goal of each goal number). The actuals declaration objects will be fed in after track-preprocessing</param>
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
    }
}
