using Coordinates;
using Coordinates.Parsers;
using JansScoring.flights;
using JansScoring.flights.impl._2;
using JansScoring.flights.impl._3;
using JansScoring.flights.impl._4;
using JansScoring.pz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Console = System.Console;
using Task = JansScoring.flights.Task;

namespace JansScoring;

public class FlightManager
{
    private Dictionary<int, Flight> flights = new();
    private PZManager pzManager;

    public void register()
    {
        pzManager = new PZManager();
        pzManager.registerPZs();


        flights.Add(1, new Flight1());
        flights.Add(2, new Flight2());
        flights.Add(3, new Flight3());
        flights.Add(4, new Flight4());


        scoreFlight(4);
    }


    public void scoreFlight(int flightNumber)
    {
        if (!flights.ContainsKey(flightNumber))
        {
            Console.Error.Write("The Flight does not exists!");
            return;
        }

        Flight flight = flights[flightNumber];
        DirectoryInfo directoryInfo = new(flight.getTracksPath());

        string scoringFolderLink = $"{directoryInfo.Parent.FullName}\\scoring";
        DirectoryInfo scoringFolder = new(scoringFolderLink);
        if (!scoringFolder.Exists)
        {
            scoringFolder.Create();
        }

        List<Track> trackList = generateTrackList(directoryInfo);


        string scoringTime = DateTime.Now.ToString("MMddHHmmss");

        Console.WriteLine("Start generate Flight Report");
        generateFlightReport(trackList, flight, scoringFolder, scoringTime);
        Console.WriteLine("Finish generate Flight Report");
        foreach (Task task in flight.getTasks())
        {
            string resultsPath = $"{scoringFolderLink}\\f{flightNumber}_t{task.number()}_Results_{scoringTime}.csv";

            using (StreamWriter writer1 = new(resultsPath))
            {
                writer1.WriteLine($"PilotNumber;Result;Comment");
                foreach (Track currentTrack in trackList)
                {
                    FileInfo trackPath = currentTrack.trackPath;
                    if (trackPath != null)
                    {
                        long fileSize = trackPath.Length / 1000;
                        if (fileSize < 50)
                        {
                            continue;
                        }
                    }

                    string[] score = task.score(currentTrack);
                    string result = score[0];
                    string comment = (score.Length > 1 ? score[1] : "");
                    writer1.WriteLine($"{currentTrack.Pilot.PilotNumber};{result};{comment}");
                }

                writer1.Close();
                Console.WriteLine($"Succesful scored Task {task.number()}");
            }
        }
    }

    private void generateFlightReport(List<Track> tracks, Flight flight, DirectoryInfo scoringFolder,
        string scoringTime)
    {
        Dictionary<Pilot, string> comments = new();

        /*
        Console.WriteLine($"Start Despiking {tracks.Count} Tracks");
        foreach (Track track in tracks)
        {
            Console.WriteLine($"Start despike for pilot {track.Pilot.PilotNumber}");
            int spikes = DeSpiker.despike(track, flight.useGPSAltitude());
            if (spikes > 0)
            {
                comments.Add(track.Pilot, $"Removed {spikes} Spikes | ");
            }
            Console.WriteLine($"Despiked {spikes} fro pilot {track.Pilot.PilotNumber}");
        }
        Console.WriteLine($"Finish Despiking {tracks.Count} Tracks");
         */


        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            foreach (Coordinate coordinate in task.goals().ToList())
            {
                goals.Add(coordinate);
            }
        }

        foreach (Track track in tracks)
        {
            string comment = comments.GetValueOrDefault(track.Pilot, "");
            comments.Remove(track.Pilot);

            FileInfo trackPath = track.trackPath;
            if (trackPath != null)
            {
                long fileSize = trackPath.Length / 1000;
                if (fileSize < 50)
                {
                    comment += "The Trackfile is smaller than 50kb and will be ignored in scoring | ";
                }
            }

            if (!TrackHelpers.CheckLaunchConstraints(track, flight.useGPSAltitude(), flight.getStartOfLaunchPeriode(),
                    flight.getStartOfLaunchPeriode().AddMinutes(flight.launchPeriode()), goals,
                    flight.distanceToAllGoals(), double.NaN,
                    out Coordinate launchPoint,
                    out bool launchInStartPeriod, out List<double> distanceToGoals, out List<bool> distanceToGoalsOk))
            {
                comment += "Failed to check launch constraints";
                comments.Add(track.Pilot, comment);
                continue;
            }

            comment += pzManager.checkPZ(flight, track);


            if (!launchInStartPeriod)
            {
                TimeSpan launchPointTimeSpan = launchPoint.TimeStamp -
                                               flight.getStartOfLaunchPeriode().AddMinutes(flight.launchPeriode());

                comment +=
                    $"Pilot started outside the launchperiode [{launchPointTimeSpan.ToString(@"hh\:mm\:ss")}]. Started {launchPoint.TimeStamp:dd.MM.yy HH:mm:ss} | ";
            }
            


            if (distanceToGoalsOk.Contains(false))
            {
                if (goals.Count > 0)
                {
                    for (int index = 1; index <= distanceToGoalsOk.Count; index++)
                    {
                        bool ok = distanceToGoalsOk[index - 1];
                        if (ok)
                        {
                            continue;
                        }

                        comment +=
                            $"Pilot started to close to goal {index} ({NumberHelper.formatDoubleToStringAndRound(distanceToGoals[index - 1])})  | ";
                    }
                }
            }

            comments.Add(track.Pilot, (comment.Length > 0 ? comment.Substring(0, comment.Length - 3) : comment));
        }

        string path = $"{scoringFolder}\\f{flight.getFlightNumber()}_FlightReport_{scoringTime}.csv";

        using (StreamWriter writer1 = new(path))
        {
            writer1.WriteLine($"PilotNumber;Comment");

            foreach ((Pilot key, string value) in comments)
            {
                writer1.WriteLine($"{key.PilotNumber};{value}");
            }

            writer1.Close();
            Console.WriteLine($"Succesful created Report for Flight {flight.getFlightNumber()}");
        }
    }

    private List<Track> generateTrackList(DirectoryInfo directoryInfo)
    {
        FileInfo[] files = directoryInfo.GetFiles("*.igc");
        Track track;
        List<Track> tracks = new();
        foreach (FileInfo fileInfo in files)
        {
            if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out track))
            {
                Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
                continue;
            }

            track.trackPath = fileInfo;
            tracks.Add(track);
        }

        tracks = tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        return tracks;
    }
}