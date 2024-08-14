using Coordinates;
using JansScoring.calculation;
using LoggerComponent;
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
        if (declaration == null)
        {
            return new[] { "No Result", "No Declaration in 1" };
        }

        if (declaration.DeclaredGoal == null)
        {
            Log(LogSeverityType.Error, declaration.ToString());
            return new[] { "No Result", "No valid Declaration in 1 (No Declared Position) | " };
        }


        Coordinate declarationPosition = declaration.PositionAtDeclaration;
        if (declarationPosition == null)
        {
            comment += "No valid Declaration in 1 (No Decleration Position) | ";
        }

        if (declarationPosition == null || declarationPosition.Latitude == 0 || declarationPosition.Longitude == 0)
        {
            comment += "Resetting Declaration Position to Backup Point because Long or Lat was null or 0 | ";
            declarationPosition = flight.getBackupCoordinates();
        }

        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out Coordinate startPoint,
            out Coordinate landingPoint);


        if (!track.GetAllMarkerNumbers().Contains(1))
        {
            if (landingPoint == null)
            {
                return new[] { "No Result", "No Marker drops at slot 1 | " };
            }
        }

        MarkerDrop markerDrop = track.MarkerDrops.FindLast(drop => drop.MarkerNumber == 1);
        if (markerDrop == null)
        {
            if (landingPoint == null)
            {
                return new[] { "No Result", "No Marker drops at slot 1 | " };
            }
            else
            {
                comment += "No Marker drops at slot 1. Using landing position | ";
            }
        }

        if (markerDrop != null && markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot 1" };
        }

        Coordinate markerLocation = null;

        if (markerDrop == null)
        {
            markerLocation = landingPoint;
        }
        else
        {
            markerLocation = markerDrop.MarkerLocation;
        }


        if (startPoint.TimeStamp < declarationPosition.TimeStamp)
        {
            comment += "Declared after takeoff | ";
        }

        if (flight.getStartOfLaunchPeriode() < declarationPosition.TimeStamp)
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

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declarationPosition,
                flight.getCalculationType()) < 3000)
        {
            comment += "Goal is to close to dec. point | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declarationPosition,
                flight.getCalculationType()) > 6000)
        {
            comment += "Goal is to far frm dec. point | ";
        }

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerLocation,
            flight.useGPSAltitude(), flight.getCalculationType());

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public override Coordinate[] goals()
    {
        return Array.Empty<Coordinate>();
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 09, 18, 49, 00);
    }
}