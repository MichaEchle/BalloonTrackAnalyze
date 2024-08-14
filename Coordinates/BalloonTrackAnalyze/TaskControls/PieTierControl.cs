using BalloonTrackAnalyze.ValidationControls;
using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class PieTierControl : UserControl
    {
        #region Properties

        private readonly ILogger<PieTierControl> Logger = LogConnector.LoggerFactory.CreateLogger<PieTierControl>();
        /// <summary>
        /// The pie tier to be created or modified by this control
        /// </summary>
        public PieTask.PieTier Tier
        {
            get; private set;
        }


        public bool IsNewTier
        {
            get; private set;
        }
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
        private Point RuleControlLocation = new(0, 0);


        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public PieTierControl()
        {
            InitializeComponent();
            IsNewTier = true;
            btCreate.Text = "Create tier";
        }

        #endregion

        #region API
        /// <summary>
        /// Pre-fill control from existing pie tier
        /// </summary>
        public void Prefill(PieTask.PieTier pieTier)
        {
            Tier = pieTier;
            if (Tier is not null)
            {
                IsNewTier = false;
                btCreate.Text = "Modify tier";
                tbGoalNumber.Text = Tier.GoalNumber.ToString();
                tbRadius.Text = Math.Round(Tier.Radius, 3, MidpointRounding.AwayFromZero).ToString();
                rbRadiusMeter.Checked = true;
                //rbInnerRadiusFeet.Checked = false;
                cbIsReetranceAllowed.Checked = Tier.IsReEntranceAllowed;
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
                lbRules.Items.Clear();
                foreach (IDeclarationValidationRules rule in Tier.DeclarationValidationRules)
                {
                    lbRules.Items.Add(rule);
                }
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
            if (!int.TryParse(tbGoalNumber.Text, out int goalNumber))
            {
                Logger?.LogError("Failed to create/modify pie tier: failed to parse Goal No. '{goalNumber}' as integer", tbGoalNumber.Text);
                isDataValid = false;
            }
            if (goalNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify pie tier: Goal No. must be greater than 0");
                isDataValid = false;
            }
            if (!double.TryParse(tbRadius.Text, out double radius))
            {
                Logger?.LogError("Failed to create/modify pie tier: failed to parse Radius '{radius}' as double", tbRadius.Text);
                isDataValid = false;
            }
            if (rbRadiusFeet.Checked)
                radius = CoordinateHelpers.ConvertToMeter(radius);
            bool isReentranceAllowed = cbIsReetranceAllowed.Checked;
            if (!double.TryParse(tbMultiplier.Text, out double multiplier))
            {
                Logger?.LogError("Failed to create/modify pie tier: failed to parse Multiplier '{multiplier}' as double", tbMultiplier.Text);
                isDataValid = false;
            }

            double lowerBoundary = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbLowerBoundary.Text))
            {
                if (!double.TryParse(tbLowerBoundary.Text, out lowerBoundary))
                {
                    Logger?.LogError("Failed to create/modify pie tier: failed to parse Lower Boundary '{lowerBoundary}' as double", tbLowerBoundary.Text);
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
                    Logger?.LogError("Failed to create/modify pie tier: failed to parse Upper Boundary '{upperBoundary}' as double", tbUpperBoundary.Text);
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
                    Logger?.LogError("Failed to create/modify pie tier: Lower Boundary '{lowerBoundary}' must be smaller than Upper Boundary '{upperBoundary}'", lowerBoundary, upperBoundary);
                    isDataValid = false;
                }
            }
            //TODO add goal validations;
            if (isDataValid)
            {
                if (IsNewTier)
                    Tier = new PieTask.PieTier();
                List<IDeclarationValidationRules> declarationValidationRules = [];
                foreach (object item in lbRules.Items)
                {
                    if (item is IDeclarationValidationRules declarationValidationRule)
                        declarationValidationRules.Add(declarationValidationRule);
                }
                Tier.SetupPieTier(goalNumber, radius, isReentranceAllowed, multiplier, lowerBoundary, upperBoundary, declarationValidationRules);
                tbGoalNumber.Text = "";
                tbRadius.Text = "";
                tbMultiplier.Text = "";
                tbLowerBoundary.Text = "";
                tbUpperBoundary.Text = "";
                lbRules.Items.Clear();
                plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                cbRuleList.SelectedIndex = 0;
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
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new();
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
                        DeclarationToGoalHeightRuleControl declarationToGoalHeigthRuleControl = new();
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
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new();
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
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new(declarationToGoalDistanceRule);
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
                        DeclarationToGoalHeightRuleControl declarationToGoalHeigthRuleControl = new(declarationToGoalHeightRule);
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
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new(goalToOtherGoalsDistance);
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
            Logger?.LogInformation("'{goalToOtherGoalsDistanceRule}' created/modified", goalToOtherGoalsDistanceRule);
        }

        /// <summary>
        /// Adds a new declaration to goal height rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void DeclarationToGoalHeigthRuleControl_DataValid()
        {
            DeclarationToGoalHeightRule declarationToGoalHeightRule = (plRuleControl.Controls["ruleControl"] as DeclarationToGoalHeightRuleControl).DeclarationToGoalHeightRule;
            if (!lbRules.Items.Contains(declarationToGoalHeightRule))
                lbRules.Items.Add(declarationToGoalHeightRule);
            Logger?.LogInformation("'{declarationToGoalHeightRule}' created/modified", declarationToGoalHeightRule);
        }

        /// <summary>
        /// Adds a new declaration to goal distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void DeclarationToGoalDistanceRuleControl_DataValid()
        {
            DeclarationToGoalDistanceRule declarationToGoalDistanceRule = (plRuleControl.Controls["ruleControl"] as DeclarationToGoalDistanceRuleControl).DeclarationToGoalDistanceRule;
            if (!lbRules.Items.Contains(declarationToGoalDistanceRule))
                lbRules.Items.Add(declarationToGoalDistanceRule);
            Logger?.LogInformation("'{declarationToGoalDistanceRule}' created/modified", declarationToGoalDistanceRule);
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
