using Coordinates;

namespace PZTesting;

public interface IPz
{
    //public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double horizontalInfringement, out double verticalInfringement);
    public bool IsInsidePz(bool useGpsAltitude, Coordinate coordinate, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant1(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant2A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant2B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant2C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant2D(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant3A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant3B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    //public void CalculatePenaltyVariant3C(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant4A(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
    public void CalculatePenaltyVariant4B(bool useGpsAltitude, List<Coordinate> pointsInPz, out double penalty, out double horizontalInfringement, out double verticalInfringement);
   
}