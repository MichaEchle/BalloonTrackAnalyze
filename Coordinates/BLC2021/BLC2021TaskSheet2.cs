using Coordinates;
using LoggerComponent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLC2021
{
    public partial class BLC2021TaskSheet2 : Form
    {

        #region Properties
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
        } = 1;

        private int Task4_SecondMarkerNumber
        {
            get; set;
        } = 2;

        private int Task4_ThridMarkerNumber
        {
            get; set;
        } = 3;


        private readonly string ResultsFileNameInternal = "BLC2021TaskSheet2_Results_Internal.xlsx";
        private readonly string ResultFileNameProvisional = "BLC2021TaskSheet2_Results_Provisional.xslx";
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
            logListView1.StartLogging();
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
                        Log(LogSeverityType.Error, "No output directory has been defined, an output directory must be configured to continue");
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
            Task4_GoalNumber = 99;
            Task4_FirstMarkerNumber = -1;
            Task4_SecondMarkerNumber = -1;
            Task4_ThridMarkerNumber = -1;
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
            Task4_GoalNumber = 1;
            Task4_FirstMarkerNumber = 1;
            Task4_SecondMarkerNumber = 2;
            Task4_ThridMarkerNumber = 3;
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


        private void Log(LogSeverityType logSeverity, string logMessage)
        {
            Logger.Log(this, logSeverity, logMessage);
        }
        #endregion

        #region BatchMode

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
