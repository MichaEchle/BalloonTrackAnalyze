using Coordinates;
using System.Collections.Generic;
using System.Linq;

namespace Shapes;

public abstract class Shapes3D
{
    public abstract bool IsWithin(Coordinate coordinate, bool useGPSAltitude);

    public virtual double Calculate3DDistanceWithin(Track track, bool useGPSAltitude, bool isReentranceAllowed)
    {
        double distance = 0.0;
        List<List<Coordinate>> pointsWithIn = [[]];
        int count = 0;
        for (int index = 0; index < track.TrackPoints.Count; index++)
        {
            if (IsWithin(track.TrackPoints[index], useGPSAltitude))
            {
                count++;
                pointsWithIn.Last().Add(track.TrackPoints[index]);
            }
            else
            {
                if (isReentranceAllowed)
                    pointsWithIn.Add([]);
                else
                {
                    if (count > 0)
                        break;
                }

            }
        }
        if (!isReentranceAllowed)
            distance = CoordinateHelpers.Calculate3DDistanceBetweenPoints(pointsWithIn[0], useGPSAltitude);
        else
        {
            for (int index = 0; index < pointsWithIn.Count; index++)
            {
                distance += CoordinateHelpers.Calculate3DDistanceBetweenPoints(pointsWithIn[index], useGPSAltitude);
            }
        }

        return distance;
    }

    protected virtual bool IsWithinAltitudeBoundary(Coordinate coordinate, bool useGPSAltitude, double lowerBoundary, double upperBoundary)
    {
        double altitude = coordinate.AltitudeGPS;
        if (!useGPSAltitude)
            altitude = coordinate.AltitudeBarometric;

        bool isWithin = true;
        if (!double.IsNaN(lowerBoundary))
            isWithin &= altitude >= lowerBoundary;
        if (!double.IsNaN(upperBoundary))
            isWithin &= altitude <= upperBoundary;
        return isWithin;

    }
}
