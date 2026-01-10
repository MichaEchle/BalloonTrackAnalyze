using Coordinates;

namespace PZTesting;

public interface IPz
{
    public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double infringement);
    public void CalculatePenaltyVariant1(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant2(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant3A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant3B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant3C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant4A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant4B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);
    public void CalculatePenaltyVariant5(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty);

}