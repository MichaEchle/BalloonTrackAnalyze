using Competition;
using LoggerComponent;
using System;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerTimingRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public MarkerTimingRule MarkerTimingRule { get; private set; }

        public MarkerTimingRuleControl()
        {
            InitializeComponent();
        }


        public MarkerTimingRuleControl(MarkerTimingRule markerTimingRule)
        {
            MarkerTimingRule = markerTimingRule;
            InitializeComponent();
            Prefill();
        }

        private void Prefill()
        {
            if (MarkerTimingRule != null)
            {
                tbOpenAtMinute.Text = MarkerTimingRule.OpenAtMinute.ToString();
                tbCloseAtMinute.Text = MarkerTimingRule.CloseAtMinute.ToString();
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify marker timing rule: ";
            int openAtMinute;
            if (!int.TryParse(tbOpenAtMinute.Text, out openAtMinute))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Open at Minute '{tbOpenAtMinute.Text}' as integer");
                isDataValid = false;
            }
            if (openAtMinute < 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Open at Minute '{openAtMinute}' must be greater than zero");
                isDataValid = false;
            }
            if (openAtMinute > 59)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Open at Minute '{openAtMinute}' must be less than 59");
                isDataValid = false;
            }
            int closeAtMinute;
            if (!int.TryParse(tbCloseAtMinute.Text, out closeAtMinute))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Close at Minute '{tbCloseAtMinute.Text}' as integer");
                isDataValid = false;
            }
            if (closeAtMinute < 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Close at Minute '{closeAtMinute}' must be greater than zero");
                isDataValid = false;
            }
            if (closeAtMinute > 59)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Close at Minute '{closeAtMinute}' must be less than 59");
                isDataValid = false;
            }

            if (isDataValid)
            {
                MarkerTimingRule ??= new MarkerTimingRule();
                MarkerTimingRule.SetupRule(openAtMinute, closeAtMinute);
                tbOpenAtMinute.Text = "";
                tbCloseAtMinute.Text = "";
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Marker Timing Rule Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
