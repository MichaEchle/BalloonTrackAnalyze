using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapes
{
    public abstract class Shapes3D
    {
        public abstract bool IsWithin(Coordinate coordinate, bool useGPSAltitude, CalculationType calculationType);

        public virtual double Calculate3DDistanceWithin(Track track, bool useGPSAltitude, bool isReentranceAllowed,
            CalculationType calculationType)
        {
            double distance = 0.0;
            List<List<Coordinate>> pointsWithIn = new List<List<Coordinate>>();
            pointsWithIn.Add(new List<Coordinate>());
            int count = 0;
            for (int index = 0; index < track.TrackPoints.Count; index++)
            {
                if (IsWithin(track.TrackPoints[index], useGPSAltitude, calculationType))
                {
                    count++;
                    pointsWithIn.Last().Add(track.TrackPoints[index]);
                }
                else
                {
                    if (isReentranceAllowed)
                        pointsWithIn.Add(new List<Coordinate>());
                    else
                    {
                        if (count > 0)
                            break;
                    }
                }
            }

            if (!isReentranceAllowed)
                distance = CoordinateHelpers.Calculate3DDistanceBetweenPoints(pointsWithIn[0], useGPSAltitude,
                    calculationType);
            else
            {
                for (int index = 0; index < pointsWithIn.Count; index++)
                {
                    distance += CoordinateHelpers.Calculate3DDistanceBetweenPoints(pointsWithIn[index], useGPSAltitude,
                        calculationType);
                }
            }

            return distance;
        }
    }
}