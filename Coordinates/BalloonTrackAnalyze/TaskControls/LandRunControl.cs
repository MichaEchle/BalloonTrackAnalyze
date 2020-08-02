using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Competition;
using LoggerComponent;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class LandRunControl : UserControl
    {
        public LandRunControl()
        {
            InitializeComponent();
        }

        public LandRunTask LandRun { get; private set; }

        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;
        public LandRunControl(LandRunTask landRun)
        {
            LandRun = landRun;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (LandRun != null)
            {
                tbTaskNumber.Text = LandRun.TaskNumber.ToString();
                tbFirstMarkerNumber.Text = LandRun.FirstMarkerNumber.ToString();
                tbSecondMarkerNumber.Text = LandRun.SecondMarkerNumber.ToString();
                tbThirdMarkerNumber.Text = LandRun.ThirdMarkerNumber.ToString();
            }
        }

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
                LandRun = new LandRunTask();
                LandRun.SetupLandRun(taskNumber, firstMarkerNumber, secondMarkerNumber, thirdMarkerNumber, null);
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Landrun Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
