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
    private Coordinate _virtualCenterCoordinate = null;

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
        _virtualCenterCoordinate =
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate(_polygon[0].utmZone, centerEasting, centerNorting,
                virtualCenterHeight);
        Console.WriteLine($"Calculated virtual center: {_virtualCenterCoordinate}");
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


    public void CalculatePenaltyVariant2A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            //sumDeltaHeight += deltaHeight;
            sumDeltaHeight += Math.Abs(deltaHeight);
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        horizontalInfringement = 0;

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_2a;
        penalty = Math.Abs(penalty);
    }


    public void CalculatePenaltyVariant2B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
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

            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        horizontalInfringement = 0;
        penalty *= CorrectionFactor.VARIANT_2b;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant2C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
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

            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_2c;
        penalty = Math.Abs(penalty);
        horizontalInfringement = 0;
    }

    public void CalculatePenaltyVariant2D(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
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
            //Console.WriteLine("PolyGon V2D: deltaHeight " + deltaHeight + " sumDeltaHeight: " + sumDeltaHeight +" verticalInfringement: " + verticalInfringement);
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("PolyGon V2D: PrePenalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_2d;
        penalty = Math.Abs(penalty);
        //Console.WriteLine("PolyGon V2D: Penalty incl. factor: " + penalty);

        horizontalInfringement = 0;
    }
public void CalculatePenaltyVariant3A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        horizontalInfringement = 0;
        verticalInfringement = 0;
        double horizontalFactor = 0;
        double verticalFactor = 0;
        double overAllHorizontal = 0;
        double overAllVertical = 0;


        Coordinate previousCoordinate = null;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            if (previousCoordinate == null)
            {
                previousCoordinate = trackpoint;
                continue;
            }

            double altPrev = useGpsAltitude ? previousCoordinate.AltitudeGPS : previousCoordinate.AltitudeBarometric;
            double altCurr = useGpsAltitude ? trackpoint.AltitudeGPS : trackpoint.AltitudeBarometric;

            double distanceBetween = CoordinateHelpers.Calculate2DDistanceUTM(previousCoordinate, trackpoint);
            double heightBetween = Math.Abs(_maxHeight - altCurr);
//            double heightBetween = Math.Abs(altCurr - altPrev);
            horizontalInfringement += Math.Abs(distanceBetween);
            verticalInfringement += Math.Abs(heightBetween);
            previousCoordinate = trackpoint;
            horizontalFactor = horizontalInfringement / _maxDistanceInPz;
            verticalFactor = verticalInfringement / _maxHeight;
            /*Console.WriteLine("Polygon 3A: horizontalInfringement: " + horizontalInfringement + " horizontalFactor: " +
                              horizontalFactor + " verticalInfringement: " + verticalInfringement +
                              " verticalFactor: " + verticalFactor);*/
        }

        penalty = (verticalFactor + horizontalFactor) / 2; // * 0.1;
        penalty *= CorrectionFactor.VARIANT_3a;
        penalty = Math.Abs(penalty) * 100;   // *1000;
        //Console.WriteLine("PolyGon V3A: Penalty: " + penalty);
    }

    public void CalculatePenaltyVariant3B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        horizontalInfringement = 0;
        verticalInfringement = 0;
        double horizontalFactor = 0;
        double verticalFactor = 0;
        double overAllHorizontal = 0;
        double overAllVertical = 0;

        int pointsCalculated = 0;

        Coordinate previousCoordinate = null;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            if (previousCoordinate == null)
            {
                previousCoordinate = trackpoint;
                continue;
            }

            double altPrev = useGpsAltitude ? previousCoordinate.AltitudeGPS : previousCoordinate.AltitudeBarometric;
            double altCurr = useGpsAltitude ? trackpoint.AltitudeGPS : trackpoint.AltitudeBarometric;

            double distanceBetween = CoordinateHelpers.Calculate2DDistanceUTM(previousCoordinate, trackpoint);
            double heightBetween = Math.Abs(_maxHeight - altCurr);
//            double heightBetween = Math.Abs(altCurr - altPrev);
            horizontalInfringement += Math.Abs(distanceBetween);
            verticalInfringement += Math.Abs(heightBetween);
            previousCoordinate = trackpoint;


            horizontalFactor = horizontalInfringement / _maxDistanceInPz;
            // Durchschnitt aller Höhenverletzungen im PZ

            // verticalFactor = Durchschnitt / PZ ceiling
            pointsCalculated++;
            verticalFactor = verticalInfringement / pointsCalculated;
            //verticalFactor = verticalInfringement / _maxHeight;

            /* Console.WriteLine("Polygon 3B: horizontalInfringement: " + horizontalInfringement + " horizontalFactor: " +
                          horizontalFactor + " verticalInfringement: " + verticalInfringement + " verticalFactor: " +
                          verticalFactor); */
        }

        penalty = (verticalFactor + horizontalFactor) / 2; // * 0.01;
        penalty *= CorrectionFactor.VARIANT_3b;
        penalty = Math.Abs(penalty) * 5;    // *500;
        verticalInfringement = verticalFactor;
        //Console.WriteLine("PolyGon V3B: Penalty: " + penalty);
    }
    public void CalculatePenaltyVariant4A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 2D distance between the track point and the virtual center, overall divided by 10
        double sumInfringement = 0;
        horizontalInfringement = 10000;
        verticalInfringement = 0;
        double worstRatio = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance =
                CalculationHelper.Calculate2DDistance(trackpoint, _virtualCenterCoordinate, CalculationType.UTM);
            sumInfringement += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));

            if (sumInfringement > worstRatio)
            {
                worstRatio = sumInfringement;
            }

            //Console.WriteLine("PolyGon V4A: deltaHeight: " + deltaHeight + " deltaDistance: " + deltaDistance + " sumInfringement: " + sumInfringement+ " worstRatio: " + worstRatio);

            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }

            if (Math.Abs(deltaDistance) < Math.Abs(horizontalInfringement))
            {
                horizontalInfringement = Math.Abs(deltaDistance);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumInfringement);
        //Console.WriteLine("PolyGon V4A: PrePenalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_4a;
        penalty = Math.Abs(penalty);
        //Console.WriteLine("PolyGon V4A: Penalty: " + penalty + " worstRatio: " + worstRatio);
    }

    public void CalculatePenaltyVariant4B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 3D distance between the track point and the virtual center, overall divided by 1
        double sumInfringement = 0;
        horizontalInfringement = 10000;
        verticalInfringement = 0;
        double worstRatio = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGpsAltitude
                ? trackpoint.AltitudeGPS - _maxHeight
                : trackpoint.AltitudeBarometric - _maxHeight;
            double deltaDistance = CoordinateHelpers.Calculate3DDistance(trackpoint, _virtualCenterCoordinate,
                useGpsAltitude, CalculationType.UTM);

            sumInfringement += (deltaHeight / deltaDistance);

            if (sumInfringement > worstRatio)
            {
                worstRatio = sumInfringement;
            }

            /*Console.WriteLine("PolyGon V4B: deltaHeight: " + deltaHeight + " deltaDistance: " + deltaDistance +
                              " sumInfringement: " + sumInfringement + " worstRatio: " + worstRatio);
            */

            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }

            if (Math.Abs(deltaDistance) < Math.Abs(horizontalInfringement))
            {
                horizontalInfringement = Math.Abs(deltaDistance);
            }
        }

        penalty = sumInfringement;
        //Console.WriteLine("PolyGon V4B: PrePenalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_4b;
        penalty = Math.Abs(penalty);
        //Console.WriteLine("PolyGon V4B: Penalty: " + penalty + " worstRatio: " + worstRatio);
    }

    
}