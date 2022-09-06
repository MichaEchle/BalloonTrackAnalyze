using Coordinates;

namespace JansScoring.pz;

public class PZ
{
    /// <summary>
    /// The ID of the PZ
    /// </summary>
    public int id;

    public PZType pzType;

    /// <summary>
    /// Only important, if not whole map
    /// </summary>
    public Coordinate center;

    /// <summary>
    /// Only important if Blue and whole Map
    /// </summary>
    public int height;

    /// <summary>
    /// Not work with a Blue PZ
    /// </summary>
    public int radius;

    public PZ(int id, PZType pzType, Coordinate center, int height, int radius)
    {
        this.id = id;
        this.pzType = pzType;
        this.center = center;
        this.height = height;
        this.radius = radius;
    }
}