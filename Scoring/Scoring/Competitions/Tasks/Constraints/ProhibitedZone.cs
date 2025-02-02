using Scoring.Competitions.Tracks;
using Scoring.Coordinates;
using Scoring.Shapes;

namespace Scoring.Competitions.Tasks.Constraints;

public enum ProhibitedZoneType
{
    Blue,
    Yellow,
    Red,
}

public class ProhibitedZone
{
    public bool IsActive
    {
        get; set;
    } = true;

    public required string Identifier
    {
        get; init;
    }

    public required ProhibitedZoneType TypeOfZone
    {
        get; init;
    }

    public required Shapes3D ZoneDefinition
    {
        get; init;
    }


}
