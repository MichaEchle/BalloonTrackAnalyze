using Scoring.Coordinates;

namespace Scoring.Shapes;

/// <summary>
/// Represents a polygon (closed shape with 3 or more vertices) in 2D space.
/// </summary>
/// <remarks>
/// Containment uses the winding number algorithm. Area is calculated via the shoelace formula.
/// Vertices are stored in order (clockwise or counterclockwise).
/// </remarks>
public class Polygon : Shapes2D
{
    /// <summary>
    /// Gets the list of vertices defining this polygon.
    /// </summary>
    /// <value>An ordered list of <see cref="Coordinate"/> vertices.</value>
    public required IList<Coordinate> PolygonPoints
    {
        get; init;
    }

    /// <summary>
    /// Determines the orientation of three points (cross product on 2D plane).
    /// </summary>
    /// <param name="coordinate1">The first point.</param>
    /// <param name="coordinate2">The second point.</param>
    /// <param name="coordinate3">The third point.</param>
    /// <returns>
    /// A value &gt; 0 if point 3 is left of the line from point 1 to point 2,
    /// &lt; 0 if right, and 0 if collinear.
    /// </returns>
    private static double IsLeft(Coordinate coordinate1, Coordinate coordinate2, Coordinate coordinate3)
    {
        return (coordinate2.Longitude - coordinate1.Longitude) * (coordinate3.Latitude - coordinate1.Latitude) - (coordinate3.Longitude - coordinate1.Longitude) * (coordinate2.Latitude - coordinate1.Latitude);
    }

    /// <summary>
    /// Calculates the winding number of the specified coordinate relative to this polygon.
    /// </summary>
    /// <param name="coordinate">The coordinate to test.</param>
    /// <returns>
    /// A non-zero winding number indicates the coordinate is inside the polygon.
    /// The sign and magnitude depend on the winding order and containment depth.
    /// </returns>
    private int CalculateWindingNumber(Coordinate coordinate)
    {
        int windingNumber = 0;
        for (int index = 0; index < PolygonPoints.Count; index++)
        {
            if (GetPolygonPointWrappedAround(index).Latitude <= coordinate.Latitude)
            {
                if (GetPolygonPointWrappedAround(index + 1).Latitude > coordinate.Latitude)
                {
                    if (IsLeft(GetPolygonPointWrappedAround(index), GetPolygonPointWrappedAround(index + 1), coordinate) > 0)
                    {
                        windingNumber++;
                    }
                }
            }
            else
            {
                if (GetPolygonPointWrappedAround(index + 1).Latitude <= coordinate.Latitude)
                {
                    if (IsLeft(GetPolygonPointWrappedAround(index), GetPolygonPointWrappedAround(index + 1), coordinate) < 0)
                    {
                        windingNumber--;
                    }
                }
            }
        }
        return windingNumber;
    }

    /// <summary>
    /// Gets the polygon vertex at the specified index, wrapping around if necessary.
    /// </summary>
    /// <param name="index">The index (may exceed polygon vertex count).</param>
    /// <returns>The vertex at index modulo the vertex count.</returns>
    private Coordinate GetPolygonPointWrappedAround(int index)
    {
        return PolygonPoints[index % PolygonPoints.Count];
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Uses the winding number algorithm for robust polygon containment testing.
    /// </remarks>
    override public bool IsCoordinateInside(Coordinate coordinate)
    {
        return (CalculateWindingNumber(coordinate) != 0);
    }

    /// <summary>
    /// Calculates the area of this polygon using the shoelace formula.
    /// </summary>
    /// <returns>The area in square degrees (approximately).</returns>
    public double CalculateArea()
    {
        double area = 0.0;
        for (int index = 0; index < PolygonPoints.Count; index++)
        {
            area += (GetPolygonPointWrappedAround(index).Longitude * GetPolygonPointWrappedAround(index + 1).Latitude) - (GetPolygonPointWrappedAround(index + 1).Longitude * GetPolygonPointWrappedAround(index).Latitude);
        }
        return Math.Abs(area) / 2;
    }
}