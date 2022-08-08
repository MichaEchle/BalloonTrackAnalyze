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
        #region Properties

        /// <summary>
        /// The pie task to be created or modified by this control
        /// </summary>
        public PieTask PieTask
        {
            get; private set;
        }

        public bool IsNewTask
        {
            get; private set;
        }

        /// <summary>
        /// The delegate for the DataValid event
        /// </summary>
        public delegate void DataValidDelegate();

        /// <summary>
        /// Event will be fired when the input for the elbow task is valid
        /// </summary>
        public event DataValidDelegate DataValid;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public PieControl()
        {
            InitializeComponent();
            IsNewTask = true;
            btCreate.Text = "Create task";
            pieTierControl1.DataValid += PieTierControl1_DataValid;
        }

        /// <summary>
        /// Constructor which pre-fills controls from existing pie task
        /// </summary>
        /// <param name="pieTask">the existing pie task</param>
        public PieControl(PieTask pieTask)
        {
            PieTask = pieTask;
            InitializeComponent();
            IsNewTask = false;
            btCreate.Text = "Modify task";
            Prefill();
            pieTierControl1.DataValid += PieTierControl1_DataValid;
        }
        #endregion

        #region API
        /// <summary>
        /// Converts the object suitable for display representation
        /// </summary>
        /// <returns>display text of the this object</returns>
        public override string ToString()
        {
            return "Pie Setup Control";
        }
        #endregion

        #region private methods
        /// <summary>
        /// Pre-fill controls from existing task
        /// </summary>
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

        /// <summary>
        /// Adds a new pie tier to the tier list box, when the pie tier user controls fires the DataVaild event
        /// </summary>
        private void PieTierControl1_DataValid()
        {
            if (Controls["pieTierControl1"] is PieTierControl tierControl)
            {
                PieTask.PieTier tier = tierControl.Tier;
                if (tierControl.IsNewTier)
                {
                    if (!lbPieTiers.Items.Contains(tier))
                        lbPieTiers.Items.Add(tier);
                    Log(LogSeverityType.Info, $"{tier} created successfully");
                }
                else
                {
                    Log(LogSeverityType.Info, $"{tier} modified successfully");
                }
            }
        }

        /// <summary>
        /// Removes the selected pie tier form the list box
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void btRemoveTier_Click(object sender, EventArgs e)
        {
            if (lbPieTiers.SelectedItem != null)
                lbPieTiers.Items.Remove(lbPieTiers.SelectedItem);
        }

        /// <summary>
        /// Called when input for pie task is valid
        /// </summary>
        protected virtual void OnDataValid()
        {
            DataValid?.Invoke();
        }

        /// <summary>
        /// Logs a user messages
        /// </summary>
        /// <param name="logSeverity">the severity of the message</param>
        /// <param name="text">the message text</param>
        private void Log(LogSeverityType logSeverity, string text)
        {
            Logger.Log(this, logSeverity, text);
        }

        /// <summary>
        /// Validates user input and creates a new pie task / modifies the existing pie task
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
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
                            tiers.Add(item as PieTask.PieTier);
                }
                PieTask.SetupPie(taskNumber, tiers);
                tbTaskNumber.Text = "";
                lbPieTiers.Items.Clear();
                OnDataValid();
            }
        }

        /// <summary>
        /// Displays the pre-filled user control of the selected pie tier
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void lbPieTiers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPieTiers.SelectedItem != null)
            {
                SuspendLayout();
                pieTierControl1.Prefill(lbPieTiers.SelectedItem as PieTask.PieTier);
                if (pieTierControl1.Controls["plRuleControl"] is Panel ruleControlPanel)
                {
                    ruleControlPanel.Controls.Remove(ruleControlPanel.Controls["ruleControl"]);
                }
                ResumeLayout();
            }
        }

        
        #endregion
    }
}
