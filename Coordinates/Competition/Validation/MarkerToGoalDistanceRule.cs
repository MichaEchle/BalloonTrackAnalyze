using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerToGoalDistanceRule : IMarkerValidationRules
    {
        #region Properties

        /// <summary>
        /// Minimum distance between marker position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MinimumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Maximum distance between marker position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MaximumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// The declared goal to check against
        /// <para>needs preprocessing of track</para>
        /// <see cref="GoalNumber"/>
        /// </summary>
        public Declaration Declaration
        {
            get; set;
        } = null;

        /// <summary>
        /// The number of the goal to be checked against (the last valid goal with that number will be fed in after track preprocessing)
        /// <para>mandatory</para>
        /// </summary>
        public int GoalNumber { get; set; }

        #endregion

        #region API

        /// <summary>
        /// Check if the marker is conform to the distance rules to the declared goal
        /// </summary>
        /// <param name="marker">the marker to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool IsComplaintToRule(MarkerDrop marker)
        {
            bool isConform = true;
            double distanceToGoal = CoordinateHelpers.Calculate2DDistance(marker.MarkerLocation, Declaration.DeclaredGoal);
            if (!double.IsNaN(MinimumDistance))
            {
                if (distanceToGoal < MinimumDistance)
                    isConform = false;
            }
            if (!double.IsNaN(MaximumDistance))
            {
                if (distanceToGoal > MaximumDistance)
                    isConform = false;
            }
            return isConform;
        }

        /// <summary>
        /// Set all properties of the rule
        /// </summary>
        /// <param name="minimumDistance">Minimum distance between marker position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumDistance">Maximum distance between marker position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="goalNumber">The number of the goal to be checked against (the last valid goal with that number will be used) (mandatory). the actual declared goal object will be fed in after track preprocessing</param>
        public void SetupRule(double minimumDistance, double maximumDistance, int goalNumber)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            GoalNumber = goalNumber;
        }

        public override string ToString()
        {
            return "Marker to Goal Distance Rule";
        }
        #endregion
    }
}
