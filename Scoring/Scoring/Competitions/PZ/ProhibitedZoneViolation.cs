using Scoring.Competitions.Tracks;
using Scoring.Coordinates;

namespace Scoring.Competitions.PZ;

internal class ProhibitedZoneViolation
{
    internal required List<Coordinate> CoordinatesInZone
    {
        get; init;
    }

    internal required TimeSpan TimeInZone
    {
        get; init;
    }

    internal required int Penalties
    {
        get; init;
    }

    internal required Track Track
    {
        get; init;
    }

    internal required ProhibitedZone PZ
    {
        get; init;
    }
}
