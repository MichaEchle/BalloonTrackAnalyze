using Coordinates;
using JansScoring.calculation;
using System;
using System.Linq;

namespace JansScoring.flights.impl._02.tasks;

public class Task08 : Task
{
    public Task08(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 08;
    }

    public override string[] score(Track track)
    {
        string result = "";
        string comment = "";
        if (track.Declarations.FindAll(declaration => declaration.GoalNumber == 1).Count > 1)
        {
            comment += "Multiple Goal declarations using latest. | ";
        }

        Declaration decleration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);
        if (decleration == null)
        {
            foreach (Declaration trackDeclaration in track.Declarations)
            {
                comment += $"Found decleration {trackDeclaration.GoalNumber} | ";
            }

            return new[] { "No Result", "No Declaration in 1" };
        }

        Coordinate[] goals = flight.getTaskByNumber(7).goals();
        double distanceToNearestGoal = CalculationHelper
            .calculate2DDistanceToAllGoals(decleration.DeclaredGoal, goals, flight.getCalculationType()).Min();
        if (distanceToNearestGoal < 1000)
        {
            comment +=
                $"Goal to close to #07 goals. [Is {distanceToNearestGoal}m] | ";
        }

        double altitudeDiffrenceDeclaration = decleration.DeclaredGoal.AltitudeBarometric -
                                              decleration.PositionAtDeclaration.AltitudeBarometric;
        if (altitudeDiffrenceDeclaration < CoordinateHelpers.ConvertToMeter(500) &&
            altitudeDiffrenceDeclaration > CoordinateHelpers.ConvertToMeter(-500))
        {
            comment +=
                $"declared goal to low/heigh from decleration point. [Is {altitudeDiffrenceDeclaration}m, Should {CoordinateHelpers.ConvertToMeter(500)}]";
        }


        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);

        if (markerDrop == null)
        {
            foreach (MarkerDrop existingDrop in track.MarkerDrops)
            {
                comment += $"Found markerdrop {existingDrop.MarkerNumber} | ";
            }

            return new[] { "No Result", "No MarkerDrop in 2" };
        }

        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop #2 outside SP | ";
        }

        if (markerDrop.MarkerTime < decleration.PositionAtDeclaration.TimeStamp)
        {
            comment += "Dropped marker before declaring. | ";
            return new[] { "No Result", comment };
        }

        double distance =
            CoordinateHelpers.Calculate3DDistance(decleration.DeclaredGoal, markerDrop.MarkerLocation,
                flight.useGPSAltitude(), flight.getCalculationType());

        return new[]
        {
            NumberHelper.formatDoubleToStringAndRound(distance).Replace(",", "."), comment.Replace(",", ".")
        };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 02, 08, 00, 00);
    }
}