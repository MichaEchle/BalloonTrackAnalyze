using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl;

public class Task04 : Task
{
    public Task04(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 04;
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

        track.AdditionalPropertiesFromScoring.TryGetValue("lastTrackPoint", out object oLastTrackPoint);

        if (oLastTrackPoint != null)
        {
            Coordinate lastTrackPoint = (Coordinate)oLastTrackPoint;
            double distanceBetweenDeclerationPointAndFirstTrackPointInXDT =
                CalculationHelper.Calculate2DDistance(lastTrackPoint, decleration.DeclaredGoal, flight.getCalculationType());
            if (distanceBetweenDeclerationPointAndFirstTrackPointInXDT <= 1000)
            {
                comment +=
                    $"Goal to close to xdt entry. [Is {distanceBetweenDeclerationPointAndFirstTrackPointInXDT}m]";
            }
        }
        else
        {
            comment += "Could not check for 1km distance: No First trackpoint | ";
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 6);

        if (markerDrop == null)
        {
            foreach (MarkerDrop existingDrop in track.MarkerDrops)
            {
                comment += $"Found markerdrop {existingDrop.MarkerNumber} | ";
            }
            return new[] { "No Result", "No MarkerDrop in 6" };
        }
        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop #6 outside SP | ";
        }

        if (markerDrop.MarkerTime.CompareTo(decleration.PositionAtDeclaration.TimeStamp) <= 0)
        {
            comment += "Dropped marker before declaring. | ";
            return new[] { "No Result", comment };
        }

        double distance =
            CoordinateHelpers.Calculate3DDistance(decleration.DeclaredGoal, markerDrop.MarkerLocation, flight.useGPSAltitude(), flight.getCalculationType());

        comment += $"Dec Alt.: {decleration.DeclaredGoal.AltitudeBarometric} | Marker Alt.: {markerDrop.MarkerLocation.AltitudeBarometric} | ";
        return new[] { NumberHelper.formatDoubleToStringAndRound(distance), comment };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2024, 03, 01, 13, 00, 00);
    }
}