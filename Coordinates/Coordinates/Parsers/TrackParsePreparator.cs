using LoggerComponent;
using System;
using System.Collections.Generic;
using System.IO;

namespace Coordinates.Parsers;

public class TrackParsePreparator
{
    private Dictionary<string, TrackParser> parsers;

    public TrackParsePreparator()
    {
        parsers = new();

        /*
         * parsers.Add("igc", new BalloonLiveParser());
        parsers.Add("igc2", new FAILoggerParser());
         */
    }

    /// <summary>
    /// Takes all files for the given path, parses them and add to the list of tracks
    /// </summary>
    /// <param name="directory">the directory of the files</param>
    /// <param name="useBalloonLiveIfIGC">true: use balloon live parser; false: use FAI parser</param>
    /// <param name="tracks">the list with the parsed tracks</param>
    /// <param name="referenceCoordinate">a reference coordinate for autocompletion</param>
    /// <returns>true: success; false: error</returns>
    public void ParseTracks(DirectoryInfo directory, bool useBalloonLiveIfIGC, out List<Track> tracks,
        Coordinate referenceCoordinate = null)
    {
        tracks = new();
        foreach (FileInfo fileInfo in directory.GetFiles())
        {
            string extension = fileInfo.Extension.ToLower();

            if (!parsers.ContainsKey(extension))
            {
                Log(LogSeverityType.Info,
                    $"Could not parse '{Path.GetFullPath(fileInfo.FullName)}'. No parser for this file-extension (\'.'{extension}'\') found.");
                continue;
            }

            TrackParser parser;

            if (extension.ToLower() == ".igc")
            {
                if (useBalloonLiveIfIGC)
                {
                    parsers.TryGetValue("igc", out parser);
                }
                else
                {
                    parsers.TryGetValue("igc2", out parser);
                }
            }
            else if (!parsers.TryGetValue(extension, out parser))
            {
                Log(LogSeverityType.Error,
                    $"Could not parse '{Path.GetFullPath(fileInfo.FullName)}'. No parser for this file-extension (\'.'{extension}'\') found.");
                continue;
            }


            parser.ParseFile(fileInfo, out Track track, referenceCoordinate);

            tracks.Add(track);
            Log(LogSeverityType.Info,
                $"Successfully parsed file '{fileInfo.FullName}' with '{parser.GetType().Name}'.");
        }

        Log(LogSeverityType.Info, $"Successfully parsed '{tracks.Count}' files.");
    }

    private static void Log(LogSeverityType logSeverity, string text)
    {
        Logger.Log("Parser Preparator", logSeverity, text);
    }
}