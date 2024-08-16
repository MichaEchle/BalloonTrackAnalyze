﻿using Coordinates;
using Coordinates.Parsers;
using JansScoring.flights;
using JansScoring.flights.impl._01;
using JansScoring.flights.impl._02;
using JansScoring.flights.impl._03;
using JansScoring.flights.impl._04;
using JansScoring.pz_rework;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Console = System.Console;
using Task = JansScoring.flights.Task;

namespace JansScoring;

public class FlightManager
{
    public static bool DEBUG = false;
    private Dictionary<int, Flight> flights = new();
    private PZManager pzManager;

    public static bool RUN_PZ_CHECKS = false;
    public static bool RUN_DESPIKER = false;

    public void register()
    {
        pzManager = new PZManager();

        flights.Add(1, new Flight01());
        flights.Add(2, new Flight02());
        flights.Add(3, new Flight03());
        flights.Add(4, new Flight04());

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

        List<Track> trackList = generateTrackList(directoryInfo, flight);


        string scoringTime = DateTime.Now.ToString("MMddHHmmss");

        Console.WriteLine("Start generate Flight Report");
        generateFlightReport(trackList, flight, scoringFolder, scoringTime, RUN_DESPIKER, RUN_PZ_CHECKS);
        Console.WriteLine("Finish generate Flight Report");
        foreach (Task task in flight.getTasks())
        {
            string resultsPath = $"{scoringFolderLink}\\f{flightNumber}_t{task.TaskNumber()}_Results_{scoringTime}.csv";

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

                    string comment = "";
                    if (task.ScoringChecks(currentTrack, comment: ref comment))
                    {
                        writer1.WriteLine($"{currentTrack.Pilot.PilotNumber};NR;{comment}");
                        continue;
                    }

                    task.Score(currentTrack, comment: ref comment, out double score);
                    writer1.WriteLine(
                        $"{currentTrack.Pilot.PilotNumber};{(score != double.MinValue && score != double.MaxValue ? NumberHelper.formatDoubleToStringAndRound(score) : "NR")};{comment}");
                }

                writer1.Close();
                Console.WriteLine($"Succesful scored Task {task.TaskNumber()}");
            }

            openFile(resultsPath);
        }
    }

    private void generateFlightReport(List<Track> tracks, Flight flight, DirectoryInfo scoringFolder,
        string scoringTime, bool despiker, bool checkPZ)
    {
        Dictionary<Pilot, string> comments = new();


        List<Coordinate> goals = new();
        foreach (Task task in flight.getTasks())
        {
            int i = 0;
            foreach (Coordinate coordinate in task.Goals(0).ToList())
            {
                i++;
                Console.WriteLine($"Loaded goal {i} for task {task.TaskNumber()}");
                goals.Add(coordinate);
            }
        }

        Thread[] generateThread = new Thread[tracks.Count];
        int currentThread = 0;
        Object lockObject = new();
        int finishThreads = 0;
        foreach (Track track in tracks)
        {
            generateThread[currentThread] = new Thread(() =>
            {
                lock (lockObject)
                {
                    comments.Remove(track.Pilot);
                }

                string comment = comments.GetValueOrDefault(track.Pilot, "");

                if (despiker)
                {
                    Console.WriteLine($"Start despike for pilot {track.Pilot.PilotNumber}");
                    int spikes = DeSpiker.despike(track, flight.useGPSAltitude());
                    if (spikes > 0)
                    {
                        comment += $"Removed {spikes} Spikes | ";
                    }
                    Console.WriteLine($"Despiked {spikes} fro pilot {track.Pilot.PilotNumber}");
                }
                Console.WriteLine($"Start Loading Track for Pilot {track.Pilot.PilotNumber}.");

                FileInfo trackPath = track.trackPath;
                if (trackPath != null)
                {
                    long fileSize = trackPath.Length / 1000;
                    if (fileSize < 50)
                    {
                        comment += "The Trackfile is smaller than 50kb and will be ignored in scoring | ";
                    }
                }

                if (!TrackHelpers.CheckLaunchConstraints(track, flight.useGPSAltitude(),
                        flight.getStartOfLaunchPeriode(),
                        flight.getStartOfLaunchPeriode().AddMinutes(flight.launchPeriode()), goals,
                        flight.distanceToAllGoals(), double.NaN,
                        out Coordinate launchPoint,
                        out bool launchInStartPeriod, out List<double> distanceToGoals,
                        out List<bool> distanceToGoalsOk))
                {
                    comment += "Failed to check launch constraints | ";
                    lock (lockObject)
                    {
                        comments.Add(track.Pilot, comment);
                        finishThreads++;
                        Console.WriteLine(
                            $"Finish generating Flight Report for Pilot {track.Pilot.PilotNumber}. [{finishThreads}/{tracks.Count}]");
                    }

                    return;
                }

                String decNumbers = "";

                foreach (Declaration trackDeclaration in track.Declarations)
                {
                    decNumbers += trackDeclaration.GoalNumber + ", ";
                }

                if (DEBUG)
                {
                    comment += $"Found Declarations: {track.Declarations.Count} ({decNumbers}) | ";
                }

                String markNumbers = "";

                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    markNumbers += markerDrop.MarkerNumber + ", ";
                }

                if (DEBUG)
                {
                    comment += $"Found Markers: {track.MarkerDrops.Count} ({markNumbers}) | ";
                }


                if (!launchInStartPeriod)
                {
                    if (flight.getStartOfLaunchPeriode() > launchPoint.TimeStamp)
                    {
                        TimeSpan launchPointTimeSpan = flight.getStartOfLaunchPeriode() - launchPoint.TimeStamp;
                        comment +=
                            $"Pilot started before the launch periode [{launchPointTimeSpan.ToString(@"hh\:mm\:ss")}]. Started {launchPoint.TimeStamp:dd.MM.yy HH:mm:ss} UTC | ";
                    }
                    else
                    {
                        TimeSpan launchPointTimeSpan = launchPoint.TimeStamp -
                                                       flight.getStartOfLaunchPeriode()
                                                           .AddMinutes(flight.launchPeriode());
                        comment +=
                            $"Pilot started after the launch periode [{launchPointTimeSpan.ToString(@"hh\:mm\:ss")}]. Started {launchPoint.TimeStamp:dd.MM.yy HH:mm:ss} UTC | ";
                    }
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

                if (checkPZ)
                {
                    comment += pzManager.CheckPz(flight, track);
                }

                lock (lockObject)
                {
                    comments.Add(track.Pilot,
                        (comment.Length > 0 ? comment.Substring(0, comment.Length - 3) : comment));
                    finishThreads++;
                    Console.WriteLine(
                        $"Finish generating Flight Report for Pilot {track.Pilot.PilotNumber}. [{finishThreads}/{tracks.Count}]");
                }
            });
            generateThread[currentThread].Start();
            currentThread++;
        }

        foreach (Thread thread in generateThread)
        {
            thread.Join();
        }

        var orderedComments = comments.OrderBy(pair => pair.Key.PilotNumber);

        string path = $"{scoringFolder}\\f{flight.getFlightNumber()}_FlightReport_{scoringTime}.csv";

        using (StreamWriter writer1 = new(path))
        {
            writer1.WriteLine($"PilotNumber;Comment");

            foreach ((Pilot key, string value) in orderedComments)
            {
                writer1.WriteLine($"{key.PilotNumber};{value}");
            }

            writer1.Close();
            Console.WriteLine($"Succesful created Report for Flight {flight.getFlightNumber()}");
        }

        openFile(path);
    }

    private static void openFile(String filePath)
    {
        ProcessStartInfo psi = new();
        psi.FileName = filePath;
        psi.UseShellExecute = true;
        Process.Start(psi);
    }

    private static void Log(LogSeverityType logSeverity, string text)
    {
        Logger.Log("Flight Manager", logSeverity, text);
        Console.WriteLine($"LOG: {logSeverity.ToString()} | {text}");
    }

    private List<Track> generateTrackList(DirectoryInfo directoryInfo, Flight flight)
    {
        FileInfo[] files = directoryInfo.GetFiles("*.igc");
        Track track;
        List<Track> tracks = new();

        foreach (FileInfo fileInfo in files)
        {
            Log(LogSeverityType.Info, $"Start loading file '{fileInfo.Name}'.");
            if (!BalloonLiveParser.ParseFile(fileInfo.FullName, out track, flight.getBackupCoordinates()))
            {
                Console.WriteLine($"Failed to parse track '{fileInfo.FullName}'");
                continue;
            }

            track.trackPath = fileInfo;


            if (!flight.useGPSAltitude())
            {
                foreach (Coordinate trackPoint in track.TrackPoints)
                {
                    trackPoint.CorrectBarometricHeight(flight.getQNH());
                }

                foreach (MarkerDrop markerDrop in track.MarkerDrops)
                {
                    markerDrop.MarkerLocation.CorrectBarometricHeight(flight.getQNH());
                }

                foreach (Declaration decleration in track.Declarations)
                {
                    decleration.PositionAtDeclaration.CorrectBarometricHeight(flight.getQNH());
                }
            }

            tracks.Add(track);
        }

        tracks = tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
        Console.WriteLine($"Loaded {tracks.Count} from {files.Length} IGC-Files");
        return tracks;
    }
}