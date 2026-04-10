using Scoring.Competitions.Pilots;
using Scoring.Competitions.PZ;
using Scoring.Converters;
using Scoring.Coordinates;

namespace Scoring.Competitions;

public enum AltitudeSourceType
{
    GPS,
    Barometric
}

public enum CommonLaunchPointType
{
    CLP_A,
    CLP_B,
    CLP_C,
    CLP_D,
    CLP_E,
}

public class Competition
{
    public string Name
    {
        get;
    }

    public double SeparationAltitude
    {
        get;
    }

    public AltitudeSourceType AltitudeSource
    {
        get;
    }

    public List<Pilot> Pilots
    {
        get;
    }

    public Ellipsoid Ellipsoid
    {
        get;
    }

    private Competition(string name, double separationAltitude, AltitudeSourceType altitudeSource, List<Pilot> pilots, Ellipsoid ellipsoid)
    {
        Name = name;
        SeparationAltitude = separationAltitude;
        AltitudeSource = altitudeSource;
        Pilots = pilots;
        Ellipsoid = ellipsoid;
    }

    public static Competition Instance
    {
        get
        {
            if (field is null)
            {
                throw new InvalidOperationException("Competition has not been created yet");
            }
            return field;
        }
        private set;
    }

    public static void Create(string name, double separationAltitude, AltitudeSourceType altitudeSource, List<Pilot> pilots, Ellipsoid ellipsoid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNull(pilots);
        ArgumentNullException.ThrowIfNull(ellipsoid);

        Instance = new Competition(name, separationAltitude, altitudeSource, pilots, ellipsoid);
    }

    public Dictionary<CommonLaunchPointType, Coordinate> CommonLaunchPoints
    {
        get;
    } = new Dictionary<CommonLaunchPointType, Coordinate>(Enum.GetValues<CommonLaunchPointType>().Length);

    //public List<Flight> Flights
    //{
    //    get;
    //}

    public List<ProhibitedZone> PZs
    {
        get;
    } = [];


}
