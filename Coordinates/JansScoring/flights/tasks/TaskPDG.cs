﻿using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;

namespace JansScoring.flights;

public abstract class TaskPDG : Task, ISingleMarkerTask , ISingleDeclarationTask
{
    protected TaskPDG(Flight flight) : base(flight)
    {
    }


    protected abstract int distanceToDeclerationPoint();
    protected abstract int distanceToOtherDeclerationPoints();
    protected abstract int maxDeclerations();
    protected abstract int minTimeInSecondsToDeclarationPoint();



    public override string[] score(Track track)
    {
        double result = -1;
        String comment = "";

        HandleMarker();

        if (markerDrop.MarkerLocation == null)
        {
            return new[] { "No Result", "No valid Marker in slot " + markerDropNumber() };
        }

        if (markerDrop.MarkerTime > getScoringPeriodUntil())
        {
            comment += "Markerdrop " + markerDropNumber() + " outside SP | ";
        }

        Coordinate startPoint;
        Coordinate landingPoint;
        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out startPoint, out landingPoint);

        if (startPoint.TimeStamp < declaration.PositionAtDeclaration.TimeStamp)
        {
            comment += "Declared after takeoff | ";
        }

        if (track.Declarations.Count > maxDeclerations())
        {
            comment += "To many Declarations | ";
        }

        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            goals.AddRange(task.goals());
        }

        foreach (Declaration trackDeclaration in track.Declarations)
        {
            if (trackDeclaration.GoalNumber == declerationNumber())
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
            if (distanceToGoal < distanceToOtherDeclerationPoints())
            {
                comment += "Goal is to close to another goal | ";
            }
        }
        if (declaration.PositionAtDeclaration.TimeStamp.AddSeconds(minTimeInSecondsToDeclarationPoint()) >
            markerDrop.MarkerTime)
        {
            TimeSpan timeSpan =
                declaration.PositionAtDeclaration.TimeStamp.AddSeconds(minTimeInSecondsToDeclarationPoint()) -
                markerDrop.MarkerTime;
            comment += $"Goal was declared to late for markerdrop. [{timeSpan.TotalSeconds}s to close] | ";
        }

        if (CalculationHelper.Calculate2DDistance(declaration.DeclaredGoal, declaration.PositionAtDeclaration,
                flight.getCalculationType()) < distanceToDeclerationPoint())
        {
            comment += "Goal is to close to dec. point | ";
        }

        result = CoordinateHelpers.Calculate3DDistance(declaration.DeclaredGoal, markerDrop.MarkerLocation,
            flight.useGPSAltitude(), flight.getCalculationType());

        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    public abstract int MarkerNumber();
    public abstract int DeclarationNumber();
}