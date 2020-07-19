using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class MarkerToGoalDistanceRule : IMarkerValidationRules
    {
        public double MinimumDistance
        {
            get; set;
        }

        public double MaximumDistance
        {
            get; set;
        }

        public DeclaredGoal Goal
        {
            get;set;
        }

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
    }
}
