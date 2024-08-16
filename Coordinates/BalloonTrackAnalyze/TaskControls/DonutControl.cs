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
    public partial class DonutControl : UserControl
    {
        #region Properties

        private readonly ILogger<DonutControl> Logger=LogConnector.LoggerFactory.CreateLogger<DonutControl>();

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
        private Point RuleControlLocation = new(0, 0);


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
                cbIsReetranceAllowed.Checked = Donut.IsReEntranceAllowed;

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
                foreach (IDeclarationValidationRule rule in Donut.DeclarationValidationRules)
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
            if (!int.TryParse(tbTaskNumber.Text, out int taskNumber))
            {
                Logger?.LogError("Failed to create/modify donut task: failed to parse Task No. '{taskNumber}' as integer", tbTaskNumber.Text);
                isDataValid = false;
            }
            if (taskNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify donut task: Task No. must be greater than 0");
                isDataValid = false;
            }
            if (!int.TryParse(tbGoalNumber.Text, out int goalNumber))
            {
                Logger?.LogError("Failed to create/modify donut task: failed to parse Goal No. '{goalNumber}' as integer", tbGoalNumber.Text);
                isDataValid = false;
            }
            if (goalNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify donut task: Goal No. must be greater than 0");
                isDataValid = false;
            }
            if (!double.TryParse(tbInnerRadius.Text, out double innerRadius))
            {
                Logger?.LogError("Failed to create/modify donut task: failed to parse Inner Radius '{innerRadius}' as double", tbInnerRadius.Text);
                isDataValid = false;
            }
            if (rbInnerRadiusFeet.Checked)
                innerRadius = CoordinateHelpers.ConvertToMeter(innerRadius);
            if (!double.TryParse(tbOuterRadius.Text, out double outerRadius))
            {
                Logger?.LogError("Failed to create/modify donut task: failed to parse Outer Radius '{outerRadius}' as double", tbOuterRadius.Text);
                isDataValid = false;
            }
            if (rbOuterRadiusFeet.Checked)
                outerRadius = CoordinateHelpers.ConvertToMeter(outerRadius);

            if (innerRadius >= outerRadius)
            {
                Logger?.LogError("Failed to create/modify donut task: Inner Radius '{innerRadius}[m]' must be smaller than Outer Radius '{outerRadius}[m]'", innerRadius, outerRadius);
                isDataValid = false;
            }
            bool isReentranceAllowed = cbIsReetranceAllowed.Checked;
            double lowerBoundary = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbLowerBoundary.Text))
            {
                if (!double.TryParse(tbLowerBoundary.Text, out lowerBoundary))
                {
                    Logger?.LogError("Failed to create/modify donut task: failed to parse Lower Boundary '{lowerBoundary}' as double", tbLowerBoundary.Text);
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
                    Logger?.LogError("Failed to create/modify donut task: failed to parse Upper Boundary '{upperBoundary}' as double", tbUpperBoundary.Text);
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
                    Logger?.LogError("Failed to create/modify donut task: Lower Boundary '{lowerBoundary}[m]' must be smaller than Upper Boundary '{upperBoundary}[m]'", lowerBoundary, upperBoundary);
                    isDataValid = false;
                }
            }
            //TODO add goal validations;
            if (isDataValid)
            {
                Donut ??= new DonutTask();
                List<IDeclarationValidationRule> declarationValidationRules = [];
                foreach (object item in lbRules.Items)
                {
                    if (item is IDeclarationValidationRule declarationValidationRule)
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
                        declarationToGoalDistanceRuleControl.TabIndex = 13;
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
                        declarationToGoalHeigthRuleControl.TabIndex = 13;
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
                        DeclarationToGoalDistanceRuleControl declarationToGoalDistanceRuleControl = new(declarationToGoalDistanceRule);
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
                        DeclarationToGoalHeightRuleControl declarationToGoalHeigthRuleControl = new(declarationToGoalHeightRule);
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
                        GoalToOtherGoalsDistanceRuleControl goalToOtherGoalsDistanceRuleControl = new(goalToOtherGoalsDistance);
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
