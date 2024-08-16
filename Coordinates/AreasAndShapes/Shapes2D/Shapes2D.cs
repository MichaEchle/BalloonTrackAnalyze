using Coordinates;
using System.Collections.Generic;
using System.Linq;

namespace Shapes;

public abstract class Shapes2D
{
    public abstract bool IsWithin(Coordinate coordinate);

    public virtual double Calculate2DDistanceWithIn(Track track, bool isReentranceAllowed)
    {
        double distance = 0.0;
        List<List<Coordinate>> pointsWithIn = [[]];
        int count = 0;
        for (int index = 0; index < track.TrackPoints.Count; index++)
        {
            if (IsWithin(track.TrackPoints[index]))
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
            distance = CoordinateHelpers.Calculate2DDistanceBetweenPoints(pointsWithIn[0]);
        else
        {
            for (int index = 0; index < pointsWithIn.Count; index++)
            {
                distance += CoordinateHelpers.Calculate2DDistanceBetweenPoints(pointsWithIn[index]);
            }
        }

        return distance;
    }
}
