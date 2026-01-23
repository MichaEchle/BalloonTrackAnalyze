using Coordinates;
using JansScoring.calculation;
using JansScoring.plt;
using PZTesting.pz;

namespace PZTesting;

public class PolygonPz : IPz
{
    private readonly List<Coordinate> _polygon;

    private readonly double _minHeight;
    private readonly double _maxHeight;
    private readonly double _virtualCenterHeight;

    private readonly double _northernmost;
    private readonly double _easternmost;
    private readonly double _southernmost;
    private readonly double _westernmost;

    private readonly double _maxDistanceInPz = 0;
    private Coordinate _virualCenterCoordinate = null;

    public PolygonPz(String pltFilePath, double minHeight, double maxHeight, double virtualCenterHeight)
    {
        _polygon = PLTParser.Parse(pltFilePath);
        this._minHeight = minHeight;
        this._maxHeight = maxHeight;
        this._virtualCenterHeight = virtualCenterHeight;
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


        double maxEastingC = _polygon.Max(coordinate => coordinate.easting);
        double minEastingC = _polygon.Min(coordinate => coordinate.easting);
        double maxNorthingC = _polygon.Max(coordinate => coordinate.northing);
        double minNorthingC = _polygon.Min(coordinate => coordinate.northing);

        double centerEasting = ((maxEastingC - minEastingC) / 2) + minEastingC;
        double centerNorting = ((maxNorthingC - minNorthingC) / 2) + minNorthingC;
        _virualCenterCoordinate =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(_polygon[0].utmZone, centerEasting, centerNorting,
                virtualCenterHeight);
    }


    //public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double infringement, out double verticalInfringement)
    public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double horizontalInfringement,
        out double verticalInfringement)
    {
        double altitude = useGpsAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric;
        if (altitude < _minHeight || altitude > _maxHeight)
        {
            horizontalInfringement = 0;
            verticalInfringement = 0;
            return false;
        }

        if (coordinate.Longitude > _easternmost || coordinate.Longitude < _westernmost ||
            coordinate.Latitude > _northernmost || coordinate.Latitude < _southernmost)
        {
            horizontalInfringement = 0;
            verticalInfringement = 0;

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

        horizontalInfringement = 0;
        verticalInfringement = 0;

        return oddNodes;
    }

    public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation of current COH rule R7.5
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double averageHeightDifference = (Math.Abs(useGPSAltitude
            ? entry.AltitudeGPS + exit.AltitudeGPS
            : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        //Console.WriteLine("Horizontal Distance: " + distanceHorizontal + " averageHeightDifference: " + averageHeightDifference);
        //Console.WriteLine("averageHeightDifference: " + averageHeightDifference);
        //var horizontalInf = distanceHorizontal / _maxDistanceInPz * 100;
        //var verticalInf = averageHeightDifference / _maxHeight* 100;
        double horizontalInf = distanceHorizontal / _maxDistanceInPz * 100;
        double verticalInf = 100 - (averageHeightDifference / _maxHeight) * 100;
        penalty = ((horizontalInf + verticalInf) / 2 / 100) * 500;

        horizontalInfringement = distanceHorizontal;
        verticalInfringement = averageHeightDifference;

        penalty *= CorrectionFactor.VARIANT_1;
        penalty = Math.Abs(penalty);
        //#######
        //Console.WriteLine("CirclePZ : V1: PRE penalty: " + penalty);
    }


    public void CalculatePenaltyVariant2(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            //summDeltaHeight += deltaHeight;
            sumDeltaHeight += Math.Abs(deltaHeight);
            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }
        }

        horizontalInfringement = 0;

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_2;
        penalty = Math.Abs(penalty);
    }


    public void CalculatePenaltyVariant3A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction is squared, overall divided by 10.000
        double sumDeltaHeight = 0;
        verticalInfringement = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            sumDeltaHeight += Math.Abs(deltaHeight * deltaHeight);

            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        horizontalInfringement = 0;
        penalty *= CorrectionFactor.VARIANT_3a;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant3B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction is doubled, overall divided by 100
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            sumDeltaHeight += Math.Abs(deltaHeight * 2);

            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_3b;
        penalty = Math.Abs(penalty);
        horizontalInfringement = 0;
    }

    public void CalculatePenaltyVariant3C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction with weighting of 20%, overall divided by 100
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            sumDeltaHeight += Math.Abs(deltaHeight * 1.2);
            Console.WriteLine("PolyGon V3: deltaHeight " + deltaHeight + " sumDeltaHeight: " + sumDeltaHeight);
            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        Console.WriteLine("PolyGon V3: PrePenalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_3c;
        penalty = Math.Abs(penalty);
        Console.WriteLine("PolyGon V3: Penalty incl. factor: " + penalty);

        horizontalInfringement = 0;
    }

    public void CalculatePenaltyVariant4A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 2D distance between the track point and the virtual center, overall divided by 10
        double sumInfringement = 0;
        horizontalInfringement = 0;
        verticalInfringement = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance =
                CalculationHelper.Calculate2DDistance(trackpoint, _virualCenterCoordinate, CalculationType.UTM);
            sumInfringement += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));
            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }

            if (Math.Abs(deltaDistance) > horizontalInfringement)
            {
                horizontalInfringement = deltaHeight;
            }

            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }

            if (Math.Abs(deltaDistance) > horizontalInfringement)
            {
                horizontalInfringement = deltaHeight;
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumInfringement);
        penalty *= CorrectionFactor.VARIANT_4a;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant4B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 3D distance between the track point and the virtual center, overall divided by 1
        double sumInfringement = 0;
        horizontalInfringement = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance = CoordinateHelpers.Calculate3DDistance(trackpoint, _virualCenterCoordinate,
                useGpsAltitude, CalculationType.UTM);
            sumInfringement += (deltaHeight / deltaDistance);
            if (Math.Abs(deltaHeight) > verticalInfringement)
            {
                verticalInfringement = deltaHeight;
            }

            if (Math.Abs(deltaDistance) > horizontalInfringement)
            {
                horizontalInfringement = deltaHeight;
            }
        }

        penalty = sumInfringement;
        penalty *= CorrectionFactor.VARIANT_4b;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant5(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double minHeightInPz = pointsInPz.Min(coordinate =>
            useGpsAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric);

        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double verticalPercentageDecimal = (_maxHeight - minHeightInPz) / (_maxHeight - _minHeight);
        double horizontalPercentage = (distanceHorizontal / _maxDistanceInPz) * 100;
        double verticalPercentage = 100 - (verticalPercentageDecimal * 100);
        penalty = ((verticalPercentage + horizontalPercentage) / 2) * 500;
        penalty *= CorrectionFactor.VARIANT_5;
        penalty = Math.Abs(penalty);
        horizontalInfringement = -1;
        verticalInfringement = -1;
    }
}