using Competition.Validation;
using Coordinates;
using Coordinates.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Competition
{
    public class Flight
    {
        #region Properties

        private readonly ILogger<Flight> Logger;

        /// <summary>
        /// The number of the flight
        /// </summary>
        public int FlightNumber { get; set; } = -1;

        /// <summary>
        /// The list of tasks for that flight
        /// </summary>
        public List<ICompetitionTask> Tasks { get; set; } = [];

        /// <summary>
        /// The list of tracks form the pilots
        /// </summary>
        public List<Track> Tracks { get; set; } = [];
        #endregion

        #region Singleton

        /// <summary>
        /// private instance object
        /// </summary>
        private static Flight flight = null;

        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private static readonly object lockObject = new();

        /// <summary>
        /// private constructor for singleton pattern
        /// </summary>
        private Flight()
        {

        }
        #endregion

        #region API
        /// <summary>
        /// Get the one and only instance
        /// </summary>
        /// <returns>the instance of flight</returns>
        public static Flight GetInstance()
        {
            lock (lockObject)
            {
                flight ??= new Flight();
                return flight;
            }
        }
        /// <summary>
        /// Takes all .igc files for the given path, parses them and add to the list of tracks
        /// </summary>
        /// <param name="path">the directory of the .igc files</param>
        /// <param name="useBalloonLiveParse">true: use balloon live parser; false: use FAI parser </param>
        /// <returns>true: success; false: error</returns>
        public bool ParseTrackFiles(string path, bool useBalloonLiveParse)
        {
            DirectoryInfo directoryInfo = new($@"{path}");
            if (!directoryInfo.Exists)
            {
                Logger?.LogError("Failed to parse track files: Directory '{path}' does not exists", path);
            }
            FileInfo[] trackFiles = directoryInfo.GetFiles("*.igc");
            flight.Tracks.Clear();
            foreach (FileInfo trackFile in trackFiles)
            {
                Track track;
                bool trackIsValid = true;
                if (useBalloonLiveParse)
                {
                    if (!BalloonLiveParser.ParseFile(trackFile.FullName, out track))
                    {
                        Logger?.LogError("Failed to parse track file '{trackFile}' and won't be used for further processing", trackFile.FullName);
                        trackIsValid = false;
                    }
                }
                else
                {
                    if (!FAILoggerParser.ParseFile(trackFile.FullName, out track))
                    {
                        Logger?.LogError("Failed to parse track file '{trackFile}' and won't be used for further processing", trackFile.FullName);
                        trackIsValid = false;
                    }
                }
                if (trackIsValid)
                    Tracks.Add(track);
            }
            return true;
        }

        /// <summary>
        /// Maps the pilot names to the tracks
        /// <para>the expected format is pilot number,first name, last name, pilot identifier (balloon live)</para>
        /// </summary>
        /// <param name="mappingFile">a csv file with the mapping info
        /// <para>semicolons are replaces with commas</para>
        /// </param>
        /// <returns>true:success; false:error</returns>
        public bool MapPilotNamesToTracks(string mappingFile)
        {
            FileInfo fileInfo = new($@"{mappingFile}");
            if (!fileInfo.Exists)
            {
                Logger?.LogError("Failed to map pilot names to tracks: The file '{mappingFile}' does not exists", mappingFile);
                return false;
            }
            if (!fileInfo.Extension.Contains("csv"))
            {
                Logger?.LogError("Failed to map pilot names to tracks: The file extension '{fileExtension}' is not supported", fileInfo.Extension);
                return false;
            }

            using (StreamReader reader = new(mappingFile))
            {
                reader.ReadLine();//ignore first line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    line = line.Replace(';', ',');
                    string[] parts = line.Split(',');
                    if (!int.TryParse(parts[0], out int pilotNumber))
                    {
                        Logger?.LogError("Failed to map pilot names to tracks: Failed to parse pilot number '{pilotNumber}' as integer", parts[0]);
                        return false;
                    }
                    string firstName = parts[1];
                    string lastName = parts[2];
                    string[] identifiers = parts[3..^0];
                    bool found = false;
                    foreach (Track track in Tracks)
                    {
                        if (identifiers.Contains(track.Pilot.PilotIdentifier))
                        {
                            track.Pilot.FirstName = firstName;
                            track.Pilot.LastName = lastName;
                            if (track.Pilot.PilotNumber != pilotNumber)
                            {
                                Logger?.LogWarning("Identifier of track matched with identifiers of '{firstName},{lastName}', but pilot numbers didn't match (Track Pilot No.'{pilotNumber}'/ File Pilot No.'{pilotNumber}'", firstName, lastName, track.Pilot.PilotNumber, pilotNumber);
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Logger?.LogWarning("No track match found for '{firstName},{lastName}'", firstName, lastName);
                    }

                }
            }
            foreach (Track track in Tracks)
            {
                if (string.IsNullOrWhiteSpace(track.Pilot.FirstName))
                {
                    Logger?.LogWarning("No match found for track from Pilot No. '{pilotNumber}' with identifier '{pilotIdentifier}'", track.Pilot.PilotNumber, track.Pilot.PilotIdentifier);
                }
            }


            return true;
        }

        /// <summary>
        /// Pre-process a track to make sure all declarations and markers are valid
        /// </summary>
        /// <param name="track">teh track to be pre-processed</param>
        /// <returns>true:success; false:error</returns>
        /// <exception cref="ArgumentNullException">track cannot be null</exception>
        public bool PreProcessTrack(Track track)
        {
            ArgumentNullException.ThrowIfNull(track);

            List<Declaration> validDeclarations = [];
            List<MarkerDrop> validMarkers = [];
            foreach (ICompetitionTask task in Tasks)
            {
                switch (task)
                {
                    case DonutTask donut:
                        {
                            Declaration declaration = ValidationHelper.GetValidDeclaration(track, donut.GoalNumber, donut.DeclarationValidationRules);
                            if (declaration != null)
                                validDeclarations.Add(declaration);
                        }
                        break;
                    case PieTask pie:
                        {
                            foreach (PieTask.PieTier tier in pie.Tiers)
                            {
                                Declaration declaration = ValidationHelper.GetValidDeclaration(track, tier.GoalNumber, tier.DeclarationValidationRules);
                                if (declaration != null)
                                    validDeclarations.Add(declaration);
                            }
                        }
                        break;
                    case ElbowTask elbowTask:
                        {
                            if (ValidationHelper.IsMarkerValid(track, elbowTask.FirstMarkerNumber, elbowTask.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == elbowTask.FirstMarkerNumber));
                            if (ValidationHelper.IsMarkerValid(track, elbowTask.SecondMarkerNumber, elbowTask.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == elbowTask.SecondMarkerNumber));
                            if (ValidationHelper.IsMarkerValid(track, elbowTask.ThirdMarkerNumber, elbowTask.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == elbowTask.ThirdMarkerNumber));
                        }
                        break;
                    case LandRunTask landRun:
                        {
                            if (ValidationHelper.IsMarkerValid(track, landRun.FirstMarkerNumber, landRun.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == landRun.FirstMarkerNumber));
                            if (ValidationHelper.IsMarkerValid(track, landRun.SecondMarkerNumber, landRun.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == landRun.SecondMarkerNumber));
                            if (ValidationHelper.IsMarkerValid(track, landRun.ThirdMarkerNumber, landRun.MarkerValidationRules))
                                validMarkers.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == landRun.ThirdMarkerNumber));
                        }
                        break;
                }
            }
            List<int> allGoalNumbers = track.GetAllGoalNumbers();
            List<int> allMarkerNubmers = track.GetAllMarkerNumbers();
            foreach (ICompetitionTask task in Tasks)
            {
                switch (task)
                {
                    case DonutTask donut:
                        {
                            foreach (IDeclarationValidationRules rule in donut.DeclarationValidationRules)
                            {
                                if (rule is GoalToOtherGoalsDistanceRule)
                                {
                                    GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule = rule as GoalToOtherGoalsDistanceRule;
                                    goalToOtherGoalsDistanceRule.Declarations.Clear();
                                    if (goalToOtherGoalsDistanceRule.GoalNumbers.Count == 0)
                                    {
                                        foreach (int goalNumber in allGoalNumbers)
                                        {
                                            if (validDeclarations.Select(x => x.GoalNumber).Contains(goalNumber))
                                            {
                                                goalToOtherGoalsDistanceRule.Declarations.Add(validDeclarations.FirstOrDefault(x => x.GoalNumber == goalNumber));
                                            }
                                            else
                                            {
                                                goalToOtherGoalsDistanceRule.Declarations.Add(track.GetLatestDeclaration(goalNumber));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (int goalNumber in goalToOtherGoalsDistanceRule.GoalNumbers)
                                        {
                                            if (!allGoalNumbers.Contains(goalNumber))
                                            {
                                                Logger?.LogWarning("No goal number '{goalNumber}' declared in track of Pilot '#{pilotNumber}{pilotName}'", goalNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                            }
                                            else
                                            {
                                                if (validDeclarations.Select(x => x.GoalNumber).Contains(goalNumber))
                                                {
                                                    goalToOtherGoalsDistanceRule.Declarations.Add(validDeclarations.FirstOrDefault(x => x.GoalNumber == goalNumber));
                                                }
                                                else
                                                {
                                                    goalToOtherGoalsDistanceRule.Declarations.Add(track.GetLatestDeclaration(goalNumber));
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        break;
                    case PieTask pie:
                        {
                            foreach (PieTask.PieTier tier in pie.Tiers)
                            {
                                foreach (IDeclarationValidationRules rule in tier.DeclarationValidationRules)
                                {
                                    if (rule is GoalToOtherGoalsDistanceRule)
                                    {
                                        GoalToOtherGoalsDistanceRule goalToOtherGoalsDistanceRule = rule as GoalToOtherGoalsDistanceRule;
                                        goalToOtherGoalsDistanceRule.Declarations.Clear();
                                        if (goalToOtherGoalsDistanceRule.GoalNumbers.Count == 0)
                                        {
                                            foreach (int goalNumber in allGoalNumbers)
                                            {
                                                if (validDeclarations.Select(x => x.GoalNumber).Contains(goalNumber))
                                                {
                                                    goalToOtherGoalsDistanceRule.Declarations.Add(validDeclarations.FirstOrDefault(x => x.GoalNumber == goalNumber));
                                                }
                                                else
                                                {
                                                    goalToOtherGoalsDistanceRule.Declarations.Add(track.GetLatestDeclaration(goalNumber));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (int goalNumber in goalToOtherGoalsDistanceRule.GoalNumbers)
                                            {
                                                if (!allGoalNumbers.Contains(goalNumber))
                                                {
                                                    Logger?.LogWarning("No goal number '{goalNumber}' declared in track of Pilot '#{pilotNumber}{pilotName}'", goalNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                                }
                                                else
                                                {
                                                    if (validDeclarations.Select(x => x.GoalNumber).Contains(goalNumber))
                                                    {
                                                        goalToOtherGoalsDistanceRule.Declarations.Add(validDeclarations.FirstOrDefault(x => x.GoalNumber == goalNumber));
                                                    }
                                                    else
                                                    {
                                                        goalToOtherGoalsDistanceRule.Declarations.Add(track.GetLatestDeclaration(goalNumber));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case ElbowTask elbow:
                        {
                            foreach (IMarkerValidationRules rule in elbow.MarkerValidationRules)
                            {
                                if (rule is MarkerToGoalDistanceRule)
                                {
                                    MarkerToGoalDistanceRule markerToGoalDistanceRule = rule as MarkerToGoalDistanceRule;
                                    markerToGoalDistanceRule.Declaration = null;
                                    if (!allGoalNumbers.Contains(markerToGoalDistanceRule.GoalNumber))
                                    {
                                        Logger?.LogWarning("No goal number '{goalNumber}' declared in track of Pilot '#{pilotNumber}{pilotName}'", markerToGoalDistanceRule.GoalNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                    }
                                    else
                                    {
                                        if (validDeclarations.Select(x => x.GoalNumber).Contains(markerToGoalDistanceRule.GoalNumber))
                                        {
                                            markerToGoalDistanceRule.Declaration = validDeclarations.FirstOrDefault(x => x.GoalNumber == markerToGoalDistanceRule.GoalNumber);
                                        }
                                        else
                                        {
                                            markerToGoalDistanceRule.Declaration = track.GetLatestDeclaration(markerToGoalDistanceRule.GoalNumber);
                                        }
                                    }
                                }
                                else if (rule is MarkerToOtherMarkersDistanceRule)
                                {
                                    MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = rule as MarkerToOtherMarkersDistanceRule;
                                    markerToOtherMarkersDistanceRule.MarkerDrops.Clear();
                                    if (markerToOtherMarkersDistanceRule.MarkerNumbers.Count == 0)
                                    {
                                        markerToOtherMarkersDistanceRule.MarkerDrops = track.MarkerDrops;
                                    }
                                    else
                                    {
                                        foreach (int markerNumber in markerToOtherMarkersDistanceRule.MarkerNumbers)
                                        {
                                            if (!allMarkerNubmers.Contains(markerNumber))
                                            {
                                                Logger?.LogWarning("No marker '{markerNumber}' dropped in track of Pilot '#{pilotNumber}{pilotName}'", markerNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                            }
                                            else
                                            {
                                                markerToOtherMarkersDistanceRule.MarkerDrops.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == markerNumber));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case LandRunTask landRun:
                        {
                            foreach (IMarkerValidationRules rule in landRun.MarkerValidationRules)
                            {
                                if (rule is MarkerToGoalDistanceRule)
                                {
                                    MarkerToGoalDistanceRule markerToGoalDistanceRule = rule as MarkerToGoalDistanceRule;
                                    markerToGoalDistanceRule.Declaration = null;
                                    if (!allGoalNumbers.Contains(markerToGoalDistanceRule.GoalNumber))
                                        {
                                        Logger?.LogWarning("No goal number '{goalNumber}' declared in track of Pilot '#{pilotNumber}{pilotName}'", markerToGoalDistanceRule.GoalNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                    }
                                    else
                                    {
                                        if (validDeclarations.Select(x => x.GoalNumber).Contains(markerToGoalDistanceRule.GoalNumber))
                                        {
                                            markerToGoalDistanceRule.Declaration = validDeclarations.FirstOrDefault(x => x.GoalNumber == markerToGoalDistanceRule.GoalNumber);
                                        }
                                        else
                                        {
                                            markerToGoalDistanceRule.Declaration = track.GetLatestDeclaration(markerToGoalDistanceRule.GoalNumber);
                                        }
                                    }
                                }
                                else if (rule is MarkerToOtherMarkersDistanceRule)
                                {
                                    MarkerToOtherMarkersDistanceRule markerToOtherMarkersDistanceRule = rule as MarkerToOtherMarkersDistanceRule;
                                    markerToOtherMarkersDistanceRule.MarkerDrops.Clear();
                                    if (markerToOtherMarkersDistanceRule.MarkerNumbers.Count == 0)
                                    {
                                        markerToOtherMarkersDistanceRule.MarkerDrops = track.MarkerDrops;
                                    }
                                    else
                                    {
                                        foreach (int markerNumber in markerToOtherMarkersDistanceRule.MarkerNumbers)
                                        {
                                            if (!allMarkerNubmers.Contains(markerNumber))
                                            {
                                                Logger?.LogWarning("No marker '{markerNumber}' dropped in track of Pilot '#{pilotNumber}{pilotName}'", markerNumber, track.Pilot.PilotNumber, (!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : ""));
                                            }
                                            else
                                            {
                                                markerToOtherMarkersDistanceRule.MarkerDrops.Add(track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == markerNumber));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                }

            }

            return true;
        }

        /// <summary>
        /// Calculates the results for all pilots and all tasks and write the results as csv to the specified path
        /// </summary>
        /// <param name="useGPSAltitude">true:use GPS altitude; false: use barometric altitude</param>
        /// <param name="filePath">the path at which the result file should be created</param>
        public void CalculateResults(bool useGPSAltitude, string filePath)
        {
            string header = "Pilot No.,First Name,Last Name," + string.Join(',', Tasks);
            Tracks = [.. Tracks.OrderBy(x => x.Pilot.PilotNumber)];
            Tasks = [.. Tasks.OrderBy(x => x.TaskNumber)];
            using StreamWriter writer = new(Path.Combine(filePath, "Results.csv"), false);
            writer.WriteLine(header);

            foreach (Track track in Tracks)
            {
                PreProcessTrack(track);
                string[] results = new string[Tasks.Count];
                for (int index = 0; index < Tasks.Count; index++)
                {
                    bool isResultValid = Tasks[index].CalculateResults(track, useGPSAltitude, out double result);
                    results[index] = Math.Round(result, 3, MidpointRounding.AwayFromZero).ToString();
                    if (!isResultValid)
                        results[index] += "*";
                }
                writer.WriteLine(string.Join(',', track.Pilot.PilotNumber, track.Pilot.FirstName, track.Pilot.LastName, string.Join(',', results)));
            }
        }

        /// <summary>
        /// Sets the specified default altitude to all declaration where no altitude was declared
        /// </summary>
        /// <param name="defaultAltitude">the default altitude in meters</param>
        /// <returns>a list containing the pilot and the declaration where default altitude has been set</returns>
        public List<(Pilot pilot, Declaration declaration)> SetDefaultGoalAltitude(double defaultAltitude)
        {
            List<(Pilot pilot, Declaration declaration)> declarationsWithAltitude = [];
            foreach (Track track in Tracks)
            {
                foreach (Declaration declaration in track.Declarations)
                {
                    if (!declaration.HasPilotDelaredGoalAltitude)
                    {

                        if (declaration.DeclaredGoal.SetDefaultAltitude(defaultAltitude))
                        {
                            declarationsWithAltitude.Add((track.Pilot, declaration));
                            Logger?.LogInformation("Default altitude set for Pilot {pilotNumber} at declaration {goalNumber}",track.Pilot.PilotNumber,declaration.GoalNumber);
                        }
                        else
                        {
                            Logger?.LogInformation("Default altitude could not be set for Pilot {pilotNumber} at declaration {goalNumber}", track.Pilot.PilotNumber, declaration.GoalNumber);
                        }
                    }
                }
            }
            return declarationsWithAltitude;
        }

        public override string ToString()
        {
            return $"Flight #{FlightNumber}";
        }
        #endregion
    }
}
