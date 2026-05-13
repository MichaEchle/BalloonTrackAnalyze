using Scoring.Competitions.Tracks;
using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Serves as the base class for two-dimensional geometric shapes.
/// </summary>
/// <remarks>
/// Derived classes must implement <see cref="IsCoordinateInside(Coordinate)"/>.
/// Distance calculations are provided by this base class using the result of the containment test.
/// All coordinates use WGS84 lat/lon; distances are computed in meters via Haversine.
/// </remarks>
public abstract class Shapes2D
{
    /// <summary>
    /// Determines whether the specified coordinate lies inside this shape.
    /// </summary>
    /// <param name="coordinate">The coordinate to test.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="coordinate"/> is inside or on the boundary;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public abstract bool IsCoordinateInside(Coordinate coordinate);

    /// <summary>
    /// Calculates the total 2D distance traveled by the track while inside this shape.
    /// </summary>
    /// <param name="track">The track to measure.</param>
    /// <param name="isReentranceAllowed">
    /// If <see langword="true"/>, calculates distance for all segments inside the shape (reentrance allowed).
    /// If <see langword="false"/>, calculates distance only up to the first exit point.
    /// </param>
    /// <returns>The cumulative distance in meters of track points within the shape.</returns>
    public virtual double Calculate2DDistanceWithIn(Track track, bool isReentranceAllowed)
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
            distance = CoordinateMath.Calculate2DDistanceBetweenPoints(pointsWithIn[0]);
        }
        else
        {
            for (int index = 0; index < pointsWithIn.Count; index++)
            {
                distance += CoordinateMath.Calculate2DDistanceBetweenPoints(pointsWithIn[index]);
            }
        }

        return distance;
    }
}
