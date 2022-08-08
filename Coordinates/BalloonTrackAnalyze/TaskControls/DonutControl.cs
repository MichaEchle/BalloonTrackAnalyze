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
        #region Properties

        /// <summary>
        /// The donut task to be created or modified with this control
        /// </summary>
        public DonutTask Donut
        {
            get; private set;
        }

        public bool IsNewTask
        {
            get; private set;
        }

        /// <summary>
        /// Delegate for DataValid event
        /// </summary>
        public delegate void DataValidDelegate();

        /// <summary>
        /// Event will be fired when the input for the donut control is valid
        /// </summary>
        public event DataValidDelegate DataValid;

        /// <summary>
        /// Location for the user controls of the different rules
        /// </summary>
        private Point RuleControlLocation = new Point(0, 0);

        
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DonutControl()
        {
            InitializeComponent();
            IsNewTask = true;
            btCreate.Text = "Create Task";
        }

        /// <summary>
        /// Constructor which pre-fills controls from existing donut task
        /// </summary>
        /// <param name="donut">the existing donut task</param>
        public DonutControl(DonutTask donut)
        {
            Donut = donut;
            IsNewTask = false;
            InitializeComponent();
            btCreate.Text = "Modify Task";
            Prefill();
        }
        #endregion

        #region API

        /// <summary>
        /// Converts the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Donut Setup Control";
        }

        #endregion

        #region private methods
        /// <summary>
        /// Pre-fill controls from existing task
        /// </summary>
        private void Prefill()
        {
            if (Donut is not null)
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
                    //rbUpperBoundaryFeet.Checked = false;
                }
                foreach (IDeclarationValidationRules rule in Donut.DeclarationValidationRules)
                {
                    lbRules.Items.Add(rule);
                }
            }
        }

        /// <summary>
        /// Validates user input and creates new donut task / modifies the existing donut task
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
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
                Donut ??= new DonutTask();
                List<IDeclarationValidationRules> declarationValidationRules = new List<IDeclarationValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IDeclarationValidationRules declarationValidationRule)
                            declarationValidationRules.Add(declarationValidationRule);
                }

                Donut.SetupDonut(taskNumber, goalNumber, 1, innerRadius, outerRadius, lowerBoundary, upperBoundary, isReentranceAllowed, declarationValidationRules);

                tbTaskNumber.Text = "";
                tbGoalNumber.Text = "";
                tbInnerRadius.Text = "";
                tbOuterRadius.Text = "";
                tbLowerBoundary.Text = "";
                tbUpperBoundary.Text = "";
                lbRules.Items.Clear();
                OnDataValid();
            }
        }

        /// <summary>
        /// Called when input for donut task is valid
        /// </summary>
        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }
        /// <summary>
        /// Logs a user messages
        /// </summary>
        /// <param name="logSeverity">the severity of the message</param>
        /// <param name="text">the message text</param>
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }

        /// <summary>
        /// Displays the corresponding user control for the selected rule
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void cbRuleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbRuleList.SelectedItem.ToString())
            {
                case "Declaration to Goal Distance":
                    {
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new DeclarationToGoalDistanceRuleControl();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        declarationToGoalDistanceRuleControl.Location = RuleControlLocation;
                        declarationToGoalDistanceRuleControl.Name = "ruleControl";
                        declarationToGoalDistanceRuleControl.TabIndex = 13;
                        declarationToGoalDistanceRuleControl.DataValid += DeclarationToGoalDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(declarationToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Declaration to Goal Height":
                    {
                        DeclarationToGoalHeigthRuleControl declarationToGoalHeigthRuleControl = new DeclarationToGoalHeigthRuleControl();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        declarationToGoalHeigthRuleControl.Location = RuleControlLocation;
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.TabIndex = 13;
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        plRuleControl.Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Goal to other Goals Distance":
                    {
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new GoalToOtherGoalsDistanceRuleControl();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        goalToOtherGoalsDistanceRuleControl.Location = RuleControlLocation;
                        goalToOtherGoalsDistanceRuleControl.Name = "ruleControl";
                        goalToOtherGoalsDistanceRuleControl.TabIndex = 13;
                        goalToOtherGoalsDistanceRuleControl.DataValid += GoalToOtherGoalsDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(goalToOtherGoalsDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        /// <summary>
        /// Displays the pre-filled user control of the selected rule
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void lbRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbRules.SelectedItem)
            {
                case DeclarationToGoalDistanceRule declarationToGoalDistanceRule:
                    {
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new DeclarationToGoalDistanceRuleControl(declarationToGoalDistanceRule);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        declarationToGoalDistanceRuleControl.Location = RuleControlLocation;
                        declarationToGoalDistanceRuleControl.Name = "ruleControl";
                        declarationToGoalDistanceRuleControl.TabIndex = 13;
                        declarationToGoalDistanceRuleControl.DataValid += DeclarationToGoalDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(declarationToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case DeclarationToGoalHeightRule declarationToGoalHeightRule:
                    {
                        DeclarationToGoalHeigthRuleControl declarationToGoalHeigthRuleControl = new DeclarationToGoalHeigthRuleControl(declarationToGoalHeightRule);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        declarationToGoalHeigthRuleControl.Location = RuleControlLocation;
                        declarationToGoalHeigthRuleControl.Name = "ruleControl";
                        declarationToGoalHeigthRuleControl.TabIndex = 13;
                        declarationToGoalHeigthRuleControl.DataValid += DeclarationToGoalHeigthRuleControl_DataValid;
                        plRuleControl.Controls.Add(declarationToGoalHeigthRuleControl);
                        ResumeLayout();
                    }
                    break;
                case GoalToOtherGoalsDistanceRule goalToOtherGoalsDistance:
                    {
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new GoalToOtherGoalsDistanceRuleControl(goalToOtherGoalsDistance);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        goalToOtherGoalsDistanceRuleControl.Location = RuleControlLocation;
                        goalToOtherGoalsDistanceRuleControl.Name = "ruleControl";
                        goalToOtherGoalsDistanceRuleControl.TabIndex = 13;
                        goalToOtherGoalsDistanceRuleControl.DataValid += GoalToOtherGoalsDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(goalToOtherGoalsDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds a new goal to other goals distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void GoalToOtherGoalsDistanceRuleControl_DataValid()
        {
            GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule = (plRuleControl.Controls["ruleControl"] as GoalToOtherGoalsDistanceRuleControl).GoalToOtherGoalsDistanceRule;
            if (!lbRules.Items.Contains(goalToOtherGoalsDistanceRule))
                lbRules.Items.Add(goalToOtherGoalsDistanceRule);
            Logger.Log(this, LogSeverityType.Info, $"{goalToOtherGoalsDistanceRule} created/modified");
        }

        /// <summary>
        /// Adds a new declaration to goal height rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void DeclarationToGoalHeigthRuleControl_DataValid()
        {
            DeclarationToGoalHeightRule declarationToGoalHeightRule = (plRuleControl.Controls["ruleControl"] as DeclarationToGoalHeigthRuleControl).DeclarationToGoalHeightRule;
            if (!lbRules.Items.Contains(declarationToGoalHeightRule))
                lbRules.Items.Add(declarationToGoalHeightRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalHeightRule} created/modified");
        }

        /// <summary>
        /// Adds a new declaration to goal distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void DeclarationToGoalDistanceRuleControl_DataValid()
        {
            DeclarationToGoalDistanceRule declarationToGoalDistanceRule = (plRuleControl.Controls["ruleControl"] as DeclarationToGoalDistanceRuleControl).DeclarationToGoalDistanceRule;
            if (!lbRules.Items.Contains(declarationToGoalDistanceRule))
                lbRules.Items.Add(declarationToGoalDistanceRule);
            Logger.Log(this, LogSeverityType.Info, $"{declarationToGoalDistanceRule} created/modified");
        }

        /// <summary>
        /// Removes the selected rule form the list box
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btRemoveRule_Click(object sender, EventArgs e)
        {
            if (lbRules.SelectedItem != null)
                lbRules.Items.Remove(lbRules.SelectedItem);
        }
        #endregion
    }
}
