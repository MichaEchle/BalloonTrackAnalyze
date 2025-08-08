using Coordinates;

namespace Competition.Tasks;

public class PizzaTaskHelper
{
    public Coordinate centerLocation
    {
        get;
        private set;
    }

    public int numberOfSlices
    {
        get;
        private set;
    }

    public double startAngleDegree
    {
        get;
        private set;
    }


    public void SetupHelper(Coordinate centerLocation, int numberOfSlices, double startAngleDegree = 0.0)
    {
        this.centerLocation = centerLocation;
        this.numberOfSlices = numberOfSlices;
        this.startAngleDegree = startAngleDegree;

        if (numberOfSlices <= 0)
        {
            throw new ArgumentException("numberOfSlices must be greater than 0");
        }
    }

    /// <summary>
    /// Determines the slice number based on the provided coordinate.
    /// </summary>
    /// <param name="coordinate">The coordinate for which the slice number is to be calculated.</param>
    /// <returns>The slice number corresponding to the provided coordinate.</returns>
    public int GetSliceNumber(Coordinate coordinate)
    {
        double deltaLongitude = coordinate.Longitude - centerLocation.Longitude;
        double deltaLatitude = coordinate.Latitude - centerLocation.Latitude;

        double angleRadians = Math.Atan2(deltaLongitude, deltaLatitude);

        double angleDegrees = angleRadians * (180.0 / Math.PI);

        if (angleDegrees < 0)
        {
            angleDegrees += 360;
        }

        double effectiveAngleDegrees = angleDegrees - startAngleDegree;

        if (effectiveAngleDegrees < 0)
        {
            effectiveAngleDegrees += 360;
        }

        double degreesPerSlice = 360.0 / numberOfSlices;

        int slice = (int)Math.Floor(effectiveAngleDegrees / degreesPerSlice) + 1;
        return slice;
    }

    /// <summary>
    /// Calculates the minimum number of slices between two coordinates in a circular arrangement. It calculates the steps needed. e.g. from 2 to 3 it takes 1 step.
    /// </summary>
    /// <param name="coord1">The first coordinate.</param>
    /// <param name="coord2">The second coordinate.</param>
    /// <returns>The minimum number of slices between the two coordinates, or -1 if one of the coordinates is invalid.</returns>
    public int GetSlicesBetweenCoordinates(Coordinate coord1, Coordinate coord2)
    {
        int slice1 = GetSliceNumber(coord1);
        int slice2 = GetSliceNumber(coord2);

        if (slice1 == -1 || slice2 == -1)
        {
            return -1;
        }

        if (slice1 == slice2)
        {
            return 0;
        }

        int clockwise = slice2 - slice1;
        if (clockwise < 0)
        {
            clockwise += numberOfSlices; 
        }

        int counterClockwise = slice1 - slice2;
        if (counterClockwise < 0)
        {
            counterClockwise += numberOfSlices; 
        }

        return Math.Min(clockwise, counterClockwise);
    }
}