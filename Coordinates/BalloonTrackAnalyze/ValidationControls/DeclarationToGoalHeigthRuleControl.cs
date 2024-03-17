using Competition;
using Coordinates;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class DeclarationToGoalHeightRuleControl : UserControl
    {
        #region Properties

        private readonly ILogger<DeclarationToGoalHeightRuleControl> Logger;

        /// <summary>
        /// The rule to be created or modified with this control
        /// </summary>
        public DeclarationToGoalHeightRule DeclarationToGoalHeightRule
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
        public DeclarationToGoalHeightRuleControl(ILogger<DeclarationToGoalHeightRuleControl> logger)
        {
            InitializeComponent();
            btCreate.Text = "Create rule";
            Logger = logger;
        }

        /// <summary>
        /// Constructor which pre-fills control from existing rule
        /// </summary>
        /// <param name="declarationToGoalHeightRule">the existing declaration to goal height rule</param>
        public DeclarationToGoalHeightRuleControl(DeclarationToGoalHeightRule declarationToGoalHeightRule, ILogger<DeclarationToGoalHeightRuleControl> logger)
        {
            DeclarationToGoalHeightRule = declarationToGoalHeightRule;
            InitializeComponent();
            Prefill();
            btCreate.Text = "Modify rule";
            Logger = logger;
        }
        #endregion

        #region API
        /// <summary>
        /// Convert the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Declaration to Goal Height Rule Setup Control";
        }

        #endregion

        #region private methods
        /// <summary>
        /// Pre-fills control form existing rule
        /// </summary>
        private void Prefill()
        {
            if (DeclarationToGoalHeightRule != null)
            {
                if (!double.IsNaN(DeclarationToGoalHeightRule.MinimumHeightDifference))
                {
                    tbMinimumHeightDifference.Text = Math.Round(DeclarationToGoalHeightRule.MinimumHeightDifference, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumHeightDifferenceMeter.Checked = true;
                }
                if (!double.IsNaN(DeclarationToGoalHeightRule.MaximumHeightDifference))
                {
                    tbMaximumHeightDifference.Text = Math.Round(DeclarationToGoalHeightRule.MaximumHeightDifference, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMaximumHeightDifferenceMeter.Checked = true;
                }
                rbGPS.Checked = DeclarationToGoalHeightRule.UseGPSAltitude;
            }
        }

        /// <summary>
        /// Validates the user input and creates new / modifies exsiting declaration to goal height rule 
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            double minimumHeightDifference = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumHeightDifference.Text))
            {
                if (!double.TryParse(tbMinimumHeightDifference.Text, out minimumHeightDifference))
                {
                    Logger?.LogError("Failed to create/modify declaration to goal height rule: failed to parse Min. Height Diff. '{minimumHeightDifference}' as double", tbMinimumHeightDifference.Text);
                    isDataValid = false;
                }
                if (minimumHeightDifference < 0)
                {
                    Logger?.LogError("Failed to create/modify declaration to goal height rule: Min. Height Diff. must be greater than zero");
                    isDataValid = false;
                }

                if (rbMinimumHeightDifferenceFeet.Checked)
                    minimumHeightDifference = CoordinateHelpers.ConvertToMeter(minimumHeightDifference);

            }
            double maximumHeightDifference = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMaximumHeightDifference.Text))
            {
                if (!double.TryParse(tbMaximumHeightDifference.Text, out maximumHeightDifference))
                {
                    Logger?.LogError("Failed to create/modify declaration to goal height rule: failed to parse Max. Height Diff. '{tbMaximumHeightDifference.Text}' as double", tbMaximumHeightDifference.Text);
                    isDataValid = false;
                }
                if (maximumHeightDifference < 0)
                {
                    Logger?.LogError("Failed to create/modify declaration to goal height rule: Max. Height Diff. must be greater than zero");
                    isDataValid = false;
                }

                if (rbMaximumHeightDifferenceFeet.Checked)
                    maximumHeightDifference = CoordinateHelpers.ConvertToMeter(maximumHeightDifference);

            }
            if (!double.IsNaN(minimumHeightDifference) && !double.IsNaN(maximumHeightDifference))
            {
                if (minimumHeightDifference >= maximumHeightDifference)
                {
                    Logger?.LogError("Failed to create/modify declaration to goal height rule: Min. Height Diff. '{minimumHeightDifference}[m]' must be smaller than Max. Height Diff. '{maximumHeightDifference}[m]'", minimumHeightDifference, maximumHeightDifference);
                    isDataValid = false;
                }
            }
            bool useGPSAltitude = rbGPS.Checked;

            if (isDataValid)
            {
                DeclarationToGoalHeightRule ??= new DeclarationToGoalHeightRule();
                DeclarationToGoalHeightRule.SetupRule(minimumHeightDifference, maximumHeightDifference, DeclarationToGoalHeightRule.HeightDifferenceType.AbsoluteDifference, useGPSAltitude);
                tbMinimumHeightDifference.Text = "";
                tbMaximumHeightDifference.Text = "";
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
