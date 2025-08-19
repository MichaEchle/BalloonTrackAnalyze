
using Competition.Validation;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Competition.Tasks;

public class AltitudeProfileTask : ICompetitionTask
{
    #region Enums
    public enum StartConditionType
    {
        MarkerDrop,
        GridLineCross,
        TimeStamp,
        CoordinateReach,
        AltitudeReach,
        Custom
    }

    public enum StepType
    {
        TimeInterval,
        GridLineCross,
        DistanceInterval,
        AltitudeChange,
        Custom
    }

    public enum BandType
    {
        Inner,
        Outer
    }

    public enum GeographicBoundaryType
    {
        None,
        NorthingGridLines,
        EastingGridLines,
        LatitudeBounds,
        LongitudeBounds,
        CoordinateBounds,
        Custom
    }
    #endregion

    #region Inner Classes
    public class GeographicBoundary
    {
        public GeographicBoundaryType Type { get; set; } = GeographicBoundaryType.None;
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public Coordinate? MinCoordinate { get; set; }
        public Coordinate? MaxCoordinate { get; set; }
        public string? UTMZone { get; set; } 
        public Func<Coordinate, bool>? CustomBoundaryCheck { get; set; }
    }

    public class AltitudeBand
    {
        public BandType Type { get; set; }
        public double Multiplier { get; set; }
        public List<AltitudePoint> Profile { get; set; } = [];
        public GeographicBoundary GeographicBounds { get; set; } = new();
    }

    public class AltitudePoint
    {
        public double Step { get; set; }
        public double MinAltitude { get; set; }
        public double MaxAltitude { get; set; }
    }

    public class StartCondition
    {
        public StartConditionType Type { get; set; }
        public int? MarkerNumber { get; set; }
        public Coordinate? GridLine { get; set; }
        public DateTime? TimeStamp { get; set; }
        public Coordinate? TargetCoordinate { get; set; }
        public double? TargetAltitude { get; set; }
        public double? Tolerance { get; set; }
        public Func<Track, Coordinate?>? CustomCondition { get; set; }
    }
    #endregion

    #region Properties
    [JsonIgnore()]
    private readonly ILogger<AltitudeProfileTask> Logger = LogConnector.LoggerFactory.CreateLogger<AltitudeProfileTask>();

    public int TaskNumber { get; set; } = -1;
    
    public StartCondition TaskStart { get; set; } = new();


    public StepType ProfileStepType { get; set; } = StepType.TimeInterval;


    public double StepInterval { get; set; } = 60.0; 

 
    public double? MaxTaskDuration { get; set; } = null;


    public List<AltitudeBand> AltitudeBands { get; set; } = [];


    public Func<Coordinate, Coordinate, double>? CustomStepCalculator { get; set; }


    public bool UseGPSAltitude { get; set; } = false;


    public IMarkerValidationRule? MarkerValidationRule { get; set; } = null;


    public ValidationStrictnessType ValidationStrictness { get; set; } = ValidationStrictnessType.FirstValid;
    
    #endregion

    #region API
    public bool CalculateResults(Track track, bool useGPSAltitude, out double result)
    {
        result = 0.0;

        try
        {
            Coordinate? startPoint = FindTaskStartPoint(track);
            if (startPoint == null)
            {
                Logger?.LogError("Failed to find task start point for '{task}' and Pilot '#{pilotNumber}{pilotName}'", 
                    ToString(), track.Pilot.PilotNumber, GetPilotName(track));
                return false;
            }

            var relevantPoints = GetRelevantCoordinates(track, startPoint, useGPSAltitude);
            if (!relevantPoints.Any())
            {
                Logger?.LogWarning("No relevant track points found after task start for '{task}' and Pilot '#{pilotNumber}{pilotName}'", 
                    ToString(), track.Pilot.PilotNumber, GetPilotName(track));
                return true; 
            }

            result = CalculateTimeInBands(relevantPoints, startPoint, useGPSAltitude);
            
            Logger?.LogInformation("Task '{task}' result calculated for Pilot '#{pilotNumber}{pilotName}': {result}", 
                ToString(), track.Pilot.PilotNumber, GetPilotName(track), result);
            
            return true;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to calculate result for '{task}' and Pilot '#{pilotNumber}{pilotName}'", 
                ToString(), track.Pilot.PilotNumber, GetPilotName(track));
            return false;
        }
    }
    
    public void SetupTimeBasedAPT(int taskNumber, StartCondition startCondition, double timeIntervalSeconds, 
        List<AltitudeBand> bands, double? maxDurationSeconds = null)
    {
        TaskNumber = taskNumber;
        TaskStart = startCondition;
        ProfileStepType = StepType.TimeInterval;
        StepInterval = timeIntervalSeconds;
        AltitudeBands = bands;
        MaxTaskDuration = maxDurationSeconds;
    }


    public void SetupDistanceBasedAPT(int taskNumber, StartCondition startCondition, double distanceIntervalMeters,
        List<AltitudeBand> bands, double? maxDurationSeconds = null)
    {
        TaskNumber = taskNumber;
        TaskStart = startCondition;
        ProfileStepType = StepType.DistanceInterval;
        StepInterval = distanceIntervalMeters;
        AltitudeBands = bands;
        MaxTaskDuration = maxDurationSeconds;
    }


    public void SetupCustomAPT(int taskNumber, StartCondition startCondition, StepType stepType,
        List<AltitudeBand> bands, Func<Coordinate, Coordinate, double>? customCalculator = null,
        double? maxDurationSeconds = null)
    {
        TaskNumber = taskNumber;
        TaskStart = startCondition;
        ProfileStepType = stepType;
        AltitudeBands = bands;
        CustomStepCalculator = customCalculator;
        MaxTaskDuration = maxDurationSeconds;
    }

    public override string ToString()
    {
        return $"Task#{TaskNumber} (APT)";
    }
    #endregion

    #region Private Methods
    private Coordinate? FindTaskStartPoint(Track track)
    {
        return TaskStart.Type switch
        {
            StartConditionType.MarkerDrop => FindMarkerStartPoint(track),
            StartConditionType.GridLineCross => FindGridLineCrossPoint(track),
            StartConditionType.TimeStamp => FindTimeStampPoint(track),
            StartConditionType.CoordinateReach => FindCoordinateReachPoint(track),
            StartConditionType.AltitudeReach => FindAltitudeReachPoint(track),
            StartConditionType.Custom => FindCustomStartPoint(track),
            _ => null
        };
    }

    private Coordinate? FindMarkerStartPoint(Track track)
    {
        if (TaskStart.MarkerNumber == null) return null;
        
        var marker = ValidationHelper.GetValidMarker(track, TaskStart.MarkerNumber.Value, 
            MarkerValidationRule, ValidationStrictness);
        
        return marker?.MarkerLocation;
    }

    private Coordinate? FindGridLineCrossPoint(Track track)
    {
        if (TaskStart.GridLine == null) return null;
        
        return track.TrackPoints.FirstOrDefault(tp => 
            Math.Abs(tp.Latitude - TaskStart.GridLine.Latitude) < (TaskStart.Tolerance ?? 0.0001) ||
            Math.Abs(tp.Longitude - TaskStart.GridLine.Longitude) < (TaskStart.Tolerance ?? 0.0001));
    }

    private Coordinate? FindTimeStampPoint(Track track)
    {
        if (TaskStart.TimeStamp == null) return null;
        
        return track.TrackPoints.FirstOrDefault(tp => tp.TimeStamp >= TaskStart.TimeStamp.Value);
    }

    private Coordinate? FindCoordinateReachPoint(Track track)
    {
        if (TaskStart.TargetCoordinate == null) return null;
        
        double tolerance = TaskStart.Tolerance ?? 100.0; 
        
        return track.TrackPoints.FirstOrDefault(tp => 
            CoordinateHelpers.Calculate2DDistanceHavercos(TaskStart.TargetCoordinate, tp) <= tolerance);
    }

    private Coordinate? FindAltitudeReachPoint(Track track)
    {
        if (TaskStart.TargetAltitude == null) return null;
        
        double tolerance = TaskStart.Tolerance ?? 10.0;
        
        return track.TrackPoints.FirstOrDefault(tp => 
            Math.Abs((UseGPSAltitude ? tp.AltitudeGPS : tp.AltitudeBarometric) - TaskStart.TargetAltitude.Value) <= tolerance);
    }

    private Coordinate? FindCustomStartPoint(Track track)
    {
        return TaskStart.CustomCondition?.Invoke(track);
    }

    private List<Coordinate> GetRelevantCoordinates(Track track, Coordinate startPoint, bool useGPSAltitude)
    {
        var points = track.TrackPoints.Where(tp => tp.TimeStamp >= startPoint.TimeStamp).ToList();
        
        if (MaxTaskDuration.HasValue)
        {
            var endTime = startPoint.TimeStamp.AddSeconds(MaxTaskDuration.Value);
            points = points.Where(tp => tp.TimeStamp <= endTime).ToList();
        }
        
        return points;
    }

    private double CalculateTimeInBands(List<Coordinate> Coordinates, Coordinate startPoint, bool useGPSAltitude)
    {
        if (!Coordinates.Any() || !AltitudeBands.Any()) return 0.0;

        double totalScore = 0.0;
        Coordinate? previousPoint = null;

        foreach (var point in Coordinates)
        {
            if (previousPoint == null)
            {
                previousPoint = point;
                continue;
            }

            double stepValue = CalculateStepValue(previousPoint, point, startPoint);
            double timeInSeconds = (point.TimeStamp - previousPoint.TimeStamp).TotalSeconds;
            double altitude = useGPSAltitude ? point.AltitudeGPS : point.AltitudeBarometric;

            foreach (var band in AltitudeBands)
            {
                if (!IsWithinGeographicBounds(point, band.GeographicBounds))
                    continue;

                var altitudeRange = GetAltitudeRangeForStep(band, stepValue);
                if (altitudeRange != null && altitude >= altitudeRange.MinAltitude && altitude <= altitudeRange.MaxAltitude)
                {
                    totalScore += timeInSeconds * band.Multiplier;
                }
            }

            previousPoint = point;
        }

        return totalScore;
    }

    private bool IsWithinGeographicBounds(Coordinate point, GeographicBoundary bounds)
    {
        return bounds.Type switch
        {
            GeographicBoundaryType.None => true,
            GeographicBoundaryType.NorthingGridLines => IsWithinNorthingBounds(point, bounds),
            GeographicBoundaryType.EastingGridLines => IsWithinEastingBounds(point, bounds),
            GeographicBoundaryType.LatitudeBounds => IsWithinLatitudeBounds(point, bounds),
            GeographicBoundaryType.LongitudeBounds => IsWithinLongitudeBounds(point, bounds),
            GeographicBoundaryType.CoordinateBounds => IsWithinCoordinateBounds(point, bounds),
            GeographicBoundaryType.Custom => bounds.CustomBoundaryCheck?.Invoke(point) ?? true,
            _ => true
        };
    }

    private bool IsWithinNorthingBounds(Coordinate point, GeographicBoundary bounds)
    {
        try
        {
           (string utmZone, int easting, int northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(point);
            
            if (bounds.MinValue.HasValue && northing < bounds.MinValue.Value)
                return false;
            if (bounds.MaxValue.HasValue && northing > bounds.MaxValue.Value)
                return false;
                
            return true;
        }
        catch (Exception ex)
        {
            Logger?.LogWarning(ex, "Failed to check northing bounds for point at {lat},{lon}", 
                point.Latitude, point.Longitude);
            return true;
        }
    }

    private bool IsWithinEastingBounds(Coordinate point, GeographicBoundary bounds)
    {
        try
        {
            (string utmZone, int easting, int northing) = CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM(point);
            
            if (bounds.MinValue.HasValue && easting < bounds.MinValue.Value)
                return false;
            if (bounds.MaxValue.HasValue && easting > bounds.MaxValue.Value)
                return false;
                
            return true;
        }
        catch (Exception ex)
        {
            Logger?.LogWarning(ex, "Failed to check easting bounds for point at {lat},{lon}", 
                point.Latitude, point.Longitude);
            return true;
        }
    }

    private bool IsWithinLatitudeBounds(Coordinate point, GeographicBoundary bounds)
    {
        if (bounds.MinValue.HasValue && point.Latitude < bounds.MinValue.Value)
            return false;
        if (bounds.MaxValue.HasValue && point.Latitude > bounds.MaxValue.Value)
            return false;
            
        return true;
    }

    private bool IsWithinLongitudeBounds(Coordinate point, GeographicBoundary bounds)
    {
        if (bounds.MinValue.HasValue && point.Longitude < bounds.MinValue.Value)
            return false;
        if (bounds.MaxValue.HasValue && point.Longitude > bounds.MaxValue.Value)
            return false;
            
        return true;
    }

    private bool IsWithinCoordinateBounds(Coordinate point, GeographicBoundary bounds)
    {
        if (bounds.MinCoordinate == null || bounds.MaxCoordinate == null)
            return true;

        return point.Latitude >= bounds.MinCoordinate.Latitude &&
               point.Latitude <= bounds.MaxCoordinate.Latitude &&
               point.Longitude >= bounds.MinCoordinate.Longitude &&
               point.Longitude <= bounds.MaxCoordinate.Longitude;
    }

    private double CalculateStepValue(Coordinate currentPoint, Coordinate nextPoint, Coordinate startPoint)
    {
        return ProfileStepType switch
        {
            StepType.TimeInterval => (currentPoint.TimeStamp - startPoint.TimeStamp).TotalSeconds / StepInterval,
            StepType.DistanceInterval => CalculateDistanceFromStart(currentPoint, startPoint) / StepInterval,
            StepType.GridLineCross => CalculateGridCrossings(currentPoint, startPoint),
            StepType.AltitudeChange => Math.Abs((UseGPSAltitude ? currentPoint.AltitudeGPS : currentPoint.AltitudeBarometric) - 
                                              (UseGPSAltitude ? startPoint.AltitudeGPS : startPoint.AltitudeBarometric)) / StepInterval,
            StepType.Custom => CustomStepCalculator?.Invoke(currentPoint, startPoint) ?? 0.0,
            _ => 0.0
        };
    }

    private double CalculateDistanceFromStart(Coordinate currentPoint, Coordinate startPoint)
    {
        return CoordinateHelpers.Calculate2DDistanceHavercos(startPoint, currentPoint);
    }

    private double CalculateGridCrossings(Coordinate currentPoint, Coordinate startPoint)
    {
        return Math.Floor(Math.Abs(currentPoint.Latitude - startPoint.Latitude) / 0.01) +
               Math.Floor(Math.Abs(currentPoint.Longitude - startPoint.Longitude) / 0.01);
    }

    private AltitudePoint? GetAltitudeRangeForStep(AltitudeBand band, double stepValue)
    {
        if (!band.Profile.Any()) return null;

        var orderedProfile = band.Profile.OrderBy(p => p.Step).ToList();
        
        AltitudePoint? lowerBound = null;
        AltitudePoint? upperBound = null;

        for (int i = 0; i < orderedProfile.Count; i++)
        {
            if (orderedProfile[i].Step <= stepValue)
            {
                lowerBound = orderedProfile[i];
            }
            if (orderedProfile[i].Step >= stepValue && upperBound == null)
            {
                upperBound = orderedProfile[i];
                break;
            }
        }

        if (lowerBound != null && (upperBound == null || Math.Abs(lowerBound.Step - stepValue) < 0.001))
        {
            return lowerBound;
        }

        if (lowerBound != null && upperBound != null && Math.Abs(upperBound.Step - lowerBound.Step) > 0.001)
        {
            double ratio = (stepValue - lowerBound.Step) / (upperBound.Step - lowerBound.Step);
            return new AltitudePoint
            {
                Step = stepValue,
                MinAltitude = lowerBound.MinAltitude + (upperBound.MinAltitude - lowerBound.MinAltitude) * ratio,
                MaxAltitude = lowerBound.MaxAltitude + (upperBound.MaxAltitude - lowerBound.MaxAltitude) * ratio
            };
        }

        return lowerBound;
    }

    private string GetPilotName(Track track)
    {
        return !string.IsNullOrWhiteSpace(track.Pilot.FirstName) 
            ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" 
            : "";
    }
    #endregion
}