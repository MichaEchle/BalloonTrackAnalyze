using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Competition.Validation
{
    public class DistanceToAllOtherGoalsRule : IDeclarationValidationRules
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

        public DistanceToAllOtherGoalsRule()
        {

        }

        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            foreach (DeclaredGoal otherGoal in DeclaredGoals)
            {
                if (declaredGoal.Equals(otherGoal))
                    continue;
                double distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.CalculateDistance2D( declaredGoal.GoalDeclared,otherGoal.GoalDeclared);

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
