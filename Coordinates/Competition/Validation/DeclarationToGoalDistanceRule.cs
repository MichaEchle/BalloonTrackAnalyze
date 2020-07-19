using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class DeclarationToGoalDistanceRule : IDeclarationValidationRules
    {
        public double MinimumDistance
        {
            get; set;
        }

        public double MaximumDistance
        {
            get; set;
        }

        public DeclarationToGoalDistanceRule()
        {

        }

        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            double distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.Calculate2DDistance(declaredGoal.PositionAtDeclaration, declaredGoal.GoalDeclared);

            if (!double.IsNaN(MinimumDistance))
                if (distanceBetweenPositionOfDeclarationAndDeclaredGoal < MinimumDistance)
                    isConform = false;
            if (!double.IsNaN(MaximumDistance))
                if (distanceBetweenPositionOfDeclarationAndDeclaredGoal > MaximumDistance)
                    isConform = false;
            return isConform;
        }
    }
}
