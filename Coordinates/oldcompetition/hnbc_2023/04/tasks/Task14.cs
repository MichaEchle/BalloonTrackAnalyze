using Coordinates;
using JansScoring.calculation;
using System;
using Windows.UI.ViewManagement.Core;

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

        MarkerDrop markerA = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);
        if (markerA == null)
        {
            return new[] { "No Result", "No Marker 3 found." };
        }

        (string utmZoneM5, double eastingM5, double northingM5) =
            CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerA.MarkerLocation);
        MarkerDrop markerB = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);
        if (markerB == null)
        {
            return new[] { "No Result", "No Marker 4 found." };
        }


        double distanceAB = CalculationHelper.Calculate2DDistance(markerA.MarkerLocation,
            markerB.MarkerLocation, flight.getCalculationType());
        if (distanceAB < 2000 || distanceAB > 3000)
        {
            return new[] { "No Result", $"Distance not valid. {distanceAB}m" };
        }


        (string utmZoneM6, double eastingM6, double northingM6) =
            CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(markerB.MarkerLocation);

        double degree = 45;

        Coordinate A = markerA.MarkerLocation;
        Coordinate B = markerB.MarkerLocation;
        Coordinate C = new Coordinate(markerA.MarkerLocation.Latitude, markerB.MarkerLocation.Longitude, 0, 0,
            new DateTime());
        double a = CalculationHelper.Calculate2DDistance(B, C, flight.getCalculationType());
        double b = CalculationHelper.Calculate2DDistance(C, A, flight.getCalculationType());
        double c = CalculationHelper.Calculate2DDistance(A, C, flight.getCalculationType());

        double tanGamma = Math.Atan(b / a) * (180 / PI);
        double alpha = 180 - (180 - 90 - tanGamma);

        if (A.Longitude > B.Longitude)
        {
            alpha += 180;
        }

        double angle = Math.Abs(degree - alpha);
        comment += $"Angle: {NumberHelper.formatDoubleToStringAndRound(alpha)} ";
        result = NumberHelper.formatDoubleToStringAndRound(angle);
        return new[] { result, comment };
    }

    private double PI =
        3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 11, 09, 00, 00);
    }
}