using System;
using System.Windows.Forms;


namespace BLC2021
{
    public partial class BLC2021Launch : Form
    {
        public BLC2021Launch()
        {
            InitializeComponent();
            Text += $" v{typeof(BLC2021Launch).Assembly.GetName().Version}";

        }

        private void btTaskSheet1_Click(object sender, EventArgs e)
        {
            BLC2021TaskSheet1 blc2021TaskSheet1 = new(rbBatchMode.Checked);
            blc2021TaskSheet1.Show();
        }

        private void btChangePilotMapping_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select pilot mapping",
                Filter = "xlsx files (*.xlsx)|*.xlsx"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.PathToPilotMapping = openFileDialog.FileName;
                lbPilotMapping.Text = Properties.Settings.Default.PathToPilotMapping;
                Properties.Settings.Default.Save();
            }
        }

        private void btChangeOutputDirectory_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
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

        private void btTaskSheet2_Click(object sender, EventArgs e)
        {
            BLC2021TaskSheet2 blc2021TaskSheet2 = new(rbBatchMode.Checked);
            blc2021TaskSheet2.Show();
        }
    }
}
