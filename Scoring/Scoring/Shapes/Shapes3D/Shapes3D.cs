using Scoring.Competitions.Tracks;
using Scoring.Coordinates;

namespace Scoring.Shapes;

public abstract class Shapes3D
{
    public abstract bool IsCoordinateInside(Coordinate coordinate);

    public virtual double Calculate3DDistanceWithin(Track track, bool isReentranceAllowed)
    {
        double distance = 0.0;
        List<List<Coordinate>> pointsWithIn = [[]];
        int count = 0;
        for (int index = 0; index < track.TrackPoints.Count; index++)
        {
            if (IsCoordinateInside(track.TrackPoints[index]))
            {
                count++;
                pointsWithIn.Last().Add(track.TrackPoints[index]);
            }
            else
            {
                if (isReentranceAllowed)
                {
                    pointsWithIn.Add([]);
                }
                else
                {
                    if (count > 0)
                    {
                        break;
                    }
                }

            }
        }
        if (!isReentranceAllowed)
        {
            distance = CoordinateMath.Calculate3DDistanceBetweenPoints(pointsWithIn[0]);
        }
        else
        {
            for (int index = 0; index < pointsWithIn.Count; index++)
            {
                distance += CoordinateMath.Calculate3DDistanceBetweenPoints(pointsWithIn[index]);
            }
        }

        return distance;
    }

    protected virtual bool IsWithinAltitudeBoundary(Coordinate coordinate, double lowerBoundary, double upperBoundary)
    {
        bool isWithin = true;
        if (!double.IsNaN(lowerBoundary))
        {
            isWithin &= coordinate.Altitude >= lowerBoundary;
        }

        if (!double.IsNaN(upperBoundary))
        {
            isWithin &= coordinate.Altitude <= upperBoundary;
        }

        return isWithin;

    }
}
