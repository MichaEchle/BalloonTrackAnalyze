using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Competition;
using LoggerComponent;
using System.Reflection;
using Coordinates;
using BalloonTrackAnalyze.ValidationControls;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class DonutControl : UserControl
    {
        public DonutTask Donut { get; private set; }

        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public DonutControl()
        {
            InitializeComponent();
        }

        public DonutControl(DonutTask donut)
        {
            Donut = donut;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (Donut != null)
            {
                tbTaskNumber.Text = Donut.TaskNumber.ToString();
                tbGoalNumber.Text = Donut.GoalNumber.ToString();
                tbInnerRadius.Text = Math.Round(Donut.InnerRadius, 3, MidpointRounding.AwayFromZero).ToString();
                rbInnerRadiusMeter.Checked = true;
                //rbInnerRadiusFeet.Checked = false;
                tbOuterRadius.Text = Math.Round(Donut.OuterRadius, 3, MidpointRounding.AwayFromZero).ToString();
                rbOuterRadiusMeter.Checked = true;
                //rbOuterRadiusFeet.Checked = false;
                cbIsReetranceAllowed.Checked = Donut.IsReentranceAllowed;
                if (!double.IsNaN(Donut.LowerBoundary))
                {
                    tbLowerBoundary.Text = Math.Round(Donut.LowerBoundary, 3, MidpointRounding.AwayFromZero).ToString();
                    rbLowerBoundaryMeter.Checked = true;
                    //rbLowerBoundaryFeet.Checked = false;
                }
                if (!double.IsNaN(Donut.UpperBoundary))
                {
                    tbUpperBoundary.Text = Math.Round(Donut.UpperBoundary, 3, MidpointRounding.AwayFromZero).ToString();
                    rbUpperBoundaryMeter.Checked = true;
                    rbUpperBoundaryFeet.Checked = false;
                }
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify donut task: ";
            int taskNumber;
            if (!int.TryParse(tbTaskNumber.Text, out taskNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Task No. '{tbTaskNumber.Text}' as integer");
                isDataValid = false;
            }
            if (taskNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Task No. must be greater than 0");
                isDataValid = false;
            }
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
            double innerRadius;
            if (!double.TryParse(tbInnerRadius.Text, out innerRadius))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Inner Radius '{tbInnerRadius.Text}' as double");
                isDataValid = false;
            }
            if (rbInnerRadiusFeet.Checked)
                innerRadius = CoordinateHelpers.ConvertToMeter(innerRadius);
            double outerRadius;
            if (!double.TryParse(tbOuterRadius.Text, out outerRadius))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Outer Radius '{tbOuterRadius.Text}' as double");
                isDataValid = false;
            }
            if (rbOuterRadiusFeet.Checked)
                outerRadius = CoordinateHelpers.ConvertToMeter(outerRadius);

            if (innerRadius >= outerRadius)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Inner Radius '{innerRadius}[m]' must be smaller than Outer Radius '{outerRadius}[m]'");
                isDataValid = false;
            }
            bool isReentranceAllowed = cbIsReetranceAllowed.Checked;
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
                Donut = new DonutTask();
                List<IDeclarationValidationRules> declarationValidationRules = new List<IDeclarationValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IDeclarationValidationRules)
                        declarationValidationRules.Add(item as IDeclarationValidationRules);
                }

                Donut.SetupDonut(taskNumber, goalNumber, 1, innerRadius, outerRadius, lowerBoundary, upperBoundary, isReentranceAllowed, declarationValidationRules);
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Donut Setup Control";
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
                        declarationToGoalDistanceRuleControl.Location = new Point(0, 368);
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
                        declarationToGoalHeigthRuleControl.Location = new Point(0, 368);
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                default:
                    break;
            }
        }

        private void DeclarationToGoalDistanceRuleControl_DataValid()
        {
            DeclarationToGoalDistanceRule declarationToGoalDistanceRule = (Controls["ruleControl"] as DeclarationToGoalDistanceRuleControl).DeclarationToGoalDistanceRule;
            lbRules.Items.Add(declarationToGoalDistanceRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalDistanceRule} created");
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
                        declarationToGoalDistanceRuleControl.Location = new Point(0, 368);
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
                        declarationToGoalHeigthRuleControl.Location = new Point(0, 368);
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                default:
                    break;
            }
        }

        private void DeclarationToGoalHeigthRuleControl_DataValid()
        {
            DeclarationToGoalHeightRule declarationToGoalHeightRule = (Controls["ruleControl"] as DeclarationToGoalHeigthRuleControl).DeclarationToGoalHeightRule;
            lbRules.Items.Add(declarationToGoalHeightRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalHeightRule} created");
        }

        private void btRemoveRule_Click(object sender, EventArgs e)
        {
            if (lbRules.SelectedItem != null)
                lbRules.Items.Remove(lbRules.SelectedItem);
        }
    }
}
