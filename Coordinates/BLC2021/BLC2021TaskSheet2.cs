using Competition;
using Competition.Validation;
using Coordinates;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLC2021
{
    public partial class BLC2021TaskSheet2 : Form
    {

        #region Properties

        private readonly ILogger<BLC2021TaskSheet2> Logger;

        private readonly PilotMapping PilotMapping;

        private bool BatchMode
        {
            get; set;
        }

        private DirectoryInfo OutputDirectory
        {
            get; set;
        }

        private Track FiddleTrack
        {
            get; set;
        }

        private int Task4_GoalNumber
        {
            get; set;
        } = 1;

        private int Task4_FirstMarkerNumber
        {
            get; set;
        } = 1;//1

        private int Task4_SecondMarkerNumber
        {
            get; set;
        } = 2;

        private int Task4_ThridMarkerNumber
        {
            get; set;
        } = 3;//3

        private int Task5_GoalNumber
        {
            get; set;
        } = 2;

        private int Task5_MarkerNumber
        {
            get; set;
        } = 4;


        private readonly string ResultsFileNameInternal = "BLC2021TaskSheet2_Results_Internal.xlsx";
        private readonly string ResultsFileNameProvisional = "BLC2021TaskSheet2_Results_Provisional.xlsx";
        #endregion

        #region Constructor
        public BLC2021TaskSheet2(bool batchMode)
        {
            BatchMode = batchMode;
            InitializeComponent();

            Text = ToString();

        }
        #endregion

        #region Methods
        #region General
        public override string ToString()
        {
            if (BatchMode)
                return $"BLC 2021 Task Sheet 2 (Batch Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
            else
                return $"BLC 2021 Task Sheet 2 (Fiddle Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
        }

        private void BLC2021TaskSheet2_Load(object sender, EventArgs e)
        {
            if (BatchMode)
                ConfigureForBatchMode();
            else
                ConfigureForFiddleMode();

            bool showDialog = false;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultOutputDirectory))
            {
                OutputDirectory = new DirectoryInfo(Properties.Settings.Default.DefaultOutputDirectory);
                if (!OutputDirectory.Exists)
                    showDialog = true;
            }
            else
            {
                showDialog = true;
            }
            if (showDialog)
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    folderBrowserDialog.UseDescriptionForTitle = true;
                    folderBrowserDialog.Description = "Select an output directory";
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        OutputDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                        Properties.Settings.Default.DefaultOutputDirectory = folderBrowserDialog.SelectedPath;
                        Properties.Settings.Default.Save();
                        if (!btSelectIGCFiles.Enabled)
                            btSelectIGCFiles.Enabled = true;
                    }
                    else
                    {
                        Logger?.LogError("No output directory has been defined, an output directory must be configured to continue");
                        btSelectIGCFiles.Enabled = false;
                    }
                }
            }
            else
            {
                if (!btSelectIGCFiles.Enabled)
                    btSelectIGCFiles.Enabled = true;
            }
        }
        private void ConfigureForBatchMode()
        {
            btSelectIGCFiles.Text = "Select IGC File";

            //////////////////
            //Task 4
            //////////////////
            Task4_GoalNumber = 1;
            Task4_FirstMarkerNumber = 1;//1
            Task4_SecondMarkerNumber = 2;
            Task4_ThridMarkerNumber = 3;//3
            cbGoalTask4.Visible = false;
            lbZoneTask4.Visible = false;
            tbZoneTask4.Visible = false;
            lbEastingTask4.Visible = false;
            tbEastingTask4.Visible = false;
            lbNorthingTask4.Visible = false;
            tbNorthingTask4.Visible = false;
            lbAltitudeTask4.Visible = false;
            tbAltitudeTask4.Visible = false;
            btCalculateTask4.Visible = false;
            cbFirstMarkerTask4.Visible = false;
            cbSecondMarkerTask4.Visible = false;
            cbThirdMarkerTask4.Visible = false;
            lbGoalNumberTask4.Visible = true;
            lbFirstMarkerNumberTask4.Visible = true;
            lbSecondMarkerNumberTask4.Visible = true;
            lbThirdMarkerNumberTask4.Visible = true;
        }

        private void ConfigureForFiddleMode()
        {

            //////////////////
            //Task 4
            //////////////////
            Task4_GoalNumber = 99;
            Task4_FirstMarkerNumber = -1;
            Task4_SecondMarkerNumber = -1;
            Task4_ThridMarkerNumber = -1;
            cbGoalTask4.Visible = true;
            lbZoneTask4.Visible = true;
            tbZoneTask4.Visible = true;
            lbEastingTask4.Visible = true;
            tbEastingTask4.Visible = true;
            lbNorthingTask4.Visible = true;
            tbNorthingTask4.Visible = true;
            lbAltitudeTask4.Visible = true;
            tbAltitudeTask4.Visible = true;
            btCalculateTask4.Visible = true;
            cbFirstMarkerTask4.Visible = true;
            cbSecondMarkerTask4.Visible = true;
            cbThirdMarkerTask4.Visible = true;
            lbGoalNumberTask4.Visible = false;
            lbFirstMarkerNumberTask4.Visible = false;
            lbSecondMarkerNumberTask4.Visible = false;
            lbThirdMarkerNumberTask4.Visible = false;
        }


        private async void btSelectIGCFiles_Click(object sender, EventArgs e)
        {
            if (BatchMode)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select .igc files";
                openFileDialog.Filter = "igc files (*.igc)|*.igc";
                openFileDialog.CheckPathExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = true;
                UseWaitCursor = true;

                HesitationWaltzTask task4_1 = new HesitationWaltzTask();
                task4_1.SetupHWZ(4, CalculateGoalsTask4_1, Task4_FirstMarkerNumber, true, null);

                HesitationWaltzTask task4_2 = new HesitationWaltzTask();
                task4_2.SetupHWZ(4, CalculateGoalsTask4_2, Task4_SecondMarkerNumber, true, null);

                HesitationWaltzTask task4_3 = new HesitationWaltzTask();
                task4_3.SetupHWZ(4, CalculateGoalsTask4_3, Task4_ThridMarkerNumber, true, null);

                MarkerTimingRule markerTimingRule = new MarkerTimingRule();
                markerTimingRule.SetupRule(new List<(int openAtMinute, int closeAtMinute)>() { (0, 5), (20, 25), (40, 45) });


                MarkerToGoalDistanceRule markerToGoalDistanceRule = new MarkerToGoalDistanceRule();
                markerToGoalDistanceRule.SetupRule(double.NaN, 300.0, true, true, Task5_GoalNumber);


                List<IMarkerValidationRules> markerValidationRules = new List<IMarkerValidationRules>();
                markerValidationRules.Add(markerTimingRule);
                markerValidationRules.Add(markerToGoalDistanceRule);
                HesitationWaltzTask task5 = new HesitationWaltzTask();
                task5.SetupHWZ(5, CalculateGoalsTask5, Task5_MarkerNumber, true, markerValidationRules);


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] igcFiles = openFileDialog.FileNames;
                    bool success = await ParseFilesAndCalculateResultsAsync(igcFiles, task4_1, task4_2, task4_3, task5);
                    if (success)
                    {
                        Logger?.LogError("Parsing and result calculation completed");
                    }
                    else
                    {
                        Logger?.LogError("Failed to parse and calculate results");
                    }
                }
                UseWaitCursor = false;


            }
            else//fiddle mode
            {
            }
        }
        #endregion

        #region BatchMode

        private List<Coordinate> CalculateGoalsTask4_1(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");
            goals.Add(declaration.DeclaredGoal);
            return goals;
        }
        private List<Coordinate> CalculateGoalsTask4_2(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            double distanceFromDeclaredGoal = 1000.0;
            double additionalAltitude = CoordinateHelpers.ConvertToMeter(300.0);
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");

            CoordinateSharp.Coordinate tempDeclaredGoal = new CoordinateSharp.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);

            double easting = Math.Round(tempDeclaredGoal.UTM.Easting, 0, MidpointRounding.AwayFromZero);
            double northing = Math.Round(tempDeclaredGoal.UTM.Northing, 0, MidpointRounding.AwayFromZero);
            string latZone = tempDeclaredGoal.UTM.LatZone;
            int longZone = tempDeclaredGoal.UTM.LongZone;

            for (int northingFactor = -1; northingFactor <= 1; northingFactor++)
            {
                for (int eastingFactor = -1; eastingFactor <= 1; eastingFactor++)
                {
                    if (northingFactor == 0 && eastingFactor == 0)
                        continue;
                    CoordinateSharp.UniversalTransverseMercator utmTempGoal = new CoordinateSharp.UniversalTransverseMercator($"{latZone}{longZone}", easting + (distanceFromDeclaredGoal * eastingFactor), northing + (distanceFromDeclaredGoal * northingFactor));
                    CoordinateSharp.Coordinate tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmTempGoal);
                    goals.Add(new Coordinate(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS + additionalAltitude, declaration.DeclaredGoal.AltitudeBarometric + additionalAltitude, DateTime.Now));
                }
            }
            return goals;
        }

        private List<Coordinate> CalculateGoalsTask4_3(Track track)
        {
            List<Coordinate> goals = new List<Coordinate>();
            double distanceFromDeclaredGoal = 2000.0;
            double additionalAltitude = CoordinateHelpers.ConvertToMeter(600.0);
            Declaration declaration = track.GetLatestDeclaration(Task4_GoalNumber);
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task4_GoalNumber}' has been found");

            CoordinateSharp.Coordinate tempDeclaredGoal = new CoordinateSharp.Coordinate(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);

            double easting = Math.Round(tempDeclaredGoal.UTM.Easting, 0, MidpointRounding.AwayFromZero);
            double northing = Math.Round(tempDeclaredGoal.UTM.Northing, 0, MidpointRounding.AwayFromZero);
            string latZone = tempDeclaredGoal.UTM.LatZone;
            int longZone = tempDeclaredGoal.UTM.LongZone;

            for (int northingFactor = -1; northingFactor <= 1; northingFactor++)
            {
                for (int eastingFactor = -1; eastingFactor <= 1; eastingFactor++)
                {
                    if (northingFactor == 0 && eastingFactor == 0)
                        continue;
                    CoordinateSharp.UniversalTransverseMercator utmTempGoal = new CoordinateSharp.UniversalTransverseMercator($"{latZone}{longZone}", easting + (distanceFromDeclaredGoal * eastingFactor), northing + (distanceFromDeclaredGoal * northingFactor));
                    CoordinateSharp.Coordinate tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmTempGoal);
                    goals.Add(new Coordinate(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS + additionalAltitude, declaration.DeclaredGoal.AltitudeBarometric + additionalAltitude, DateTime.Now));
                }
            }
            return goals;
        }

        private List<Coordinate> CalculateGoalsTask5(Track track)
        {
            {
                List<Coordinate> goals = new List<Coordinate>();
                Declaration declaration = track.GetLatestDeclaration(Task5_GoalNumber);
                if (declaration is null || declaration == default)
                    throw new Exception($"No goal with goal number '{Task5_GoalNumber}' has been found");
                goals.Add(declaration.DeclaredGoal);
                return goals;
            }
        }

        private async Task<bool> ParseFilesAndCalculateResultsAsync(string[] igcFiles, HesitationWaltzTask task4_1, HesitationWaltzTask task4_2, HesitationWaltzTask task4_3, HesitationWaltzTask task5)
        {
            return await Task.Run(() =>
            {
                string functionErrorMessage = "Failed to parse files and calculate results: ";
                Track track;
                List<(string pilotIdenifier, int pilotNumber, double result_T4, double result_T5)> results = new List<(string pilotIdenifier, int pilotNumber, double result_T4, double result_T5)>();
                try
                {
                    DeclarationToGoalDistanceRule declarationToGoalDistanceRule = new DeclarationToGoalDistanceRule();
                    declarationToGoalDistanceRule.SetupRule(5000.0, double.NaN);
                    DeclarationToGoalHeightRule declarationToGoalHeightRule = new DeclarationToGoalHeightRule();
                    declarationToGoalHeightRule.SetupRule(CoordinateHelpers.ConvertToMeter(1000), double.NaN, DeclarationToGoalHeightRule.HeightDifferenceType.PositiveDifferenceOnly, true);
                    List<IDeclarationValidationRules> declarationValidationRules = new List<IDeclarationValidationRules>();
                    declarationValidationRules.Add(declarationToGoalDistanceRule);
                    declarationValidationRules.Add(declarationToGoalHeightRule);
                    foreach (string igcFile in igcFiles)
                    {
                        if (!Coordinates.Parsers.BalloonLiveParser.ParseFile(igcFile, out track))
                        {
                            Logger?.LogError("Failed to parse files and calculate results: Failed to parse '{igcFile}'", igcFile);
                            continue;
                        }
                        double result_Task4;
                        if (ValidationHelper.GetValidDeclaration(track, Task4_GoalNumber, declarationValidationRules) != null)
                        {
                            double result_Task4_1;
                            if (!task4_1.CalculateResults(track, true, out result_Task4_1))
                            {
                                Logger?.LogError("Failed to parse files and calculate results: Failed to calculate result at Task 4 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                                result_Task4_1 = double.NaN;
                            }
                            task4_1.Goals.Clear();

                            double result_Task4_2;
                            if (!task4_2.CalculateResults(track, true, out result_Task4_2))
                            {
                                Logger?.LogError("Failed to parse files and calculate results: Failed to calculate result at Task 4 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                                result_Task4_2 = double.NaN;
                            }
                            task4_2.Goals.Clear();

                            double result_Task4_3;
                            if (!task4_3.CalculateResults(track, true, out result_Task4_3))
                            {
                                Logger?.LogError("Failed to parse files and calculate results: Failed to calculate result at Task 4 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                                result_Task4_3 = double.NaN;
                            }
                            task4_3.Goals.Clear();

                            if (!double.IsNaN(result_Task4_1) && !double.IsNaN(result_Task4_2) && !double.IsNaN(result_Task4_3))
                            {
                                result_Task4 = result_Task4_1 + result_Task4_2 + result_Task4_3;
                                Logger?.LogInformation("Calculated result of '{result_Task4}m' ({result_Task4_1} + {result_Task4_2} + {result_Task4_3}) at Task 4 for Pilot No '{pilotNumber}'", Math.Round(result_Task4, 0, MidpointRounding.AwayFromZero), $"{result_Task4_1: 0.#}", $"{result_Task4_2: 0.#}",$"{result_Task4_3:0.#}", track.Pilot.PilotNumber);
                            }
                            else
                            {
                                result_Task4 = double.NaN;
                            }

                        }
                        else
                        {
                            result_Task4 = double.NaN;
                        }
                        double result_Task5;
                        Declaration declaration = ValidationHelper.GetValidDeclaration(track, Task5_GoalNumber, declarationValidationRules);
                        if (declaration != null)
                        {
                            task5.MarkerValidationRules.OfType<MarkerToGoalDistanceRule>().First().Declaration = declaration;
                            if (!task5.CalculateResults(track, true, out result_Task5))
                            {
                                Logger?.LogError("Failed to parse files and calculate results: Failed to calculate result at Task 5 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                                result_Task5 = double.NaN;
                            }
                            else
                            {
                                Logger?.LogInformation("Calculated result of '{result_Task5}m' at Task 5 for Pilot No '{pilotNumber}'", Math.Round(result_Task5, 0, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                            }
                            task5.Goals.Clear();
                        }
                        else
                        {
                            result_Task5 = double.NaN;
                        }
                        results.Add((track.Pilot.PilotIdentifier, track.Pilot.PilotNumber, result_Task4, result_Task5));
                    }

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    FileInfo resultsFileInternal = new FileInfo(Path.Combine(OutputDirectory.FullName, ResultsFileNameInternal));
                    using (ExcelPackage package = new ExcelPackage(resultsFileInternal))
                    {
                        ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                        if (wsResults == default)
                        {
                            wsResults = package.Workbook.Worksheets.Add("Results");
                            wsResults.Cells[1, 1].Value = "Pilot Identifier";
                            wsResults.Cells[1, 2].Value = "Pilot Number";
                            wsResults.Cells[1, 3].Value = "Task 4 PDG [m]";
                            wsResults.Cells[1, 4].Value = "Task 5 CRT [m]";
                            wsResults.Cells[1, 5].Value = "Task 4 Comment";
                            wsResults.Cells[1, 6].Value = "Task 5 Comment";

                        }
                        foreach ((string pilotIdentifier, int pilotNumber, double result_T4, double result_T5) result in results)
                        {
                            wsResults.Cells[result.pilotNumber + 1, 1].Value = result.pilotIdentifier;
                            wsResults.Cells[result.pilotNumber + 1, 2].Value = result.pilotNumber;
                            wsResults.Cells[result.pilotNumber + 1, 3].Value = Math.Round(result.result_T4, 0, MidpointRounding.AwayFromZero);
                            wsResults.Cells[result.pilotNumber + 1, 4].Value = Math.Round(result.result_T5, 0, MidpointRounding.AwayFromZero);

                            wsResults.Cells[result.pilotNumber + 1, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[result.pilotNumber + 1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                        }
                        wsResults.Cells.AutoFitColumns();
                        package.Save();
                    }
                    Logger?.LogInformation("Successfully created or modified internal results file '{resultsFileInternal.Name}'",resultsFileInternal.Name);
                    FileInfo resultsFileProvisional = new FileInfo(Path.Combine(OutputDirectory.FullName, ResultsFileNameProvisional));
                    using (ExcelPackage package = new ExcelPackage(resultsFileProvisional))
                    {
                        ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                        if (wsResults == default)
                        {
                            wsResults = package.Workbook.Worksheets.Add("Results");
                            wsResults.Cells[1, 1].Value = "Pilot Identifier";
                            wsResults.Cells[1, 2].Value = "Pilot Number";
                            wsResults.Cells[1, 3].Value = "Pilot Last Name";
                            wsResults.Cells[1, 4].Value = "Pilot First Name";
                            wsResults.Cells[1, 5].Value = "Task 4 PDG [m]";
                            wsResults.Cells[1, 6].Value = "Task 5 CRT [m]";

                        }
                        string firstName;
                        string lastName;
                        foreach ((string pilotIdentifier, int pilotNumber, double result_T4, double result_T5) result in results)
                        {
                            wsResults.Cells[result.pilotNumber + 1, 1].Value = result.pilotIdentifier;
                            wsResults.Cells[result.pilotNumber + 1, 2].Value = result.pilotNumber;
                            if (!PilotMapping.GetPilotName(result.pilotNumber, out lastName, out firstName))
                            {
                                Logger?.LogWarning("Last name and first name of pilot will be omitted");
                            }
                            else
                            {
                                wsResults.Cells[result.pilotNumber + 1, 3].Value = lastName;
                                wsResults.Cells[result.pilotNumber + 1, 4].Value = firstName;
                            }
                            wsResults.Cells[result.pilotNumber + 1, 5].Value = Math.Round(result.result_T4, 0, MidpointRounding.AwayFromZero);
                            wsResults.Cells[result.pilotNumber + 1, 6].Value = Math.Round(result.result_T5, 0, MidpointRounding.AwayFromZero);


                            wsResults.Cells[result.pilotNumber + 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[result.pilotNumber + 1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                        }
                        wsResults.Cells.AutoFitColumns();
                        package.Save();
                    }
                    Logger?.LogInformation("Successfully created or modified provisional results file '{resultsFileProvisional.Name}'", resultsFileProvisional.Name);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, functionErrorMessage);
                    return false;
                }

                return true;
            });
        }
        #endregion

        #region Fiddle Task4
        private void btCalculateTask4_Click(object sender, EventArgs e)
        {

        }



        #endregion

        #region Fiddle Task5
        #endregion

        #endregion

    }
}
