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
using BalloonTrackAnalyze.ValidationControls;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class PieTierControl : UserControl
    {
        public PieTask.PieTier Tier { get; private set; }

        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        private Point RuleControlLocation = new Point(314, 168);

        private bool IsExisting { get; set; } = false;

        public PieTierControl()
        {
            InitializeComponent();
        }

        public void Prefill(PieTask.PieTier tier)
        {
            Tier = tier;
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
                foreach (IDeclarationValidationRules rule in Tier.DeclarationValidationRules)
                {
                    lbRules.Items.Add(rule);
                }
                IsExisting = true;
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
                if (!IsExisting)
                    Tier = new PieTask.PieTier();
                List<IDeclarationValidationRules> declarationValidationRules = new List<IDeclarationValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IDeclarationValidationRules)
                        if (!Tier.DeclarationValidationRules.Contains(item as IDeclarationValidationRules))
                            declarationValidationRules.Add(item as IDeclarationValidationRules);
                }
                Tier.SetupPieTier(goalNumber, radius, isReentranceAllowed, multiplier, lowerBoundary, upperBoundary, declarationValidationRules);
                IsExisting = false;
                tbGoalNumber.Text = "";
                tbRadius.Text = "";
                tbMultiplier.Text = "";
                tbLowerBoundary.Text = "";
                tbUpperBoundary.Text = "";
                lbRules.Items.Clear();
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

        private void cbRuleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbRuleList.SelectedItem.ToString())
            {
                case "Declaration to Goal Distance":
                    {
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new DeclarationToGoalDistanceRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        declarationToGoalDistanceRuleControl.Location = RuleControlLocation;
                        declarationToGoalDistanceRuleControl.Name = "ruleControl";
                        declarationToGoalDistanceRuleControl.DataValid += DeclarationToGoalDistanceRuleControl_DataValid;
                        Controls.Add(declarationToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Declaration to Goal Height":
                    {
                        DeclarationToGoalHeigthRuleControl declarationToGoalHeigthRuleControl = new DeclarationToGoalHeigthRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        declarationToGoalHeigthRuleControl.Location = RuleControlLocation;
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Goal to other Goals Distance":
                    {
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new GoalToOtherGoalsDistanceRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        goalToOtherGoalsDistanceRuleControl.Location = RuleControlLocation;
                        goalToOtherGoalsDistanceRuleControl.Name = "ruleControl";
                        goalToOtherGoalsDistanceRuleControl.DataValid += GoalToOtherGoalsDistanceRuleControl_DataValid;
                        Controls.Add(goalToOtherGoalsDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        private void lbRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbRules.SelectedItem)
            {
                case DeclarationToGoalDistanceRule declarationToGoalDistanceRule:
                    {
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new DeclarationToGoalDistanceRuleControl(declarationToGoalDistanceRule);
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        declarationToGoalDistanceRuleControl.Location = RuleControlLocation;
                        declarationToGoalDistanceRuleControl.Name = "ruleControl";
                        declarationToGoalDistanceRuleControl.DataValid += DeclarationToGoalDistanceRuleControl_DataValid;
                        Controls.Add(declarationToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case DeclarationToGoalHeightRule declarationToGoalHeightRule:
                    {
                        DeclarationToGoalHeigthRuleControl declarationToGoalHeigthRuleControl = new DeclarationToGoalHeigthRuleControl(declarationToGoalHeightRule);
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        declarationToGoalHeigthRuleControl.Location = RuleControlLocation;
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                case GoalToOtherGoalsDistanceRule goalToOtherGoalsDistance:
                    {
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new GoalToOtherGoalsDistanceRuleControl(goalToOtherGoalsDistance);
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        goalToOtherGoalsDistanceRuleControl.Location = RuleControlLocation;
                        goalToOtherGoalsDistanceRuleControl.Name = "ruleControl";
                        goalToOtherGoalsDistanceRuleControl.DataValid += GoalToOtherGoalsDistanceRuleControl_DataValid;
                        Controls.Add(goalToOtherGoalsDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                default:
                    break;
            }
        }

        private void GoalToOtherGoalsDistanceRuleControl_DataValid()
        {
            GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule = (Controls["ruleControl"] as GoalToOtherGoalsDistanceRuleControl).GoalToOtherGoalsDistanceRule;
            if (!lbRules.Items.Contains(goalToOtherGoalsDistanceRule))
                lbRules.Items.Add(goalToOtherGoalsDistanceRule);
            Logger.Log(this, LogSeverityType.Info, $"{goalToOtherGoalsDistanceRule} created/modified");
        }

        private void DeclarationToGoalHeigthRuleControl_DataValid()
        {
            DeclarationToGoalHeightRule declarationToGoalHeightRule = (Controls["ruleControl"] as DeclarationToGoalHeigthRuleControl).DeclarationToGoalHeightRule;
            if (!lbRules.Items.Contains(declarationToGoalHeightRule))
                lbRules.Items.Add(declarationToGoalHeightRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalHeightRule} created/modified");
        }
        private void DeclarationToGoalDistanceRuleControl_DataValid()
        {
            DeclarationToGoalDistanceRule declarationToGoalDistanceRule = (Controls["ruleControl"] as DeclarationToGoalDistanceRuleControl).DeclarationToGoalDistanceRule;
            if (!lbRules.Items.Contains(declarationToGoalDistanceRule))
                lbRules.Items.Add(declarationToGoalDistanceRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalDistanceRule} created/modified");
        }

        private void btRemoveRule_Click(object sender, EventArgs e)
        {
            if (lbRules.SelectedItem != null)
                lbRules.Items.Remove(lbRules.SelectedItem);
        }
    }
}
