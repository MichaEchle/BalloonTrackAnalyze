using Scoring.Coordinates;

namespace Scoring.Competitions.Tracks;

public class Declaration
{
    public required int GoalNumber
    {
        get; init;
    }

    public required Coordinate DeclaredGoal
    {
        get; init;
    }

    public required Coordinate PositionAtDeclaration
    {
        get; init;
    }

    public required int OriginalEastingDeclarationUTM
    {
        get; init;
    }

    public required int OriginalNorthingDeclarationUTM
    {
        get; init;
    }

    public required bool HasPilotDeclaredGoalAltitude
    {
        get; init;
    }
}
