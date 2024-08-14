using BalloonTrackAnalyze.TaskControls;
using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class GoalToOtherGoalsDistanceRuleControl : UserControl
    {
        #region Properties

        private readonly ILogger<GoalToOtherGoalsDistanceRuleControl> Logger = LogConnector.LoggerFactory.CreateLogger<GoalToOtherGoalsDistanceRuleControl>();
        /// <summary>
        /// The rule to be created or modified with this control
        /// </summary>
        public GoalToOtherGoalsDistanceRule GoalToOtherGoalsDistanceRule
        {
            get; private set;
        }

        /// <summary>
        /// Delegate for the DataValid event
        /// </summary>
        public delegate void DataValidDelegate();

        /// <summary>
        /// Event will be fired when the input for the rule is value
        /// </summary>
        public event DataValidDelegate DataValid;
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GoalToOtherGoalsDistanceRuleControl()
        {
            InitializeComponent();
            btCreate.Text = "Create rule";

        }

        /// <summary>
        /// Constructor which pre-fills control from existing rule
        /// </summary>
        /// <param name="goalToOtherGoalsDistanceRule">the existing goal to other goals distance rule</param>
        public GoalToOtherGoalsDistanceRuleControl(GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule)
        {
            GoalToOtherGoalsDistanceRule = goalToOtherGoalsDistanceRule;
            InitializeComponent();
            Prefill();
            btCreate.Text = "Create rule";
        }
        #endregion

        #region API
        /// <summary>
        /// Convert the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Declaration to Goal Distance Rule Setup Control";
        }
        #endregion

        #region private methods
        /// <summary>
        /// Pre-fills control form existing rule
        /// </summary>
        private void Prefill()
        {
            if (GoalToOtherGoalsDistanceRule != null)
            {
                if (!double.IsNaN(GoalToOtherGoalsDistanceRule.MinimumDistance))
                {
                    tbMinimumDistance.Text = Math.Round(GoalToOtherGoalsDistanceRule.MinimumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumDistanceMeter.Checked = true;
                }
                if (!double.IsNaN(GoalToOtherGoalsDistanceRule.MaximumDistance))
                {
                    tbMaximumDistance.Text = Math.Round(GoalToOtherGoalsDistanceRule.MaximumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMaximumDistanceMeter.Checked = true;
                }
                tbGoalNumbers.Text = string.Join(',', GoalToOtherGoalsDistanceRule.GoalNumbers);
            }
        }

        /// <summary>
        /// Validates the user input and creates new / modifies exsiting goal to other goals distance rule 
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            double minimumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumDistance.Text))
            {
                if (!double.TryParse(tbMinimumDistance.Text, out minimumDistance))
                {
                    Logger?.LogError("Failed to create/modify goal to other goals distance rule: failed to parse Min. Distance '{minimumDistance}' as double", tbMinimumDistance.Text);
                    isDataValid = false;
                }
                if (minimumDistance < 0)
                {
                    Logger?.LogError("Failed to create/modify goal to other goals distance rule: Min. Distance must be greater than zero");
                    isDataValid = false;
                }

                if (rbMinimumDistanceFeet.Checked)
                    minimumDistance = CoordinateHelpers.ConvertToMeter(minimumDistance);

            }
            double maximumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMaximumDistance.Text))
            {
                if (!double.TryParse(tbMaximumDistance.Text, out maximumDistance))
                {
                    Logger?.LogError("Failed to create/modify goal to other goals distance rule: failed to parse Max. Distance '{tbMinimumDistance.Text}' as double", tbMinimumDistance.Text);
                    isDataValid = false;
                }
                if (maximumDistance < 0)
                {
                    Logger?.LogError("Failed to create/modify goal to other goals distance rule: Max. Distance must be greater than zero");
                    isDataValid = false;
                }

                if (rbMaximumDistanceFeet.Checked)
                    maximumDistance = CoordinateHelpers.ConvertToMeter(maximumDistance);

            }
            if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
            {
                if (minimumDistance >= maximumDistance)
                {
                    Logger?.LogError("Failed to create/modify goal to other goals distance rule: Min. Distance '{minimumDistance}[m]' must be smaller than Max. Distance '{maximumDistance}[m]'", minimumDistance, maximumDistance);
                    isDataValid = false;
                }
            }

            List<int> goalNumbers = [];
            if (!string.IsNullOrWhiteSpace(tbGoalNumbers.Text))
                if (!tbGoalNumbers.Text.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                    goalNumbers = [.. Array.ConvertAll(tbGoalNumbers.Text.Split(','), int.Parse)];



            if (isDataValid)
            {
                GoalToOtherGoalsDistanceRule ??= new GoalToOtherGoalsDistanceRule();
                GoalToOtherGoalsDistanceRule.SetupRule(minimumDistance, maximumDistance, goalNumbers);
                tbMinimumDistance.Text = "";
                tbMaximumDistance.Text = "";
                tbGoalNumbers.Text = "";
                OnDataValid();
            }
        }

        /// <summary>
        /// Called when input for rule is valid
        /// </summary>
        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }
        #endregion
    }
}
