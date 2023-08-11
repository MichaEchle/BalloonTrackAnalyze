using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._03.tasks;

public class Task11 : Task
{
    public Task11(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 11;
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

        if (allDeclerations.Count > 3)
        {
            comment += $"To many Declarations ({allDeclerations.Count}) | ";
        }

        if (!track.GetAllMarkerNumbers().Contains(3))
        {
            return new[] { "No Result", "No Marker in 3" };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 3);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 3 | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 3" };
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

        if (declaration.PositionAtDeclaration.utmZone.ToLower().Equals("31n"))
        {
            comment += "Position at Declaration is 0 / 0 | ";
        }

        List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(declaration.DeclaredGoal,
            goals.ToArray(),
            flight.getCalculationType());
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

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 08, 10, 20, 49, 00);
    }
}