using BalloonTrackAnalyze.TaskControls;
using Competition;
using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BalloonTrackAnalyze
{
    public partial class Form1 : Form
    {
        private readonly ILogger<Form1> Logger = LogConnector.LoggerFactory.CreateLogger<Form1>();


        private Point TaskControlLocation { get; set; } = new Point(0, 0);

        private Flight flight
        {
            get; set;
        }

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.CompetitionFolder))
            {
                tbCompetitionFolder.Text = Properties.Settings.Default.CompetitionFolder;
                Logger?.LogInformation("Loaded Competition Folder '{competitionFolder}' from settings", tbCompetitionFolder.Text);
            }
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PilotNameMappingFile))
            {
                tbPilotMappingFile.Text = Properties.Settings.Default.PilotNameMappingFile;
                Logger?.LogInformation("Loaded Pilot Name Mapping File '{pilotNameMappingFile}' from settings", tbPilotMappingFile.Text);
            }
            rbBalloonLive.Checked = Properties.Settings.Default.UseBalloonLiveParser;
            rbFAILogger.Checked = !Properties.Settings.Default.UseBalloonLiveParser;
            Logger?.LogInformation("Loaded Track Source '{trackSource}' from settings", (Properties.Settings.Default.UseBalloonLiveParser ? "Balloon Live" : "FAI Logger"));
            rbGPSAltitude.Checked = Properties.Settings.Default.UseGPSAltitude;
            rbBarometricAltitude.Checked = !Properties.Settings.Default.UseGPSAltitude;
            Logger?.LogInformation("Loaded Height Source '{heightSource}' from settings", (Properties.Settings.Default.UseGPSAltitude ? "GPS" : "Barometric"));
        }

        private void cbTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbTaskList.SelectedItem.ToString())
            {
                case "Donut":
                    {
                        DonutControl donutControl = new DonutControl();
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        donutControl.Location = TaskControlLocation;
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        plUserControl.Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    break;
                case "Pie":
                    {
                        PieControl pieControl = new();
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        pieControl.Location = TaskControlLocation;
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        plUserControl.Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    break;
                case "Elbow":
                    {
                        ElbowControl elbowControl = new();
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        elbowControl.Location = TaskControlLocation;
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        plUserControl.Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    break;
                case "Landrun":
                    {
                        LandRunControl landrunControl = new();
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        landrunControl.Location = TaskControlLocation;
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        plUserControl.Controls.Add(landrunControl);
                        ResumeLayout();
                    }
                    break;
                default:
                    SuspendLayout();
                    plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                    ResumeLayout();
                    break;
            }
        }

        private void LandrunControl_DataValid()
        {
            if (plUserControl.Controls["taskControl"] is LandRunControl landRunControl)
            {
                LandRunTask landRun = landRunControl.LandRun;
                if (landRunControl.IsNewTask)
                {
                    if (DoesTaskNumberAlreadyExists(landRun.TaskNumber))
                    {
                        Logger?.LogError("Failed to create '{landRun}': A task with the number '{landRun.TaskNumber}' already exists", landRun, landRun.TaskNumber);
                    }
                    else
                    {
                        if (!lbTaskList.Items.Contains(landRun))
                        {
                            lbTaskList.Items.Add(landRun);
                            Logger?.LogInformation("{landRun} created successfully", landRun);
                        }
                    }
                }
                else
                {
                    Logger?.LogInformation("{landRun} modified successfully", landRun);
                }
            }
        }

        private void ElbowControl_DataValid()
        {
            if (plUserControl.Controls["taskControl"] is ElbowControl elbowControl)
            {
                ElbowTask elbow = elbowControl.Elbow;
                if (elbowControl.IsNewTask)
                {
                    if (DoesTaskNumberAlreadyExists(elbow.TaskNumber))
                    {
                        Logger?.LogError("Failed to create {elbow}: A task with the number '{elbow.TaskNumber}' already exists", elbow, elbow.TaskNumber);
                    }
                    else
                    {
                        if (!lbTaskList.Items.Contains(elbow))
                        {
                            lbTaskList.Items.Add(elbow);
                            Logger?.LogInformation("{elbow} created successfully", elbow);
                        }
                    }
                }
                else
                {
                    Logger?.LogInformation("{elbow} modified successfully", elbow);
                }
            }
        }

        private void PieControl_DataValid()
        {
            if (plUserControl.Controls["taskControl"] is PieControl pieControl)
            {
                PieTask pie = pieControl.PieTask;
                if (pieControl.IsNewTask)
                {
                    if (DoesTaskNumberAlreadyExists(pie.TaskNumber))
                    {
                        Logger?.LogError("Failed to create {pie}: A task with the number '{pie.TaskNumber}' already exists", pie, pie.TaskNumber);
                    }
                    else
                    {
                        if (!lbTaskList.Items.Contains(pie))
                        {
                            lbTaskList.Items.Add(pie);
                            Logger?.LogInformation("{pie} created successfully", pie);
                        }
                    }
                }
                else
                {
                    Logger?.LogInformation("{pie} modified successfully", pie);
                }
            }
        }

        private void DonutControl_DataValid()
        {
            if (plUserControl.Controls["taskControl"] is DonutControl donutControl)
            {

                DonutTask donut = donutControl.Donut;
                if (donutControl.IsNewTask)
                {
                    if (DoesTaskNumberAlreadyExists(donut.TaskNumber))
                    {
                        Logger?.LogError("Failed to create {donut}: A task with the number '{donut.TaskNumber}' already exists", donut, donut.TaskNumber);
                    }
                    else
                    {
                        if (!lbTaskList.Items.Contains(donut))
                        {
                            lbTaskList.Items.Add(donut);
                            Logger?.LogInformation("{donut} created successfully", donut);
                        }
                    }
                }
                else
                {
                    Logger?.LogInformation("{donut} modified successfully", donut);
                }
            }
        }

        private void btSelectCompetitionFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                tbCompetitionFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btSelectPilotMappingFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbCompetitionFolder.Text))
                openFileDialog1.InitialDirectory = tbCompetitionFolder.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                tbPilotMappingFile.Text = openFileDialog1.FileName;

        }

        private void btSaveCompetitionSettings_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbCompetitionFolder.Text))
                Properties.Settings.Default.CompetitionFolder = tbCompetitionFolder.Text;
            if (!string.IsNullOrWhiteSpace(tbPilotMappingFile.Text))
                Properties.Settings.Default.PilotNameMappingFile = tbPilotMappingFile.Text;
            Properties.Settings.Default.UseBalloonLiveParser = rbBalloonLive.Checked;
            Properties.Settings.Default.UseGPSAltitude = rbGPSAltitude.Checked;
            Properties.Settings.Default.Save();
            Logger?.LogInformation("Saved competition settings");
        }

        private void btDeleteTask_Click(object sender, EventArgs e)
        {
            if (lbTaskList.SelectedItem != null)
                lbTaskList.Items.Remove(lbTaskList.SelectedItem);
        }

        private bool DoesTaskNumberAlreadyExists(int taskNumber)
        {
            bool taskExists = false;
            foreach (object item in lbTaskList.Items)
            {
                if (item is ICompetitionTask)
                    if ((item as ICompetitionTask).TaskNumber == taskNumber)
                    {
                        taskExists = true;
                        break;
                    }
            }
            return taskExists;
        }

        private void lbTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lbTaskList.SelectedItem)
            {
                case DonutTask donut:
                    {
                        DonutControl donutControl = new(donut);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        donutControl.Location = TaskControlLocation;
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        plUserControl.Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    break;
                case PieTask pie:
                    {
                        PieControl pieControl = new(pie);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        pieControl.Location = TaskControlLocation;
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        plUserControl.Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    break;
                case ElbowTask elbow:
                    {
                        ElbowControl elbowControl = new(elbow);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        elbowControl.Location = TaskControlLocation;
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        plUserControl.Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    break;
                case LandRunTask landRun:
                    {
                        LandRunControl landrunControl = new(landRun);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        landrunControl.Location = TaskControlLocation;
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        plUserControl.Controls.Add(landrunControl);
                        ResumeLayout();
                    }
                    break;
            }
        }

        private void btCalculateResults_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFlightNumber.Text))
            {
                Logger?.LogError("Failed to calculate Results: Please enter a Flight No first");
                return;
            }
            if (!int.TryParse(tbFlightNumber.Text, out int flightNumber))
            {
                Logger?.LogError("Failed to calculate Results: Failed to parse Flight No '{flightNumber}' as integer", tbFlightNumber.Text);
                return;
            }
            CreateFolderStructure(flightNumber);
            flight = Flight.GetInstance();
            flight.FlightNumber = flightNumber;
            foreach (object item in lbTaskList.Items)
            {
                if (item is ICompetitionTask)
                {
                    if (!flight.Tasks.Contains(item as ICompetitionTask))
                        flight.Tasks.Add(item as ICompetitionTask);
                    try
                    {

                        //JsonSerializerOptions options = new JsonSerializerOptions();
                        //options.WriteIndented = true;

                        //string jsonString = JsonSerializer.Serialize(item, options);
                        string jsonString = JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        File.WriteAllText($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Tasks", item.ToString() + ".json")}", jsonString);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogWarning("Failed to serialize task '{item}': {ex.Message} ", item, ex.Message);
                    }
                }
            }
            try
            {
                if (!flight.ParseTrackFiles(Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Tracks"), rbBalloonLive.Checked))
                {
                    Logger?.LogError("Failed to calculate Results");
                    return;
                }
                Logger?.LogInformation("{flight.Tracks.Count} Track files parsed successfully", flight.Tracks.Count);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to calculate Results");
                return;
            }
            try
            {
                if (!flight.MapPilotNamesToTracks(tbPilotMappingFile.Text))
                {
                    Logger?.LogError("Failed to calculate Results");
                    return;
                }
                Logger?.LogInformation("Successfully mapped pilot names to tracks");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to calculate Results: failed to map pilot names to tracks");
                return;
            }
            try
            {
                CreateMarkerAndDeclarationFiles(flightNumber);
                Logger?.LogInformation("Successfully created file with markers and declarations for each pilot");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to calculate Results: failed to create marker and declaration files");
            }
            try
            {
                flight.CalculateResults(rbGPSAltitude.Checked, Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}"));
                Logger?.LogInformation("Successfully calculated results of all pilots for each task");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to calculate Results: failed to calculate results of all pilots for each task");
            }
        }

        private void CreateFolderStructure(int flightNumber)
        {
            DirectoryInfo directoryInfo = new($@"{tbCompetitionFolder.Text}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            directoryInfo = new DirectoryInfo($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring")}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            directoryInfo = new DirectoryInfo($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}")}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            directoryInfo = new DirectoryInfo($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Tasks")}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            directoryInfo = new DirectoryInfo($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Tracks")}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            directoryInfo = new DirectoryInfo($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Pilots")}");
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }

        private void CreateMarkerAndDeclarationFiles(int flightNumber)
        {
            foreach (Track track in flight.Tracks)
            {
                using StreamWriter writer = new($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Pilots", $"Pilot{track.Pilot.PilotNumber:D3}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"_{track.Pilot.FirstName}_{track.Pilot.LastName}" : "")}.csv")}", false);
                string markerHeader = "Marker Number, Time Stamp, Latitude[dec], Longitude[dec], Latitude[deg], Longitude[deg], UTM Zone, Easting, Norting, Altitude GPS[m], Altitude Barometric[m]";
                writer.WriteLine(markerHeader);
                foreach (MarkerDrop marker in track.MarkerDrops)
                {
                    string timeStamp = marker.MarkerLocation.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss");
                    CoordinateSharp.Coordinate coordinateSharp = new(marker.MarkerLocation.Latitude, marker.MarkerLocation.Longitude);
                    string line = string.Join(',', marker.MarkerNumber, timeStamp, marker.MarkerLocation.Latitude, marker.MarkerLocation.Longitude,
                        coordinateSharp.Latitude.Display, coordinateSharp.Longitude.Display, coordinateSharp.UTM.LongZone + coordinateSharp.UTM.LatZone,
                        coordinateSharp.UTM.Easting, coordinateSharp.UTM.Northing, marker.MarkerLocation.AltitudeGPS, marker.MarkerLocation.AltitudeBarometric);
                    writer.WriteLine(line);
                }
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();
                string declarationHeader = "Declaration Number,Time Stamp,Position Latitude[dec],Position Longitude[dec],Position Latitude[deg],Position Longitude[deg],Position Altitude GPS[m],Position Altitude Barometric[m],Position UTM Zone,Position Easting,Position Norting,Goal Latitude[dec],Goal Longitude[dec],Goal Latitude[deg],Goal Longitude[deg],Goal UTM Zone,Goal Easting,Goal Norting,Goal Altitude GPS m],Goal Altitude Barometric[m]";
                writer.WriteLine(declarationHeader);
                foreach (Declaration declaration in track.Declarations)
                {
                    string timeStamp = declaration.PositionAtDeclaration.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss");
                    CoordinateSharp.Coordinate posCoordinate = new(declaration.PositionAtDeclaration.Latitude, declaration.PositionAtDeclaration.Longitude);
                    CoordinateSharp.Coordinate goalCoordinate = new(declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude);
                    string line = string.Join(',', declaration.GoalNumber, timeStamp, declaration.PositionAtDeclaration.Latitude,
                        declaration.PositionAtDeclaration.Longitude, posCoordinate.Latitude.Display, posCoordinate.Longitude.Display,
                        posCoordinate.UTM.LongZone + posCoordinate.UTM.LatZone, posCoordinate.UTM.Easting, posCoordinate.UTM.Northing,
                        declaration.PositionAtDeclaration.AltitudeGPS, declaration.PositionAtDeclaration.AltitudeBarometric,
                        declaration.DeclaredGoal.Latitude, declaration.DeclaredGoal.Longitude, goalCoordinate.Latitude.Display,
                        goalCoordinate.Longitude.Display, goalCoordinate.UTM.LongZone + goalCoordinate.UTM.LatZone, goalCoordinate.UTM.Easting,
                        goalCoordinate.UTM.Northing, declaration.DeclaredGoal.AltitudeGPS, declaration.DeclaredGoal.AltitudeBarometric);
                    writer.WriteLine(line);
                }
            }
        }

        private void btImportTask_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                DefaultExt = "json",
                Filter = "json files(*.json) | *.json",
                FilterIndex = 1
            };
            if (!string.IsNullOrWhiteSpace(tbCompetitionFolder.Text))
                openFileDialog.InitialDirectory = tbCompetitionFolder.Text;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (openFileDialog.FileName.Contains("(Donut)"))
                    {
                        string jsonString = File.ReadAllText(openFileDialog.FileName);
                        //DonutTask donut = JsonSerializer.Deserialize<DonutTask>(jsonString);
                        DonutTask donut = JsonConvert.DeserializeObject<DonutTask>(jsonString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        DonutControl donutControl = new(donut);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        donutControl.Location = TaskControlLocation;
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        plUserControl.Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    else if (openFileDialog.FileName.Contains("(Pie)"))
                    {
                        string jsonString = File.ReadAllText(openFileDialog.FileName);
                        //PieTask pie = JsonSerializer.Deserialize<PieTask>(jsonString);
                        PieTask pie = JsonConvert.DeserializeObject<PieTask>(jsonString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        PieControl pieControl = new(pie);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        pieControl.Location = TaskControlLocation;
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        plUserControl.Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    else if (openFileDialog.FileName.Contains("(Elbow)"))
                    {
                        string jsonString = File.ReadAllText(openFileDialog.FileName);
                        //ElbowTask elbow = JsonSerializer.Deserialize<ElbowTask>(jsonString);
                        ElbowTask elbow = JsonConvert.DeserializeObject<ElbowTask>(jsonString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        ElbowControl elbowControl = new(elbow);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        elbowControl.Location = TaskControlLocation;
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        plUserControl.Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    else if (openFileDialog.FileName.Contains("(Land Run)"))
                    {
                        string jsonString = File.ReadAllText(openFileDialog.FileName);
                        //LandRunTask landRun = JsonSerializer.Deserialize<LandRunTask>(jsonString);
                        LandRunTask landRun = JsonConvert.DeserializeObject<LandRunTask>(jsonString, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
                        LandRunControl landrunControl = new(landRun);
                        SuspendLayout();
                        plUserControl.Controls.Remove(plUserControl.Controls["taskControl"]);
                        landrunControl.Location = TaskControlLocation;
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        plUserControl.Controls.Add(landrunControl);
                        ResumeLayout();
                    }
                    else
                    {
                        Logger?.LogError("Failed import task: Unkown task type");
                        return;
                    }
                }
                catch (Exception ex)
                {

                    Logger?.LogError(ex, "Failed to import task");
                    return;
                }
            }
        }

        private void tbFlightNumber_Leave(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want the default folder structure to be created now?", "Default Folder Structure", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (string.IsNullOrWhiteSpace(tbFlightNumber.Text))
                {
                    Logger?.LogError("Failed to create default folder structure: Please enter a Flight No first");
                    return;
                }
                if (!int.TryParse(tbFlightNumber.Text, out int flightNumber))
                {
                    Logger?.LogError("Failed to create default folder structure: Failed to parse Flight No '{flightNumber}' as integer", tbFlightNumber.Text);
                    return;
                }
                CreateFolderStructure(flightNumber);
            }
        }
    }
}
