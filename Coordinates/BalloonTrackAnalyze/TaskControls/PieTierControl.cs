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

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class PieTierControl : UserControl
    {
        public PieTask.PieTier Tier { get; private set; }

        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public PieTierControl()
        {
            InitializeComponent();
        }

        public PieTierControl(PieTask.PieTier tier)
        {
            Tier = tier;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (Tier != null)
            {
                tbGoalNumber.Text = Tier.GoalNumber.ToString();
                tbRadius.Text = Math.Round(Tier.Radius, 3, MidpointRounding.AwayFromZero).ToString();
                rbRadiusMeter.Checked = true;
                //rbInnerRadiusFeet.Checked = false;
                cbIsReetranceAllowed.Checked = Tier.IsReentranceAllowed;
                tbMultiplier.Text = Tier.Multiplier.ToString();

                if (!double.IsNaN(Tier.LowerBoundary))
                {
                    tbLowerBoundary.Text = Math.Round(Tier.LowerBoundary, 3, MidpointRounding.AwayFromZero).ToString();
                    rbLowerBoundaryMeter.Checked = true;
                    //rbLowerBoundaryFeet.Checked = false;
                }
                if (!double.IsNaN(Tier.UpperBoundary))
                {
                    tbUpperBoundary.Text = Math.Round(Tier.UpperBoundary, 3, MidpointRounding.AwayFromZero).ToString();
                    rbUpperBoundaryMeter.Checked = true;
                    rbUpperBoundaryFeet.Checked = false;
                }
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify pie tier: ";
            int goalNumber;
            if (!int.TryParse(tbGoalNumber.Text, out goalNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Goal No. '{tbGoalNumber.Text}' as integer");
                isDataValid = false;
            }
            if (goalNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Goal No. must be greater than 0");
                isDataValid = false;
            }
            double radius;
            if (!double.TryParse(tbRadius.Text, out radius))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Radius '{tbRadius.Text}' as double");
                isDataValid = false;
            }
            if (rbRadiusFeet.Checked)
                radius = CoordinateHelpers.ConvertToMeter(radius);
            bool isReentranceAllowed = cbIsReetranceAllowed.Checked;
            double multiplier;
            if (!double.TryParse(tbMultiplier.Text, out multiplier))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Multiplier '{tbMultiplier.Text}' as double");
                isDataValid = false;
            }

            double lowerBoundary = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbLowerBoundary.Text))
            {
                if (!double.TryParse(tbLowerBoundary.Text, out lowerBoundary))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Lower Boundary '{tbLowerBoundary.Text}' as double");
                    isDataValid = false;
                }
                if (!double.IsNaN(lowerBoundary))
                    if (rbLowerBoundaryFeet.Checked)
                        lowerBoundary = CoordinateHelpers.ConvertToMeter(lowerBoundary);
            }
            double upperBoundary = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbUpperBoundary.Text))
            {
                if (!double.TryParse(tbUpperBoundary.Text, out upperBoundary))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Upper Boundary '{tbUpperBoundary.Text}' as double");
                    isDataValid = false;
                }
                if (!double.IsNaN(upperBoundary))
                    if (rbUpperBoundaryFeet.Checked)
                        upperBoundary = CoordinateHelpers.ConvertToMeter(upperBoundary);
            }
            if (!double.IsNaN(lowerBoundary) && double.IsNaN(upperBoundary))
            {
                if (lowerBoundary >= upperBoundary)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Lower Boundary '{lowerBoundary}[m]' must be smaller than Upper Boundary '{upperBoundary}[m]'");
                    isDataValid = false;
                }
            }
            //TODO add goal validations;
            if (isDataValid)
            {
                Tier = new PieTask.PieTier();
                Tier.SetupPieTier( goalNumber, radius,isReentranceAllowed,multiplier, lowerBoundary, upperBoundary, null);
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Pie Tier Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
