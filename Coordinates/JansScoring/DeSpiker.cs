using Coordinates;
using JansScoring.calculation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace JansScoring;

public class DeSpiker
{
    public static int despike(Track track, bool useGPSAltitude)
    {
        List<int> removal = new List<int>();


        ParallelLoopResult parallelLoopResult = Parallel.For(0, track.TrackPoints.Count - 2, i =>
        {
            i = track.TrackPoints.Count - 1 - i;
            Coordinate currentPoint = track.TrackPoints[i];
            Coordinate nextPoint = track.TrackPoints[i - 1];

            double distance2D =
                CoordinateHelpers.Calculate3DDistance(currentPoint, nextPoint, useGPSAltitude,
                    CalculationType.UTMPrecise);
            if (distance2D > 1000)
            {
                Coordinate checkPoint;
                if (i <= 1)
                {
                    checkPoint = track.TrackPoints[i + 1];
                }
                else
                {
                    checkPoint = track.TrackPoints[i - 2];
                }

                double distanceCheckpointToCurrentPoint = CoordinateHelpers.Calculate3DDistance(currentPoint,
                    checkPoint, useGPSAltitude, CalculationType.UTMPrecise);
                double distanceCheckpointToNextPoint =
                    CoordinateHelpers.Calculate3DDistance(currentPoint, checkPoint, useGPSAltitude,
                        CalculationType.UTMPrecise);
                if (distanceCheckpointToCurrentPoint < distanceCheckpointToNextPoint)
                {
                    if (!removal.Contains(i - 1))
                        removal.Add(i - 1);
                }
                else
                {
                    if (!removal.Contains(i))
                        removal.Add(i);
                }
            }
        });

        while (!parallelLoopResult.IsCompleted)
        {
            Thread.Sleep(2);
        }


        foreach (int i in removal)
        {
            track.TrackPoints.RemoveAt(i);
        }


        return removal.Count;
    }
}