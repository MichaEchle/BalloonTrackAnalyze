using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights.impl._02.tasks;

public class Task06 : Task
{
    public Task06(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 06;
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

        List<Declaration> allDeclerations = track.Declarations.FindAll(declaration => declaration.GoalNumber == 1);

        if (allDeclerations.Count > 4)
        {
            comment += $"To many Declarations ({allDeclerations.Count}) | ";
        }

        if (!track.GetAllMarkerNumbers().Contains(4))
        {
            return new[] { "No Result", "No Marker in 4" };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 4);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 4 | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 4" };
        }


        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            goals.AddRange(task.goals());
        }

        foreach (Declaration trackDeclaration in track.Declarations)
        {
            if (trackDeclaration.GoalNumber == 3)
            {
                continue;
            }

            goals.Add(trackDeclaration.DeclaredGoal);
        }

        List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(declaration.DeclaredGoal,
            goals.ToArray(),
            flight.getCalculationType());

        if (distanceToAllGoals.FindLast(d => d > 1000) != null)
        {
            comment += "Declared goal is to close to another Goal | ";
        }

        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out Coordinate launchPoint,
            out Coordinate landingPoint);
        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, launchPoint, flight.getCalculationType()) <
            100)
        {
            comment += "Goal is to close to launch | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < 1500)
        {
            comment += "Goal is to close to dec. point | ";
        }


        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation,
            flight.useGPSAltitude(), flight.getCalculationType());

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 10, 07, 30, 00);
    }
}