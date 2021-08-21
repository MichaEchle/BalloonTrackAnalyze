using Competition.Validation;
using Coordinates;
using Coordinates.Parsers;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Competition
{
    public class Flight
    {
        #region Properties

        /// <summary>
        /// The number of the flight
        /// </summary>
        public int FlightNumber { get; set; } = -1;

        /// <summary>
        /// The list of tasks for that flight
        /// </summary>
        public List<ICompetitionTask> Tasks { get; set; } = new List<ICompetitionTask>();

        /// <summary>
        /// The list of tracks form the pilots
        /// </summary>
        public List<Track> Tracks { get; set; } = new List<Track>();
        #endregion

        #region Singleton

        /// <summary>
        /// private instance object
        /// </summary>
        private static Flight flight = null;

        /// <summary>
        /// Lock object for thread safety
        /// </summary>
        private static readonly object lockObject = new object();

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
                if (flight == null)
                    flight = new Flight();
                return flight;
            }
        }

        public bool ParseTrackFiles(string path, bool useBalloonLiveParse)
        {
            string functionErrorMessage = "Failed to parse track files: ";
            DirectoryInfo directoryInfo = new DirectoryInfo($@"{path}");
            if (!directoryInfo.Exists)
                Log(LogSeverityType.Error, functionErrorMessage + $"Directory '{path}' does not exists");
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
                        Log(LogSeverityType.Error, functionErrorMessage + $"Track from '{trackFile.FullName}' won't be used for further processing");
                        trackIsValid = false;
                    }
                }
                else
                {
                    if (!FAILoggerParser.ParseFile(trackFile.FullName, out track))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + $"Track from '{trackFile.FullName}' won't be used for further processing");
                        trackIsValid = false;
                    }
                }
                if (trackIsValid)
                    Tracks.Add(track);
            }
            return true;
        }

        public bool MapPilotNamesToTracks(string mappingFile)
        {
            string functionErrorMessage = "Failed to map pilot names to tracks";
            FileInfo fileInfo = new FileInfo($@"{mappingFile}");
            if (!fileInfo.Exists)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"The file '{mappingFile}' does not exists");
                return false;
            }
            if (!fileInfo.Extension.Contains("csv"))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"The file extension '{fileInfo.Extension}' is not supported");
                return false;
            }

            using (StreamReader reader = new StreamReader(mappingFile))
            {
                reader.ReadLine();//ignore first line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    int pilotNumber;
                    if (!int.TryParse(parts[0], out pilotNumber))
                    {
                        Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse pilot number '{parts[0]}' as integer");
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
                                Log(LogSeverityType.Warning, $"Identifier of track matched with identifiers of {firstName},{lastName}, but pilot numbers didn't match (Track Pilot No.'{track.Pilot.PilotNumber}'/ File Pilot No.'{pilotNumber}'");
                            }
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        Log(LogSeverityType.Warning, $"No track match found for {firstName},{lastName}");
                }
            }
            foreach (Track track in Tracks)
            {
                if (string.IsNullOrWhiteSpace(track.Pilot.FirstName))
                {
                    Log(LogSeverityType.Warning, $"No match found for track from Pilot No.{track.Pilot.PilotNumber} with identifier '{track.Pilot.PilotIdentifier}'");
                }
            }


            return true;
        }

        public bool PreprocessTrack(Track track)
        {
            List<Declaration> validDeclarations = new List<Declaration>();
            List<MarkerDrop> validMarkers = new List<MarkerDrop>();
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
                                                Log(LogSeverityType.Warning, $"No goal number '{goalNumber}' declared in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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
                                                    Log(LogSeverityType.Warning, $"No goal number '{goalNumber}' declared in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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
                                        Log(LogSeverityType.Warning, $"No goal number '{markerToGoalDistanceRule.GoalNumber}' declared in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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
                                                Log(LogSeverityType.Warning, $"No marker '{markerNumber}' dropped in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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
                                        Log(LogSeverityType.Warning, $"No goal number '{markerToGoalDistanceRule.GoalNumber}' declared in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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
                                                Log(LogSeverityType.Warning, $"No marker '{markerNumber}' dropped in track of Pilot '#{track.Pilot.PilotNumber}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"({track.Pilot.FirstName},{track.Pilot.LastName})" : "")}'");
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


        public void CalculateResults(bool useGPSAltitude, string filePath)
        {
            string header = "Pilot No.,First Name,Last Name," + string.Join(',', Tasks);
            Tracks = Tracks.OrderBy(x => x.Pilot.PilotNumber).ToList();
            Tasks = Tasks.OrderBy(x => x.TaskNumber).ToList();
            using (StreamWriter writer = new StreamWriter(Path.Combine(filePath, "Results.csv"), false))
            {
                writer.WriteLine(header);

                foreach (Track track in Tracks)
                {
                    PreprocessTrack(track);
                    string[] results = new string[Tasks.Count];
                    for (int index = 0; index < Tasks.Count; index++)
                    {
                        double result;
                        bool isResultValid = Tasks[index].CalculateResults(track, useGPSAltitude, out result);
                        results[index] = Math.Round(result, 3, MidpointRounding.AwayFromZero).ToString();
                        if (!isResultValid)
                            results[index] += "*";
                    }
                    writer.WriteLine(string.Join(',', track.Pilot.PilotNumber, track.Pilot.FirstName, track.Pilot.LastName, string.Join(',', results)));
                }
            }
        }


        private void Log(LogSeverityType logSeverityType, string text)
        {
            Logger.Log(this, logSeverityType, text);
        }

        public override string ToString()
        {
            return $"Flight #{FlightNumber}";
        }
        #endregion
    }
}
