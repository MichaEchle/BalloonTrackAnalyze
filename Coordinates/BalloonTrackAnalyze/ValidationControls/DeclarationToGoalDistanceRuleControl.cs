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

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class DeclarationToGoalDistanceRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public DeclarationToGoalDistanceRule DeclarationToGoalDistanceRule { get; private set; }
        public DeclarationToGoalDistanceRuleControl()
        {
            InitializeComponent();
        }

        public DeclarationToGoalDistanceRuleControl(DeclarationToGoalDistanceRule declarationToGoalDistanceRule)
        {
            DeclarationToGoalDistanceRule = declarationToGoalDistanceRule;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (DeclarationToGoalDistanceRule != null)
            {
                if (!double.IsNaN(DeclarationToGoalDistanceRule.MinimumDistance))
                {
                    tbMinimumDistance.Text = Math.Round(DeclarationToGoalDistanceRule.MinimumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumDistanceMeter.Checked = true;
                }
                if (!double.IsNaN(DeclarationToGoalDistanceRule.MaximumDistance))
                {
                    tbMaximumDistance.Text = Math.Round(DeclarationToGoalDistanceRule.MaximumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMaximumDistanceMeter.Checked = true;
                }
            }
        }
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify declaration to goal distance rule: ";
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

            if (isDataValid)
            {
                DeclarationToGoalDistanceRule ??= new DeclarationToGoalDistanceRule();
                DeclarationToGoalDistanceRule.SetupRule(minimumDistance, maximumDistance);
                tbMaximumDistance.Text = "";
                tbMinimumDistance.Text = "";
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
