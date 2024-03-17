namespace BalloonTrackAnalyze
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
            cbTaskList = new System.Windows.Forms.ComboBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            plUserControl = new System.Windows.Forms.Panel();
            btCalculateResults = new System.Windows.Forms.Button();
            btDeleteTask = new System.Windows.Forms.Button();
            btImportTask = new System.Windows.Forms.Button();
            lbTaskList = new System.Windows.Forms.ListBox();
            tbFlightNumber = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            tabPage2 = new System.Windows.Forms.TabPage();
            label5 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            rbBarometricAltitude = new System.Windows.Forms.RadioButton();
            rbGPSAltitude = new System.Windows.Forms.RadioButton();
            panel1 = new System.Windows.Forms.Panel();
            rbFAILogger = new System.Windows.Forms.RadioButton();
            rbBalloonLive = new System.Windows.Forms.RadioButton();
            label4 = new System.Windows.Forms.Label();
            btSaveCompetitionSettings = new System.Windows.Forms.Button();
            btSelectPilotMappingFile = new System.Windows.Forms.Button();
            btSelectCompetitionFolder = new System.Windows.Forms.Button();
            tbPilotMappingFile = new System.Windows.Forms.TextBox();
            tbCompetitionFolder = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            winFormsLogList1 = new WinFormsLoggerControl.WinFormsLogList();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // cbTaskList
            // 
            cbTaskList.BackColor = System.Drawing.SystemColors.Control;
            cbTaskList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbTaskList.FormattingEnabled = true;
            cbTaskList.Items.AddRange(new object[] { "", "Donut", "Pie", "Landrun", "Elbow" });
            cbTaskList.Location = new System.Drawing.Point(14, 244);
            cbTaskList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cbTaskList.Name = "cbTaskList";
            cbTaskList.Size = new System.Drawing.Size(218, 33);
            cbTaskList.TabIndex = 2;
            cbTaskList.SelectedIndexChanged += cbTaskList_SelectedIndexChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1649, 1055);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(plUserControl);
            tabPage1.Controls.Add(btCalculateResults);
            tabPage1.Controls.Add(btDeleteTask);
            tabPage1.Controls.Add(btImportTask);
            tabPage1.Controls.Add(lbTaskList);
            tabPage1.Controls.Add(tbFlightNumber);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(cbTaskList);
            tabPage1.Location = new System.Drawing.Point(4, 34);
            tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage1.Size = new System.Drawing.Size(1641, 1017);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Flight";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // plUserControl
            // 
            plUserControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            plUserControl.AutoScroll = true;
            plUserControl.Location = new System.Drawing.Point(0, 291);
            plUserControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            plUserControl.Name = "plUserControl";
            plUserControl.Size = new System.Drawing.Size(1644, 715);
            plUserControl.TabIndex = 7;
            // 
            // btCalculateResults
            // 
            btCalculateResults.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            btCalculateResults.Location = new System.Drawing.Point(1036, 11);
            btCalculateResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btCalculateResults.Name = "btCalculateResults";
            btCalculateResults.Size = new System.Drawing.Size(204, 64);
            btCalculateResults.TabIndex = 6;
            btCalculateResults.Text = "Calculate Results";
            btCalculateResults.UseVisualStyleBackColor = true;
            btCalculateResults.Click += btCalculateResults_Click;
            // 
            // btDeleteTask
            // 
            btDeleteTask.Location = new System.Drawing.Point(394, 244);
            btDeleteTask.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btDeleteTask.Name = "btDeleteTask";
            btDeleteTask.Size = new System.Drawing.Size(142, 39);
            btDeleteTask.TabIndex = 5;
            btDeleteTask.Text = "Delete Task";
            btDeleteTask.UseVisualStyleBackColor = true;
            btDeleteTask.Click += btDeleteTask_Click;
            // 
            // btImportTask
            // 
            btImportTask.Location = new System.Drawing.Point(242, 244);
            btImportTask.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btImportTask.Name = "btImportTask";
            btImportTask.Size = new System.Drawing.Size(142, 39);
            btImportTask.TabIndex = 4;
            btImportTask.Text = "Import Task";
            btImportTask.UseVisualStyleBackColor = true;
            btImportTask.Click += btImportTask_Click;
            // 
            // lbTaskList
            // 
            lbTaskList.FormattingEnabled = true;
            lbTaskList.ItemHeight = 25;
            lbTaskList.Location = new System.Drawing.Point(14, 76);
            lbTaskList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            lbTaskList.Name = "lbTaskList";
            lbTaskList.Size = new System.Drawing.Size(522, 154);
            lbTaskList.TabIndex = 3;
            lbTaskList.SelectedIndexChanged += lbTaskList_SelectedIndexChanged;
            // 
            // tbFlightNumber
            // 
            tbFlightNumber.Location = new System.Drawing.Point(108, 29);
            tbFlightNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tbFlightNumber.Name = "tbFlightNumber";
            tbFlightNumber.Size = new System.Drawing.Size(118, 31);
            tbFlightNumber.TabIndex = 1;
            tbFlightNumber.Leave += tbFlightNumber_Leave;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(14, 34);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(89, 25);
            label3.TabIndex = 2;
            label3.Text = "Flight No.";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(panel2);
            tabPage2.Controls.Add(panel1);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(btSaveCompetitionSettings);
            tabPage2.Controls.Add(btSelectPilotMappingFile);
            tabPage2.Controls.Add(btSelectCompetitionFolder);
            tabPage2.Controls.Add(tbPilotMappingFile);
            tabPage2.Controls.Add(tbCompetitionFolder);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label1);
            tabPage2.Location = new System.Drawing.Point(4, 34);
            tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPage2.Size = new System.Drawing.Size(1641, 776);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Competition";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(14, 179);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(124, 25);
            label5.TabIndex = 0;
            label5.Text = "Height Source";
            // 
            // panel2
            // 
            panel2.Controls.Add(rbBarometricAltitude);
            panel2.Controls.Add(rbGPSAltitude);
            panel2.Location = new System.Drawing.Point(191, 171);
            panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(276, 36);
            panel2.TabIndex = 6;
            // 
            // rbBarometricAltitude
            // 
            rbBarometricAltitude.AutoSize = true;
            rbBarometricAltitude.Location = new System.Drawing.Point(74, 0);
            rbBarometricAltitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbBarometricAltitude.Name = "rbBarometricAltitude";
            rbBarometricAltitude.Size = new System.Drawing.Size(122, 29);
            rbBarometricAltitude.TabIndex = 7;
            rbBarometricAltitude.TabStop = true;
            rbBarometricAltitude.Text = "Barometric";
            rbBarometricAltitude.UseVisualStyleBackColor = true;
            // 
            // rbGPSAltitude
            // 
            rbGPSAltitude.AutoSize = true;
            rbGPSAltitude.Checked = true;
            rbGPSAltitude.Location = new System.Drawing.Point(0, 0);
            rbGPSAltitude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbGPSAltitude.Name = "rbGPSAltitude";
            rbGPSAltitude.Size = new System.Drawing.Size(69, 29);
            rbGPSAltitude.TabIndex = 0;
            rbGPSAltitude.TabStop = true;
            rbGPSAltitude.Text = "GPS";
            rbGPSAltitude.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(rbFAILogger);
            panel1.Controls.Add(rbBalloonLive);
            panel1.Location = new System.Drawing.Point(190, 125);
            panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(278, 36);
            panel1.TabIndex = 5;
            // 
            // rbFAILogger
            // 
            rbFAILogger.AutoSize = true;
            rbFAILogger.Location = new System.Drawing.Point(136, 0);
            rbFAILogger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbFAILogger.Name = "rbFAILogger";
            rbFAILogger.Size = new System.Drawing.Size(123, 29);
            rbFAILogger.TabIndex = 1;
            rbFAILogger.TabStop = true;
            rbFAILogger.Text = "FAI Logger";
            rbFAILogger.UseVisualStyleBackColor = true;
            // 
            // rbBalloonLive
            // 
            rbBalloonLive.AutoSize = true;
            rbBalloonLive.Checked = true;
            rbBalloonLive.Location = new System.Drawing.Point(0, 0);
            rbBalloonLive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbBalloonLive.Name = "rbBalloonLive";
            rbBalloonLive.Size = new System.Drawing.Size(131, 29);
            rbBalloonLive.TabIndex = 0;
            rbBalloonLive.TabStop = true;
            rbBalloonLive.Text = "Balloon Live";
            rbBalloonLive.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(14, 129);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(110, 25);
            label4.TabIndex = 0;
            label4.Text = "Track Source";
            // 
            // btSaveCompetitionSettings
            // 
            btSaveCompetitionSettings.Location = new System.Drawing.Point(14, 219);
            btSaveCompetitionSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btSaveCompetitionSettings.Name = "btSaveCompetitionSettings";
            btSaveCompetitionSettings.Size = new System.Drawing.Size(290, 54);
            btSaveCompetitionSettings.TabIndex = 7;
            btSaveCompetitionSettings.Text = "Save Competition Settings";
            btSaveCompetitionSettings.UseVisualStyleBackColor = true;
            btSaveCompetitionSettings.Click += btSaveCompetitionSettings_Click;
            // 
            // btSelectPilotMappingFile
            // 
            btSelectPilotMappingFile.Location = new System.Drawing.Point(1482, 75);
            btSelectPilotMappingFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btSelectPilotMappingFile.Name = "btSelectPilotMappingFile";
            btSelectPilotMappingFile.Size = new System.Drawing.Size(126, 39);
            btSelectPilotMappingFile.TabIndex = 4;
            btSelectPilotMappingFile.Text = "Select File";
            btSelectPilotMappingFile.UseVisualStyleBackColor = true;
            btSelectPilotMappingFile.Click += btSelectPilotMappingFile_Click;
            // 
            // btSelectCompetitionFolder
            // 
            btSelectCompetitionFolder.Location = new System.Drawing.Point(1482, 26);
            btSelectCompetitionFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btSelectCompetitionFolder.Name = "btSelectCompetitionFolder";
            btSelectCompetitionFolder.Size = new System.Drawing.Size(126, 39);
            btSelectCompetitionFolder.TabIndex = 2;
            btSelectCompetitionFolder.Text = "Select Folder";
            btSelectCompetitionFolder.UseVisualStyleBackColor = true;
            btSelectCompetitionFolder.Click += btSelectCompetitionFolder_Click;
            // 
            // tbPilotMappingFile
            // 
            tbPilotMappingFile.Location = new System.Drawing.Point(205, 76);
            tbPilotMappingFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tbPilotMappingFile.Name = "tbPilotMappingFile";
            tbPilotMappingFile.PlaceholderText = "Select a the mapping file. Expected format PilotNo, First Name, Last Name, Pilot identifier (, and ; supported as separator)";
            tbPilotMappingFile.Size = new System.Drawing.Size(1268, 31);
            tbPilotMappingFile.TabIndex = 3;
            // 
            // tbCompetitionFolder
            // 
            tbCompetitionFolder.Location = new System.Drawing.Point(205, 29);
            tbCompetitionFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tbCompetitionFolder.Name = "tbCompetitionFolder";
            tbCompetitionFolder.PlaceholderText = "Select the folder in which the tool with search for tracks and saves results";
            tbCompetitionFolder.Size = new System.Drawing.Size(1268, 31);
            tbCompetitionFolder.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(14, 81);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(176, 25);
            label2.TabIndex = 0;
            label2.Text = "Pilot Name Mapping";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 34);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(166, 25);
            label1.TabIndex = 0;
            label1.Text = "Competition Folder";
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.Description = "Select Competition Folder";
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "csv";
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Select Pilot Name Mapping File";
            // 
            // winFormsLogList1
            // 
            winFormsLogList1.Dock = System.Windows.Forms.DockStyle.Bottom;
            winFormsLogList1.Location = new System.Drawing.Point(0, 855);
            winFormsLogList1.Name = "winFormsLogList1";
            winFormsLogList1.Size = new System.Drawing.Size(1649, 200);
            winFormsLogList1.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1649, 1055);
            Controls.Add(winFormsLogList1);
            Controls.Add(tabControl1);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

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
        private WinFormsLoggerControl.WinFormsLogList winFormsLogList1;
    }
}