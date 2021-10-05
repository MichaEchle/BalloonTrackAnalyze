using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Competition
{
    public class DeclarationToGoalHeightRule : IDeclarationValidationRules
    {
        #region Enum(s)

        public enum HeightDifferenceType
        {
            /// <summary>
            /// The declared goal must be lower than the declaration position
            /// </summary>
            NegativeDifferenceOnly=-1,
            /// <summary>
            /// The declared goal must be higher or lower than the declaration position
            /// </summary>
            AbsoluteDifference=0,
            /// <summary>
            /// The declared goal must be higher then the declaration position
            /// </summary>
            PositiveDifferenceOnly=1
        }
        #endregion Enum(s)


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
        /// <para>mandatory</para>
        /// </summary>
        public bool UseGPSAltitude
        {
            get; set;
        } = true;

        /// <summary>
        /// Defines the type of height difference
        /// <para>NegativeDifferenceOnly: Declared goal must be lower than declaration position</para>
        /// <para>AbsoltueDifference: Declared goal must be higher or lower than declaration position</para>
        /// <para>PostivieDifferenceOnly: Declared goal must be higher than declaration position</para>
        /// </summary>
        public HeightDifferenceType HeightDifference
        {
            get; set;
        } = HeightDifferenceType.AbsoluteDifference;
        #endregion

        public DeclarationToGoalHeightRule()
        {

        }

        #region API

        /// <summary>
        /// Check if the declaration is conform to the height difference rules
        /// </summary>
        /// <param name="declaration">the declaration to be checked</param>
        /// <returns>true: is conform; false: is not conform</returns>
        public bool IsComplaintToRule(Declaration declaration)
        {
            bool isConform = true;
            double heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
            if (UseGPSAltitude)
                heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal =   declaration.DeclaredGoal.AltitudeGPS- declaration.PositionAtDeclaration.AltitudeGPS;
            else
                heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal =   declaration.DeclaredGoal.AltitudeBarometric- declaration.PositionAtDeclaration.AltitudeBarometric;

            switch (HeightDifference)
            {
                case HeightDifferenceType.NegativeDifferenceOnly:
                    if (!double.IsNaN(MinimumHeightDifference))
                    {
                        double tempMinimumDifference = MinimumHeightDifference;
                        if (tempMinimumDifference > 0)
                            tempMinimumDifference *= -1;
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal > tempMinimumDifference)
                        {
                            double absoluteInfringement = tempMinimumDifference - heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
                            double relativeInfringement = Math.Abs(absoluteInfringement / tempMinimumDifference);
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {tempMinimumDifference:0.#}m - {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }
                    }
                    if (!double.IsNaN(MaximumHeightDifference))
                    {
                        double tempMaximumDifference = MaximumHeightDifference;
                        if (tempMaximumDifference > 0)
                            tempMaximumDifference *= -1;
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal < tempMaximumDifference)
                        {
                            double absoluteInfringement =  heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal-tempMaximumDifference;
                            double relativeInfringement = Math.Abs(absoluteInfringement / tempMaximumDifference);
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m - {tempMaximumDifference:0.#}m  = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }
                    }
                    break;
                case HeightDifferenceType.AbsoluteDifference:
                    heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal = Math.Abs(heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal);
                    if (!double.IsNaN(MinimumHeightDifference))
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal < Math.Abs(MinimumHeightDifference))
                        {
                            double absoluteInfringement = Math.Abs(MinimumHeightDifference) - heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
                            double relativeInfringement = Math.Abs(absoluteInfringement / Math.Abs(MinimumHeightDifference));
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {Math.Abs(MinimumHeightDifference):0.#}m - {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }
                    if (!double.IsNaN(MaximumHeightDifference))
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal > Math.Abs(MaximumHeightDifference))
                        {
                            double absoluteInfringement = heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal - Math.Abs(MaximumHeightDifference);
                            double relativeInfringement = Math.Abs(absoluteInfringement / Math.Abs(MaximumHeightDifference));
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m - {Math.Abs(MaximumHeightDifference):0.#}m  = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }
                    break;
                case HeightDifferenceType.PositiveDifferenceOnly:
                    if (!double.IsNaN(MinimumHeightDifference))
                    {
                        double tempMinimumDifference = MinimumHeightDifference;
                        if (tempMinimumDifference < 0)
                            tempMinimumDifference *= -1;
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal < tempMinimumDifference)
                        {
                            double absoluteInfringement = tempMinimumDifference - heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal;
                            double relativeInfringement = Math.Abs(absoluteInfringement / tempMinimumDifference);
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {tempMinimumDifference:0.#}m - {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }
                    }
                    if (!double.IsNaN(MaximumHeightDifference))
                    {
                        double tempMaximumDifference = MaximumHeightDifference;
                        if (tempMaximumDifference < 0)
                            tempMaximumDifference *= -1;
                        if (heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal > tempMaximumDifference)
                        {
                            double absoluteInfringement = heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal - tempMaximumDifference;
                            double relativeInfringement = Math.Abs(absoluteInfringement / tempMaximumDifference);
                            Log(LogSeverityType.Warning, $"Declaration {declaration.GoalNumber} is not conform: {heightDifferenceBetweenPositionOfDeclarationAndDeclaredGoal:0.#}m - {tempMaximumDifference:0.#}m  = {absoluteInfringement:0.#}m ({relativeInfringement:P1}) [minimum - actual = absolute (relative)]");
                            isConform = false;
                        }    
                    }
                    break;
            }

            return isConform;
        }

        /// <summary>
        /// Set all properties of the rule
        /// </summary>
        /// <param name="minimumHeightDifference">Minimum difference in height between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="maximumHeightDifference">Maximum difference in height between declaration position and declared goal in meter (optional; use double.NaN to omit)</param>
        /// <param name="useGPSAltitude">rue: use GPS altitude;false: use barometric altitude</param>
        public void SetupRule(double minimumHeightDifference, double maximumHeightDifference,HeightDifferenceType heightDifference, bool useGPSAltitude)
        {
            MinimumHeightDifference = minimumHeightDifference;
            MaximumHeightDifference = maximumHeightDifference;
            HeightDifference = heightDifference;
            UseGPSAltitude = useGPSAltitude;
        }

        public override string ToString()
        {
            return "Declaration to Goal Height Rules";
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
