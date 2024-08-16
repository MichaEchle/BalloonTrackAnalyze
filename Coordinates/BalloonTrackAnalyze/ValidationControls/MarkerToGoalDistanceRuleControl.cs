using BalloonTrackAnalyze.TaskControls;
using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace BalloonTrackAnalyze.ValidationControls;

public partial class MarkerToGoalDistanceRuleControl : UserControl
{
    #region Properties
    private readonly ILogger<MarkerToGoalDistanceRuleControl> Logger = LogConnector.LoggerFactory.CreateLogger<MarkerToGoalDistanceRuleControl>();
    /// <summary>
    /// The rule to be created or modified with this control
    /// </summary>
    public MarkerToGoalDistanceRule MarkerToGoalDistanceRule
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
    public MarkerToGoalDistanceRuleControl()
    {
        InitializeComponent();
        btCreate.Text = "Create rule";
    }

    /// <summary>
    /// Constructor which pre-fills control from existing rule
    /// </summary>
    /// <param name="markerToGoalDistanceRule">the existing marker to goal distance rule</param>
    public MarkerToGoalDistanceRuleControl(MarkerToGoalDistanceRule markerToGoalDistanceRule)
    {
        MarkerToGoalDistanceRule = markerToGoalDistanceRule;
        InitializeComponent();
        Prefill();
        btCreate.Text = "Modify rule";
    }
    #endregion

    #region API
    /// <summary>
    /// Convert the object suitable for display representation
    /// </summary>
    /// <returns>display text of the this object</returns>
    public override string ToString()
    {
        return "Marker to Goal Distance Rule Setup Control";
    }

    #endregion

    #region private methods
    /// <summary>
    /// Pre-fills control form existing rule
    /// </summary>
    private void Prefill()
    {
        if (MarkerToGoalDistanceRule != null)
        {
            if (!double.IsNaN(MarkerToGoalDistanceRule.MinimumDistance))
            {
                tbMinimumDistance.Text = Math.Round(MarkerToGoalDistanceRule.MinimumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                rbMinimumDistanceMeter.Checked = true;
            }
            if (!double.IsNaN(MarkerToGoalDistanceRule.MaximumDistance))
            {
                tbMaximumDistance.Text = Math.Round(MarkerToGoalDistanceRule.MaximumDistance, 3, MidpointRounding.AwayFromZero).ToString();
                rbMinimumDistanceMeter.Checked = true;
            }
            cbUse3DDistance.Checked = MarkerToGoalDistanceRule.Use3DDistance;
            cbUseGPSAltitude.Checked = MarkerToGoalDistanceRule.UseGPSAltitude;
            tbGoalNumber.Text = MarkerToGoalDistanceRule.GoalNumber.ToString();
        }
    }

    /// <summary>
    /// Validates the user input and creates new / modifies exsiting marker to goal distance rule
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
                Logger?.LogError("Failed to create/modify marker to goal distance rule: failed to parse Min. Distance '{tbMinimumDistance.Text}' as double",tbMinimumDistance.Text);
                isDataValid = false;
            }
            if (minimumDistance < 0)
            {
                Logger?.LogError("Failed to create/modify marker to goal distance rule: Min. Distance '{minimumDistance}' must be greater than zero", minimumDistance);
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
                Logger?.LogError("Failed to create/modify marker to goal distance rule: Failed to parse Max. Distance '{tbMinimumDistance.Text}' as double", tbMaximumDistance.Text);
                isDataValid = false;
            }
            if (maximumDistance < 0)
            {
                Logger?.LogError("Failed to create/modify marker to goal distance rule: Max. Distance '{maximumDistance}' must be greater than zero", maximumDistance);
                isDataValid = false;
            }

            if (rbMaximumDistanceFeet.Checked)
                maximumDistance = CoordinateHelpers.ConvertToMeter(maximumDistance);

        }
        if (!double.IsNaN(minimumDistance) && !double.IsNaN(maximumDistance))
        {
            if (minimumDistance >= maximumDistance)
            {
                Logger?.LogError("Failed to create/modify marker to goal distance rule: Min. Distance '{minimumDistance}[m]' must be smaller than Max. Distance '{maximumDistance}[m]'", minimumDistance, maximumDistance);
                isDataValid = false;
            }
        }
        if (!int.TryParse(tbGoalNumber.Text, out int goalNumber))
        {
            Logger?.LogError("Failed to create/modify marker to goal distance rule: failed to parse Goal No. '{tbGoalNumber.Text}' as integer", tbGoalNumber.Text);
            isDataValid = false;
        }
        if (goalNumber <= 0)
        {
            Logger?.LogError("Failed to create/modify marker to goal distance rule: Goal No. '{goalNumber}' must be greater than zero", goalNumber);
            isDataValid = false;
        }
        if (isDataValid)
        {
            MarkerToGoalDistanceRule ??= new MarkerToGoalDistanceRule();
            MarkerToGoalDistanceRule.SetupRule(minimumDistance, maximumDistance, cbUse3DDistance.Checked, cbUseGPSAltitude.Checked, goalNumber);
            tbMinimumDistance.Text = "";
            tbMaximumDistance.Text = "";
            tbGoalNumber.Text = "";
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
