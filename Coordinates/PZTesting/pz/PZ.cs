using Coordinates;

namespace PZTesting;

public interface PZ
{
    public bool IsInsidePz(bool useGPSAltitude, Coordinate coordinate, out double infringement);
}