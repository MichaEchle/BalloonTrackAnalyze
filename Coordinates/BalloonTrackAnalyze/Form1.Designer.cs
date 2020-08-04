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
            this.logListView1 = new LoggerComponent.LogListView();
            this.cbTaskList = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
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
            this.SuspendLayout();
            // 
            // logListView1
            // 
            this.logListView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logListView1.Location = new System.Drawing.Point(0, 664);
            this.logListView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.logListView1.Name = "logListView1";
            this.logListView1.Size = new System.Drawing.Size(1160, 145);
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
            this.cbTaskList.Location = new System.Drawing.Point(23, 29);
            this.cbTaskList.Name = "cbTaskList";
            this.cbTaskList.Size = new System.Drawing.Size(121, 23);
            this.cbTaskList.TabIndex = 1;
            this.cbTaskList.SelectedIndexChanged += new System.EventHandler(this.cbTaskList_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1160, 664);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbTaskList);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1152, 636);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Flight";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btSaveCompetitionSettings);
            this.tabPage2.Controls.Add(this.btSelectPilotMappingFile);
            this.tabPage2.Controls.Add(this.btSelectCompetitionFolder);
            this.tabPage2.Controls.Add(this.tbPilotMappingFile);
            this.tabPage2.Controls.Add(this.tbCompetitionFolder);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1152, 636);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Competition";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btSaveCompetitionSettings
            // 
            this.btSaveCompetitionSettings.Location = new System.Drawing.Point(9, 80);
            this.btSaveCompetitionSettings.Name = "btSaveCompetitionSettings";
            this.btSaveCompetitionSettings.Size = new System.Drawing.Size(203, 32);
            this.btSaveCompetitionSettings.TabIndex = 3;
            this.btSaveCompetitionSettings.Text = "Save Competition Settings";
            this.btSaveCompetitionSettings.UseVisualStyleBackColor = true;
            this.btSaveCompetitionSettings.Click += new System.EventHandler(this.btSaveCompetitionSettings_Click);
            // 
            // btSelectPilotMappingFile
            // 
            this.btSelectPilotMappingFile.Location = new System.Drawing.Point(1038, 45);
            this.btSelectPilotMappingFile.Name = "btSelectPilotMappingFile";
            this.btSelectPilotMappingFile.Size = new System.Drawing.Size(88, 23);
            this.btSelectPilotMappingFile.TabIndex = 3;
            this.btSelectPilotMappingFile.Text = "Select File";
            this.btSelectPilotMappingFile.UseVisualStyleBackColor = true;
            this.btSelectPilotMappingFile.Click += new System.EventHandler(this.btSelectPilotMappingFile_Click);
            // 
            // btSelectCompetitionFolder
            // 
            this.btSelectCompetitionFolder.Location = new System.Drawing.Point(1038, 16);
            this.btSelectCompetitionFolder.Name = "btSelectCompetitionFolder";
            this.btSelectCompetitionFolder.Size = new System.Drawing.Size(88, 23);
            this.btSelectCompetitionFolder.TabIndex = 3;
            this.btSelectCompetitionFolder.Text = "Select Folder";
            this.btSelectCompetitionFolder.UseVisualStyleBackColor = true;
            this.btSelectCompetitionFolder.Click += new System.EventHandler(this.btSelectCompetitionFolder_Click);
            // 
            // tbPilotMappingFile
            // 
            this.tbPilotMappingFile.Location = new System.Drawing.Point(132, 46);
            this.tbPilotMappingFile.Name = "tbPilotMappingFile";
            this.tbPilotMappingFile.Size = new System.Drawing.Size(900, 23);
            this.tbPilotMappingFile.TabIndex = 2;
            // 
            // tbCompetitionFolder
            // 
            this.tbCompetitionFolder.Location = new System.Drawing.Point(132, 17);
            this.tbCompetitionFolder.Name = "tbCompetitionFolder";
            this.tbCompetitionFolder.Size = new System.Drawing.Size(900, 23);
            this.tbCompetitionFolder.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pilot Name Mapping";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 15);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1160, 809);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.logListView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
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
        private System.Windows.Forms.TextBox ompe;
    }
}