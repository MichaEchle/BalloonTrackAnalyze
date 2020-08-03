using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Coordinates;
using LoggerComponent;
using Competition;
using System.Linq;

namespace BalloonTrackAnalyze.ValidationControls
{
    public partial class MarkerToOtherMarkerDistanceRuleControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public MarkerToOtherMarkersDistanceRule MarkerToOtherMarkersDistanceRule { get; private set; }
        public MarkerToOtherMarkerDistanceRuleControl()
        {
            InitializeComponent();
        }

        public MarkerToOtherMarkerDistanceRuleControl(MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule)
        {
            MarkerToOtherMarkersDistanceRule = markerToOtherMarkersDistanceRule;
            InitializeComponent();
            Prefill();
        }

        public void Prefill()
        {
            if (MarkerToOtherMarkersDistanceRule != null)
            {
                tbMinimumDistance.Text = MarkerToOtherMarkersDistanceRule.MinimumDistance.ToString();
                rbMinimumDistanceMeter.Checked = true;
                tbMaximumDistance.Text = MarkerToOtherMarkersDistanceRule.MaximumDistance.ToString();
                rbMaximumDistanceMeter.Checked = true;
                tbMarkerNumbers.Text = string.Join(',', MarkerToOtherMarkersDistanceRule.MarkerNumbers);
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = false;
            string functionErrorMessage = "Failed to create/modify marker to other markers distance rule: ";
            double minimumDistance = double.NaN;
            if (!string.IsNullOrWhiteSpace(tbMinimumDistance.Text))
            {
                if (!double.TryParse(tbMinimumDistance.Text, out minimumDistance))
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Min. Distance '{tbMinimumDistance.Text}' as double");
                    isDataValid = false;
                }
                if (minimumDistance < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Distance '{minimumDistance}' must be greater than zero");
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
                    Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Max. Distance '{tbMinimumDistance.Text}' as double");
                    isDataValid = false;
                }
                if (maximumDistance < 0)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Max. Distance '{maximumDistance}' must be greater than zero");
                    isDataValid = false;
                }

                if (rbMaximumDistanceFeet.Checked)
                    maximumDistance = CoordinateHelpers.ConvertToMeter(maximumDistance);

            }
            if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
            {
                if (minimumDistance >= maximumDistance)
                {
                    Log(LogSeverityType.Error, functionErrorMessage + $"Min. Distance '{minimumDistance}[m]' must be smaller than Max. Distance '{maximumDistance}[m]'");
                    isDataValid = false;
                }
            }
            List<int> markerNumbers = new List<int>();
            if (tbMarkerNumbers.Text.ToLowerInvariant() != "all")
            {
                markerNumbers = Array.ConvertAll(tbMarkerNumbers.Text.Split(','), int.Parse).ToList();
            }

            if (isDataValid)
            {
                MarkerToOtherMarkersDistanceRule ??= new MarkerToOtherMarkersDistanceRule();
                MarkerToOtherMarkersDistanceRule.SetupRule(minimumDistance, maximumDistance, markerNumbers);
                OnDataValid();
            }
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Marker to other Markers Distance Rule Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }
    }
}
