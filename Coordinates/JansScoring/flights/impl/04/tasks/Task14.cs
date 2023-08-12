using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._04.tasks;

public class Task14 : Task
{
    public Task14(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 14;
    }

    public override string[] score(Track track)
    {
        string result = "";
        string comment = "";

        MarkerDrop markerDrop3 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);
        if (markerDrop3 == null)
        {
            return new[] { "No Result", "No Marker 3 found." };
        }

        (string utmZoneM5, double eastingM5, double northingM5) =
            CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerDrop3.MarkerLocation);
        MarkerDrop markerDrop4 = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);
        if (markerDrop4 == null)
        {
            return new[] { "No Result", "No Marker 4 found." };
        }


        double distanceAB = CalculationHelper.Calculate2DDistance(markerDrop3.MarkerLocation,
            markerDrop4.MarkerLocation, flight.getCalculationType());
        if (distanceAB < 2000 || distanceAB > 3000)
        {
            return new[] { "No Result", $"Distance not valid. {distanceAB}m" };
        }


        (string utmZoneM6, double eastingM6, double northingM6) =
            CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerDrop4.MarkerLocation);

        double degree = 45;

        double trieAngleBottom = Math.Abs(eastingM5 - eastingM6);
        double trieAngleWall = Math.Abs(northingM5 - northingM6);
        double alpha = Math.Atan2(trieAngleBottom, trieAngleWall) * 180 / PI;
        double beta = degree - alpha;
        if (beta > 180)
        {
            beta = (360 - degree) + alpha;
        }

        comment +=
            $"Angle Bottom: {NumberHelper.formatDoubleToStringAndRound(trieAngleBottom)} | Angle Wall {NumberHelper.formatDoubleToStringAndRound(trieAngleWall)} | Alpha: {NumberHelper.formatDoubleToStringAndRound(alpha)} | Beta: {NumberHelper.formatDoubleToStringAndRound(beta)} | ";
        result = NumberHelper.formatDoubleToStringAndRound(Math.Abs(beta));
        return new[] { result, comment };
    }

    private double PI =
        3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 08, 11, 09, 00, 00);
    }
}