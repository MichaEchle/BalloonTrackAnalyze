using Coordinates;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Competition.Validation
{
    public class HeightToGoalRule : IValidationRules
    {
        public double MinimumHeightDifference
        {
            get; set;
        }

        public double MaximumHeightDifference
        {
            get; set;
        }

        public bool UseGPSAltiude
        {
            get; set;
        }

        public HeightToGoalRule()
        {

        }

        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            double heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
            if (UseGPSAltiude)
                heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal = declaredGoal.PositionAtDeclaration.AltitudeGPS - declaredGoal.GoalDeclared.AltitudeGPS;
            else
                heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal = declaredGoal.PositionAtDeclaration.AltitudeBarometric - declaredGoal.GoalDeclared.AltitudeBarometric;
            if (!double.IsNaN(MinimumHeightDifference))
                if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal < MinimumHeightDifference)
                    isConform = false;
            if (!double.IsNaN(MaximumHeightDifference))
                if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal > MaximumHeightDifference)
                    isConform = false;
            return isConform;
        }
    }
}
