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
    private int ctr = 0;

    public CirclePZ(Coordinate centerCoordinate, double height, int radius)
    {
        this.centerCoordinate = centerCoordinate;
        this.height = height;
        this.radius = radius;
    }

    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double horizontalInfringement,
        out double verticalInfringement)
    {
        double distanceBetweenRedPa =
            CalculationHelper.Calculate2DDistance(coordinate, centerCoordinate, CalculationType.UTM);

        if (distanceBetweenRedPa <= radius &&
            (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) <=
            height)
        {
            horizontalInfringement = radius - distanceBetweenRedPa;
            //verticalInfringement = (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) - height;
            verticalInfringement = (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) - height;

            //Console.WriteLine("CirclePZ :im PZ # "+ ctr + " infringement horizontal: " + infringement +  " infringement altitude: " + infringementAltitude);

            //ctr++;

            return true;
        }

        horizontalInfringement = 0;
        verticalInfringement = 0;
        return false;
    }

    public void CalculatePenaltyVariant1(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation of current COH rule R7.5
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

        horizontalInfringement = distanceHorizontal;
        verticalInfringement = averageHeightDifference;

        var percentage = (verticalPercentage + horizontalPercentage) / 2;
        penalty = (percentage / 100) * 500;
        //Console.WriteLine("CirclePZ : V1: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_1;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant2A(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        double sumDeltaHeight = 0;
        //double deltaHeight = 0;
        //double deltaHeight;
        verticalInfringement = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            //deltaHeight = useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            sumDeltaHeight += Math.Abs(deltaHeight);
            //Console.WriteLine("CirclePZ : V2: deltaHeight: "+ deltaHeight + " summDeltaHeight: " + sumDeltaHeight);
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V2A: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_2a;
        penalty = Math.Abs(penalty);
        //Console.WriteLine("CorrectionFactor für 2: " + CorrectionFactor.VARIANT_2);
        //Console.WriteLine("CirclePZ : V2A: penalty: " + penalty);
        horizontalInfringement = 0;
    }

    public void CalculatePenaltyVariant2B(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction is squared, overall divided by 10.000
        double sumDeltaHeight = 0;
        verticalInfringement = 0;

        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            sumDeltaHeight += Math.Abs(deltaHeight * deltaHeight);

            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        horizontalInfringement = 0;
        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V2B: PRE penalty: " + penalty);

        penalty *= CorrectionFactor.VARIANT_2b;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant2C(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction is doubled , overall divided by 100
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            sumDeltaHeight += Math.Abs(deltaHeight * 2);
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        horizontalInfringement = 0;
        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V2C: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_2c;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant2D(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //implementation analog to current COH rule R7.5 of blue PZ, but infraction with weighting of 20%, overall divided by 100
        double sumDeltaHeight = 0;
        verticalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            sumDeltaHeight += Math.Abs(deltaHeight * 1.2);
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V2D: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_2d;
        penalty = Math.Abs(penalty);
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
            double heightBetween = Math.Abs(height - altCurr);
//            double heightBetween = Math.Abs(altCurr - altPrev);
            horizontalInfringement += Math.Abs(distanceBetween);
            verticalInfringement += Math.Abs(heightBetween);
            previousCoordinate = trackpoint;
            horizontalFactor = horizontalInfringement / (radius*2);
            verticalFactor = verticalInfringement / height;
            /*Console.WriteLine("Polygon 3A: horizontalInfringement: " + horizontalInfringement + " horizontalFactor: " +
                              horizontalFactor + " verticalInfringement: " + verticalInfringement +
                              " verticalFactor: " + verticalFactor);*/
        }

        //penalty = (verticalFactor + horizontalFactor) / 2 * 0.1;
        penalty = (verticalFactor + horizontalFactor) / 2; // * 0.1;
        penalty *= CorrectionFactor.VARIANT_3a;
        //penalty = Math.Abs(penalty) * 1000;
        penalty = Math.Abs(penalty) * 100;
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
            double heightBetween = Math.Abs(height - altCurr);
//            double heightBetween = Math.Abs(altCurr - altPrev);
            horizontalInfringement += Math.Abs(distanceBetween);
            verticalInfringement += Math.Abs(heightBetween);
            previousCoordinate = trackpoint;


            horizontalFactor = horizontalInfringement / (radius *2);
            // Durchschnitt aller Höhenverletzungen im PZ

            // verticalFactor = Durchschnitt / PZ ceiling
            pointsCalculated++;
            verticalFactor = verticalInfringement / pointsCalculated;
            //verticalFactor = verticalInfringement / _maxHeight;

            /*Console.WriteLine("Circle 3B: horizontalInfringement: " + horizontalInfringement + " horizontalFactor: " +
                          horizontalFactor + " verticalInfringement: " + verticalInfringement + " verticalFactor: " +
                          verticalFactor); */
        }

        //penalty = (verticalFactor + horizontalFactor) / 2 * 0.01;
        penalty = (verticalFactor + horizontalFactor) / 2; // * 0.01;
        penalty *= CorrectionFactor.VARIANT_3b;
        //penalty = Math.Abs(penalty) * 500;
        penalty = Math.Abs(penalty) * 5;
        verticalInfringement = verticalFactor;
        //Console.WriteLine("PolyGon V3B: Penalty: " + penalty);
    }
    public void CalculatePenaltyVariant4A(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 2D distance between the track point and the virtual center, overall divided by 10
        double sumDeltaHeight = 0;

        verticalInfringement = 0;
        horizontalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaDistance =
                CalculationHelper.Calculate2DDistance(trackpoint, centerCoordinate, CalculationType.UTM);
            sumDeltaHeight += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }

            if (Math.Abs(deltaDistance) > Math.Abs(horizontalInfringement))
            {
                horizontalInfringement = Math.Abs(deltaDistance);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V4A: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_4a;
        penalty = Math.Abs(penalty);
    }

    public void CalculatePenaltyVariant4B(bool useGPSAltitude, List<Coordinate> pointsInPz, out double penalty,
        out double horizontalInfringement, out double verticalInfringement)
    {
        //ratio of the vertical infraction to the 3D distance between the track point and the virtual center, overall divided by 1
        double sumDeltaHeight = 0;

        verticalInfringement = 0;
        horizontalInfringement = 0;
        foreach (Coordinate trackpoint in pointsInPz)
        {
            double deltaHeight =
                useGPSAltitude ? trackpoint.AltitudeGPS - height : trackpoint.AltitudeBarometric - height;
            double deltaDistance =
                CoordinateHelpers.Calculate3DDistance(trackpoint, centerCoordinate, useGPSAltitude,
                    CalculationType.UTM);
            sumDeltaHeight += (Math.Abs(deltaHeight) / Math.Abs(deltaDistance));
            /*Console.WriteLine("PZ circle V4B: deltaDistance: " + deltaDistance + " deltaHeight: " + deltaHeight +
                              " summDeltaHeight: " + sumDeltaHeight); */
            if (Math.Abs(deltaHeight) > Math.Abs(verticalInfringement))
            {
                verticalInfringement = Math.Abs(deltaHeight);
            }

            if (Math.Abs(deltaDistance) > Math.Abs(horizontalInfringement))
            {
                horizontalInfringement = Math.Abs(deltaDistance);
            }
        }

        penalty = CoordinateHelpers.ConvertToFeet(sumDeltaHeight);
        //Console.WriteLine("CirclePZ : V4B: PRE penalty: " + penalty);
        penalty *= CorrectionFactor.VARIANT_4b;
        penalty = Math.Abs(penalty);
    }


}