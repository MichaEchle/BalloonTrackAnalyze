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
        /// <para>mandotory</para>
        /// </summary>
        public DeclaredGoal Goal
        {
            get; set;
        } = null;
        #endregion

        #region API

        /// <summary>
        /// Check if the marker is conform to the distance rules to the declared goal
        /// </summary>
        /// <param name="marker">the marker to be checked</param>
        /// <returns>true: is conform; false: is not confrom</returns>
        public bool CheckConformance(MarkerDrop marker)
        {
            bool isConform = true;
            double distanceToGoal = CoordinateHelpers.Calculate2DDistance(marker.MarkerLocation, Goal.GoalDeclared);
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
        /// <param name="declaredGoal">The declared goal to check against (mandotory)</param>
        public void SetupRule(double minimumDistance, double maximumDistance, DeclaredGoal declaredGoal)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            Goal = declaredGoal;
        }

        public override string ToString()
        {
            return "Marker to Goal Distance Rule";
        }
        #endregion
    }
}
