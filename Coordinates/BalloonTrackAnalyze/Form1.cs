using BalloonTrackAnalyze.TaskControls;
using Competition;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BalloonTrackAnalyze
{
    public partial class Form1 : Form
    {
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
        }

        private void cbTaskList_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbTaskList.SelectedItem.ToString())
            {
                case "Donut":
                    {
                        DonutControl donutControl = new DonutControl();
                        SuspendLayout();
                        Controls.Remove(Controls["taskControl"]);
                        donutControl.Location = new Point(12, 90);
                        donutControl.Name = "taskControl";
                        donutControl.DataValid += DonutControl_DataValid;
                        Controls.Add(donutControl);
                        ResumeLayout();
                    }
                    break;
                case "Pie":
                    {
                        PieControl pieControl = new PieControl();
                        SuspendLayout();
                        Controls.Remove(Controls["taskControl"]);
                        pieControl.Location = new Point(12, 90);
                        pieControl.Name = "taskControl";
                        pieControl.DataValid += PieControl_DataValid;
                        Controls.Add(pieControl);
                        ResumeLayout();
                    }
                    break;
                case "Elbow":
                    {
                        ElbowControl elbowControl = new ElbowControl();
                        SuspendLayout();
                        Controls.Remove(Controls["taskControl"]);
                        elbowControl.Location = new Point(12, 90);
                        elbowControl.Name = "taskControl";
                        elbowControl.DataValid += ElbowControl_DataValid;
                        Controls.Add(elbowControl);
                        ResumeLayout();
                    }
                    break;
                case "Landrun":
                    {
                        LandRunControl landrunControl = new LandRunControl();
                        SuspendLayout();
                        Controls.Remove(Controls["taskControl"]);
                        landrunControl.Location = new Point(12, 90);
                        landrunControl.Name = "taskControl";
                        landrunControl.DataValid += LandrunControl_DataValid;
                        Controls.Add(landrunControl);
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
            LandRunTask landRun = (Controls["taskControl"] as LandRunControl).LandRun;
            Logger.Log(this, LogSeverityType.Info, $"{landRun} created/modified");
        }

        private void ElbowControl_DataValid()
        {
            ElbowTask elbow = (Controls["taskControl"] as ElbowControl).Elbow;
            Logger.Log(this, LogSeverityType.Info, $"{elbow} created/modified");
        }

        private void PieControl_DataValid()
        {
            PieTask pie = (Controls["taskControl"] as PieControl).PieTask;
            Logger.Log(this, LogSeverityType.Info, $"{pie} created/modified");
        }

        private void DonutControl_DataValid()
        {
            DonutTask donut = (Controls["taskControl"] as DonutControl).Donut;
            Logger.Log(this, LogSeverityType.Info, $"{donut} created/modified");
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
            if(!string.IsNullOrWhiteSpace(tbCompetitionFolder.Text))
                Properties.Settings.Default.CompetitionFolder = tbCompetitionFolder.Text;
            if (!string.IsNullOrWhiteSpace(tbPilotMappingFile.Text))
                Properties.Settings.Default.PilotNameMappingFile = tbPilotMappingFile.Text;
            Properties.Settings.Default.Save();
        }
    }
}
