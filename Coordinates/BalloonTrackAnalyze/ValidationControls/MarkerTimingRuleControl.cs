using Competition;
using LoggerComponent;
using System;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerTimingRuleControl : UserControl
    {
        #region Properties
        /// <summary>
        /// The rule to be created or modified with this control
        /// </summary>
        public MarkerTimingRule MarkerTimingRule { get; private set; }

        /// <summary>
        /// Delegate for the DataValid event
        /// </summary>
        public delegate void DataValidDelegate();

        /// <summary>
        /// Event will be fired when the input for the rule is value
        /// </summary>
        public event DataValidDelegate DataValid;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MarkerTimingRuleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor which pre-fills control from existing rule
        /// </summary>
        /// <param name="markerTimingRule">the existing marker timing rule</param>
        public MarkerTimingRuleControl(MarkerTimingRule markerTimingRule)
        {
            MarkerTimingRule = markerTimingRule;
            InitializeComponent();
            Prefill();
        }
        #endregion

        #region API
        /// <summary>
        /// Convert the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Marker Timing Rule Setup Control";
        }

        #endregion

        #region private methods
        /// <summary>
        /// Pre-fills control form existing rule
        /// </summary>
        private void Prefill()
        {
            if (MarkerTimingRule != null)
            {
                tbOpenAtMinute.Text = MarkerTimingRule.OpenAtMinute.ToString();
                tbCloseAtMinute.Text = MarkerTimingRule.CloseAtMinute.ToString();
            }
        }

        /// <summary>
        /// Validates the user input and creates new / modifies exsiting marker timing rule
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
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

        /// <summary>
        /// Called when input for rule is valid
        /// </summary>
        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        /// <summary>
        /// Logs a user message
        /// </summary>
        /// <param name="logSeverity">the severity of the message</param>
        /// <param name="text">the message text</param>
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
        #endregion
    }
}
