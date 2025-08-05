using Coordinates;
using JansScoring.calculation;
using System;

namespace JansScoring.flights.impl._05.tasks;

public class Task18 : Task
{
    public Task18(Flight flight) : base(flight)
    {
    }

    public override int number()
    {
        return 18;
    }

    public override string[] score(Track track)
    {
        string comment = "";
        double result = 0;


        foreach (Coordinate coordinate in goals())
        {
            CalculateGoal(track, coordinate, out double goalResult, out double usedPoints);
            if (usedPoints == 0)
            {
                continue;
            }

            result += goalResult;
            comment += $"Fly in goal {Array.IndexOf(goals(), coordinate)} for {goalResult}m ({usedPoints} Points) | ";
        }


        return new[] { NumberHelper.formatDoubleToStringAndRound(result), comment };
    }

    private void CalculateGoal(Track track, Coordinate goal, out double result, out double calculatedTrackPoints)
    {
        Coordinate lastTrackPoint = null;
        calculatedTrackPoints = 0;
        result = 0;

        foreach (Coordinate trackTrackPoint in track.TrackPoints)
        {
            double distance = CalculationHelper.Calculate2DDistance(trackTrackPoint, goal,
                flight.getCalculationType());
            if (distance is < 200)
            {
                if (lastTrackPoint != null && trackTrackPoint != null)
                {
                    result += CalculationHelper.Calculate2DDistance(trackTrackPoint, lastTrackPoint,
                        flight.getCalculationType());
                    calculatedTrackPoints++;
                }

                lastTrackPoint = trackTrackPoint;
            }
            else
            {
                if (lastTrackPoint != null)
                {
                    return;
                }
            }
        }
    }

    public override Coordinate[] goals()
    {
        return new[]
        {
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479390, 5372360, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480170, 5372060, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480500, 5371280, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 480190, 5370500, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 479420, 5370160, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478630, 5370480, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478300, 5371250, 0),
            CoordinateHelpers.ConvertUTMToLatitudeLongitudeCoordinate("32U", 478610, 5372030, 0),
        };
    }

    public override DateTime getScoringPeriodUntil()
    {
        return new DateTime(2023, 08, 11, 20, 49, 00);
    }
}