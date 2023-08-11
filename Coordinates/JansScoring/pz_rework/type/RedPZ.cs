using Coordinates;

namespace JansScoring.pz_rework.type;

public class RedPZ : PZ
{
    public RedPZ(int id) : base(id)
    {
    }

    public override bool IsInsidePz(Coordinate coordinate)
    {
        throw new System.NotImplementedException();
    }
}