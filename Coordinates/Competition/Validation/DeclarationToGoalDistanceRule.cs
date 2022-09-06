using Coordinates;
using System;
using System.Collections.Generic;
using System.Text;
using LoggerComponent;

namespace Competition
{
    public class DeclarationToGoalDistanceRule : IDeclarationValidationRules
    {
        #region Properties

        /// <summary>
        /// Minimum distance between declaration position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MinimumDistance
        {
            get; set;
        } = double.NaN;

        /// <summary>
        /// Maximum distance between declaration position and declared goal in meter
        /// <para>optional; use double.NaN to omit</para>
        /// </summary>
        public double MaximumDistance
        {
            get; set;
        } = double.NaN;

        #endregion
        public DeclarationToGoalDistanceRule()
        {

        }
        #region API

        /// <summary>
        /// Check if declaration is conform to the distance rules
        /// </summary>
        /// <param name="declaration">the declaration to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool IsComplaintToRule(Declaration declaration)
        {
            bool isConform = true;
            double distanceBetweenPositionOfDeclarationAndDeclaredGoal = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);

            if (!double.IsNaN(MinimumDistance))
                if (distanceBetweenPositionOfDeclarationAndDeclaredGoal < MinimumDistance)
                {
                    double absoluteInfringement = MinimumDistance - distanceBetweenPositionOfDeclarationAndDeclaredGoal;
                    double relativeInfringement = absoluteInfringement / MinimumDistance;
                    Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {MinimumDistance:0.#}m - {distanceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}%) [minimum - actual = absolute (relative)] ");
                    isConform = false;
                }
            if (!double.IsNaN(MaximumDistance))
                if (distanceBetweenPositionOfDeclarationAndDeclaredGoal > MaximumDistance)
                {
                    double absoluteInfringement = distanceBetweenPositionOfDeclarationAndDeclaredGoal - MaximumDistance;
                    double relativeInfringement = absoluteInfringement / MaximumDistance;
                    Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {distanceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m - {MaximumDistance:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}%) [actual - minimum = absolute (relative)] ");
                    isConform = false;

                }
            return isConform;
        }

        /// <summary>
        /// Set all properties of the rule
        /// </summary>
        /// <param name="minimumDistance">Minimum distance between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumDistance">Maximum distance between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        public void SetupRule(double minimumDistance, double maximumDistance)
        {
            MinimumDistance = minimumDistance;
            MaximumDistance = maximumDistance;
        }

        public override string ToString()
        {
            return "Declaration to Goal Distance Rule";
        }

        #endregion
        #region Private methods
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
        #endregion
    }
}
