using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights.impl._07.tasks;

public class Task25 : Task
{
    public Task25(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 25;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        double result = 0;

         if (!track.GetAllGoalNumbers().Contains(3))
        {
            return new[] { "No Result", "No Declaration in 3" };
        }

        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 3);

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            return new[] { "No Result", "No valid Declaration in 3" };
        }

        List<Declaration> allDeclerations = track.Declarations.FindAll(declaration => declaration.GoalNumber == 3);

        if (allDeclerations.Count > 2)
        {
            comment += $"To many Declarations ({allDeclerations.Count}) | ";
        }

        if (!track.GetAllMarkerNumbers().Contains(3))
        {
            return new[] { "No Result", "No Marker in 3" };
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 2);
        if (markerDrop == null)
        {
            return new[] { "No Result", "No Marker drops at slot 2 | " };
        }

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 2" };
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

        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out Coordinate launchPoint,
            out Coordinate landingPoint);

        double declaredGoalAltitude = flight.useGPSAltitude() ? declaration.DeclaredGoal.AltitudeGPS: declaration.DeclaredGoal.AltitudeBarometric;
        double declarationAltitude = flight.useGPSAltitude() ? declaration.PositionAtDeclaration.AltitudeGPS: declaration.PositionAtDeclaration.AltitudeBarometric;

        if ((declarationAltitude + 750 < declaredGoalAltitude) && (declarationAltitude - 750 < declaredGoalAltitude))
        {
            comment += "Goal is inside of ± 750m | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, launchPoint, flight.getCalculationType()) <
            2000)
        {
            comment += "Goal is to close to launch | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < 2000)
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
        return new DateTime(2023, 08, 12, 20, 48, 00);
    }
}