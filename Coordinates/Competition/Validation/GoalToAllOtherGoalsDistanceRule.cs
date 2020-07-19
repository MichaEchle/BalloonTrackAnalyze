using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition
{
    public class GoalToAllOtherGoalsDistanceRule : IDeclarationValidationRules
    {
        public double MinimumDistance
        {
            get; set;
        }

        public double MaximumDistance
        {
            get; set;
        }

        public List<DeclaredGoal> DeclaredGoals
        {
            get; set;
        }

        public GoalToAllOtherGoalsDistanceRule()
        {

        }

        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            foreach (DeclaredGoal otherGoal in DeclaredGoals)
            {
                if (declaredGoal.Equals(otherGoal))
                    continue;
                double distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.Calculate2DDistance( declaredGoal.GoalDeclared,otherGoal.GoalDeclared);

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
    }
}
