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
        #region Properties

        /// <summary>
        /// The pie tier to be created or modified by this control
        /// </summary>
        public PieTask.PieTier Tier { get; private set; }

        /// <summary>
        /// The delegate for the DataValid event
        /// </summary>
        public delegate void DataValidDelegate();

        /// <summary>
        /// Event will be fired when the input for the elbow task is valid
        /// </summary>
        public event DataValidDelegate DataValid;

        /// <summary>
        /// Location for the user controls of the different rules
        /// </summary>
        private Point RuleControlLocation = new Point(0, 0);

        /// <summary>
        /// Indicates whether the pie tier shall be created or modified
        /// </summary>
        private bool IsExisting { get; set; } = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public PieTierControl()
        {
            InitializeComponent();
        }

        #endregion

        #region API
        /// <summary>
        /// Pre-fill control from existing pie tier
        /// </summary>
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

        /// <summary>
        /// Converts the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Pie Tier Setup Control";
        }
        #endregion


        #region private methods
        /// <summary>
        /// Validates user input and creates a new pie tier / modifies the existing pie tier
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
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

        /// <summary>
        /// Called when input for pie tier is valid
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
                        declarationToGoalDistanceRuleControl.TabIndex = 11;
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
                        declarationToGoalHeigthRuleControl.TabIndex = 11;
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
                        goalToOtherGoalsDistanceRuleControl.TabIndex = 11;
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
                        declarationToGoalDistanceRuleControl.TabIndex = 11;
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
                        declarationToGoalHeigthRuleControl.TabIndex = 11;
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
                        goalToOtherGoalsDistanceRuleControl.TabIndex = 11;
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
