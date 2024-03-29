﻿namespace BalloonTrackAnalyze
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.logListView1 = new LoggerComponent.LogListView();
            this.cbTaskList = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.plUserControl = new System.Windows.Forms.Panel();
            this.btCalculateResults = new System.Windows.Forms.Button();
            this.btDeleteTask = new System.Windows.Forms.Button();
            this.btImportTask = new System.Windows.Forms.Button();
            this.lbTaskList = new System.Windows.Forms.ListBox();
            this.tbFlightNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbBarometricAltitude = new System.Windows.Forms.RadioButton();
            this.rbGPSAltitude = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbFAILogger = new System.Windows.Forms.RadioButton();
            this.rbBalloonLive = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btSaveCompetitionSettings = new System.Windows.Forms.Button();
            this.btSelectPilotMappingFile = new System.Windows.Forms.Button();
            this.btSelectCompetitionFolder = new System.Windows.Forms.Button();
            this.tbPilotMappingFile = new System.Windows.Forms.TextBox();
            this.tbCompetitionFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logListView1
            // 
            this.logListView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logListView1.Location = new System.Drawing.Point(0, 651);
            this.logListView1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.logListView1.Name = "logListView1";
            this.logListView1.Size = new System.Drawing.Size(1319, 193);
            this.logListView1.TabIndex = 0;
            // 
            // cbTaskList
            // 
            this.cbTaskList.BackColor = System.Drawing.SystemColors.Control;
            this.cbTaskList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTaskList.FormattingEnabled = true;
            this.cbTaskList.Items.AddRange(new object[] {
            "",
            "Donut",
            "Pie",
            "Landrun",
            "Elbow"});
            this.cbTaskList.Location = new System.Drawing.Point(11, 195);
            this.cbTaskList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTaskList.Name = "cbTaskList";
            this.cbTaskList.Size = new System.Drawing.Size(175, 28);
            this.cbTaskList.TabIndex = 2;
            this.cbTaskList.SelectedIndexChanged += new System.EventHandler(this.cbTaskList_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1319, 651);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.plUserControl);
            this.tabPage1.Controls.Add(this.btCalculateResults);
            this.tabPage1.Controls.Add(this.btDeleteTask);
            this.tabPage1.Controls.Add(this.btImportTask);
            this.tabPage1.Controls.Add(this.lbTaskList);
            this.tabPage1.Controls.Add(this.tbFlightNumber);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.cbTaskList);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(1311, 618);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Flight";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // plUserControl
            // 
            this.plUserControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plUserControl.AutoScroll = true;
            this.plUserControl.Location = new System.Drawing.Point(0, 233);
            this.plUserControl.Name = "plUserControl";
            this.plUserControl.Size = new System.Drawing.Size(1315, 379);
            this.plUserControl.TabIndex = 7;
            // 
            // btCalculateResults
            // 
            this.btCalculateResults.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btCalculateResults.Location = new System.Drawing.Point(829, 9);
            this.btCalculateResults.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCalculateResults.Name = "btCalculateResults";
            this.btCalculateResults.Size = new System.Drawing.Size(163, 51);
            this.btCalculateResults.TabIndex = 6;
            this.btCalculateResults.Text = "Calculate Results";
            this.btCalculateResults.UseVisualStyleBackColor = true;
            this.btCalculateResults.Click += new System.EventHandler(this.btCalculateResults_Click);
            // 
            // btDeleteTask
            // 
            this.btDeleteTask.Location = new System.Drawing.Point(315, 195);
            this.btDeleteTask.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btDeleteTask.Name = "btDeleteTask";
            this.btDeleteTask.Size = new System.Drawing.Size(114, 31);
            this.btDeleteTask.TabIndex = 5;
            this.btDeleteTask.Text = "Delete Task";
            this.btDeleteTask.UseVisualStyleBackColor = true;
            this.btDeleteTask.Click += new System.EventHandler(this.btDeleteTask_Click);
            // 
            // btImportTask
            // 
            this.btImportTask.Location = new System.Drawing.Point(194, 195);
            this.btImportTask.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btImportTask.Name = "btImportTask";
            this.btImportTask.Size = new System.Drawing.Size(114, 31);
            this.btImportTask.TabIndex = 4;
            this.btImportTask.Text = "Import Task";
            this.btImportTask.UseVisualStyleBackColor = true;
            this.btImportTask.Click += new System.EventHandler(this.btImportTask_Click);
            // 
            // lbTaskList
            // 
            this.lbTaskList.FormattingEnabled = true;
            this.lbTaskList.ItemHeight = 20;
            this.lbTaskList.Location = new System.Drawing.Point(11, 61);
            this.lbTaskList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbTaskList.Name = "lbTaskList";
            this.lbTaskList.Size = new System.Drawing.Size(418, 124);
            this.lbTaskList.TabIndex = 3;
            this.lbTaskList.SelectedIndexChanged += new System.EventHandler(this.lbTaskList_SelectedIndexChanged);
            // 
            // tbFlightNumber
            // 
            this.tbFlightNumber.Location = new System.Drawing.Point(86, 23);
            this.tbFlightNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFlightNumber.Name = "tbFlightNumber";
            this.tbFlightNumber.Size = new System.Drawing.Size(95, 27);
            this.tbFlightNumber.TabIndex = 1;
            this.tbFlightNumber.Leave += new System.EventHandler(this.tbFlightNumber_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Flight No.";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.btSaveCompetitionSettings);
            this.tabPage2.Controls.Add(this.btSelectPilotMappingFile);
            this.tabPage2.Controls.Add(this.btSelectCompetitionFolder);
            this.tabPage2.Controls.Add(this.tbPilotMappingFile);
            this.tabPage2.Controls.Add(this.tbCompetitionFolder);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(1311, 618);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Competition";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Height Source";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbBarometricAltitude);
            this.panel2.Controls.Add(this.rbGPSAltitude);
            this.panel2.Location = new System.Drawing.Point(153, 137);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(221, 29);
            this.panel2.TabIndex = 6;
            // 
            // rbBarometricAltitude
            // 
            this.rbBarometricAltitude.AutoSize = true;
            this.rbBarometricAltitude.Location = new System.Drawing.Point(59, 0);
            this.rbBarometricAltitude.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBarometricAltitude.Name = "rbBarometricAltitude";
            this.rbBarometricAltitude.Size = new System.Drawing.Size(103, 24);
            this.rbBarometricAltitude.TabIndex = 7;
            this.rbBarometricAltitude.TabStop = true;
            this.rbBarometricAltitude.Text = "Barometric";
            this.rbBarometricAltitude.UseVisualStyleBackColor = true;
            // 
            // rbGPSAltitude
            // 
            this.rbGPSAltitude.AutoSize = true;
            this.rbGPSAltitude.Checked = true;
            this.rbGPSAltitude.Location = new System.Drawing.Point(0, 0);
            this.rbGPSAltitude.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbGPSAltitude.Name = "rbGPSAltitude";
            this.rbGPSAltitude.Size = new System.Drawing.Size(56, 24);
            this.rbGPSAltitude.TabIndex = 0;
            this.rbGPSAltitude.TabStop = true;
            this.rbGPSAltitude.Text = "GPS";
            this.rbGPSAltitude.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFAILogger);
            this.panel1.Controls.Add(this.rbBalloonLive);
            this.panel1.Location = new System.Drawing.Point(152, 100);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 29);
            this.panel1.TabIndex = 5;
            // 
            // rbFAILogger
            // 
            this.rbFAILogger.AutoSize = true;
            this.rbFAILogger.Location = new System.Drawing.Point(109, 0);
            this.rbFAILogger.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbFAILogger.Name = "rbFAILogger";
            this.rbFAILogger.Size = new System.Drawing.Size(101, 24);
            this.rbFAILogger.TabIndex = 1;
            this.rbFAILogger.TabStop = true;
            this.rbFAILogger.Text = "FAI Logger";
            this.rbFAILogger.UseVisualStyleBackColor = true;
            // 
            // rbBalloonLive
            // 
            this.rbBalloonLive.AutoSize = true;
            this.rbBalloonLive.Checked = true;
            this.rbBalloonLive.Location = new System.Drawing.Point(0, 0);
            this.rbBalloonLive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBalloonLive.Name = "rbBalloonLive";
            this.rbBalloonLive.Size = new System.Drawing.Size(111, 24);
            this.rbBalloonLive.TabIndex = 0;
            this.rbBalloonLive.TabStop = true;
            this.rbBalloonLive.Text = "Balloon Live";
            this.rbBalloonLive.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Track Source";
            // 
            // btSaveCompetitionSettings
            // 
            this.btSaveCompetitionSettings.Location = new System.Drawing.Point(11, 175);
            this.btSaveCompetitionSettings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSaveCompetitionSettings.Name = "btSaveCompetitionSettings";
            this.btSaveCompetitionSettings.Size = new System.Drawing.Size(232, 43);
            this.btSaveCompetitionSettings.TabIndex = 7;
            this.btSaveCompetitionSettings.Text = "Save Competition Settings";
            this.btSaveCompetitionSettings.UseVisualStyleBackColor = true;
            this.btSaveCompetitionSettings.Click += new System.EventHandler(this.btSaveCompetitionSettings_Click);
            // 
            // btSelectPilotMappingFile
            // 
            this.btSelectPilotMappingFile.Location = new System.Drawing.Point(1186, 60);
            this.btSelectPilotMappingFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSelectPilotMappingFile.Name = "btSelectPilotMappingFile";
            this.btSelectPilotMappingFile.Size = new System.Drawing.Size(101, 31);
            this.btSelectPilotMappingFile.TabIndex = 4;
            this.btSelectPilotMappingFile.Text = "Select File";
            this.btSelectPilotMappingFile.UseVisualStyleBackColor = true;
            this.btSelectPilotMappingFile.Click += new System.EventHandler(this.btSelectPilotMappingFile_Click);
            // 
            // btSelectCompetitionFolder
            // 
            this.btSelectCompetitionFolder.Location = new System.Drawing.Point(1186, 21);
            this.btSelectCompetitionFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSelectCompetitionFolder.Name = "btSelectCompetitionFolder";
            this.btSelectCompetitionFolder.Size = new System.Drawing.Size(101, 31);
            this.btSelectCompetitionFolder.TabIndex = 2;
            this.btSelectCompetitionFolder.Text = "Select Folder";
            this.btSelectCompetitionFolder.UseVisualStyleBackColor = true;
            this.btSelectCompetitionFolder.Click += new System.EventHandler(this.btSelectCompetitionFolder_Click);
            // 
            // tbPilotMappingFile
            // 
            this.tbPilotMappingFile.Location = new System.Drawing.Point(164, 61);
            this.tbPilotMappingFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbPilotMappingFile.Name = "tbPilotMappingFile";
            this.tbPilotMappingFile.PlaceholderText = "Select a the mapping file. Expected format PilotNo, First Name, Last Name, Pilot " +
    "identifier (, and ; supported as separator)";
            this.tbPilotMappingFile.Size = new System.Drawing.Size(1015, 27);
            this.tbPilotMappingFile.TabIndex = 3;
            // 
            // tbCompetitionFolder
            // 
            this.tbCompetitionFolder.Location = new System.Drawing.Point(164, 23);
            this.tbCompetitionFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbCompetitionFolder.Name = "tbCompetitionFolder";
            this.tbCompetitionFolder.PlaceholderText = "Select the folder in which the tool with search for tracks and saves results";
            this.tbCompetitionFolder.Size = new System.Drawing.Size(1015, 27);
            this.tbCompetitionFolder.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pilot Name Mapping";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Competition Folder";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Competition Folder";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "csv";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Select Pilot Name Mapping File";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1319, 844);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.logListView1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LoggerComponent.LogListView logListView1;
        private System.Windows.Forms.ComboBox cbTaskList;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btSaveCompetitionSettings;
        private System.Windows.Forms.Button btSelectPilotMappingFile;
        private System.Windows.Forms.Button btSelectCompetitionFolder;
        private System.Windows.Forms.TextBox tbPilotMappingFile;
        private System.Windows.Forms.TextBox tbCompetitionFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btDeleteTask;
        private System.Windows.Forms.Button btImportTask;
        private System.Windows.Forms.ListBox lbTaskList;
        private System.Windows.Forms.TextBox tbFlightNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbFAILogger;
        private System.Windows.Forms.RadioButton rbBalloonLive;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btCalculateResults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbBarometricAltitude;
        private System.Windows.Forms.RadioButton rbGPSAltitude;
        private System.Windows.Forms.Panel plUserControl;
    }
}