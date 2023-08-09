using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._3.tasks;

public class Task6 : Task
{
    public Task6(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 6;
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
        
        Coordinate startPoint;
        Coordinate landingPoint;
        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out startPoint, out landingPoint);

        if (startPoint.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
        {
            comment += "Declared after takeoff | ";
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

        List<double> distanceToAllGoals = CalculationHelper.calculate2DDistanceToAllGoals(declaration.DeclaredGoal,
            goals.ToArray(),
            flight.getCalculationType());

        foreach (double distanceToGoal in distanceToAllGoals)
        {
            if (distanceToGoal < 1000)
            {
                comment += "Goal is to close to another goal | ";
            }
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < 1000)
        {
            comment += "Goal is to close to dec. point | ";
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
        return new DateTime(2023, 06, 10, 05, 00, 00);
    }
}