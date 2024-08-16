using Coordinates;
using JansScoring.check;
using System;

namespace JansScoring.flights.tasks;

public abstract class TaskRTA : Task
{
    protected TaskRTA(Flight flight) : base(flight)
    {
    }

    public override void Score(Track track, ref string comment, out double result)
    {
        DeclarationChecks.LoadDeclaration(track, DeclarationNumber(), out Declaration declaration, ref comment);

        if (declaration == null)
        {
            result = double.MaxValue;
            return;
        }


        string northing = declaration.OrignalNorhtingDeclarationUTM.ToString();

        if (northing.Length != 4)
        {
            for (int i = 0; i < 4 - northing.Length; i++)
            {
                northing = "0" + northing;
            }
        }

        string minutes = northing.Substring(0, 2);
        string seconds = northing.Substring(2, 2);
        int estimatedSeconds = int.Parse(seconds) + (int.Parse(minutes) * 60);

        comment += $"Estimated {minutes}m {seconds}s | ";

        Coordinate entryPoint = null;
        Coordinate exitPoint = null;

        foreach (Coordinate trackPoint in track.TrackPoints)
        {
            if (entryPoint == null && IsInsideScoringArea(trackPoint))
            {
                entryPoint = trackPoint;
                continue;
            }

            if (entryPoint != null && !IsInsideScoringArea(trackPoint))
            {
                exitPoint = trackPoint;
                break;
            }
        }

        if (entryPoint == null)
        {
            comment += $"Could not find any entry point. | ";
            result = Double.MaxValue;
            return;
        }

        if (exitPoint == null)
        {
            comment += $"Could not find any exit point. | ";
            result = Double.MaxValue;
            return;
        }

        double neededSeconds = (exitPoint.TimeStamp - entryPoint.TimeStamp).TotalSeconds;
        comment += $"needed {neededSeconds}s | ";

        result = Math.Abs(neededSeconds - estimatedSeconds);
    }

    public abstract bool IsInsideScoringArea(Coordinate coordinate);

    public override Coordinate[] Goals(int pilot)
    {
        return Array.Empty<Coordinate>();
    }

    public abstract int DeclarationNumber();
}