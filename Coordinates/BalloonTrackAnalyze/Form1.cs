using BalloonTrackAnalyze.TaskControls;
using Competition;
using Coordinates;
using LoggerComponent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BalloonTrackAnalyze
{
    public partial class Form1 : Form
    {
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
            logListView1.StartLogging(@".\logfile.txt", 5);
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.CompetitionFolder))
            {
                tbCompetitionFolder.Text = Properties.Settings.Default.CompetitionFolder;
                Log(LogSeverityType.Info, $"Loaded Competition Folder '{tbCompetitionFolder.Text}' from settings");
            }
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PilotNameMappingFile))
            {
                tbPilotMappingFile.Text = Properties.Settings.Default.PilotNameMappingFile;
                Log(LogSeverityType.Info, $"Loaded Pilot Name Mapping File '{tbPilotMappingFile.Text}' from settings");
            }
            rbBalloonLive.Checked = Properties.Settings.Default.UseBalloonLiveParser;
            rbFAILogger.Checked = !Properties.Settings.Default.UseBalloonLiveParser;
            Log(LogSeverityType.Info, $"Loaded Track Source '{(Properties.Settings.Default.UseBalloonLiveParser ? "Balloon Live" : "FAI Logger")}' from settings");
            rbGPSAltitude.Checked = Properties.Settings.Default.UseGPSAltitude;
            rbBarometricAltitude.Checked = !Properties.Settings.Default.UseGPSAltitude;
            Log(LogSeverityType.Info, $"Loaded Height Source '{(Properties.Settings.Default.UseGPSAltitude ? "GPS" : "Barometric")}' from settings");
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
                        PieControl pieControl = new PieControl();
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
                        ElbowControl elbowControl = new ElbowControl();
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
                        LandRunControl landrunControl = new LandRunControl();
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

        private void Log(LogSeverityType logSeverityType, string text)
        {
            Logger.Log(this, logSeverityType, text);
        }

        private void LandrunControl_DataValid()
        {
            LandRunTask landRun = (plUserControl.Controls["taskControl"] as LandRunControl).LandRun;
            if (DoesTaskNumberAlreadyExists(landRun.TaskNumber))
            {
                Log(LogSeverityType.Error, $"Failed to create {landRun}: A task with the number '{landRun.TaskNumber}' already exists");
            }
            else
            {
                if (!lbTaskList.Items.Contains(landRun))
                {
                    lbTaskList.Items.Add(landRun);
                    Log(LogSeverityType.Info, $"{landRun} created/modified");
                }
            }
        }

        private void ElbowControl_DataValid()
        {
            ElbowTask elbow = (plUserControl.Controls["taskControl"] as ElbowControl).Elbow;
            if (DoesTaskNumberAlreadyExists(elbow.TaskNumber))
            {
                Log(LogSeverityType.Error, $"Failed to create {elbow}: A task with the number '{elbow.TaskNumber}' already exists");
            }
            else
            {
                if (!lbTaskList.Items.Contains(elbow))
                {
                    lbTaskList.Items.Add(elbow);
                    Log(LogSeverityType.Info, $"{elbow} created/modified");
                }
            }
        }

        private void PieControl_DataValid()
        {
            PieTask pie = (plUserControl.Controls["taskControl"] as PieControl).PieTask;
            if (DoesTaskNumberAlreadyExists(pie.TaskNumber))
            {
                Log(LogSeverityType.Error, $"Failed to create {pie}: A task with the number '{pie.TaskNumber}' already exists");
            }
            else
            {
                if (!lbTaskList.Items.Contains(pie))
                {
                    lbTaskList.Items.Add(pie);
                    Log(LogSeverityType.Info, $"{pie} created/modified");
                }
            }
        }

        private void DonutControl_DataValid()
        {
            DonutTask donut = (plUserControl.Controls["taskControl"] as DonutControl).Donut;
            if (DoesTaskNumberAlreadyExists(donut.TaskNumber))
            {
                Log(LogSeverityType.Error, $"Failed to create {donut}: A task with the number '{donut.TaskNumber}' already exists");
            }
            else
            {
                if (!lbTaskList.Items.Contains(donut))
                {
                    lbTaskList.Items.Add(donut);
                    Log(LogSeverityType.Info, $"{donut} created/modified");
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
            Log(LogSeverityType.Info, "Saved competition settings");
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
                        DonutControl donutControl = new DonutControl(donut);
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
                        PieControl pieControl = new PieControl(pie);
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
                        ElbowControl elbowControl = new ElbowControl(elbow);
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
                        LandRunControl landrunControl = new LandRunControl(landRun);
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
            string functionErrorMessage = "Failed to calculate Results :";
            if (string.IsNullOrWhiteSpace(tbFlightNumber.Text))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Please enter a Flight No first");
                return;
            }
            int flightNumber;
            if (!int.TryParse(tbFlightNumber.Text, out flightNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Flight No '{tbFlightNumber.Text}' as integer");
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
                        Log(LogSeverityType.Warning, $"Failed to serialize task '{item}': {ex.Message} ");
                    }
                }
            }
            try
            {
                if (!flight.ParseTrackFiles(Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Tracks"), rbBalloonLive.Checked))
                {
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return;
                }
                Log(LogSeverityType.Info, $"{flight.Tracks.Count} Track files parsed successfully");
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse track files: {ex.Message}");
                return;
            }
            try
            {
                if (!flight.MapPilotNamesToTracks(tbPilotMappingFile.Text))
                {
                    Log(LogSeverityType.Error, functionErrorMessage);
                    return;
                }
                Log(LogSeverityType.Info, "Successfully mapped pilot names to tracks");
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to map pilot names to tracks: {ex.Message}");
                return;
            }
            try
            {
                CreateMarkerAndDeclarationFiles(flightNumber);
                Log(LogSeverityType.Info, "Successfully created file with markers and declarations for each pilot");
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Warning, functionErrorMessage + $"Failed to create marker and declaration files: {ex.Message}");
            }
            try
            {
                flight.CalculateResults(rbGPSAltitude.Checked, Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}"));
                Log(LogSeverityType.Info, "Successfully calculated results of all pilots for each task");
            }
            catch (Exception ex)
            {
                Log(LogSeverityType.Warning, functionErrorMessage + $"Failed to calculate results: {ex.Message}");
            }
        }

        private void CreateFolderStructure(int flightNumber)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($@"{tbCompetitionFolder.Text}");
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
                using (StreamWriter writer = new StreamWriter($@"{Path.Combine(tbCompetitionFolder.Text, "Scoring", $"Flight{flightNumber:D2}", "Pilots", $"Pilot{track.Pilot.PilotNumber:D3}{(!string.IsNullOrWhiteSpace(track.Pilot.FirstName) ? $"_{track.Pilot.FirstName}_{track.Pilot.LastName}" : "")}.csv")}", false))
                {
                    string markerHeader = "Marker Number, Time Stamp, Latitude[dec], Longitude[dec], Latitude[deg], Longitude[deg], UTM Zone, Easting, Norting, Altitude GPS[m], Altitude Barometric[m]";
                    writer.WriteLine(markerHeader);
                    foreach (MarkerDrop marker in track.MarkerDrops)
                    {
                        string timeStamp = marker.MarkerLocation.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss");
                        CoordinateSharp.Coordinate coordinateSharp = new CoordinateSharp.Coordinate(marker.MarkerLocation.Latitude, marker.MarkerLocation.Longitude);
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
                    foreach (DeclaredGoal declaredGoal in track.DeclaredGoals)
                    {
                        string timeStamp = declaredGoal.PositionAtDeclaration.TimeStamp.ToString("dd.MM.yyyy HH:mm:ss");
                        CoordinateSharp.Coordinate posCoordinate = new CoordinateSharp.Coordinate(declaredGoal.PositionAtDeclaration.Latitude, declaredGoal.PositionAtDeclaration.Longitude);
                        CoordinateSharp.Coordinate goalCoordinate = new CoordinateSharp.Coordinate(declaredGoal.GoalDeclared.Latitude, declaredGoal.GoalDeclared.Longitude);
                        string line = string.Join(',', declaredGoal.GoalNumber, timeStamp, declaredGoal.PositionAtDeclaration.Latitude,
                            declaredGoal.PositionAtDeclaration.Longitude, posCoordinate.Latitude.Display, posCoordinate.Longitude.Display,
                            posCoordinate.UTM.LongZone + posCoordinate.UTM.LatZone, posCoordinate.UTM.Easting, posCoordinate.UTM.Northing,
                            declaredGoal.PositionAtDeclaration.AltitudeGPS, declaredGoal.PositionAtDeclaration.AltitudeBarometric,
                            declaredGoal.GoalDeclared.Latitude, declaredGoal.GoalDeclared.Longitude, goalCoordinate.Latitude.Display,
                            goalCoordinate.Longitude.Display, goalCoordinate.UTM.LongZone + goalCoordinate.UTM.LatZone, goalCoordinate.UTM.Easting,
                            goalCoordinate.UTM.Northing, declaredGoal.GoalDeclared.AltitudeGPS, declaredGoal.GoalDeclared.AltitudeBarometric);
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private void btImportTask_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "json";
            openFileDialog.Filter = "json files(*.json) | *.json";
            openFileDialog.FilterIndex = 1;
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
                        DonutControl donutControl = new DonutControl(donut);
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
                        PieControl pieControl = new PieControl(pie);
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
                        ElbowControl elbowControl = new ElbowControl(elbow);
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
                        LandRunControl landrunControl = new LandRunControl(landRun);
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
                        Log(LogSeverityType.Error, "Failed import task: Unkown task type");
                        return;
                    }
                }
                catch (Exception ex)
                {

                    Log(LogSeverityType.Error, $"Failed to import task: {ex.Message}");
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
                    Log(LogSeverityType.Error,  "Please enter a Flight No first");
                    return;
                }
                int flightNumber;
                if (!int.TryParse(tbFlightNumber.Text, out flightNumber))
                {
                    Log(LogSeverityType.Error,  $"Failed to parse Flight No '{tbFlightNumber.Text}' as integer");
                    return;
                }
                CreateFolderStructure(flightNumber);
            }
        }
    }
}
