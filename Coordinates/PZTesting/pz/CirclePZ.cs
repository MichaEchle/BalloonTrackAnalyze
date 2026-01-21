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
    private int ctr =0;

    public CirclePZ(Coordinate centerCoordinate, double height, int radius)
    {
        this.centerCoordinate = centerCoordinate;
        this.height = height;
        this.radius = radius;
    }

    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double infringement)
    {
        double distanceBetweenRedPa = CalculationHelper.Calculate2DDistance(coordinate, centerCoordinate, CalculationType.UTM);
        double infringementAltitude;

        if (distanceBetweenRedPa <= radius &&
            (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) <=
            height)
        {
            infringement = radius - distanceBetweenRedPa;
            infringementAltitude = (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) - height;
            
            //Console.WriteLine("CirclePZ :im PZ # "+ ctr + " infringement horizontal: " + infringement +  " infringement altitude: " + infringementAltitude);
         
            //ctr++;
            
            return true;
        }

        infringement = 0;
        return false;
    }

    //public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty, out double distanceHorizontal, out double averageHeightDifference)
    public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        Coordinate entry = pointsInPz.First();
        Coordinate exit = pointsInPz.Last();
        
        double distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        //distanceHorizontal = CalculationHelper.Calculate2DDistance(entry, exit, CalculationType.UTM);
        double averageHeightDifference = Math.Abs((useGPSAltitude
        ? entry.AltitudeGPS + exit.AltitudeGPS
            : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        //averageHeightDifference = Math.Abs((useGPSAltitude
        //    ? entry.AltitudeGPS + exit.AltitudeGPS
        //    : entry.AltitudeBarometric + exit.AltitudeBarometric) / 2);
        //Console.WriteLine("Horizontal Distance: " + distanceHorizontal + " averageHeightDifference: " + averageHeightDifference);
        //Console.WriteLine("averageHeightDifference: " + averageHeightDifference);
        double horizontalPercentage = (distanceHorizontal / (radius * 2)) * 100;
        double verticalPercentage = 100 - ((averageHeightDifference / height) * 100);
        
        var percentage = (verticalPercentage + horizontalPercentage) / 2;
        penalty =  (percentage / 100) * 500;
        //Console.WriteLine("CirclePZ : V1: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_1;
    }



    public void CalculatePenaltyVariant2(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty)
    {
        double sumDeltaHeight = 0;
        //double deltaHeight = 0;
        //double deltaHeight;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            //deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            sumDeltaHeight += Math.Abs(deltaHeight);
            //Console.WriteLine("CirclePZ : V2: deltaHeight: "+ deltaHeight + " summDeltaHeight: " + sumDeltaHeight);
        }
        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V2: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_2;
        //Console.WriteLine("CorrectionFactor für 2: " + CorrectionFactor.VARIANT_2);
        //Console.WriteLine("CirclePZ : V2: penalty: " + penalty);
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
        //Console.WriteLine("CirclePZ : V3A: PRE penalty: " + penalty);
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
        //Console.WriteLine("CirclePZ : V3B: PRE penalty: " + penalty);
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
        //Console.WriteLine("CirclePZ : V3C: PRE penalty: " + penalty);
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
        //Console.WriteLine("CirclePZ : V4A: PRE penalty: " + penalty);
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
            Console.WriteLine("PZ circle V4B: deltaDistance: " + deltaDistance + " deltaHeight: " + deltaHeight+ " summDeltaHeight: " + summDeltaHeight);
        }
        penalty = CoordinateHelpers.ConvertToFeet(summDeltaHeight);
        Console.WriteLine("CirclePZ : V4B: PRE penalty: " + penalty);
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
        //Console.WriteLine("CirclePZ : V5: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_5;
    }
}