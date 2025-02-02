using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

public class MarkerDrop
{
    public required int MarkerNumber
    {
        get; init;
    }

    public required Coordinate MarkerLocation
    {
        get; init;
    }
}
