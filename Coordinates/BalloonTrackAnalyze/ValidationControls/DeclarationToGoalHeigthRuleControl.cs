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
    public partial class DeclarationToGoalHeigthRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public DeclarationToGoalHeightRule DeclarationToGoalHeightRule { get; private set; }

        public DeclarationToGoalHeigthRuleControl()
        {
            InitializeComponent();
        }

        public DeclarationToGoalHeigthRuleControl(DeclarationToGoalHeightRule declarationToGoalHeightRule)
        {
            DeclarationToGoalHeightRule = declarationToGoalHeightRule;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (DeclarationToGoalHeightRule != null)
            {
                tbMinimumHeightDifference.Text = DeclarationToGoalHeightRule.MinimumHeightDifference.ToString();
                rbMinimumHeightDifferenceMeter.Checked = true;
                tbMaximumHeightDifference.Text = DeclarationToGoalHeightRule.MaximumHeightDifference.ToString();
                rbMaximumHeightDifferenceMeter.Checked = true;
                rbGPS.Checked = DeclarationToGoalHeightRule.UseGPSAltitude;
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify declaration to goal height rule: ";
            double minimumHeightDifference = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumHeightDifference.Text))
            {
                if (!double.TryParse(tbMinimumHeightDifference.Text, out minimumHeightDifference))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Min. Height Diff. '{tbMinimumHeightDifference.Text}' as double");
                    isDataValid = false;
                }
                if (minimumHeightDifference < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Height Diff. '{minimumHeightDifference}' must be greater than zero");
                    isDataValid = false;
                }

                if (rbMinimumHeightDifferenceFeet.Checked)
                    minimumHeightDifference = CoordinateHelpers.ConvertToMeter(minimumHeightDifference);

            }
            double maximumHeightDifference = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMaximumHeightDifference.Text))
            {
                if (!double.TryParse(tbMaximumHeightDifference.Text, out maximumHeightDifference))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Max. Height Diff. '{tbMaximumHeightDifference.Text}' as double");
                    isDataValid = false;
                }
                if (maximumHeightDifference < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Max. Height Diff. '{maximumHeightDifference}' must be greater than zero");
                    isDataValid = false;
                }

                if (rbMaximumHeightDifferenceFeet.Checked)
                    maximumHeightDifference = CoordinateHelpers.ConvertToMeter(maximumHeightDifference);

            }
            if (!double.IsNaN(minimumHeightDifference) && !double.IsNaN(maximumHeightDifference))
            {
                if (minimumHeightDifference >= maximumHeightDifference)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Height Diff. '{minimumHeightDifference}[m]' must be smaller than Max. Height Diff. '{maximumHeightDifference}[m]'");
                    isDataValid = false;
                }
            }
            bool useGPSAltitude = rbGPS.Checked;

            if (isDataValid)
            {
                DeclarationToGoalHeightRule ??= new DeclarationToGoalHeightRule();
                DeclarationToGoalHeightRule.SetupRule(minimumHeightDifference,maximumHeightDifference,useGPSAltitude);

                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Declaration to Goal Height Rule Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
