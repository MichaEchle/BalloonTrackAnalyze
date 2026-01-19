using Coordinates;
using JansScoring.calculation;
using PZTesting;
using PZTesting.pz;


namespace JansScoring.pz_rework.type;

public class CirclePZ : IPz
{
    private readonly Coordinate centerCoordinate;
    private readonly double height;
    private readonly int radius;

    public CirclePZ(Coordinate centerCoordinate, double height, int radius)
    {
        this.centerCoordinate = centerCoordinate;
        this.height = height;
        this.radius = radius;
    }

    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double infringement)
    {
        double distanceBetweenRedPa = CalculationHelper.Calculate2DDistance(coordinate, centerCoordinate, CalculationType.UTM);

        if (distanceBetweenRedPa <= radius &&
            (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) <=
            height)
        {
            infringement = radius - distanceBetweenRedPa;
            return true;
        }

        infringement = 0;
        return false;
    }

    public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double averageHeigtDiffrence = Math.Abs((useGPSAltitude
            ? entry.AltitudeGPS + exit.AltitudeGPS
            : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        double horizontalPercentage = (distanceHorizontal / radius * 2) * 100;
        double verticalPercentage = 100 - ((averageHeigtDiffrence / height) * 100);
        var percentage = (verticalPercentage + horizontalPercentage) / 2;
        penalty =  percentage * 500;
        penalty *= CorrectionFactor.VARIANT_1;
    }



    public void CalculatePenaltyVariant2(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            summDeltaHeight += Math.Abs(deltaHeight);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_2;
    }

    public void CalculatePenaltyVariant3A(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            summDeltaHeight += Math.Abs(deltaHeight * deltaHeight);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_3a;
    }

    public void CalculatePenaltyVariant3B(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            summDeltaHeight += Math.Abs(deltaHeight * 2);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_3b;
    }

    public void CalculatePenaltyVariant3C(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            summDeltaHeight += Math.Abs(deltaHeight * 1.2);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_3c;
    }

    public void CalculatePenaltyVariant4A(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaDistance = CalculationHelper.Calculate2DDistance(trackpoint, centerCoordinate, CalculationType.UTM);
            summDeltaHeight += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_4a;
    }

    public void CalculatePenaltyVariant4B(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double summDeltaHeight = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaDistance = CoordinateHelpers.Calculate3DDistance(trackpoint, centerCoordinate, useGPSAltitude, CalculationType.UTM);
            summDeltaHeight += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        penalty *= CorrectionFactor.VARIANT_4b;
    }

    public void CalculatePenaltyVariant5(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        double minHeightInPz = pointsInPz.Min(coordinate => useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric);

        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double verticalPercentageDecimal = (height - minHeightInPz) / height;
        double horizontalPercentage = (distanceHorizontal / (radius * 2)) * 100;
        double verticalPercentage = 100 - (verticalPercentageDecimal * 100);
        penalty = ((verticalPercentage + horizontalPercentage) / 2) * 500;
        penalty *= CorrectionFactor.VARIANT_5;
    }
}