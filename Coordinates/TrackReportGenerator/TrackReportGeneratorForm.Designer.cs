
namespace TrackReportGenerator
{
    partial class TrackReportGeneratorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackReportGeneratorForm));
            this.btSelectFiles = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.logListView1 = new LoggerComponent.LogListView();
            this.lbStatus = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbFAILoggerParser = new System.Windows.Forms.RadioButton();
            this.rbBallonLiveParser = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbGPSAltitude = new System.Windows.Forms.RadioButton();
            this.rbBarometricAltitude = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCheckMaxAltitude = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbMaxAltitude = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbMeter = new System.Windows.Forms.RadioButton();
            this.rbFeet = new System.Windows.Forms.RadioButton();
            this.cbSkipCoordinates = new System.Windows.Forms.CheckBox();
            this.cbSkipExistingReports = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSelectFiles
            // 
            this.btSelectFiles.Location = new System.Drawing.Point(14, 16);
            this.btSelectFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSelectFiles.Name = "btSelectFiles";
            this.btSelectFiles.Size = new System.Drawing.Size(489, 31);
            this.btSelectFiles.TabIndex = 0;
            this.btSelectFiles.Text = "Select IGC Files";
            this.btSelectFiles.UseVisualStyleBackColor = true;
            this.btSelectFiles.Click += new System.EventHandler(this.btSelectFiles_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(14, 127);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1170, 31);
            this.progressBar1.TabIndex = 1;
            // 
            // logListView1
            // 
            this.logListView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logListView1.Location = new System.Drawing.Point(0, 182);
            this.logListView1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.logListView1.Name = "logListView1";
            this.logListView1.Size = new System.Drawing.Size(1198, 267);
            this.logListView1.TabIndex = 2;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(731, 21);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(50, 20);
            this.lbStatus.TabIndex = 3;
            this.lbStatus.Text = "Ready";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFAILoggerParser);
            this.panel1.Controls.Add(this.rbBallonLiveParser);
            this.panel1.Location = new System.Drawing.Point(510, 16);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 31);
            this.panel1.TabIndex = 4;
            // 
            // rbFAILoggerParser
            // 
            this.rbFAILoggerParser.AutoSize = true;
            this.rbFAILoggerParser.Location = new System.Drawing.Point(109, 3);
            this.rbFAILoggerParser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbFAILoggerParser.Name = "rbFAILoggerParser";
            this.rbFAILoggerParser.Size = new System.Drawing.Size(101, 24);
            this.rbFAILoggerParser.TabIndex = 1;
            this.rbFAILoggerParser.Text = "FAI Logger";
            this.rbFAILoggerParser.UseVisualStyleBackColor = true;
            // 
            // rbBallonLiveParser
            // 
            this.rbBallonLiveParser.AutoSize = true;
            this.rbBallonLiveParser.Checked = true;
            this.rbBallonLiveParser.Location = new System.Drawing.Point(0, 3);
            this.rbBallonLiveParser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbBallonLiveParser.Name = "rbBallonLiveParser";
            this.rbBallonLiveParser.Size = new System.Drawing.Size(111, 24);
            this.rbBallonLiveParser.TabIndex = 0;
            this.rbBallonLiveParser.TabStop = true;
            this.rbBallonLiveParser.Text = "Balloon Live";
            this.rbBallonLiveParser.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rbGPSAltitude);
            this.flowLayoutPanel1.Controls.Add(this.rbBarometricAltitude);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(129, 51);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(172, 30);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // rbGPSAltitude
            // 
            this.rbGPSAltitude.AutoSize = true;
            this.rbGPSAltitude.Checked = true;
            this.rbGPSAltitude.Location = new System.Drawing.Point(3, 3);
            this.rbGPSAltitude.Name = "rbGPSAltitude";
            this.rbGPSAltitude.Size = new System.Drawing.Size(56, 24);
            this.rbGPSAltitude.TabIndex = 0;
            this.rbGPSAltitude.TabStop = true;
            this.rbGPSAltitude.Text = "GPS";
            this.rbGPSAltitude.UseVisualStyleBackColor = true;
            this.rbGPSAltitude.CheckedChanged += new System.EventHandler(this.rbGPSAltitude_CheckedChanged);
            // 
            // rbBarometricAltitude
            // 
            this.rbBarometricAltitude.AutoSize = true;
            this.rbBarometricAltitude.Location = new System.Drawing.Point(65, 3);
            this.rbBarometricAltitude.Name = "rbBarometricAltitude";
            this.rbBarometricAltitude.Size = new System.Drawing.Size(103, 24);
            this.rbBarometricAltitude.TabIndex = 5;
            this.rbBarometricAltitude.Text = "Barometric";
            this.rbBarometricAltitude.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Altitude source";
            // 
            // cbCheckMaxAltitude
            // 
            this.cbCheckMaxAltitude.AutoSize = true;
            this.cbCheckMaxAltitude.Checked = true;
            this.cbCheckMaxAltitude.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckMaxAltitude.Location = new System.Drawing.Point(14, 87);
            this.cbCheckMaxAltitude.Name = "cbCheckMaxAltitude";
            this.cbCheckMaxAltitude.Size = new System.Drawing.Size(157, 24);
            this.cbCheckMaxAltitude.TabIndex = 8;
            this.cbCheckMaxAltitude.Text = "Check max altitude";
            this.cbCheckMaxAltitude.UseVisualStyleBackColor = true;
            this.cbCheckMaxAltitude.CheckedChanged += new System.EventHandler(this.cbCheckMaxAltitude_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(177, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Max allowed altitude";
            // 
            // tbMaxAltitude
            // 
            this.tbMaxAltitude.Location = new System.Drawing.Point(332, 84);
            this.tbMaxAltitude.Name = "tbMaxAltitude";
            this.tbMaxAltitude.PlaceholderText = "max Altitude";
            this.tbMaxAltitude.Size = new System.Drawing.Size(71, 27);
            this.tbMaxAltitude.TabIndex = 10;
            this.tbMaxAltitude.Text = "10000";
            this.tbMaxAltitude.Leave += new System.EventHandler(this.tbMaxAltitude_Leave);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.rbMeter);
            this.flowLayoutPanel2.Controls.Add(this.rbFeet);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(413, 80);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(117, 32);
            this.flowLayoutPanel2.TabIndex = 11;
            // 
            // rbMeter
            // 
            this.rbMeter.AutoSize = true;
            this.rbMeter.Location = new System.Drawing.Point(3, 3);
            this.rbMeter.Name = "rbMeter";
            this.rbMeter.Size = new System.Drawing.Size(53, 24);
            this.rbMeter.TabIndex = 0;
            this.rbMeter.Text = "[m]";
            this.rbMeter.UseVisualStyleBackColor = true;
            this.rbMeter.CheckedChanged += new System.EventHandler(this.rbMeter_CheckedChanged);
            // 
            // rbFeet
            // 
            this.rbFeet.AutoSize = true;
            this.rbFeet.Checked = true;
            this.rbFeet.Location = new System.Drawing.Point(62, 3);
            this.rbFeet.Name = "rbFeet";
            this.rbFeet.Size = new System.Drawing.Size(50, 24);
            this.rbFeet.TabIndex = 1;
            this.rbFeet.TabStop = true;
            this.rbFeet.Text = "[ft]";
            this.rbFeet.UseVisualStyleBackColor = true;
            // 
            // cbSkipCoordinates
            // 
            this.cbSkipCoordinates.AutoSize = true;
            this.cbSkipCoordinates.Checked = true;
            this.cbSkipCoordinates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSkipCoordinates.Location = new System.Drawing.Point(510, 54);
            this.cbSkipCoordinates.Name = "cbSkipCoordinates";
            this.cbSkipCoordinates.Size = new System.Drawing.Size(253, 24);
            this.cbSkipCoordinates.TabIndex = 12;
            this.cbSkipCoordinates.Text = "Skip coordiantes without location";
            this.cbSkipCoordinates.UseVisualStyleBackColor = true;
            this.cbSkipCoordinates.CheckedChanged += new System.EventHandler(this.cbSkipCoordinates_CheckedChanged);
            // 
            // cbSkipExisting
            // 
            this.cbSkipExistingReports.AutoSize = true;
            this.cbSkipExistingReports.Checked = true;
            this.cbSkipExistingReports.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSkipExistingReports.Location = new System.Drawing.Point(769, 54);
            this.cbSkipExistingReports.Name = "cbSkipExisting";
            this.cbSkipExistingReports.Size = new System.Drawing.Size(165, 24);
            this.cbSkipExistingReports.TabIndex = 13;
            this.cbSkipExistingReports.Text = "Skip existing reports";
            this.cbSkipExistingReports.UseVisualStyleBackColor = true;
            // 
            // TrackReportGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 449);
            this.Controls.Add(this.cbSkipExistingReports);
            this.Controls.Add(this.cbSkipCoordinates);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.tbMaxAltitude);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbCheckMaxAltitude);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.logListView1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btSelectFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TrackReportGeneratorForm";
            this.Text = "Track Report Generator";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btSelectFiles;
        private System.Windows.Forms.ProgressBar progressBar1;
        private LoggerComponent.LogListView logListView1;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbFAILoggerParser;
        private System.Windows.Forms.RadioButton rbBallonLiveParser;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rbGPSAltitude;
        private System.Windows.Forms.RadioButton rbBarometricAltitude;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbCheckMaxAltitude;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbMaxAltitude;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.RadioButton rbMeter;
        private System.Windows.Forms.RadioButton rbFeet;
        private System.Windows.Forms.CheckBox cbSkipCoordinates;
        private System.Windows.Forms.CheckBox cbSkipExistingReports;
    }
}

