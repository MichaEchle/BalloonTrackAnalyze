using Coordinates;
using JansScoring.calculation;
using PZTesting;


namespace JansScoring.pz_rework.type;

public class CirclePZ : PZ
{
    private Coordinate centerCoordinate;
    private double height;
    private int radius;

    public CirclePZ(Coordinate centerCoordinate, double height, int radius)
    {
        this.centerCoordinate = centerCoordinate;
        this.height = height;
        this.radius = radius;
    }

    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double infringement)
    {
        double disctanceBetweenRedPZ =
            CalculationHelper.Calculate2DDistance(coordinate, centerCoordinate, CalculationType.UTM);

        if (disctanceBetweenRedPZ <= radius &&
            (useGPSAltitude ? coordinate.AltitudeGPS : coordinate.AltitudeBarometric) <=
            height)
        {
            infringement = radius - disctanceBetweenRedPZ;
            return true;
        }

        infringement = 0;
        return false;
    }
}