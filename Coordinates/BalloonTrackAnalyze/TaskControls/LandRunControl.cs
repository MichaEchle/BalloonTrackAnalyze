using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Competition;
using LoggerComponent;
using BalloonTrackAnalyze.ValidationControls;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class LandRunControl : UserControl
    {
        #region Properties

        /// <summary>
        /// The land run task to be created or modified by this control
        /// </summary>
        public LandRunTask LandRun { get; private set; }

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
        public LandRunControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor which pre-fills controls from existing land run task
        /// </summary>
        /// <param name="landRun">the existing land run task</param>
        public LandRunControl(LandRunTask landRun)
        {
            LandRun = landRun;
            InitializeComponent();
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
            string functionErrorMessage = "Failed to create/modify landrun task: ";
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
            int firstMarkerNumber;
            if (!int.TryParse(tbFirstMarkerNumber.Text, out firstMarkerNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed ot parse 1st Marker No '{tbFirstMarkerNumber.Text}' as integer");
                isDataValid = false;
            }
            if (firstMarkerNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"1st Marker No. must be greater than 0");
                isDataValid = false;
            }

            int secondMarkerNumber;
            if (!int.TryParse(tbSecondMarkerNumber.Text, out secondMarkerNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed ot parse 2nd Marker No '{tbSecondMarkerNumber.Text}' as integer");
                isDataValid = false;
            }
            if (secondMarkerNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"2nd Marker No. must be greater than 0");
                isDataValid = false;
            }

            int thirdMarkerNumber;
            if (!int.TryParse(tbThirdMarkerNumber.Text, out thirdMarkerNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed ot parse 3rd Marker No '{tbThirdMarkerNumber.Text}' as integer");
                isDataValid = false;
            }
            if (thirdMarkerNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"3rd Marker No. must be greater than 0");
                isDataValid = false;
            }

            if (isDataValid)
            {
                LandRun ??= new LandRunTask();
                List<IMarkerValidationRules> rules = new List<IMarkerValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IMarkerValidationRules)
                        if (!LandRun.MarkerValidationRules.Contains(item as IMarkerValidationRules))
                            rules.Add(item as IMarkerValidationRules);
                }
                LandRun.SetupLandRun(taskNumber, firstMarkerNumber, secondMarkerNumber, thirdMarkerNumber, rules);
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
        /// Logs a user messages
        /// </summary>
        /// <param name="logSeverity">the severity of the message</param>
        /// <param name="text">the message text</param>
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
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
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl(markerTimingRule);
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
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl();
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
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl(markerToGoalDistanceRule);
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
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl();
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
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl();
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
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl();
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
            Log(LogSeverityType.Info, $"{markerToGoalDistanceRule} created/modified");
        }

        /// <summary>
        /// Adds a new marker to other markers distance rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerToOtherMarkersDistanceRuleControl_DataValid()
        {
            MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = (plRuleControl.Controls["ruleControl"] as MarkerToOtherMarkersDistanceRuleControl).MarkerToOtherMarkersDistanceRule;
            if (!lbRules.Items.Contains(markerToOtherMarkersDistanceRule))
                lbRules.Items.Add(markerToOtherMarkersDistanceRule);
            Log(LogSeverityType.Info, $"{markerToOtherMarkersDistanceRule} created/modified");
        }

        /// <summary>
        /// Adds a new marker timing rule to the rule list box, when the rule user controls fires the DataVaild event
        /// </summary>
        private void MarkerTimingRuleControl_DataValid()
        {
            MarkerTimingRule markerTimingRule = (plRuleControl.Controls["ruleControl"] as MarkerTimingRuleControl).MarkerTimingRule;
            if (!lbRules.Items.Contains(markerTimingRule))
                lbRules.Items.Add(markerTimingRule);
            Log(LogSeverityType.Info, $"{markerTimingRule} created/modified");
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
