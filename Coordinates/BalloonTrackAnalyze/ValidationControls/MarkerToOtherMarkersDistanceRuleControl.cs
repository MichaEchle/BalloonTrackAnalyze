using Competition;
using Coordinates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerToOtherMarkersDistanceRuleControl : UserControl
    {
        #region Properties

        private readonly ILogger<MarkerToOtherMarkersDistanceRuleControl> Logger;
        /// <summary>
        /// The rule to be created or modified with this control
        /// </summary>
        public MarkerToOtherMarkersDistanceRule MarkerToOtherMarkersDistanceRule
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
        public MarkerToOtherMarkersDistanceRuleControl(ILogger<MarkerToOtherMarkersDistanceRuleControl> logger)
        {
            InitializeComponent();
            btCreate.Text = "Create rule";
            Logger = logger;
        }

        /// <summary>
        /// Constructor which pre-fills control from existing rule
        /// </summary>
        /// <param name="markerToOtherMarkersDistanceRule">the existing marker to other markers distance rule</param>
        public MarkerToOtherMarkersDistanceRuleControl(MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule, ILogger<MarkerToOtherMarkersDistanceRuleControl> logger)
        {
            MarkerToOtherMarkersDistanceRule = markerToOtherMarkersDistanceRule;
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
            return "Marker to other Markers Distance Rule Setup Control";
        }

        #endregion

        #region private methods
        /// <summary>
        /// Pre-fills control form existing rule
        /// </summary>
        public void Prefill()
        {
            if (MarkerToOtherMarkersDistanceRule != null)
            {
                if (!double.IsNaN(MarkerToOtherMarkersDistanceRule.MinimumDistance))
                {
                    tbMinimumDistance.Text = Math.Round(MarkerToOtherMarkersDistanceRule.MinimumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMinimumDistanceMeter.Checked = true;
                }
                if (!double.IsNaN(MarkerToOtherMarkersDistanceRule.MaximumDistance))
                {
                    tbMaximumDistance.Text = Math.Round(MarkerToOtherMarkersDistanceRule.MaximumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                    rbMaximumDistanceMeter.Checked = true;
                }
                tbMarkerNumbers.Text = string.Join(',', MarkerToOtherMarkersDistanceRule.MarkerNumbers);
            }
        }

        /// <summary>
        /// Validates the user input and creates new / modifies exsiting marker to other markers distance rule
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            double minimumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumDistance.Text))
            {
                if (!double.TryParse(tbMinimumDistance.Text, out minimumDistance))
                {
                    Logger?.LogError("Failed to create/modify marker to other markers distance rule: failed to parse Min. Distance '{tbMinimumDistance.Text}' as double", tbMinimumDistance.Text);
                    isDataValid = false;
                }
                if (minimumDistance < 0)
                {
                    Logger?.LogError("Failed to create/modify marker to other markers distance rule: Min. Distance '{minimumDistance}' must be greater than zero", minimumDistance);
                    isDataValid = false;
                }

                if (rbMinimumDistanceFeet.Checked)
                    minimumDistance = CoordinateHelpers.ConvertToMeter(minimumDistance);

            }
            double maximumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMaximumDistance.Text))
            {
                if (!double.TryParse(tbMaximumDistance.Text, out maximumDistance))
                {
                    Logger?.LogError("Failed to create/modify marker to other markers distance rule: Failed to parse Max. Distance '{tbMinimumDistance.Text}' as double", tbMaximumDistance.Text);
                    isDataValid = false;
                }
                if (maximumDistance < 0)
                {
                    Logger?.LogError("Failed to create/modify marker to other markers distance rule: Max. Distance '{maximumDistance}' must be greater than zero", maximumDistance);
                    isDataValid = false;
                }

                if (rbMaximumDistanceFeet.Checked)
                    maximumDistance = CoordinateHelpers.ConvertToMeter(maximumDistance);

            }
            if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
            {
                if (minimumDistance >= maximumDistance)
                {
                    Logger?.LogError("Failed to create/modify marker to other markers distance rule: Min. Distance '{minimumDistance}[m]' must be smaller than Max. Distance '{maximumDistance}[m]'", minimumDistance, maximumDistance);
                    isDataValid = false;
                }
            }
            List<int> markerNumbers = new List<int>();
            if (!string.IsNullOrWhiteSpace(tbMarkerNumbers.Text))
                if (tbMarkerNumbers.Text.ToLowerInvariant() != "all")
                    markerNumbers = Array.ConvertAll(tbMarkerNumbers.Text.Split(','), int.Parse).ToList();


            if (isDataValid)
            {
                MarkerToOtherMarkersDistanceRule ??= new MarkerToOtherMarkersDistanceRule();
                MarkerToOtherMarkersDistanceRule.SetupRule(minimumDistance, maximumDistance, markerNumbers);
                tbMinimumDistance.Text = "";
                tbMaximumDistance.Text = "";
                tbMarkerNumbers.Text = "";
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
