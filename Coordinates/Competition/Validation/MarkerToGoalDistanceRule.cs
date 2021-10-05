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
        /// Specify whether 3D or 2D distance between marker and goal should be considered
        /// </summary>
        public bool Use3DDistance
        {
            get; set;
        } = false;

        /// <summary>
        /// Specify whether GPS or barometric altitude should be used to calculate 3D distance
        /// </summary>
        public bool UseGPSAltitude
        {
            get; set;
        } = false;

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
            if (Declaration != null)
            {
                double distanceBetweenMarkerAndGoal;
                if (Use3DDistance)
                {
                    distanceBetweenMarkerAndGoal = CoordinateHelpers.Calculate3DDistance(marker.MarkerLocation, Declaration.DeclaredGoal, UseGPSAltitude);
                }
                else
                {
                    distanceBetweenMarkerAndGoal = CoordinateHelpers.Calculate2DDistance(marker.MarkerLocation, Declaration.DeclaredGoal);
                }
                if (!double.IsNaN(MinimumDistance))
                {
                    if (distanceBetweenMarkerAndGoal < MinimumDistance)
                        isConform = false;
                }
                if (!double.IsNaN(MaximumDistance))
                {
                    if (distanceBetweenMarkerAndGoal > MaximumDistance)
                        isConform = false;
                }
            }
            else
            {
                isConform = false;
            }
            return isConform;
        }

        /// <summary>
        /// Set all properties of the rule
        /// </summary>
        /// <param name="minimumDistance">Minimum distance between marker position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumDistance">Maximum distance between marker position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="use3DDistance">True: 3D distance is considered; false: only 2D distance is considered</param>
        /// <param name="useGPSAltitude">True: GPS altitude is used to calculate 3D distance; false: barometric altitude is used to calculate 3D distance</param>
        /// <param name="goalNumber">The number of the goal to be checked against (the last valid goal with that number will be used) (mandatory). the actual declared goal object will be fed in after track preprocessing</param>
        public void SetupRule(double minimumDistance, double maximumDistance,bool use3DDistance,bool useGPSAltitude, int goalNumber)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
            Use3DDistance = use3DDistance;
            UseGPSAltitude = useGPSAltitude;
            GoalNumber = goalNumber;
        }

        public override string ToString()
        {
            return "Marker to Goal Distance Rule";
        }
        #endregion
    }
}
