using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Coordinates;
using LoggerComponent;
using Competition;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerToGoalDistanceRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public MarkerToGoalDistanceRule MarkerToGoalDistanceRule { get; private set; }
        public MarkerToGoalDistanceRuleControl()
        {
            InitializeComponent();
        }
        public MarkerToGoalDistanceRuleControl(MarkerToGoalDistanceRule markerToGoalDistanceRule)
        {
            MarkerToGoalDistanceRule = markerToGoalDistanceRule;
            InitializeComponent();
            Prefill();
        }


        private void Prefill()
        {
            if (MarkerToGoalDistanceRule != null)
            {
                if (!double.IsNaN(MarkerToGoalDistanceRule.MinimumDistance))
                {
                    tbMinimumDistance.Text = Math.Round(MarkerToGoalDistanceRule.MinimumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumDistanceMeter.Checked = true;
                }
                if (!double.IsNaN(MarkerToGoalDistanceRule.MaximumDistance))
                {
                    tbMaximumDistance.Text = Math.Round(MarkerToGoalDistanceRule.MaximumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumDistanceMeter.Checked = true;
                }
                tbGoalNumber.Text = MarkerToGoalDistanceRule.GoalNumber.ToString();
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify marker to goal distance rule: ";
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
            int goalNumber;
            if (!int.TryParse(tbGoalNumber.Text, out goalNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Goal No. '{tbGoalNumber.Text}' as integer");
                isDataValid = false;
            }
            if (goalNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Goal No '{goalNumber}' must be greater than zero");
                isDataValid = false;
            }
            if (isDataValid)
            {
                MarkerToGoalDistanceRule ??= new MarkerToGoalDistanceRule();
                MarkerToGoalDistanceRule.SetupRule(minimumDistance, maximumDistance, goalNumber);
                tbMinimumDistance.Text = "";
                tbMaximumDistance.Text = "";
                tbGoalNumber.Text = "";
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Marker to Goal Distance Rule Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
