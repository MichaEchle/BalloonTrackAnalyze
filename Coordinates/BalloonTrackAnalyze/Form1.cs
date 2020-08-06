using BalloonTrackAnalyze.TaskControls;
using Competition;
using LoggerComponent;
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
        private Point TaskControlLocation { get; set; } = new Point(0, 165);

        private Flight flight { get; set; }

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
        }

        private void cbTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbTaskList.SelectedItem.ToString())
            {
                case "Donut":
                    {
                        DonutControl donutControl = new DonutControl();
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        donutControl.Location = TaskControlLocation;
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        tabPage1.Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    break;
                case "Pie":
                    {
                        PieControl pieControl = new PieControl();
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        pieControl.Location = TaskControlLocation;
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        tabPage1.Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    break;
                case "Elbow":
                    {
                        ElbowControl elbowControl = new ElbowControl();
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        elbowControl.Location = TaskControlLocation;
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        tabPage1.Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    break;
                case "Landrun":
                    {
                        LandRunControl landrunControl = new LandRunControl();
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        landrunControl.Location = TaskControlLocation;
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        tabPage1.Controls.Add(landrunControl);
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
            LandRunTask landRun = (tabPage1.Controls["taskControl"] as LandRunControl).LandRun;
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
            ElbowTask elbow = (tabPage1.Controls["taskControl"] as ElbowControl).Elbow;
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
            PieTask pie = (tabPage1.Controls["taskControl"] as PieControl).PieTask;
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
            DonutTask donut = (tabPage1.Controls["taskControl"] as DonutControl).Donut;
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
            Properties.Settings.Default.Save();
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
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        donutControl.Location = TaskControlLocation;
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        tabPage1.Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    break;
                case PieTask pie:
                    {
                        PieControl pieControl = new PieControl(pie);
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        pieControl.Location = TaskControlLocation;
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        tabPage1.Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    break;
                case ElbowTask elbow:
                    {
                        ElbowControl elbowControl = new ElbowControl(elbow);
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        elbowControl.Location = TaskControlLocation;
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        tabPage1.Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    break;
                case LandRunTask landRun:
                    {
                        LandRunControl landrunControl = new LandRunControl(landRun);
                        SuspendLayout();
                        tabPage1.Controls.Remove(tabPage1.Controls["taskControl"]);
                        landrunControl.Location = TaskControlLocation;
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        tabPage1.Controls.Add(landrunControl);
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
            flight = Flight.GetInstance();
            flight.FlightNumber = flightNumber;
            foreach (object item in lbTaskList.Items)
            {
                if (item is ICompetitionTask)
                    flight.Tasks.Add(item as ICompetitionTask);
            }
            if (!flight.ParseTrackFiles(Path.Combine(tbCompetitionFolder.Text, "Scouring", $"Flight{flightNumber:D2}", "Tracks"), rbBalloonLive.Checked))
            {
                Log(LogSeverityType.Error, functionErrorMessage);
                return;
            }
        }


    }
}
