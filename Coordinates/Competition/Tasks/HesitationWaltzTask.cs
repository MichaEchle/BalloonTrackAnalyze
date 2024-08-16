using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition;

public class HesitationWaltzTask : ICompetitionTask
{
    #region Properties
    [JsonIgnore()]
    private readonly ILogger<HesitationWaltzTask> Logger = LogConnector.LoggerFactory.CreateLogger<HesitationWaltzTask>();

    public int TaskNumber
    {
        get;
        set;
    } = -1;


    /// <summary>
    /// A list of predefined goals that the pilot has to reach
    /// </summary>
    public List<Coordinate> Goals
    {
        get; set;
    } = [];

    /// <summary>
    /// The marker number that the pilot has to drop
    /// </summary>
    public int MarkerNumber
    {
        get; set;
    } = -1;

    /// <summary>
    /// The separation altitude used (in meter) in case <see cref="DistanceCalculation"/> is set to <see cref="DistanceCalculationType.WithSeparationAlitude"/>.
    /// Otherwise this value is ignored
    /// </summary>
    public double SeparationAltitude
    {
        get; set;
    } = double.NaN;

    public DistanceCalculationType DistanceCalculation
    {
        get; set;
    } = DistanceCalculationType.WithSeparationAlitude;


    /// <summary>
    /// Allows to define a function that define the goals for this task using the track of the pilot
    /// <example>
    /// <para>here is an example</para>
    /// <code>
    /// public List&lt;Coordinate&gt; CalculateGoals2(Track track)
    /// {
    ///     return track.Declarations
    ///            .Where(x =&gt; x.GoalNumber &gt;= 5 &amp;&amp; x.GoalNumber &lt;= 8) //get all goals 5-8
    ///            .GroupBy(x =&gt; x.GoalNumber) //group by goal number
    ///            .Select(x =&gt; x.MaxBy(y =&gt; y.PositionAtDeclaration.TimeStamp)) //get the latest declaration for each goal
    ///            .Select(x =&gt; x.DeclaredGoal).ToList(); //get the declared goal
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public Func<Track, List<Coordinate>> DefineGoals
    {
        get; set;
    }

    /// <summary>
    /// The rule that defines if a marker is valid
    /// <para>optional. use null to omit</para>
    /// <para>use <see cref="MarkerAndRule"/> or <see cref="MarkerOrRule"/> to chain multiple rules together</para>
    /// </summary>
    public IMarkerValidationRule MarkerValidationRule
    {
        get; set;
    } = null;

    /// <summary>
    /// The strictness of the validation
    /// </summary>
    public ValidationStrictnessType ValidationStrictness
    {
        get; set;
    } = ValidationStrictnessType.FirstValid;




    #endregion

    #region API
    public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
    {
        result = double.NaN;
        MarkerDrop markerDrop = ValidationHelper.GetValidMarker(track, MarkerNumber, MarkerValidationRule, ValidationStrictness);
        if (markerDrop is null)
        {
            Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': Marker '{markerNumber}' is invalid or doesn't exists", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""), MarkerNumber);
            return false;
        }

        List<double> distances = [];
        result = 0.0;

        if (Goals.Count == 0 && DefineGoals != null)
        {
            try
            {
                Goals = DefineGoals(track);
                if (Goals.Count == 0)
                {
                    Logger?.LogError("Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}': No goals could be calculated", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}'", ToString(), track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                return false;
            }
        }

        foreach (Coordinate goal in Goals)
        {

            switch (DistanceCalculation)
            {
                case DistanceCalculationType.TwoDimensional:
                    distances.Add(CoordinateHelpers.Calculate2DDistanceHavercos(goal, markerDrop.MarkerLocation));
                    break;
                case DistanceCalculationType.ThreeDimensional:
                    distances.Add(CoordinateHelpers.Calculate3DDistance(goal, markerDrop.MarkerLocation, useGPSAltitude));
                    break;
                case DistanceCalculationType.WithSeparationAlitude:
                    distances.Add(CoordinateHelpers.CalculateDistanceWithSeparationAltitude(goal, markerDrop.MarkerLocation, SeparationAltitude, useGPSAltitude));
                    break;
                default:
                    break;
            }
        }
        result = distances.Min();

        return true;
    }

    /// <summary>
    /// Sets up the task with a list of predefined goals
    /// </summary>
    /// <param name="taskNumber">the number of the task</param>
    /// <param name="goals">a list of predefined goals</param>
    /// <param name="markerNumber">the marker number to be used in this task</param>
    /// <param name="distanceCalculation">the method used to calculate distances</param>
    /// <param name="markerValidationRule">The rule for marker validation
    /// <para>optional. use null to omit</para>
    /// <para>use <see cref="MarkerAndRule"/> or <see cref="MarkerOrRule"/> to chain multiple rules together</para>
    /// </param>
    /// <param name="validationStrictness">The strictness of the validation</param>
    public void SetupHWZ(int taskNumber, List<Coordinate> goals, int markerNumber, DistanceCalculationType distanceCalculation, IMarkerValidationRule markerValidationRule, ValidationStrictnessType validationStrictness)
    {
        TaskNumber = taskNumber;
        Goals = goals;
        MarkerNumber = markerNumber;
        DistanceCalculation = distanceCalculation;
        MarkerValidationRule = markerValidationRule;
        ValidationStrictness = validationStrictness;

    }

    /// <summary>
    /// Sets up the task with a list of custom goals
    /// </summary>
    /// <param name="taskNumber">the number of the task</param>
    /// <param name="defineGoals">a method used to determine the goals</param>
    /// <param name="markerNumber">the marker number to be used in this task</param>
    /// <param name="distanceCalculation">the method used to calculate distances</param>
    /// <param name="markerValidationRule">The rule for marker validation
    /// <para>optional. use null to omit</para>
    /// <para>use <see cref="MarkerAndRule"/> or <see cref="MarkerOrRule"/> to chain multiple rules together</para>
    /// </param>
    /// <param name="validationStrictness">The strictness of the validation</param>
    public void SetupHWZ(int taskNumber, Func<Track, List<Coordinate>> defineGoals, int markerNumber, DistanceCalculationType distanceCalculation, IMarkerValidationRule markerValidationRule, ValidationStrictnessType validationStrictness)
    {
        TaskNumber = taskNumber;
        DefineGoals = defineGoals;
        MarkerNumber = markerNumber;
        DistanceCalculation = distanceCalculation;
        MarkerValidationRule = markerValidationRule;
        ValidationStrictness = validationStrictness;

    }

    public override string ToString()
    {
        return $"Task#{TaskNumber} (HWZ)";
    }
    #endregion

    #region Private methods
    #endregion
}
