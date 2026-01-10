using Coordinates;
using JansScoring.calculation;
using JansScoring.plt;

namespace PZTesting;

public class PolygonPz : IPz
{
    private readonly List<Coordinate> _polygon;

    private readonly double _minHeight;
    private readonly double _maxHeight;


    private readonly double _northernmost;
    private readonly double _easternmost;
    private readonly double _southernmost;
    private readonly double _westernmost;

    private readonly double _maxDistanceInPz = 0;
    private Coordinate _virualCenterCoordinate = null;

    public PolygonPz(String pltFilePath, double minHeight, double maxHeight)
    {
        _polygon = PLTParser.Parse(pltFilePath);
        this._minHeight = minHeight;
        this._maxHeight = maxHeight;
        Console.WriteLine($"Loaded {_polygon.Count} corners from PLT-File");

        if (_polygon.Count == 0)
        {
            return;
        }

        _northernmost = _polygon[0].Latitude;
        _easternmost = _polygon[0].Longitude;
        _southernmost = _polygon[0].Latitude;
        _westernmost = _polygon[0].Longitude;
        foreach (Coordinate coordinate in _polygon)
        {
            if (_northernmost < coordinate.Latitude)
            {
                _northernmost = coordinate.Latitude;
            }

            if (_easternmost < coordinate.Longitude)
            {
                _easternmost = coordinate.Longitude;
            }

            if (_southernmost > coordinate.Latitude)
            {
                _southernmost = coordinate.Latitude;
            }

            if (_westernmost > coordinate.Longitude)
            {
                _westernmost = coordinate.Longitude;
            }

            foreach (Coordinate p2 in _polygon)
            {
                double distance = CalculationHelper.Calculate2DDistance(coordinate, p2, CalculationType.UTM);
                if (distance > _maxDistanceInPz)
                {
                    _maxDistanceInPz = distance;
                }
            }
        }


        double maxEasting = _polygon.Max(coordinate => coordinate.easting);
        double maxNorthing = _polygon.Max(coordinate => coordinate.northing);
        _virualCenterCoordinate = CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(_polygon[0].utmZone, maxEasting, maxNorthing);
    }


    public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double infringement)
    {
        double altitude = useGpsAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric;
        if (altitude < _minHeight || altitude > _maxHeight)
        {
            infringement = 0;
            return false;
        }

        if (coordinate.Longitude > _easternmost || coordinate.Longitude < _westernmost ||
            coordinate.Latitude > _northernmost || coordinate.Latitude < _southernmost)
        {
            infringement = 0;
            return false;
        }


        int polygonLength = _polygon.Count;
        int i, j = polygonLength - 1;
        bool oddNodes = false;

        for (i = 0; i < polygonLength; i++)
        {
            if (_polygon[i].Latitude < coordinate.Latitude && _polygon[j].Latitude >= coordinate.Latitude
                || _polygon[j].Latitude < coordinate.Latitude && _polygon[i].Latitude >= coordinate.Latitude)
            {
                if (_polygon[i].Longitude + (coordinate.Latitude - _polygon[i].Latitude) /
                    (_polygon[j].Latitude - _polygon[i].Latitude) *
                    (_polygon[j].Longitude - _polygon[i].Longitude) < coordinate.Longitude)
                {
                    oddNodes = !oddNodes;
                }
            }

            j = i;
        }

        infringement = 0;
        return oddNodes;
    }

    public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double averageHeightDifference = Math.Abs((useGPSAltitude
            ? entry.AltitudeGPS + exit.AltitudeGPS
            : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        double horizontalPercentage = (distanceHorizontal / _maxDistanceInPz) * 100;
        double verticalPercentage = 100 - ((averageHeightDifference / _maxHeight) * 100);
        var percentage = (verticalPercentage + horizontalPercentage) / 2;
        penalty =  percentage * 500;
    }


    public void CalculatePenaltyVariant2(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            summDeltaHeight += deltaHeight;
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }


    public void CalculatePenaltyVariant3A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            summDeltaHeight += (deltaHeight * deltaHeight);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }

    public void CalculatePenaltyVariant3B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            summDeltaHeight += (deltaHeight * 2);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }

    public void CalculatePenaltyVariant3C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            summDeltaHeight += (deltaHeight * 1.2);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }

    public void CalculatePenaltyVariant4A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance = CalculationHelper.Calculate2DDistance(trackpoint, _virualCenterCoordinate, CalculationType.UTM);
            summDeltaHeight += (deltaHeight / deltaDistance);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }

    public void CalculatePenaltyVariant4B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude ? trackpoint.AltitudeGPS - _maxHeight : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance = CoordinateHelpers.Calculate3DDistance(trackpoint, _virualCenterCoordinate, useGpsAltitude, CalculationType.UTM);
            summDeltaHeight += (deltaHeight / deltaDistance);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
    }

    public void CalculatePenaltyVariant5(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double minHeightInPz = pointsInPz.Min(coordinate => useGpsAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric);

        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double verticalPercentageDecimal = (_maxHeight - minHeightInPz) / (_maxHeight - _minHeight);
        double horizontalPercentage = (distanceHorizontal / _maxDistanceInPz) * 100;
        double verticalPercentage = 100 - (verticalPercentageDecimal * 100);
        penalty = ((verticalPercentage + horizontalPercentage) / 2) * 500;
    }
}