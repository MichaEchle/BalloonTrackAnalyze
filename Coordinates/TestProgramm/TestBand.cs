using Competition.Tasks;
using Coordinates;

namespace TestProgramm;

public class TestBand
{
    public void Score()
    {
        var innerBand = new AltitudeProfileTask.AltitudeBand
        {
            Type = AltitudeProfileTask.BandType.Inner,
            Multiplier = 2.0,
            Profile = CreateInnerBandProfile(),
            GeographicBounds = new AltitudeProfileTask.GeographicBoundary
            {
                Type = AltitudeProfileTask.GeographicBoundaryType.None 
            }
        };
        
        var outerBand = new AltitudeProfileTask.AltitudeBand
        {
            Type = AltitudeProfileTask.BandType.Outer,
            Multiplier = 1.0,
            Profile = CreateOuterBandProfile(),
            GeographicBounds = new AltitudeProfileTask.GeographicBoundary
            {
                Type = AltitudeProfileTask.GeographicBoundaryType.None
            }
        };


        var task = new AltitudeProfileTask();
        task.SetupTimeBasedAPT(
            taskNumber: 1,
            startCondition: new AltitudeProfileTask.StartCondition 
            { 
                Type = AltitudeProfileTask.StartConditionType.MarkerDrop, 
                MarkerNumber = 3
            },
            timeIntervalSeconds: 60,
            bands: [
                innerBand, outerBand
            ],
            maxDurationSeconds: 600
        );

    }
    
    private static List<AltitudeProfileTask.AltitudePoint> CreateInnerBandProfile()
    {
        return new List<AltitudeProfileTask.AltitudePoint>
        {
            new() { Step = 0, MinAltitude = CoordinateHelpers.ConvertToMeter(2100), MaxAltitude = CoordinateHelpers.ConvertToMeter(2200) },
            new() { Step = 1, MinAltitude = CoordinateHelpers.ConvertToMeter(2000), MaxAltitude = CoordinateHelpers.ConvertToMeter(2100) },
            new() { Step = 2, MinAltitude = CoordinateHelpers.ConvertToMeter(1900), MaxAltitude = CoordinateHelpers.ConvertToMeter(2000) },
            new() { Step = 3, MinAltitude = CoordinateHelpers.ConvertToMeter(1600), MaxAltitude = CoordinateHelpers.ConvertToMeter(1700) },
            new() { Step = 4, MinAltitude = CoordinateHelpers.ConvertToMeter(1500), MaxAltitude = CoordinateHelpers.ConvertToMeter(1600) },
            new() { Step = 5, MinAltitude = CoordinateHelpers.ConvertToMeter(1400), MaxAltitude = CoordinateHelpers.ConvertToMeter(1500) },
            new() { Step = 6, MinAltitude = CoordinateHelpers.ConvertToMeter(1100), MaxAltitude = CoordinateHelpers.ConvertToMeter(1200) },
            new() { Step = 7, MinAltitude = CoordinateHelpers.ConvertToMeter(1000), MaxAltitude = CoordinateHelpers.ConvertToMeter(1100) },
            new() { Step = 8, MinAltitude = CoordinateHelpers.ConvertToMeter(1200), MaxAltitude = CoordinateHelpers.ConvertToMeter(1300) },
            new() { Step = 9, MinAltitude = CoordinateHelpers.ConvertToMeter(1000), MaxAltitude = CoordinateHelpers.ConvertToMeter(1100) },
            new() { Step = 10, MinAltitude = CoordinateHelpers.ConvertToMeter(900), MaxAltitude = CoordinateHelpers.ConvertToMeter(1000) }
        };
    }
    
    private static List<AltitudeProfileTask.AltitudePoint> CreateOuterBandProfile()
    {
        return new List<AltitudeProfileTask.AltitudePoint>
        {
            new() { Step = 0, MinAltitude = CoordinateHelpers.ConvertToMeter(2000), MaxAltitude = CoordinateHelpers.ConvertToMeter(2300) },
            new() { Step = 1, MinAltitude = CoordinateHelpers.ConvertToMeter(1900), MaxAltitude = CoordinateHelpers.ConvertToMeter(2200) },
            new() { Step = 2, MinAltitude = CoordinateHelpers.ConvertToMeter(1800), MaxAltitude = CoordinateHelpers.ConvertToMeter(2100) },
            new() { Step = 3, MinAltitude = CoordinateHelpers.ConvertToMeter(1500), MaxAltitude = CoordinateHelpers.ConvertToMeter(1800) },
            new() { Step = 4, MinAltitude = CoordinateHelpers.ConvertToMeter(1400), MaxAltitude = CoordinateHelpers.ConvertToMeter(1700) },
            new() { Step = 5, MinAltitude = CoordinateHelpers.ConvertToMeter(1300), MaxAltitude = CoordinateHelpers.ConvertToMeter(1600) },
            new() { Step = 6, MinAltitude = CoordinateHelpers.ConvertToMeter(1000), MaxAltitude = CoordinateHelpers.ConvertToMeter(1300) },
            new() { Step = 7, MinAltitude = CoordinateHelpers.ConvertToMeter(900), MaxAltitude = CoordinateHelpers.ConvertToMeter(1200) },
            new() { Step = 8, MinAltitude = CoordinateHelpers.ConvertToMeter(1100), MaxAltitude = CoordinateHelpers.ConvertToMeter(1400) },
            new() { Step = 9, MinAltitude = CoordinateHelpers.ConvertToMeter(900), MaxAltitude = CoordinateHelpers.ConvertToMeter(1200) },
            new() { Step = 10, MinAltitude = CoordinateHelpers.ConvertToMeter(800), MaxAltitude = CoordinateHelpers.ConvertToMeter(1100) }
        };
    }


}