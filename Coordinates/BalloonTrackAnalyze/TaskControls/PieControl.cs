using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Competition;
using LoggerComponent;

namespace BalloonTrackAnalyze.TaskControls
{
    public partial class PieControl : UserControl
    {
        public delegate void DataValidDelegate();

        public event DataValidDelegate DataValid;

        public PieTask PieTask { get; private set; }
        public PieControl()
        {
            InitializeComponent();
            pieTierControl1.DataValid += PieTierControl1_DataValid;
        }

        public PieControl(PieTask pieTask)
        {
            PieTask = pieTask;
            InitializeComponent();
            Prefill();
            pieTierControl1.DataValid += PieTierControl1_DataValid;
        }


        private void Prefill()
        {
            if (PieTask != null)
            {
                tbTaskNumber.Text = PieTask.TaskNumber.ToString();
                foreach (PieTask.PieTier tier in PieTask.Tiers)
                {
                    lbPieTiers.Items.Add(tier);
                }
            }
        }
        private void PieTierControl1_DataValid()
        {
            PieTask.PieTier tier = (Controls["pieTierControl1"] as PieTierControl).Tier;
            if (!lbPieTiers.Items.Contains(tier))
                lbPieTiers.Items.Add(tier);
            Log(LogSeverityType.Info, $"{tier} created/modified");
        }

        private void btRemoveTier_Click(object sender, EventArgs e)
        {
            if (lbPieTiers.SelectedItem != null)
                lbPieTiers.Items.Remove(lbPieTiers.SelectedItem);
        }

        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        public override string ToString()
        {
            return "Pie Setup Control";
        }

        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            bool isDataValid = true;
            string functionErrorMessage = "Failed to create/modify donut task: ";
            int taskNumber;
            if (!int.TryParse(tbTaskNumber.Text, out taskNumber))
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Failed to parse Task No. '{tbTaskNumber.Text}' as integer");
                isDataValid = false;
            }
            if (taskNumber <= 0)
            {
                Log(LogSeverityType.Error, functionErrorMessage + $"Task No. must be greater than 0");
                isDataValid = false;
            }
            if (isDataValid)
            {
                PieTask ??= new PieTask();
                List<PieTask.PieTier> tiers = new List<PieTask.PieTier>();
                foreach (object item in lbPieTiers.Items)
                {
                    if (item is PieTask.PieTier)
                        if (!PieTask.Tiers.Contains(item as PieTask.PieTier))
                            tiers.Add(item as PieTask.PieTier);
                }
                PieTask.SetupPie(taskNumber, tiers);
                tbTaskNumber.Text = "";
                lbPieTiers.Items.Clear();
                OnDataValid();
            }
        }

        private void lbPieTiers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPieTiers.SelectedItem != null)
            {
                pieTierControl1.Prefill(lbPieTiers.SelectedItem as PieTask.PieTier);
            }
        }
    }
}
