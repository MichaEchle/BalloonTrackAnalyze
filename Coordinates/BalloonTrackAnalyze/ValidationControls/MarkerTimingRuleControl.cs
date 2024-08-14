using BalloonTrackAnalyze.TaskControls;
using Competition;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerTimingRuleControl : UserControl
    {
        #region Properties
        private readonly ILogger<MarkerTimingRuleControl> Logger = LogConnector.LoggerFactory.CreateLogger<MarkerTimingRuleControl>();
        /// <summary>
        /// The rule to be created or modified with this control
        /// </summary>
        public MarkerTimingRule MarkerTimingRule
        {
            get; private set;
        }

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
            btCreate.Text = "Create rule";
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
            btCreate.Text = "Create rule";
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
                //TODO adapt to changed definition of markertiming rule
                //tbOpenAtMinute.Text = MarkerTimingRule.OpenAtMinute.ToString();
                //tbCloseAtMinute.Text = MarkerTimingRule.CloseAtMinute.ToString();
            }
        }

        /// <summary>
        /// Validates the user input and creates new / modifies existing marker timing rule
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            if (!int.TryParse(tbOpenAtMinute.Text, out int openAtMinute))
            {
                Logger?.LogError("Failed to create/modify marker timing rule: failed to parse Open at Minute '{openAtMinute}' as integer", tbOpenAtMinute.Text);
                isDataValid = false;
            }
            if (openAtMinute < 0)
            {
                Logger?.LogError("Open at Minute '{openAtMinute}' must be greater than zero", openAtMinute);
                isDataValid = false;
            }
            if (openAtMinute > 59)
            {
                Logger?.LogError("Open at Minute '{openAtMinute}' must be less than 59", openAtMinute);
                isDataValid = false;
            }
            if (!int.TryParse(tbCloseAtMinute.Text, out int closeAtMinute))
            {
                Logger?.LogError("Failed to parse Close at Minute '{tbCloseAtMinute}' as integer", tbCloseAtMinute.Text);
                isDataValid = false;
            }
            if (closeAtMinute < 0)
            {
                Logger?.LogError("Close at Minute '{closeAtMinute}' must be greater than zero", closeAtMinute);
                isDataValid = false;
            }
            if (closeAtMinute > 59)
            {
                Logger?.LogError("Close at Minute '{closeAtMinute}' must be less than 59", closeAtMinute);
                isDataValid = false;
            }

            if (isDataValid)
            {
                MarkerTimingRule ??= new MarkerTimingRule();
                //TODO adapt to changed definition of markertiming rule
                //MarkerTimingRule.SetupRule(openAtMinute, closeAtMinute);
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
        #endregion
    }
}
