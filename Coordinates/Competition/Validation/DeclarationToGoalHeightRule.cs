using Coordinates;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Competition
{
    public class DeclarationToGoalHeightRule : IDeclarationValidationRules
    {
        #region Properties

        /// <summary>
        /// Minimum difference in height between declaration position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MinimumHeightDifference
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Maximum difference in height between declaration position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MaximumHeightDifference
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// true: use GPS altitude;false: use barometric altitude
        /// <para>mandotory</para>
        /// </summary>
        public bool UseGPSAltitude
        {
            get; set;
        } = true;
        #endregion

        public DeclarationToGoalHeightRule()
        {

        }

        #region API

        /// <summary>
        /// Check if the declared goal is conform to the height difference rules
        /// </summary>
        /// <param name="declaredGoal">the declared goal to be checked</param>
        /// <returns>true: is conform; false: is not confrom</returns>
        public bool CheckConformance(DeclaredGoal declaredGoal)
        {
            bool isConform = true;
            double heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
            if (UseGPSAltitude)
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

        /// <summary>
        /// Set all porperties of the rule
        /// </summary>
        /// <param name="minimumHeightDifference">Minimum difference in height between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumHeightDifference">Maximum difference in height between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="useGPSAltitude">rue: use GPS altitude;false: use barometric altitude</param>
        public void SetupRule(double minimumHeightDifference, double maximumHeightDifference, bool useGPSAltitude)
        {
            MinimumHeightDifference = minimumHeightDifference;
            MaximumHeightDifference = maximumHeightDifference;
            UseGPSAltitude = useGPSAltitude;
        }

        public override string ToString()
        {
            return "Declaration to Goal Height Rules";
        }
        #endregion
    }
}
