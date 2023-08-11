using Coordinates;

namespace JansScoring.pz_rework;

public abstract class PZ
{
    /// <summary>
    /// The ID of the PZ
    /// </summary>
    private int id
    {
        get;
    }

    protected PZ(int id)
    {
        this.id = id;
    }

    public abstract bool IsInsidePz(Coordinate coordinate);
}