using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Competition;
using LoggerComponent;
using Coordinates;
using System.Linq;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class GoalToOtherGoalsDistanceRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public GoalToOtherGoalsDistanceRule GoalToOtherGoalsDistanceRule { get; private set; }
        public GoalToOtherGoalsDistanceRuleControl()
        {
            InitializeComponent();
        }

        public GoalToOtherGoalsDistanceRuleControl(GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule)
        {
            GoalToOtherGoalsDistanceRule = goalToOtherGoalsDistanceRule;
            InitializeComponent();
            Prefill();
        }

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

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify goal to other goals distance rule: ";
            double minimumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumDistance.Text))
            {
                if (!double.TryParse(tbMinimumDistance.Text, out minimumDistance))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Min. Distance '{tbMinimumDistance.Text}' as double");
                    isDataValid = false;
                }
                if (minimumDistance < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Distance '{minimumDistance}' must be greater than zero");
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
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Max. Distance '{tbMinimumDistance.Text}' as double");
                    isDataValid = false;
                }
                if (maximumDistance < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Max. Distance '{maximumDistance}' must be greater than zero");
                    isDataValid = false;
                }

                if (rbMaximumDistanceFeet.Checked)
                    maximumDistance = CoordinateHelpers.ConvertToMeter(maximumDistance);

            }
            if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
            {
                if (minimumDistance >= maximumDistance)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Distance '{minimumDistance}[m]' must be smaller than Max. Distance '{maximumDistance}[m]'");
                    isDataValid = false;
                }
            }

            List<int> goalNumbers = new List<int>();
            if (!string.IsNullOrWhiteSpace(tbGoalNumbers.Text))
                if (tbGoalNumbers.Text.ToLowerInvariant() != "all")
                    goalNumbers = Array.ConvertAll(tbGoalNumbers.Text.Split(','), int.Parse).ToList();



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

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Declaration to Goal Distance Rule Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
