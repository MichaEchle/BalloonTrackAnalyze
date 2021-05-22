using Coordinates;
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

namespace TrackReportGenerator
{
    public partial class TrackReportGeneratorForm : Form
    {
        public TrackReportGeneratorForm()
        {
            InitializeComponent();
            logListView1.StartLogging();
        }

        private async void btSelectFiles_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.InitalDirectory))
                openFileDialog.InitialDirectory = Properties.Settings.Default.InitalDirectory;
            openFileDialog.Title = "Select .igc files";
            openFileDialog.Filter = "igc files (*.igc)|*.igc";
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Multiselect = true;
            UseWaitCursor = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] igcFiles = openFileDialog.FileNames;
                int successful = 0;
                int erroneous = 0;
                if (igcFiles.Length > 0)
                {
                    Properties.Settings.Default.InitalDirectory = new FileInfo(igcFiles[0]).DirectoryName;
                    Properties.Settings.Default.Save();
                }
                for (int index = 0; index < igcFiles.Length; index++)
                {
                    lbStatus.Text = $"{index} of {igcFiles.Length} processed ({successful} successful / {erroneous} erroneous)";
                    bool success = await ProcessFileAsync(igcFiles[index]);
                    if (success)
                        successful++;
                    else
                        erroneous++;
                    progressBar1.Value = (int)Math.Round((double)(((double)(index + 1) / igcFiles.Length) * 100), 0, MidpointRounding.AwayFromZero);
                }
                UseWaitCursor = false;
                progressBar1.Value = 100;
                lbStatus.Text = $"{igcFiles.Length} of {igcFiles.Length} processed ({successful} successful / {erroneous} erroneous)";
            }
        }

        private async Task<bool> ProcessFileAsync(string igcFile)
        {
            return await Task<bool>.Run(() =>
            {
                Track track;
                if (rbBallonLiveParser.Checked)
                {
                    if (!Coordinates.Parsers.BalloonLiveParser.ParseFile(igcFile, out track))
                        return false;
                }
                else
                {
                    if (!Coordinates.Parsers.FAILoggerParser.ParseFile(igcFile, out track))
                        return false;
                }

                string reportFileName = Path.Combine(Path.GetDirectoryName(igcFile), Path.GetFileNameWithoutExtension(igcFile) + ".xlsx");
                if (!ExcelTrackReportGenerator.GenerateTrackReport(reportFileName, track, true))
                    return false;

                return true;
            });
        }
    }
}
