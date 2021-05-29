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
    public partial class BLC2021Launch : Form
    {
        public BLC2021Launch()
        {
            InitializeComponent();
            Text += typeof(BLC2021Launch).Assembly.GetName().Version;

        }

        private void btTaskSheet1_Click(object sender, EventArgs e)
        {
            BLC2021TaskSheet1 blc2021TaskSheet1 = new BLC2021TaskSheet1(rbBatchMode.Checked);
            blc2021TaskSheet1.Show();
        }

        private void btChangePilotMapping_Click(object sender, EventArgs e)
        {
            bool showDialog = false;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PathToPilotMapping))
            {
                FileInfo pilotMappingFile = new FileInfo(Properties.Settings.Default.PathToPilotMapping);
                if (!pilotMappingFile.Exists)
                    showDialog = true;
            }
            else
                showDialog = true;
            if (showDialog)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.AddExtension = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Select pilot mapping";
                openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.PathToPilotMapping = openFileDialog.FileName;
                    lbPilotMapping.Text = Properties.Settings.Default.PathToPilotMapping;
                    Properties.Settings.Default.Save();
                }

            }
        }

        private void btChangeOutputDirectory_Click(object sender, EventArgs e)
        {
            bool showDialog = false;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultOutputDirectory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Properties.Settings.Default.DefaultOutputDirectory);
                if (!directoryInfo.Exists)
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
                        Properties.Settings.Default.DefaultOutputDirectory = folderBrowserDialog.SelectedPath;
                        lbOutputDirectory.Text = Properties.Settings.Default.DefaultOutputDirectory;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private void BLC2021Launch_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.PathToPilotMapping))
                btChangePilotMapping.PerformClick();
            else
                lbPilotMapping.Text = Properties.Settings.Default.PathToPilotMapping;

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultOutputDirectory))
                btChangeOutputDirectory.PerformClick();
            else
                lbOutputDirectory.Text = Properties.Settings.Default.DefaultOutputDirectory;
        }
    }
}
