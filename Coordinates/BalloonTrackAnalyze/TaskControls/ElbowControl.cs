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
    public partial class ElbowControl : UserControl
    {
        public ElbowTask Elbow { get; private set; }

        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        private Point RuleControlLocation = new Point(314, 168);
        public ElbowControl()
        {
            InitializeComponent();
        }
        public ElbowControl(ElbowTask elbow)
        {
            Elbow = elbow;
            InitializeComponent();
            Prefill();
        }

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

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify elbow task: ";
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
                Elbow ??= new ElbowTask();
                List<IMarkerValidationRules> rules = new List<IMarkerValidationRules>();
                foreach (object item in lbRules.Items)
                {
                    if (item is IMarkerValidationRules)
                        if (!Elbow.MarkerValidationRules.Contains(item as IMarkerValidationRules))
                            rules.Add(item as IMarkerValidationRules);
                }
                Elbow.SetupElbow(taskNumber, firstMarkerNumber, secondMarkerNumber, thirdMarkerNumber, rules);
                tbTaskNumber.Text = "";
                tbFirstMarkerNumber.Text = "";
                tbSecondMarkerNumber.Text = "";
                tbThirdMarkerNumber.Text = "";
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
            return "Elbow Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }

        private void lbRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbRules.SelectedItem)
            {
                case MarkerTimingRule markerTimingRule:
                    {
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl(markerTimingRule);
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerTimingRuleControl.Location = RuleControlLocation;
                        markerTimingRuleControl.Name = "ruleControl";
                        markerTimingRuleControl.DataValid += MarkerTimingRuleControl_DataValid;
                        Controls.Add(markerTimingRuleControl);
                        ResumeLayout();
                    }
                    break;
                case MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule:
                    {
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerToOtherMarkerDistanceRuleControl.Location = RuleControlLocation;
                        markerToOtherMarkerDistanceRuleControl.Name = "ruleControl";
                        markerToOtherMarkerDistanceRuleControl.DataValid += MarkerToOtherMarkersDistanceRuleControl_DataValid;
                        Controls.Add(markerToOtherMarkerDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case MarkerToGoalDistanceRule markerToGoalDistanceRule:
                    {
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl(markerToGoalDistanceRule);
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerToGoalDistanceRuleControl.Location = RuleControlLocation;
                        markerToGoalDistanceRuleControl.Name = "ruleControl";
                        markerToGoalDistanceRuleControl.DataValid += MarkerToGoalDistanceRuleControl_DataValid;
                        Controls.Add(markerToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        private void cbRuleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbRuleList.SelectedItem.ToString())
            {
                case "Marker Timing":
                    {
                        MarkerTimingRuleControl markerTimingRuleControl = new MarkerTimingRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerTimingRuleControl.Location = RuleControlLocation;
                        markerTimingRuleControl.Name = "ruleControl";
                        markerTimingRuleControl.DataValid += MarkerTimingRuleControl_DataValid;
                        Controls.Add(markerTimingRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Marker to other Markers Distance":
                    {
                        MarkerToOtherMarkersDistanceRuleControl markerToOtherMarkerDistanceRuleControl = new MarkerToOtherMarkersDistanceRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerToOtherMarkerDistanceRuleControl.Location = RuleControlLocation;
                        markerToOtherMarkerDistanceRuleControl.Name = "ruleControl";
                        markerToOtherMarkerDistanceRuleControl.DataValid += MarkerToOtherMarkersDistanceRuleControl_DataValid;
                        Controls.Add(markerToOtherMarkerDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
                case "Marker to Goal Distance":
                    {
                        MarkerToGoalDistanceRuleControl markerToGoalDistanceRuleControl = new MarkerToGoalDistanceRuleControl();
                        SuspendLayout();
                        Controls.Remove(Controls["ruleControl"]);
                        markerToGoalDistanceRuleControl.Location = RuleControlLocation;
                        markerToGoalDistanceRuleControl.Name = "ruleControl";
                        markerToGoalDistanceRuleControl.DataValid += MarkerToGoalDistanceRuleControl_DataValid;
                        Controls.Add(markerToGoalDistanceRuleControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        private void MarkerToGoalDistanceRuleControl_DataValid()
        {
            MarkerToGoalDistanceRule markerToGoalDistanceRule = (Controls["ruleControl"] as MarkerToGoalDistanceRuleControl).MarkerToGoalDistanceRule;
            if (!lbRules.Items.Contains(markerToGoalDistanceRule))
                lbRules.Items.Add(markerToGoalDistanceRule);
            Log(LogSeverityType.Info, $"{markerToGoalDistanceRule} created/modified");
        }

        private void MarkerToOtherMarkersDistanceRuleControl_DataValid()
        {
            MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = (Controls["ruleControl"] as MarkerToOtherMarkersDistanceRuleControl).MarkerToOtherMarkersDistanceRule;
            if (!lbRules.Items.Contains(markerToOtherMarkersDistanceRule))
                lbRules.Items.Add(markerToOtherMarkersDistanceRule);
            Log(LogSeverityType.Info, $"{markerToOtherMarkersDistanceRule} created/modified");
        }

        private void MarkerTimingRuleControl_DataValid()
        {
            MarkerTimingRule markerTimingRule = (Controls["ruleControl"] as MarkerTimingRuleControl).MarkerTimingRule;
            if (!lbRules.Items.Contains(markerTimingRule))
                lbRules.Items.Add(markerTimingRule);
            Log(LogSeverityType.Info, $"{markerTimingRule} created/modified");
        }

        private void btRemoveRule_Click(object sender, EventArgs e)
        {
            if (lbRules.SelectedItem != null)
                lbRules.Items.Remove(lbRules.SelectedItem);
        }
    }
}
