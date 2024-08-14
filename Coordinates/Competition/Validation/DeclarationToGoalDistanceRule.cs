using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;

namespace Competition
{
    public class DeclarationToGoalDistanceRule : IDeclarationValidationRules
    {
        #region Properties
        private readonly ILogger<DeclarationToGoalDistanceRule> Logger = LogConnector.LoggerFactory.CreateLogger<DeclarationToGoalDistanceRule>();

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
                    Logger?.LogWarning("Declaration '{goalNumber}' is not conform: '{minimumDistance}m' - '{distance}m' = '{abosluteInfringement}m' ('{relativeInfrigement}%') [minimum - actual = absolute (relative)]", declaration.GoalNumber, MinimumDistance.ToString("0.#"), distanceBetweenPositionOfDeclarationAndDeclaredGoal.ToString("0.#"), absoluteInfringement.ToString("0.#"), relativeInfringement.ToString("P1"));
                    isConform = false;
                }
            if (!double.IsNaN(MaximumDistance))
                if (distanceBetweenPositionOfDeclarationAndDeclaredGoal > MaximumDistance)
                {
                    double absoluteInfringement = distanceBetweenPositionOfDeclarationAndDeclaredGoal - MaximumDistance;
                    double relativeInfringement = absoluteInfringement / MaximumDistance;
                    Logger?.LogWarning("Declaration '{goalNumber}' is not conform: '{distance}m' - '{maximumDistance}m' = '{abosluteInfringement}m' ('{relativeInfrigement}%') [actual - minimum = absolute (relative)]", declaration.GoalNumber, distanceBetweenPositionOfDeclarationAndDeclaredGoal.ToString("0.#"), MaximumDistance.ToString("0.#"), absoluteInfringement.ToString("0.#"), relativeInfringement.ToString("P1"));
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
        #endregion
    }
}
