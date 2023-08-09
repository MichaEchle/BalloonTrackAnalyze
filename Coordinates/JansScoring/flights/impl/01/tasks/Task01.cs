using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._01.tasks;

public class Task01 : Task
{
    public Task01(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 1;
    }

    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";

        if (!track.GetAllGoalNumbers().Contains(1))
        {
            return new[] { "No Result", "No Declaration in 1" };
        }

        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            return new[] { "No Result", "No valid Declaration in 1" };
        }

        if (!track.GetAllMarkerNumbers().Contains(1))
        {
            return new[] { "No Result", "No Marker in 1" };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 1 | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 1" };
        }

        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out Coordinate startPoint,
            out Coordinate _);

        if (startPoint.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
        {
            comment += "Declared after takeoff | ";
        }

        if (flight.getStartOfLaunchPeriode() < declaration.PositionAtDeclaration.TimeStamp)
        {
            comment += "Declared after Green Flag | ";
        }

        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            goals.AddRange(task.goals());
        }

        foreach (Declaration trackDeclaration in track.Declarations)
        {
            if (trackDeclaration.GoalNumber == 1)
            {
                continue;
            }

            goals.Add(trackDeclaration.DeclaredGoal);
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < 3000)
        {
            comment += "Goal is to close to dec. point | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) > 6000)
        {
            comment += "Goal is to far frm dec. point | ";
        }

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation,
            flight.useGPSAltitude(), flight.getCalculationType());

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 08, 09, 18, 49, 00);
    }
}