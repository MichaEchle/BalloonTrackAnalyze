using Coordinates;
using JansScoring.calculation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public abstract class TaskPDDounat : Task
{
    protected TaskPDDounat(Flight flight) : base(flight)
    {
    }

    protected abstract int declerationNumber();
    protected abstract int distanceFromDeclerationPointToCenter();
    protected abstract int innerCircle();
    protected abstract int outerCircle();
    protected abstract bool reenter();
    protected abstract int bottomPlateHeightM();
    protected abstract int maxDeclerations();


    public override string[] score(Track track)
    {
        List<int> allGoalDeclerationNumber = track.GetAllGoalNumbers();
        if (allGoalDeclerationNumber == null || !allGoalDeclerationNumber.Any())
        {
            return new[] { "No Result", "No Decleration | " };
        }

        Declaration declaration = track.Declarations.FindLast(drop => drop.GoalNumber == declerationNumber());

        if (declaration == null)
        {
            String data = "Data:";
            foreach (Declaration trackDeclaration in track.Declarations)
            {
                data += " || " + trackDeclaration.ToString();
            }

            return new[]
            {
                "No Result",
                $"No Declaration at slot {declerationNumber()}. Available ({StringHelper.IntListToString(track.GetAllGoalNumbers())}) | " +
                data + " | "
            };
        }

        Coordinate declarationPosition = declaration.PositionAtDeclaration;
        if (declarationPosition == null || declaration.DeclaredGoal == null)
        {
            return new[] { "No Result", $"No Valid-Declaration at slot {declerationNumber()}. | " };
        }


        double result = -1;
        String comment = "";


        Coordinate startPoint;
        Coordinate landingPoint;
        TrackHelpers.EstimateLaunchAndLandingTime(track, flight.useGPSAltitude(), out startPoint, out landingPoint);

        if (declarationPosition.TimeStamp < startPoint.TimeStamp)
        {
            var difference = CalculationHelper.Calculate2DDistance(declaration.PositionAtDeclaration, startPoint,
                flight.getCalculationType());
            comment += $"Declared before takeoff. Setting startPoint as declarationPoint. Difference {difference}m | ";
            declarationPosition = startPoint;
        }


        if (CalculationHelper.Calculate2DDistance(declarationPosition, declaration.DeclaredGoal,
                flight.getCalculationType()) < distanceFromDeclerationPointToCenter())
        {
            comment +=
                $"Declared inside {distanceFromDeclerationPointToCenter()}m radius [{NumberHelper.formatDoubleToStringAndRound(CalculationHelper.Calculate2DDistance(declarationPosition, declaration.DeclaredGoal, flight.getCalculationType()))}] | ";
        }

        if ((flight.useGPSAltitude()
                ? declaration.DeclaredGoal.AltitudeGPS < bottomPlateHeightM()
                : declaration.DeclaredGoal.AltitudeBarometric < bottomPlateHeightM()))
        {
            comment +=
                $"Declared Goal is below the airspace {(flight.useGPSAltitude() ? declaration.DeclaredGoal.AltitudeGPS : declaration.DeclaredGoal.AltitudeBarometric)}m / {bottomPlateHeightM()} | ";
        }
        if (track.Declarations.Count > maxDeclerations())
        {
            comment += "To many Declarations | ";
        }


        Coordinate lastTrackPoint = null;
        int calculatedTrackPoints = 0;

        bool hasEntered = false;
        foreach (Coordinate trackTrackPoint in track.TrackPoints)
        {
            double distance = CalculationHelper.Calculate2DDistance(trackTrackPoint, declaration.DeclaredGoal,
                flight.getCalculationType());
            if (distance > innerCircle() && distance < outerCircle() && (flight.useGPSAltitude()
                    ? trackTrackPoint.AltitudeGPS > bottomPlateHeightM()
                    : trackTrackPoint.AltitudeBarometric < bottomPlateHeightM()))
            {
                if (hasEntered && !reenter() && lastTrackPoint == null)
                {
                    comment += $"Reentered but not scored. In: {track.TrackPoints.IndexOf(trackTrackPoint) + 1} | ";
                    continue;
                }

                if (!hasEntered)
                    hasEntered = true;
                if (lastTrackPoint != null && trackTrackPoint != null)
                {
                    result += CalculationHelper.Calculate2DDistance(trackTrackPoint, lastTrackPoint,
                        flight.getCalculationType());
                    calculatedTrackPoints++;
                }
                else
                {
                    comment += $"In: {track.TrackPoints.IndexOf(trackTrackPoint) + 1} | ";
                }

                lastTrackPoint = trackTrackPoint;
            }
            else
            {
                if (lastTrackPoint != null)
                {
                    comment += $"Out: {track.TrackPoints.IndexOf(trackTrackPoint) + 1} | ";
                    lastTrackPoint = null;
                }
            }
        }

        comment += $"Calculated with {calculatedTrackPoints} TrackPoints.";


        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }
}