using BalloonTrackAnalyze.ValidationControls;
using Competition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class ElbowControl : UserControl
    {

        #region Properties

        private readonly ILogger<ElbowControl> Logger;

        private readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// The elbow task to be created or modified by this control
        /// </summary>
        public ElbowTask Elbow
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
        private Point RuleControlLocation = new Point(0, 0);
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ElbowControl(ILogger<ElbowControl> logger, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            btCreate.Text = "Create task";
            IsNewTask = true;
            Logger = logger;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Constructor which pre-fills controls from existing elbow task
        /// </summary>
        /// <param name="elbow">the existing elbow task</param>
        public ElbowControl(ElbowTask elbow, ILogger<ElbowControl> logger, IServiceProvider serviceProvider)
        {
            Elbow = elbow;
            InitializeComponent();
            btCreate.Text = "Modify task";
            IsNewTask = false;
            Prefill();
            Logger = logger;
            ServiceProvider = serviceProvider;
            
        }
        #endregion

        #region API
        /// <summary>
        /// Converts the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Elbow Setup Control";
        }

        #endregion

        #region private methods

        /// <summary>
        /// Pre-fill controls from existing task
        /// </summary>
        private void Prefill()
        {
            if (Elbow != null)
            {
                tbTaskNumber.Text = Elbow.TaskNumber.ToString();
                tbFirstMarkerNumber.Text = Elbow.FirstMarkerNumber.ToString();
                tbSecondMarkerNumber.Text = Elbow.SecondMarkerNumber.ToString();
                tbThirdMarkerNumber.Text = Elbow.ThirdMarkerNumber.ToString();
                foreach (IMarkerValidationRules rule in Elbow.MarkerValidationRules)
                {
                    lbRules.Items.Add(rule);
                }
            }
        }
        /// <summary>
        /// Validates user input and creates a new elbow task / modifies the existing elbow task
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify elbow task: ";
            int taskNumber;
            if (!int.TryParse(tbTaskNumber.Text, out taskNumber))
            {
                Logger?.LogError("Failed to create/modify elbow task: failed to parse Task No. '{taskNumber}' as integer", tbTaskNumber.Text);
                isDataValid = false;
            }
            if (taskNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify elbow task: Task No. must be greater than 0");
                isDataValid = false;
            }
            int firstMarkerNumber;
            if (!int.TryParse(tbFirstMarkerNumber.Text, out firstMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify elbow task: failed to parse 1st Marker No '{firstMarkerNumber}' as integer", tbFirstMarkerNumber.Text);
                isDataValid = false;
            }
            if (firstMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify elbow task: 1st Marker No. must be greater than 0");
                isDataValid = false;
            }

            int secondMarkerNumber;
            if (!int.TryParse(tbSecondMarkerNumber.Text, out secondMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify elbow task: failed to parse 2nd Marker No '{secondMarkerNumber}' as integer", tbSecondMarkerNumber.Text);
                isDataValid = false;
            }
            if (secondMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify elbow task: 2nd Marker No. must be greater than 0");
                isDataValid = false;
            }

            int thirdMarkerNumber;
            if (!int.TryParse(tbThirdMarkerNumber.Text, out thirdMarkerNumber))
            {
                Logger?.LogError("Failed to create/modify elbow task: failed to parse 3rd Marker No '{thirdMarkerNumber}' as integer", tbThirdMarkerNumber.Text);
                isDataValid = false;
            }
            if (thirdMarkerNumber <= 0)
            {
                Logger?.LogError("Failed to create/modify elbow task: 3rd Marker No. must be greater than 0");
                isDataValid = false;
            }

            if (isDataValid)
            {
                Elbow ??= new ElbowTask();
                List<IMarkerValidationRules> markerValidaitonRules = new List<IMarkerValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IMarkerValidationRules markerValidationRule)
                        markerValidaitonRules.Add(markerValidationRule);
                }
                Elbow.SetupElbow(taskNumber, firstMarkerNumber, secondMarkerNumber, thirdMarkerNumber, markerValidaitonRules);
                tbTaskNumber.Text = "";
                tbFirstMarkerNumber.Text = "";
                tbSecondMarkerNumber.Text = "";
                tbThirdMarkerNumber.Text = "";
                lbRules.Items.Clear();
                OnDataValid();
            }
        }

        /// <summary>
        /// Called when input for elbow task is valid
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
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl(markerTimingRule,ServiceProvider.GetRequiredService<ILogger<MarkerTimingRuleControl>>());
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
                case MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule:
                    {
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl(markerToOtherMarkersDistanceRule,ServiceProvider.GetRequiredService<ILogger<MarkerToOtherMarkersDistanceRuleControl>>());
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
                case MarkerToGoalDistanceRule markerToGoalDistanceRule:
                    {
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl(markerToGoalDistanceRule, ServiceProvider.GetRequiredService<ILogger<MarkerToGoalDistanceRuleControl>>());
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
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl(ServiceProvider.GetRequiredService<ILogger<MarkerTimingRuleControl>>());
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
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl( ServiceProvider.GetRequiredService<ILogger<MarkerToOtherMarkersDistanceRuleControl>>());
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
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl( ServiceProvider.GetRequiredService<ILogger<MarkerToGoalDistanceRuleControl>>());
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
            Logger?.LogInformation("'{markerToGoalDistanceRule}' created/modified",markerToGoalDistanceRule);
        }

        /// <summary>
        /// Adds a new marker to other markers distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerToOtherMarkersDistanceRuleControl_DataValid()
        {
            MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = (plRuleControl.Controls["ruleControl"] as MarkerToOtherMarkersDistanceRuleControl).MarkerToOtherMarkersDistanceRule;
            if (!lbRules.Items.Contains(markerToOtherMarkersDistanceRule))
                lbRules.Items.Add(markerToOtherMarkersDistanceRule);
            Logger?.LogInformation("'{markerToOtherMarkersDistanceRule}' created/modified", markerToOtherMarkersDistanceRule);
        }

        /// <summary>
        /// Adds a new marker timing rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerTimingRuleControl_DataValid()
        {
            MarkerTimingRule markerTimingRule = (plRuleControl.Controls["ruleControl"] as MarkerTimingRuleControl).MarkerTimingRule;
            if (!lbRules.Items.Contains(markerTimingRule))
                lbRules.Items.Add(markerTimingRule);
            Logger?.LogInformation("'{markerTimingRule}' created/modified", markerTimingRule);
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
