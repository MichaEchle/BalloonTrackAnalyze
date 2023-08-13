using Coordinates;
using System;

namespace JansScoring.flights.impl._06.tasks;

public class Task21 : Task
{
    public override int number()
    {
        return 21;
    }

    public override string[] score(Track track)
    {
        Console.WriteLine($"Start scoring task 21 for pilot {track.Pilot.PilotNumber}");
        int flytime = CalculateTimeDiffernceBetweenEnterAndExit(track, out string flycomment);

        if (flytime == Double.MaxValue)
        {
            return new[] { "NR", "Not crossed both grid-lines" };
        }

        if (!track.GetAllGoalNumbers().Contains(1))
        {
            return new[] { "No Result", "No Declaration in 1" };
        }

        Declaration declaration = track.Declarations.FindLast(declaration => declaration.GoalNumber == 1);

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            return new[] { "No Result", "No valid Declaration in 1" };
        }

        string s = declaration.OrignalNorhtingDeclarationUTM.ToString();

        if (s.Length < 4)
        {
            for (int i = 0; i < 4 - s.Length; i++)
            {
                s = "0" + s;
            }
        }

        string minuteString = s.Substring(0, 2);
        string secondString = s.Substring(3, 1);

        int minutes = int.Parse(minuteString);
        int seconds = int.Parse(secondString);

        int totalSeconds = (minutes * 60) + seconds;

        double timeDifference = Math.Max(flytime - totalSeconds, totalSeconds - flytime);

        return new[]
        {
            NumberHelper.formatDoubleToStringAndRound(timeDifference),
            $"Fly-Time: {new TimeSpan(0, 0, 0, flytime)} | Decleration-Time: {new TimeSpan(0, 0, 0, totalSeconds)} | Decleration-String: {s} | Fly-Comment: {flycomment}"
        };
    }


    public int CalculateTimeDiffernceBetweenEnterAndExit(Track track, out string comment)
    {
        comment = "";
        Coordinate fistCoordinate = null;
        Coordinate before = null;

        foreach (Coordinate trackTrackPoint in track.TrackPoints)
        {
            (string utmZone, double easting, double northing) =
                CoordinateHelpers.ConvertLatitudeLongitudeCoordinateToUTM_Precise(trackTrackPoint);
            if (fistCoordinate == null)
            {
                if (northing > 5367000 &&
                    northing < 5369000)
                {
                    fistCoordinate = trackTrackPoint;
                }
            }
            else
            {
                if (northing > 5369000)
                {
                    comment = $"{utmZone} {easting} {northing} | ";
                    return (int)(before.TimeStamp - fistCoordinate.TimeStamp).TotalSeconds;
                }
            }

            before = trackTrackPoint;
        }

        return int.MaxValue;
    }

    public override Coordinate[] goals()
    {
        return new Coordinate[] { };
    }

    public override DateTime getScoringPeriodeUntil()
    {
        return new DateTime(2023, 08, 12, 09, 00, 00);
    }

    public Task21(Flight flight) : base(flight)
    {
    }
}