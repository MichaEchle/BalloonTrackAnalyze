using Scoring.Competitions.Tracks;
using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Serves as the base class for three-dimensional geometric shapes.
/// </summary>
/// <remarks>
/// 3D shapes extend 2D shapes with altitude (z-axis) boundaries.
/// A coordinate is inside if it passes both the 2D containment test and falls within altitude bounds.
/// Altitude bounds use <see cref="double.NaN"/> to represent "no limit" (infinite).
/// </remarks>
public abstract class Shapes3D
{
    /// <summary>
    /// Determines whether the specified coordinate (with altitude) lies inside this 3D shape.
    /// </summary>
    /// <param name="coordinate">The coordinate to test (altitude may be null).</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="coordinate"/> passes both 2D and altitude tests;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public abstract bool IsCoordinateInside(Coordinate coordinate);

    /// <summary>
    /// Calculates the total 3D distance traveled by the track while inside this shape.
    /// </summary>
    /// <param name="track">The track to measure.</param>
    /// <param name="isReentranceAllowed">
    /// If <see langword="true"/>, calculates distance for all segments inside the shape (reentrance allowed).
    /// If <see langword="false"/>, calculates distance only up to the first exit point.
    /// </param>
    /// <returns>The cumulative 3D distance in meters of track points within the shape.</returns>
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

    /// <summary>
    /// Determines whether the specified coordinate's altitude falls within the given boundaries.
    /// </summary>
    /// <param name="coordinate">The coordinate to test.</param>
    /// <param name="lowerBoundary">
    /// The lower altitude bound in meters, or <see cref="double.NaN"/> for no limit.
    /// </param>
    /// <param name="upperBoundary">
    /// The upper altitude bound in meters, or <see cref="double.NaN"/> for no limit.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the coordinate's altitude is within bounds;
    /// otherwise, <see langword="false"/>.
    /// </returns>
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
