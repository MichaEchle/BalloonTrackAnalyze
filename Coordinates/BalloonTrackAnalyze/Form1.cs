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
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Donut")
            {
                DonutControl donutControl = new DonutControl();
                SuspendLayout();
                donutControl.Location = new Point(12, 90);
                donutControl.Name = "donut1";
                donutControl.DataValid += DonutControl_DataValid;
                Controls.Add(donutControl);
                ResumeLayout();
            }
        }

        private void DonutControl_DataValid()
        {
            DonutTask donut=(Controls["donut1"] as DonutControl).Donut;
            Logger.Log(this, LogSeverityType.Info, $"{donut} created");
        }
    }
}
