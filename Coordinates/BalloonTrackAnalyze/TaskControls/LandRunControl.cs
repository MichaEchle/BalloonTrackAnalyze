using BalloonTrackAnalyze.ValidationControls;
using Competition;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class LandRunControl : UserControl
    {
        #region Properties

        private readonly ILogger<LandRunControl> Logger = LogConnector.LoggerFactory.CreateLogger<LandRunControl>();


        /// <summary>
        /// The land run task to be created or modified by this control
        /// </summary>
        public LandRunTask LandRun
        {
            get; private set;
        }

        public bool IsNewTask
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

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public LandRunControl()
        {
            InitializeComponent();
            IsNewTask = true;
            btCreate.Text = "Create task";
            ;
        }

        /// <summary>
        /// Constructor which pre-fills controls from existing land run task
        /// </summary>
        /// <param name="landRun">the existing land run task</param>
        public LandRunControl(LandRunTask landRun)
        {
            LandRun = landRun;
            InitializeComponent();
            IsNewTask = false;
            btCreate.Text = "Modify task";
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
            return "Landrun Setup Control";
        }

        #endregion

        #region private methods
        /// <summary>
        /// Pre-fill controls from existing task
        /// </summary>
        private void Prefill()
        {
            if (LandRun != null)
            {
                tbTaskNumber.Text = LandRun.TaskNumber.ToString();
                tbFirstMarkerNumber.Text = LandRun.FirstMarkerNumber.ToString();
                tbSecondMarkerNumber.Text = LandRun.SecondMarkerNumber.ToString();
                tbThirdMarkerNumber.Text = LandRun.ThirdMarkerNumber.ToString();
                foreach (IMarkerValidationRules rule in LandRun.MarkerValidationRules)
                {
                    lbRules.Items.Add(rule);
                }
            }
        }

        /// <summary>
        /// Validates user input and creates a new land run task / modifies the existing land run task
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            if (!int.TryParse(tbTaskNumber.Text, out int taskNumber))
            {
                Logger?.LogError("Failed to create/modify landrun task: failed to parse Task No. '{TaskNumber}' as integer", tbTaskNumber.Text);
                isDataValid = false;
            }
            if (taskNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify landrun task: Task No. must be greater than 0");
                isDataValid = false;
            }
            if (!int.TryParse(tbFirstMarkerNumber.Text, out int firstMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify landrun task: failed to parse 1st Marker No '{FirstMarkerNumber}' as integer", tbFirstMarkerNumber.Text);
                isDataValid = false;
            }
            if (firstMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify landrun task: 1st Marker No. must be greater than 0");
                isDataValid = false;
            }

            if (!int.TryParse(tbSecondMarkerNumber.Text, out int secondMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify landrun task: failed to parse 2nd Marker No '{SecondMarkerNumber}' as integer", tbSecondMarkerNumber.Text);
                isDataValid = false;
            }
            if (secondMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify landrun task: 2nd Marker No. must be greater than 0");
                isDataValid = false;
            }

            if (!int.TryParse(tbThirdMarkerNumber.Text, out int thirdMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify landrun task: failed to parse 3rd Marker No '{ThirdMarkerNumber}' as integer", tbThirdMarkerNumber.Text);
                isDataValid = false;
            }
            if (thirdMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify landrun task: 3rd Marker No. must be greater than 0");
                isDataValid = false;
            }

            if (isDataValid)
            {
                LandRun ??= new LandRunTask();
                List<IMarkerValidationRules> markerValidationRules = [];
                foreach (object item in lbRules.Items)
                {
                    if (item is IMarkerValidationRules markerValidationRule)
                        markerValidationRules.Add(markerValidationRule);
                }
                LandRun.SetupLandRun(taskNumber, firstMarkerNumber, secondMarkerNumber, thirdMarkerNumber, markerValidationRules);
                tbTaskNumber.Text = "";
                tbFirstMarkerNumber.Text = "";
                tbSecondMarkerNumber.Text = "";
                tbThirdMarkerNumber.Text = "";
                lbRules.Items.Clear();
                OnDataValid();
            }
        }

        /// <summary>
        /// Called when input for land run task is valid
        /// </summary>
        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
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
                case MarkerTimingRule markerTimingRule:
                    {
                        MarkerTimingRuleControl markerTimingRuleControl = new(markerTimingRule);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerTimingRuleControl.Location = RuleControlLocation;
                        markerTimingRuleControl.TabIndex = 6;
                        markerTimingRuleControl.Name = "ruleControl";
                        markerTimingRuleControl.DataValid += MarkerTimingRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerTimingRuleControl);
                        ResumeLayout();
                    }
                    break;
                case MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule:
                    {
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new(markerToOtherMarkersDistanceRule);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerToOtherMarkerDistanceRuleControl.Location = RuleControlLocation;
                        markerToOtherMarkerDistanceRuleControl.TabIndex = 6;
                        markerToOtherMarkerDistanceRuleControl.Name = "ruleControl";
                        markerToOtherMarkerDistanceRuleControl.DataValid += MarkerToOtherMarkersDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerToOtherMarkerDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case MarkerToGoalDistanceRule markerToGoalDistanceRule:
                    {
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new(markerToGoalDistanceRule);
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerToGoalDistanceRuleControl.Location = RuleControlLocation;
                        markerToGoalDistanceRuleControl.TabIndex = 6;
                        markerToGoalDistanceRuleControl.Name = "ruleControl";
                        markerToGoalDistanceRuleControl.DataValid += MarkerToGoalDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
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
                case "Marker Timing":
                    {
                        MarkerTimingRuleControl markerTimingRuleControl = new();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerTimingRuleControl.Location = RuleControlLocation;
                        markerTimingRuleControl.Name = "ruleControl";
                        markerTimingRuleControl.TabIndex = 6;
                        markerTimingRuleControl.DataValid += MarkerTimingRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerTimingRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Marker to other Markers Distance":
                    {
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerToOtherMarkerDistanceRuleControl.Location = RuleControlLocation;
                        markerToOtherMarkerDistanceRuleControl.Name = "ruleControl";
                        markerToOtherMarkerDistanceRuleControl.TabIndex = 6;
                        markerToOtherMarkerDistanceRuleControl.DataValid += MarkerToOtherMarkersDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerToOtherMarkerDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Marker to Goal Distance":
                    {
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new();
                        SuspendLayout();
                        plRuleControl.Controls.Remove(plRuleControl.Controls["ruleControl"]);
                        markerToGoalDistanceRuleControl.Location = RuleControlLocation;
                        markerToGoalDistanceRuleControl.Name = "ruleControl";
                        markerToGoalDistanceRuleControl.TabIndex = 6;
                        markerToGoalDistanceRuleControl.DataValid += MarkerToGoalDistanceRuleControl_DataValid;
                        plRuleControl.Controls.Add(markerToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds a new marker to goal distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerToGoalDistanceRuleControl_DataValid()
        {
            MarkerToGoalDistanceRule markerToGoalDistanceRule = (plRuleControl.Controls["ruleControl"] as MarkerToGoalDistanceRuleControl).MarkerToGoalDistanceRule;
            if (!lbRules.Items.Contains(markerToGoalDistanceRule))
                lbRules.Items.Add(markerToGoalDistanceRule);
            Logger?.LogInformation("{MarkerToGoalDistanceRule} created/modified", markerToGoalDistanceRule);
        }

        /// <summary>
        /// Adds a new marker to other markers distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerToOtherMarkersDistanceRuleControl_DataValid()
        {
            MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = (plRuleControl.Controls["ruleControl"] as MarkerToOtherMarkersDistanceRuleControl).MarkerToOtherMarkersDistanceRule;
            if (!lbRules.Items.Contains(markerToOtherMarkersDistanceRule))
                lbRules.Items.Add(markerToOtherMarkersDistanceRule);
            Logger?.LogInformation("{MarkerToOtherMarkersDistanceRule} created/modified", markerToOtherMarkersDistanceRule);
        }

        /// <summary>
        /// Adds a new marker timing rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerTimingRuleControl_DataValid()
        {
            MarkerTimingRule markerTimingRule = (plRuleControl.Controls["ruleControl"] as MarkerTimingRuleControl).MarkerTimingRule;
            if (!lbRules.Items.Contains(markerTimingRule))
                lbRules.Items.Add(markerTimingRule);
            Logger?.LogInformation("{MarkerTimingRule} created/modified", markerTimingRule);
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
