using Competition;
using Coordinates;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLC2021
{
    public partial class BLC2021TaskSheet1 : Form
    {
        #region Properties

        private readonly ILogger<BLC2021TaskSheet1> Logger;

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

        private int Task1_MarkerNumber
        {
            get; set;
        } = 1;

        private int Task1_GoalNumber
        {
            get; set;
        } = 1;

        private int Task2_FirstMarkerNumber
        {
            get; set;
        } = 2;

        private int Task2_SecondMarkerNumber
        {
            get; set;
        } = 3;

        private int Task2_ThirdMarkerNumber
        {
            get; set;
        } = 4;

        private int Task3_GoalNumber
        {
            get; set;
        } = 2;

        private int Task3_MarkerNumber
        {
            get; set;
        } = 5;

        private double Task3_DistanceDeclarationToGoal
        {
            get; set;
        } = 3000;

        private double Task3_AltitudeDifferenceFeet
        {
            get; set;
        } = 1000;

        private readonly string ResultsFileNameInternal = "BLC2021TaskSheet1_Results_Internal.xlsx";
        private readonly string ResultsFileNameProvisional = "BLC2021TaskSheet1_Results_Provisional.xlsx";
        #endregion

        #region Constructor

        public BLC2021TaskSheet1(bool batchMode)
        {
            InitializeComponent();
            BatchMode = batchMode;

            Text = ToString();

        }

        #endregion


        #region Methods
        #region General

        private void BLC2021TaskSheet1_Load(object sender, EventArgs e)
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
                using FolderBrowserDialog folderBrowserDialog = new();
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
            else
            {
                if (!btSelectIGCFiles.Enabled)
                    btSelectIGCFiles.Enabled = true;
            }

        }
        private void ConfigureForBatchMode()
        {
            btSelectIGCFiles.Text = "Select IGC Files";
            //////////////////////////
            ///Task1
            //////////////////////////
            btCalculateTask1.Visible = false;
            cbGoalTask1.Visible = false;
            lbGoalNumberTask1.Visible = true;
            cbMarkerTask1.Visible = false;
            lbMarkerNumberTask1.Visible = true;

            lbZoneTask1.Visible = false;
            tbZoneTask1.Visible = false;
            lbCommentTask1.Visible = false;
            tbCommentTask1.Visible = false;
            lbEastingTask1.Visible = false;
            lbNorthingTask1.Visible = false;
            tbNorthingTask1.Visible = false;
            tbEastingTask1.Visible = false;
            lbAltitudeTask1.Visible = false;
            tbAltitudeTask1.Visible = false;

            Task1_GoalNumber = 1;
            Task1_MarkerNumber = 1;
            //////////////////////////
            ///Task2
            //////////////////////////
            btCalculateTask2.Visible = false;
            cbFristMarkerNumber.Visible = false;
            lbFirstMarkerNumber.Visible = true;
            cbSecondMarkerNumber.Visible = false;
            lbSecondMarkerNumber.Visible = true;
            cbThirdMarkerNumber.Visible = false;
            lbThirdMarkerNumber.Visible = true;
            tbCommentTask2.Visible = false;
            lbCommentTask2.Visible = false;

            Task2_FirstMarkerNumber = 2;
            Task2_SecondMarkerNumber = 3;
            Task2_ThirdMarkerNumber = 4;

            //////////////////////////
            ///Task3
            //////////////////////////
            btCalculateTask3.Visible = false;
            cbGoalTask3.Visible = false;
            lbGoalTask3.Visible = true;
            cbMarkerTask3.Visible = false;
            lbMakerTask3.Visible = true;

            lbZoneTask3.Visible = false;
            tbZoneTask3.Visible = false;
            lbCommentTask3.Visible = false;
            tbCommentTask3.Visible = false;
            lbEastingTask3.Visible = false;
            tbEastingTask3.Visible = false;
            lbNorthingTask3.Visible = false;
            tbNorthingTask3.Visible = false;
            lbAltitudeTask3.Visible = false;
            tbAltitudeTask3.Visible = false;

            Task3_GoalNumber = 2;
            Task3_MarkerNumber = 5;
            richTextBox6.Text = " * Declared goal at least 1000ft higher than declaration position\r\n * Minimum distance of 3km between declaration and goal\r\n * Use 3D distance between marker and goal\r\n * Divide marker to goal distance(in m) through declaration to goal distance(in km)";

        }

        private void ConfigureForFiddleMode()
        {

            btSelectIGCFiles.Text = "Select IGC File";
            //////////////////////////
            ///Task2
            //////////////////////////
            btCalculateTask1.Visible = true;
            cbGoalTask1.Visible = true;
            lbGoalNumberTask1.Visible = false;
            cbMarkerTask1.Visible = true;
            lbMarkerNumberTask1.Visible = false;

            lbZoneTask1.Visible = true;
            tbZoneTask1.Visible = true;
            lbCommentTask1.Visible = true;
            tbCommentTask1.Visible = true;
            lbEastingTask1.Visible = true;
            tbEastingTask1.Visible = true;
            lbNorthingTask1.Visible = true;
            tbNorthingTask1.Visible = true;
            lbAltitudeTask1.Visible = true;
            tbAltitudeTask1.Visible = true;


            Task1_GoalNumber = 99;
            Task1_MarkerNumber = -1;

            //////////////////////////
            ///Task2
            //////////////////////////
            btCalculateTask2.Visible = true;
            cbFristMarkerNumber.Visible = true;
            lbFirstMarkerNumber.Visible = false;
            cbSecondMarkerNumber.Visible = true;
            lbSecondMarkerNumber.Visible = false;
            cbThirdMarkerNumber.Visible = true;
            lbThirdMarkerNumber.Visible = false;
            tbCommentTask2.Visible = true;
            lbCommentTask2.Visible = true;
            Task2_FirstMarkerNumber = -1;
            Task2_SecondMarkerNumber = -1;
            Task2_ThirdMarkerNumber = -1;

            //////////////////////////
            ///Task3
            //////////////////////////
            btCalculateTask3.Visible = true;
            cbGoalTask3.Visible = true;
            lbGoalTask3.Visible = false;
            cbMarkerTask3.Visible = true;
            lbMakerTask3.Visible = false;

            lbZoneTask3.Visible = true;
            tbZoneTask3.Visible = true;
            lbCommentTask3.Visible = true;
            tbCommentTask3.Visible = true;
            lbEastingTask3.Visible = true;
            tbEastingTask3.Visible = true;
            lbNorthingTask3.Visible = true;
            tbNorthingTask3.Visible = true;
            lbAltitudeTask3.Visible = true;
            tbAltitudeTask3.Visible = true;

            Task3_GoalNumber = 999;
            Task3_MarkerNumber = -1;
            richTextBox6.Select(0, 122);
            richTextBox6.SelectionFont = new Font(richTextBox6.Font, FontStyle.Strikeout);
            //richTextBox6.Text = " * Use 3D distance between marker and goal\r\n * Divide marker to goal distance(in m) through declaration to goal distance(in km)";

        }
        public override string ToString()
        {
            if (BatchMode)
                return $"BLC 2021 Task Sheet 1 (Batch Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
            else
                return $"BLC 2021 Task Sheet 1 (Fiddle Mode) v{typeof(BLC2021TaskSheet1).Assembly.GetName().Version}";
        }

        private async void btSelectIGCFiles_Click(object sender, EventArgs e)
        {
            if (BatchMode)
            {
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Select .igc files",
                    Filter = "igc files (*.igc)|*.igc",
                    CheckPathExists = true
                };
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = true;
                UseWaitCursor = true;

                HesitationWaltzTask task1 = new();
                task1.SetupHWZ(1, CalculateGoals, Task1_MarkerNumber, true, null);
                LandRunTask task2 = new();
                task2.SetupLandRun(2, Task2_FirstMarkerNumber, Task2_SecondMarkerNumber, Task2_ThirdMarkerNumber, null);


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] igcFiles = openFileDialog.FileNames;

                    bool success = await ParseFilesAndCalculateResultsAsync(igcFiles, task1, task2);
                    if (success)
                    {
                        Logger?.LogInformation("Parsing and result calculation completed");
                    }
                    else
                    {
                        Logger?.LogError("Parsing and result calculation failed");
                    }
                }
                UseWaitCursor = false;
            }
            else//Fiddle mode
            {
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Select .igc files",
                    Filter = "igc files (*.igc)|*.igc",
                    CheckPathExists = true
                };
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!Coordinates.Parsers.BalloonLiveParser.ParseFile(openFileDialog.FileName, out Track track))
                    {
                        Logger?.LogError("Failed to parse '{fileName}'", Path.GetFileName(openFileDialog.FileName));
                        return;
                    }
                    if (track.AdditionalPropertiesFromIGCFile.TryGetValue("Change of position source", out string changeOfPositionSource))
                    {
                        if (changeOfPositionSource.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                        {
                            MessageBox.Show("Caution: change of position source has been detected, refer the log for more details");
                        }
                    }
                    FiddleTrack = track;
                    cbMarkerTask1.Items.Clear();
                    cbFristMarkerNumber.Items.Clear();
                    cbSecondMarkerNumber.Items.Clear();
                    cbThirdMarkerNumber.Items.Clear();
                    cbMarkerTask3.Items.Clear();
                    foreach (MarkerDrop markerDrop in track.MarkerDrops.OrderBy(x => x.MarkerNumber))
                    {
                        cbMarkerTask1.Items.Add(markerDrop.MarkerNumber);
                        cbFristMarkerNumber.Items.Add(markerDrop.MarkerNumber);
                        cbSecondMarkerNumber.Items.Add(markerDrop.MarkerNumber);
                        cbThirdMarkerNumber.Items.Add(markerDrop.MarkerNumber);
                        cbMarkerTask3.Items.Add(markerDrop.MarkerNumber);
                    }
                    cbGoalTask1.Items.Clear();
                    cbGoalTask3.Items.Clear();
                    foreach (Declaration declaration in track.Declarations.OrderBy(x => x.GoalNumber).ThenByDescending(x => x.PositionAtDeclaration.TimeStamp))
                    {
                        CoordinateSharp.Coordinate tempCoordinate = new(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);
                        cbGoalTask1.Items.Add($"No.{declaration.GoalNumber} {Math.Round(tempCoordinate.UTM.Easting, 0, MidpointRounding.AwayFromZero)} , {Math.Round(tempCoordinate.UTM.Northing, 0, MidpointRounding.AwayFromZero)} / {declaration.DeclaredGoal.AltitudeGPS}");
                        cbGoalTask3.Items.Add($"No.{declaration.GoalNumber} {Math.Round(tempCoordinate.UTM.Easting, 0, MidpointRounding.AwayFromZero)} , {Math.Round(tempCoordinate.UTM.Northing, 0, MidpointRounding.AwayFromZero)} / {declaration.DeclaredGoal.AltitudeGPS}");
                    }
                    btCalculateTask1.Enabled = true;
                    btCalculateTask2.Enabled = true;
                    btCalculateTask3.Enabled = true;
                }
            }
        }
        #endregion


        #region Batch Mode
        private List<Coordinate> CalculateGoals(Track track)
        {
            Declaration declaration = track.Declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).FirstOrDefault(x => x.GoalNumber == Task1_GoalNumber);
            double distanceFromDeclaredGoal = 1000.0;
            List<Coordinate> goals = [];
            if (declaration is null || declaration == default)
                throw new Exception($"No goal with goal number '{Task1_GoalNumber}' has been found");

            CoordinateSharp.Coordinate tempDeclaredGoal = new(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);

            double easting = Math.Round(tempDeclaredGoal.UTM.Easting, 0, MidpointRounding.AwayFromZero);
            double northing = Math.Round(tempDeclaredGoal.UTM.Northing, 0, MidpointRounding.AwayFromZero);
            string latZone = tempDeclaredGoal.UTM.LatZone;
            int longZone = tempDeclaredGoal.UTM.LongZone;

            double northingNorth = Math.Round(northing + distanceFromDeclaredGoal, 0, MidpointRounding.AwayFromZero);
            double northingSouth = Math.Round(northing - distanceFromDeclaredGoal, 0, MidpointRounding.AwayFromZero);

            double eastingEast = Math.Round(easting + distanceFromDeclaredGoal, 0, MidpointRounding.AwayFromZero);
            double eastingWest = Math.Round(easting - distanceFromDeclaredGoal, 0, MidpointRounding.AwayFromZero);



            CoordinateSharp.UniversalTransverseMercator utmGoalNorth = new($"{latZone}{longZone}", easting, northingNorth);
            CoordinateSharp.Coordinate tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmGoalNorth);
            Coordinate goalNorth = new(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS, declaration.DeclaredGoal.AltitudeBarometric, DateTime.Now);

            CoordinateSharp.UniversalTransverseMercator utmGoalEast = new($"{latZone}{longZone}", eastingEast, northing);
            tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmGoalEast);
            Coordinate goalEast = new(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS, declaration.DeclaredGoal.AltitudeBarometric, DateTime.Now);

            CoordinateSharp.UniversalTransverseMercator utmGoalSouth = new($"{latZone}{longZone}", easting, northingSouth);
            tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmGoalSouth);
            Coordinate goalSouth = new(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS, declaration.DeclaredGoal.AltitudeBarometric, DateTime.Now);

            CoordinateSharp.UniversalTransverseMercator utmGoalWest = new($"{latZone}{longZone}", eastingWest, northing);
            tempGoal = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoLatLong(utmGoalWest);
            Coordinate goalWest = new(tempGoal.Latitude.DecimalDegree, tempGoal.Longitude.DecimalDegree, declaration.DeclaredGoal.AltitudeGPS, declaration.DeclaredGoal.AltitudeBarometric, DateTime.Now);

            goals.Add(goalNorth);
            goals.Add(goalEast);
            goals.Add(goalSouth);
            goals.Add(goalWest);


            return goals;
        }


        private async Task<bool> ParseFilesAndCalculateResultsAsync(string[] igcFiles, HesitationWaltzTask task1, LandRunTask task2)
        {
            return await Task.Run(() =>
            {
                string functionErrorMessage = "Failed to parse files and calculate results: ";
                List<(string pilotIdentifier, int pilotNumber, double result_T1, double result_T2, double result_T3)> results = [];
                try
                {

                    foreach (string igcFile in igcFiles)
                    {
                        if (!Coordinates.Parsers.BalloonLiveParser.ParseFile(igcFile, out Track track))
                        {
                            Logger?.LogError("Failed to parse files and calculate results: cannot parse '{igcFile}'", igcFile);
                            continue;
                        }
                        if (track.AdditionalPropertiesFromIGCFile.TryGetValue("Change of position source", out string changeOfPositionSource))
                        {
                            if (changeOfPositionSource.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                            {
                                MessageBox.Show("Caution: change of position source has been detected, refer the log for more details");
                            }
                        }

                        if (!task1.CalculateResults(track, true, out double result_Task1))
                        {
                            Logger?.LogError("Failed to parse files and calculate results: cannot calculate result at Task 1 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                            result_Task1 = double.NaN;
                        }
                        else
                        {
                            Logger?.LogInformation("Calculated result of '{result_Task1}m' at Task 1 for Pilot No '{pilotNumber}'", Math.Round(result_Task1, 3, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                        }
                        task1.Goals.Clear();
                        if (!task2.CalculateResults(track, true, out double result_Task2))
                        {
                            Logger?.LogError("Failed to parse files and calculate results: cannot calculate result at Task 2 for Pilot No '{pilotNumber}'", track.Pilot.PilotNumber);
                            result_Task2 = double.NaN;
                        }
                        else
                        {
                            Logger?.LogInformation("Calculated result of '{result_Task2}km²' at Task 2 for Pilot No '{pilotNumber}'", Math.Round(result_Task2 / 1.0e6, 3, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                        }

                        double result_Task3;
                        List<Declaration> declarations = track.Declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).Where(x => x.GoalNumber == Task3_GoalNumber).ToList();
                        bool isValid = false;
                        Declaration selectedDeclaration = null;
                        double distanceDeclarationToGoal_Task3 = double.NaN;
                        foreach (Declaration declaration in declarations)
                        {
                            double tempDistance = CoordinateHelpers.Calculate2DDistanceHavercos(declaration.PositionAtDeclaration, declaration.DeclaredGoal);
                            if (tempDistance > Task3_DistanceDeclarationToGoal && declaration.PositionAtDeclaration.AltitudeGPS <= declaration.DeclaredGoal.AltitudeGPS - CoordinateHelpers.ConvertToMeter(Task3_AltitudeDifferenceFeet))
                            {
                                isValid = true;
                                distanceDeclarationToGoal_Task3 = tempDistance;
                                selectedDeclaration = declaration;
                                break;
                            }
                        }
                        if (!isValid)
                        {
                            Logger?.LogError("Failed to parse files and calculate results: cannot calculate result at Task 3 for Pilot No '{pilotNumber}' as no declaration is valid", track.Pilot.PilotNumber);
                            result_Task3 = double.NaN;
                        }
                        else
                        {
                            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == Task3_MarkerNumber);
                            if (markerDrop == default)
                            {
                                Logger?.LogError("Failed to parse files and calculate results: cannot calculate result at Task 3 for Pilot No '{pilotNumber}' as no Marker with Maker Number '{Task3_MarkerNumber}' exists", track.Pilot.PilotNumber, Task3_MarkerNumber);
                                result_Task3 = double.NaN;
                            }
                            else
                            {
                                double distanceGoalToMarker_Task3 = CoordinateHelpers.Calculate3DDistance(selectedDeclaration.DeclaredGoal, markerDrop.MarkerLocation, true);
                                result_Task3 = distanceGoalToMarker_Task3 / (distanceDeclarationToGoal_Task3 / 1000.0);

                                Logger?.LogInformation("Task 3 Pilot No {pilotNumber}: Distance between declaration position and declared goal is '{distanceDeclarationToGoal_Task3}m' / Distance goal to marker is '{distanceGoalToMarker_Task3}km'", track.Pilot.PilotNumber, Math.Round(distanceDeclarationToGoal_Task3, 3, MidpointRounding.AwayFromZero), Math.Round(distanceGoalToMarker_Task3 / 1000.0, 3, MidpointRounding.AwayFromZero));
                                Logger?.LogInformation("Calculated result of '{result_Task3}m/km' at Task 3 for Pilot No '{pilotNumber}'", Math.Round(result_Task3, 6, MidpointRounding.AwayFromZero), track.Pilot.PilotNumber);
                            }
                        }

                        results.Add((track.Pilot.PilotIdentifier, track.Pilot.PilotNumber, result_Task1, result_Task2, result_Task3));

                    }
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    FileInfo resultsFileInternal = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameInternal));
                    using (ExcelPackage package = new(resultsFileInternal))
                    {
                        ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                        if (wsResults == default)
                        {
                            wsResults = package.Workbook.Worksheets.Add("Results");
                            wsResults.Cells[1, 1].Value = "Pilot Identifier";
                            wsResults.Cells[1, 2].Value = "Pilot Number";
                            wsResults.Cells[1, 3].Value = "Task 1 HWZ [m]";
                            wsResults.Cells[1, 4].Value = "Task 2 LRN [km²]";
                            wsResults.Cells[1, 5].Value = "Task 3 PGD [m/km]";
                            wsResults.Cells[1, 6].Value = "Task 1 Comment";
                            wsResults.Cells[1, 7].Value = "Task 2 Comment";
                            wsResults.Cells[1, 8].Value = "Task 3 Comment";
                        }
                        foreach ((string pilotIdentifier, int pilotNumber, double result_T1, double result_T2, double result_T3) in results)
                        {
                            wsResults.Cells[pilotNumber + 1, 1].Value = pilotIdentifier;
                            wsResults.Cells[pilotNumber + 1, 2].Value = pilotNumber;
                            wsResults.Cells[pilotNumber + 1, 3].Value = Math.Round(result_T1, 0, MidpointRounding.AwayFromZero);
                            wsResults.Cells[pilotNumber + 1, 4].Value = Math.Round(result_T2 / 1.0e6, 3, MidpointRounding.AwayFromZero);
                            wsResults.Cells[pilotNumber + 1, 5].Value = Math.Round(result_T3, 3, MidpointRounding.AwayFromZero);

                            wsResults.Cells[pilotNumber + 1, 3].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[pilotNumber + 1, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[pilotNumber + 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                        }
                        wsResults.Cells.AutoFitColumns();
                        package.Save();
                    }
                    Logger?.LogInformation("Successfully created or modified internal results file '{resultsFile}'", resultsFileInternal.Name);
                    FileInfo resultsFileProvisional = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameProvisional));
                    using (ExcelPackage package = new(resultsFileProvisional))
                    {
                        ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                        if (wsResults == default)
                        {
                            wsResults = package.Workbook.Worksheets.Add("Results");
                            wsResults.Cells[1, 1].Value = "Pilot Identifier";
                            wsResults.Cells[1, 2].Value = "Pilot Number";
                            wsResults.Cells[1, 3].Value = "Pilot Last Name";
                            wsResults.Cells[1, 4].Value = "Pilot First Name";
                            wsResults.Cells[1, 5].Value = "Task 1 HWZ [m]";
                            wsResults.Cells[1, 6].Value = "Task 2 LRN [km²]";
                            wsResults.Cells[1, 7].Value = "Task 3 PGD [m/km]";
                        }
                        foreach ((string pilotIdentifier, int pilotNumber, double result_T1, double result_T2, double result_T3) in results)
                        {
                            wsResults.Cells[pilotNumber + 1, 1].Value = pilotIdentifier;
                            wsResults.Cells[pilotNumber + 1, 2].Value = pilotNumber;
                            if (!PilotMapping.GetPilotName(pilotNumber, out string lastName, out string firstName))
                            {
                                Logger?.LogWarning("Last name and first name of pilot No '{pilotNumber}' will be omitted", pilotNumber);
                            }
                            else
                            {
                                wsResults.Cells[pilotNumber + 1, 3].Value = lastName;
                                wsResults.Cells[pilotNumber + 1, 4].Value = firstName;
                            }
                            wsResults.Cells[pilotNumber + 1, 5].Value = Math.Round(result_T1, 0, MidpointRounding.AwayFromZero);
                            wsResults.Cells[pilotNumber + 1, 6].Value = Math.Round(result_T2 / 1.0e6, 3, MidpointRounding.AwayFromZero);
                            wsResults.Cells[pilotNumber + 1, 7].Value = Math.Round(result_T3, 3, MidpointRounding.AwayFromZero);

                            wsResults.Cells[pilotNumber + 1, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[pilotNumber + 1, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                            wsResults.Cells[pilotNumber + 1, 7].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                        }
                        wsResults.Cells.AutoFitColumns();
                        package.Save();
                    }
                    Logger?.LogInformation("Successfully created or modified provisional results file '{resultsFileProvisional}'",resultsFileProvisional.Name);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Failed to parse files and calculate results");
                    return false;
                }
                return true;


            });
        }
        #endregion

        #region Fiddle Task 1

        private void cbGoalTask1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Declaration selectedDeclaration = FiddleTrack.Declarations.OrderBy(x => x.GoalNumber).ThenByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[cbGoalTask1.SelectedIndex];
            CoordinateSharp.Coordinate tempCoordinate = new(selectedDeclaration.DeclaredGoal.Latitude, selectedDeclaration.DeclaredGoal.Longitude);
            tbZoneTask1.Text = $"{tempCoordinate.UTM.LongZone}{tempCoordinate.UTM.LatZone}";
            tbEastingTask1.Text = ((int)(Math.Round(tempCoordinate.UTM.Easting, 0, MidpointRounding.AwayFromZero))).ToString("D6");
            tbNorthingTask1.Text = ((int)(Math.Round(tempCoordinate.UTM.Northing, 0, MidpointRounding.AwayFromZero))).ToString("D7");
            tbAltitudeTask1.Text = Math.Round(selectedDeclaration.DeclaredGoal.AltitudeGPS, 3, MidpointRounding.AwayFromZero).ToString();
        }

        private void cbMarkerTask1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task1_MarkerNumber = (int)cbMarkerTask1.SelectedItem;
        }

        private void btCalculateTask1_Click(object sender, EventArgs e)
        {
            List<string> messages = [];
            if (Task1_MarkerNumber < 0)
            {
                messages.Add("Please select a marker number first");
            }
            int easting = -1;
            int northing = -1;
            double altitude = double.NaN;
            string gridZone = string.Empty;
            if (string.IsNullOrWhiteSpace(tbZoneTask1.Text))
            {
                messages.Add("Please select goal or enter a grid zone");
            }
            else
            {
                if (tbZoneTask1.Text.Length != 3)
                {
                    messages.Add("Unexpected format for gird zone. Please enter 2 digits and 1 character");
                }
                else
                {
                    gridZone = tbZoneTask1.Text;
                }
            }
            if (string.IsNullOrWhiteSpace(tbEastingTask1.Text))
            {
                messages.Add("Please select a goal or enter a easting");
            }
            else
            {
                if (tbEastingTask1.Text.Length != 6)
                {
                    messages.Add("Only 6/7 declaration are currently supported. Please enter 6 digits");
                }
                else
                {
                    if (!int.TryParse(tbEastingTask1.Text, out easting))
                    {
                        messages.Add("Failed to convert easting to integer. Please check entry");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(tbNorthingTask1.Text))
            {
                messages.Add("Please select a goal or enter a northing");
            }
            else
            {
                if (tbNorthingTask1.Text.Length != 7)
                {
                    messages.Add("Only 6/7 declaration are currently supported. Please enter 7 digits");
                }
                else
                {
                    if (!int.TryParse(tbNorthingTask1.Text, out northing))
                    {
                        messages.Add("Failed to convert northing to integer. Please check entry");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(tbAltitudeTask1.Text))
            {
                messages.Add("Please select a goal or enter an altitude");
            }
            else
            {
                if (!double.TryParse(tbAltitudeTask1.Text, out altitude))
                {
                    messages.Add("Failed to convert altitude to double. Please check entry");
                }
            }
            if (messages.Count > 0)
            {
                foreach (string message in messages)
                {
                    Logger?.LogError("Cannot calculate Task 1: {message}", message);
                }
                MessageBox.Show(string.Join("\r\n", messages));
                return;
            }
            CoordinateSharp.UniversalTransverseMercator tempUTM = new(gridZone, easting, northing);
            double[] latLong = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoSignedDegree(tempUTM);
            Coordinate coordinate = new(latLong[0], latLong[1], altitude, altitude, DateTime.Now);
            Declaration fiddleDeclaration = new(Task1_GoalNumber, coordinate, coordinate, true, -1, -1);
            FiddleTrack.Declarations.Add(fiddleDeclaration);

            HesitationWaltzTask task1 = new();
            task1.SetupHWZ(1, CalculateGoals, Task1_MarkerNumber, true, null);

            if (!task1.CalculateResults(FiddleTrack, true, out double result))
            {
                Logger?.LogError("Failed to calculate result at Task 1 for Pilot No '{pilotNumber}'", FiddleTrack.Pilot.PilotNumber);
                return;
            }
            else
            {
                Logger?.LogError("Calculated result of '{result}m' at Task 1 for Pilot No '{pilotNumber}'", Math.Round(result, 3, MidpointRounding.AwayFromZero), FiddleTrack.Pilot.PilotNumber);
            }
            try
            {

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                FileInfo resultsFileInternal = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameInternal));
                using (ExcelPackage package = new(resultsFileInternal))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 4].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 5].Value = "Task 3 PGD [m/km]";
                        wsResults.Cells[1, 6].Value = "Task 1 Comment";
                        wsResults.Cells[1, 7].Value = "Task 2 Comment";
                        wsResults.Cells[1, 8].Value = "Task 3 Comment";
                    }

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 3].Value = Math.Round(result, 0, MidpointRounding.AwayFromZero);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 6].Value = "Fiddled: " + tbCommentTask1.Text;

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 3].Style.Fill.SetBackground(Color.Orange);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 6].Style.Fill.SetBackground(Color.Orange);

                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified internal results file '{resultsFileInternal}'", resultsFileInternal.Name);
                FileInfo resultsFileProvisional = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameProvisional));
                using (ExcelPackage package = new(resultsFileProvisional))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Pilot Last Name";
                        wsResults.Cells[1, 4].Value = "Pilot First Name";
                        wsResults.Cells[1, 5].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 6].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 7].Value = "Task 3 PGD [m/km]";
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    if (!PilotMapping.GetPilotName(FiddleTrack.Pilot.PilotNumber, out string lastName, out string firstName))
                    {
                        Logger?.LogWarning("Last name and first name of pilot will be omitted");
                    }
                    else
                    {
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 3].Value = lastName;
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 4].Value = firstName;
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 5].Value = Math.Round(result, 0, MidpointRounding.AwayFromZero);


                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified provisional results file '{resultsFileProvisional}'", resultsFileProvisional.Name);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to create or modified results file");
                return;
            }
        }
        #endregion

        #region Fiddle Task 2

        private void cbFristMarkerNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task2_FirstMarkerNumber = (int)cbFristMarkerNumber.SelectedItem;
        }
        private void cbSecondMarkerNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task2_SecondMarkerNumber = (int)cbSecondMarkerNumber.SelectedItem;
        }

        private void cbThirdMarkerNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task2_ThirdMarkerNumber = (int)cbThirdMarkerNumber.SelectedItem;
        }
        private void btCalculateTask2_Click(object sender, EventArgs e)
        {
            List<string> messages = [];
            if (Task2_FirstMarkerNumber < 0)
            {
                messages.Add("Please select a number for the 1rst marker");
            }
            if (Task2_SecondMarkerNumber < 0)
            {
                messages.Add("Please select a number for the 2nd marker");
            }
            if (Task2_ThirdMarkerNumber < 0)
            {
                messages.Add("Please select a number for the 3rd marker");
            }
            if (messages.Count > 0)
            {
                foreach (string message in messages)
                {
                    Logger?.LogError("Cannot calculate Task 2: {message}", message);
                }
                MessageBox.Show(string.Join("\r\n", messages));
                return;
            }
            LandRunTask task2 = new();
            task2.SetupLandRun(2, Task2_FirstMarkerNumber, Task2_SecondMarkerNumber, Task2_ThirdMarkerNumber, null);

            if (!task2.CalculateResults(FiddleTrack, true, out double result))
            {
                Logger?.LogError("Failed to calculate result at Task 2 for Pilot No '{pilotNumber}'", FiddleTrack.Pilot.PilotNumber);
                return;
            }
            else
            {
                Logger?.LogInformation("Calculated result of '{result}km²' at Task 2 for Pilot No '{pilotNumber}'", Math.Round(result / 1.0e6, 3, MidpointRounding.AwayFromZero), FiddleTrack.Pilot.PilotNumber);
            }
            try
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo resultsFileInternal = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameInternal));
                using (ExcelPackage package = new(resultsFileInternal))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 4].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 5].Value = "Task 3 PGD [m/km]";
                        wsResults.Cells[1, 6].Value = "Task 1 Comment";
                        wsResults.Cells[1, 7].Value = "Task 2 Comment";
                        wsResults.Cells[1, 8].Value = "Task 3 Comment";
                    }

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 4].Value = Math.Round(result / 1.0e6, 3, MidpointRounding.AwayFromZero);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 7].Value = "Fiddled: " + tbCommentTask2.Text;

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 4].Style.Fill.SetBackground(Color.Orange);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 7].Style.Fill.SetBackground(Color.Orange);

                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified internal results file '{resultsFileInternal}'", resultsFileInternal.Name);
                FileInfo resultsFileProvisional = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameProvisional));
                using (ExcelPackage package = new(resultsFileProvisional))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Pilot Last Name";
                        wsResults.Cells[1, 4].Value = "Pilot First Name";
                        wsResults.Cells[1, 5].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 6].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 7].Value = "Task 3 PGD [m/km]";
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    if (!PilotMapping.GetPilotName(FiddleTrack.Pilot.PilotNumber, out string lastName, out string firstName))
                    {
                        Logger?.LogWarning("Last name and first name of pilot will be omitted");
                    }
                    else
                    {
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 3].Value = lastName;
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 4].Value = firstName;
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 6].Value = Math.Round(result / 1.0e6, 3, MidpointRounding.AwayFromZero);


                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified provisional results file '{resultsFileProvisional}'", resultsFileProvisional.Name);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to create or modified results file");
                return;
            }
        }



        #endregion

        #region Fiddle Task 3

        private void cbGoalTask3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Declaration selectedDeclaration = FiddleTrack.Declarations.OrderBy(x => x.GoalNumber).ThenByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[cbGoalTask3.SelectedIndex];
            CoordinateSharp.Coordinate tempCoordinate = new(selectedDeclaration.DeclaredGoal.Latitude, selectedDeclaration.DeclaredGoal.Longitude);
            tbZoneTask3.Text = $"{tempCoordinate.UTM.LongZone}{tempCoordinate.UTM.LatZone}";
            tbEastingTask3.Text = ((int)(Math.Round(tempCoordinate.UTM.Easting, 0, MidpointRounding.AwayFromZero))).ToString("D6");
            tbNorthingTask3.Text = ((int)(Math.Round(tempCoordinate.UTM.Northing, 0, MidpointRounding.AwayFromZero))).ToString("D7");
            tbAltitudeTask3.Text = Math.Round(selectedDeclaration.DeclaredGoal.AltitudeGPS, 3, MidpointRounding.AwayFromZero).ToString();
        }

        private void cbMarkerTask3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Task3_MarkerNumber = (int)cbMarkerTask3.SelectedItem;
        }

        private void btCalculateTask3_Click(object sender, EventArgs e)
        {
            List<string> messages = [];
            if (Task3_MarkerNumber < 0)
            {
                messages.Add("Please select a marker number first");
            }
            int easting = -1;
            int northing = -1;
            double altitude = double.NaN;
            string gridZone = string.Empty;
            if (cbGoalTask3.SelectedItem == null)
            {
                messages.Add("Please select a goal. The position at declaration is needed");
            }
            if (string.IsNullOrWhiteSpace(tbZoneTask3.Text))
            {
                messages.Add("Please select goal or enter a grid zone");
            }
            else
            {
                if (tbZoneTask3.Text.Length != 3)
                {
                    messages.Add("Unexpected format for gird zone. Please enter 2 digits and 1 character");
                }
                else
                {
                    gridZone = tbZoneTask3.Text;
                }
            }
            if (string.IsNullOrWhiteSpace(tbEastingTask3.Text))
            {
                messages.Add("Please select a goal or enter a easting");
            }
            else
            {
                if (tbEastingTask3.Text.Length != 6)
                {
                    messages.Add("Only 6/7 declaration are currently supported. Please enter 6 digits");
                }
                else
                {
                    if (!int.TryParse(tbEastingTask3.Text, out easting))
                    {
                        messages.Add("Failed to convert easting to integer. Please check entry");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(tbNorthingTask3.Text))
            {
                messages.Add("Please select a goal or enter a northing");
            }
            else
            {
                if (tbNorthingTask3.Text.Length != 7)
                {
                    messages.Add("Only 6/7 declaration are currently supported. Please enter 7 digits");
                }
                else
                {
                    if (!int.TryParse(tbNorthingTask3.Text, out northing))
                    {
                        messages.Add("Failed to convert northing to integer. Please check entry");
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(tbAltitudeTask3.Text))
            {
                messages.Add("Please select a goal or enter an altitude");
            }
            else
            {
                if (!double.TryParse(tbAltitudeTask3.Text, out altitude))
                {
                    messages.Add("Failed to convert altitude to double. Please check entry");
                }
            }
            if (messages.Count > 0)
            {
                foreach (string message in messages)
                {
                    Logger?.LogError("Cannot calculate Task 3: {message}", message);
                }
                MessageBox.Show(string.Join("\r\n", messages));
                return;
            }
            CoordinateSharp.UniversalTransverseMercator tempUTM = new(gridZone, easting, northing);
            double[] latLong = CoordinateSharp.UniversalTransverseMercator.ConvertUTMtoSignedDegree(tempUTM);
            Coordinate fiddleDeclaredGoal = new(latLong[0], latLong[1], altitude, altitude, DateTime.Now);
            Coordinate positionOfDeclaration = FiddleTrack.Declarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).Select(x => x.PositionAtDeclaration).ToList()[cbGoalTask3.SelectedIndex];
            Declaration fiddleDeclaration = new(Task3_GoalNumber, fiddleDeclaredGoal, positionOfDeclaration, true, -1, -1);


            MarkerDrop markerDrop = FiddleTrack.MarkerDrops.First(x => x.MarkerNumber == Task3_MarkerNumber);
            double distanceDeclarationToGoal = CoordinateHelpers.Calculate3DDistance(fiddleDeclaration.DeclaredGoal, markerDrop.MarkerLocation, true);
            double distanceGoalToMarker = CoordinateHelpers.Calculate2DDistanceHavercos(fiddleDeclaration.PositionAtDeclaration, fiddleDeclaration.DeclaredGoal);
            double result = distanceDeclarationToGoal / (distanceGoalToMarker / 1000.0);
            Logger?.LogInformation("Task 3 Pilot No {pilotNumber}: Distance between declaration position and declared goal is '{distanceDeclarationToGoal}m' / Distance goal to marker is '{distanceGoalToMarker}km'", FiddleTrack.Pilot.PilotNumber, Math.Round(distanceDeclarationToGoal, 3, MidpointRounding.AwayFromZero), Math.Round(distanceGoalToMarker / 1000.0, 3, MidpointRounding.AwayFromZero));      
            Logger?.LogInformation("Calculated result of '{result}m/km' at Task 3 for Pilot No '{pilotNumber}'", Math.Round(result, 3, MidpointRounding.AwayFromZero), FiddleTrack.Pilot.PilotNumber);
            try
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo resultsFileInternal = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameInternal));
                using (ExcelPackage package = new(resultsFileInternal))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 4].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 5].Value = "Task 3 PGD [m/km]";
                        wsResults.Cells[1, 6].Value = "Task 1 Comment";
                        wsResults.Cells[1, 7].Value = "Task 2 Comment";
                        wsResults.Cells[1, 8].Value = "Task 3 Comment";
                    }

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 5].Value = Math.Round(result, 3, MidpointRounding.AwayFromZero);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 8].Value = "Fiddled: " + tbCommentTask3.Text;

                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 5].Style.Fill.SetBackground(Color.Orange);
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 8].Style.Fill.SetBackground(Color.Orange);

                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified internal results file '{resultsFileInternal}'", resultsFileInternal.Name);
                FileInfo resultsFileProvisional = new(Path.Combine(OutputDirectory.FullName, ResultsFileNameProvisional));
                using (ExcelPackage package = new(resultsFileProvisional))
                {
                    ExcelWorksheet wsResults = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Results");
                    if (wsResults == default)
                    {
                        wsResults = package.Workbook.Worksheets.Add("Results");
                        wsResults.Cells[1, 1].Value = "Pilot Identifier";
                        wsResults.Cells[1, 2].Value = "Pilot Number";
                        wsResults.Cells[1, 3].Value = "Pilot Last Name";
                        wsResults.Cells[1, 4].Value = "Pilot First Name";
                        wsResults.Cells[1, 5].Value = "Task 1 HWZ [m]";
                        wsResults.Cells[1, 6].Value = "Task 2 LRN [km²]";
                        wsResults.Cells[1, 7].Value = "Task 3 PGD [m/km]";
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 1].Value = FiddleTrack.Pilot.PilotIdentifier;
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 2].Value = FiddleTrack.Pilot.PilotNumber;
                    if (!PilotMapping.GetPilotName(FiddleTrack.Pilot.PilotNumber, out string lastName, out string firstName))
                    {
                        Logger?.LogWarning("Last name and first name of pilot will be omitted");
                    }
                    else
                    {
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 3].Value = lastName;
                        wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 4].Value = firstName;
                    }
                    wsResults.Cells[FiddleTrack.Pilot.PilotNumber + 1, 7].Value = Math.Round(result, 3, MidpointRounding.AwayFromZero);


                    wsResults.Cells.AutoFitColumns();
                    package.Save();
                }
                Logger?.LogInformation("Successfully created or modified provisional results file '{resultsFileProvisional}'", resultsFileProvisional.Name);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to create or modified results file");
                return;
            }
        }
        #endregion

        #endregion


    }
}
