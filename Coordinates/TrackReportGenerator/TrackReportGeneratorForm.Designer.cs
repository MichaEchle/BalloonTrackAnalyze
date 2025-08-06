
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
            btSelectFiles = new Button();
            progressBar1 = new ProgressBar();
            lbStatus = new Label();
            panel1 = new Panel();
            rbFAILoggerParser = new RadioButton();
            rbBallonLiveParser = new RadioButton();
            flowLayoutPanel1 = new FlowLayoutPanel();
            rbGPSAltitude = new RadioButton();
            rbBarometricAltitude = new RadioButton();
            label1 = new Label();
            cbCheckMaxAltitude = new CheckBox();
            label2 = new Label();
            tbMaxAltitude = new TextBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            rbMeter = new RadioButton();
            rbFeet = new RadioButton();
            cbSkipCoordinates = new CheckBox();
            cbSkipExistingReports = new CheckBox();
            winFormsLogList1 = new WinFormsLoggerControl.WinFormsLogList();
            panel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // btSelectFiles
            // 
            btSelectFiles.Location = new Point(13, 12);
            btSelectFiles.Name = "btSelectFiles";
            btSelectFiles.Size = new Size(428, 23);
            btSelectFiles.TabIndex = 0;
            btSelectFiles.Text = "Select IGC Files";
            btSelectFiles.UseVisualStyleBackColor = true;
            btSelectFiles.Click += btSelectFiles_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(13, 95);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1023, 23);
            progressBar1.TabIndex = 1;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            lbStatus.Location = new Point(640, 16);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(39, 15);
            lbStatus.TabIndex = 3;
            lbStatus.Text = "Ready";
            // 
            // panel1
            // 
            panel1.Controls.Add(rbFAILoggerParser);
            panel1.Controls.Add(rbBallonLiveParser);
            panel1.Location = new Point(447, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(181, 23);
            panel1.TabIndex = 4;
            // 
            // rbFAILoggerParser
            // 
            rbFAILoggerParser.AutoSize = true;
            rbFAILoggerParser.Location = new Point(95, 2);
            rbFAILoggerParser.Name = "rbFAILoggerParser";
            rbFAILoggerParser.Size = new Size(81, 19);
            rbFAILoggerParser.TabIndex = 1;
            rbFAILoggerParser.Text = "FAI Logger";
            rbFAILoggerParser.UseVisualStyleBackColor = true;
            // 
            // rbBallonLiveParser
            // 
            rbBallonLiveParser.AutoSize = true;
            rbBallonLiveParser.Checked = true;
            rbBallonLiveParser.Location = new Point(0, 2);
            rbBallonLiveParser.Name = "rbBallonLiveParser";
            rbBallonLiveParser.Size = new Size(89, 19);
            rbBallonLiveParser.TabIndex = 0;
            rbBallonLiveParser.TabStop = true;
            rbBallonLiveParser.Text = "Balloon Live";
            rbBallonLiveParser.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(rbGPSAltitude);
            flowLayoutPanel1.Controls.Add(rbBarometricAltitude);
            flowLayoutPanel1.Location = new Point(113, 38);
            flowLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(150, 23);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // rbGPSAltitude
            // 
            rbGPSAltitude.AutoSize = true;
            rbGPSAltitude.Checked = true;
            rbGPSAltitude.Location = new Point(3, 2);
            rbGPSAltitude.Margin = new Padding(3, 2, 3, 2);
            rbGPSAltitude.Name = "rbGPSAltitude";
            rbGPSAltitude.Size = new Size(46, 19);
            rbGPSAltitude.TabIndex = 0;
            rbGPSAltitude.TabStop = true;
            rbGPSAltitude.Text = "GPS";
            rbGPSAltitude.UseVisualStyleBackColor = true;
            rbGPSAltitude.CheckedChanged += rbGPSAltitude_CheckedChanged;
            // 
            // rbBarometricAltitude
            // 
            rbBarometricAltitude.AutoSize = true;
            rbBarometricAltitude.Location = new Point(55, 2);
            rbBarometricAltitude.Margin = new Padding(3, 2, 3, 2);
            rbBarometricAltitude.Name = "rbBarometricAltitude";
            rbBarometricAltitude.Size = new Size(83, 19);
            rbBarometricAltitude.TabIndex = 5;
            rbBarometricAltitude.Text = "Barometric";
            rbBarometricAltitude.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 42);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 6;
            label1.Text = "Altitude source";
            // 
            // cbCheckMaxAltitude
            // 
            cbCheckMaxAltitude.AutoSize = true;
            cbCheckMaxAltitude.Checked = true;
            cbCheckMaxAltitude.CheckState = CheckState.Checked;
            cbCheckMaxAltitude.Location = new Point(13, 65);
            cbCheckMaxAltitude.Margin = new Padding(3, 2, 3, 2);
            cbCheckMaxAltitude.Name = "cbCheckMaxAltitude";
            cbCheckMaxAltitude.Size = new Size(127, 19);
            cbCheckMaxAltitude.TabIndex = 8;
            cbCheckMaxAltitude.Text = "Check max altitude";
            cbCheckMaxAltitude.UseVisualStyleBackColor = true;
            cbCheckMaxAltitude.CheckedChanged += cbCheckMaxAltitude_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(155, 66);
            label2.Name = "label2";
            label2.Size = new Size(116, 15);
            label2.TabIndex = 9;
            label2.Text = "Max allowed altitude";
            // 
            // tbMaxAltitude
            // 
            tbMaxAltitude.Location = new Point(290, 63);
            tbMaxAltitude.Margin = new Padding(3, 2, 3, 2);
            tbMaxAltitude.Name = "tbMaxAltitude";
            tbMaxAltitude.PlaceholderText = "max Altitude";
            tbMaxAltitude.Size = new Size(63, 23);
            tbMaxAltitude.TabIndex = 10;
            tbMaxAltitude.Text = "10000";
            tbMaxAltitude.Leave += tbMaxAltitude_Leave;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(rbMeter);
            flowLayoutPanel2.Controls.Add(rbFeet);
            flowLayoutPanel2.Location = new Point(361, 60);
            flowLayoutPanel2.Margin = new Padding(3, 2, 3, 2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(102, 24);
            flowLayoutPanel2.TabIndex = 11;
            // 
            // rbMeter
            // 
            rbMeter.AutoSize = true;
            rbMeter.Location = new Point(3, 2);
            rbMeter.Margin = new Padding(3, 2, 3, 2);
            rbMeter.Name = "rbMeter";
            rbMeter.Size = new Size(44, 19);
            rbMeter.TabIndex = 0;
            rbMeter.Text = "[m]";
            rbMeter.UseVisualStyleBackColor = true;
            rbMeter.CheckedChanged += rbMeter_CheckedChanged;
            // 
            // rbFeet
            // 
            rbFeet.AutoSize = true;
            rbFeet.Checked = true;
            rbFeet.Location = new Point(53, 2);
            rbFeet.Margin = new Padding(3, 2, 3, 2);
            rbFeet.Name = "rbFeet";
            rbFeet.Size = new Size(41, 19);
            rbFeet.TabIndex = 1;
            rbFeet.TabStop = true;
            rbFeet.Text = "[ft]";
            rbFeet.UseVisualStyleBackColor = true;
            // 
            // cbSkipCoordinates
            // 
            cbSkipCoordinates.AutoSize = true;
            cbSkipCoordinates.Checked = true;
            cbSkipCoordinates.CheckState = CheckState.Checked;
            cbSkipCoordinates.Location = new Point(447, 41);
            cbSkipCoordinates.Margin = new Padding(3, 2, 3, 2);
            cbSkipCoordinates.Name = "cbSkipCoordinates";
            cbSkipCoordinates.Size = new Size(203, 19);
            cbSkipCoordinates.TabIndex = 12;
            cbSkipCoordinates.Text = "Skip coordiantes without location";
            cbSkipCoordinates.UseVisualStyleBackColor = true;
            cbSkipCoordinates.CheckedChanged += cbSkipCoordinates_CheckedChanged;
            // 
            // cbSkipExistingReports
            // 
            cbSkipExistingReports.AutoSize = true;
            cbSkipExistingReports.Checked = true;
            cbSkipExistingReports.CheckState = CheckState.Checked;
            cbSkipExistingReports.Location = new Point(673, 41);
            cbSkipExistingReports.Margin = new Padding(3, 2, 3, 2);
            cbSkipExistingReports.Name = "cbSkipExistingReports";
            cbSkipExistingReports.Size = new Size(131, 19);
            cbSkipExistingReports.TabIndex = 13;
            cbSkipExistingReports.Text = "Skip existing reports";
            cbSkipExistingReports.UseVisualStyleBackColor = true;
            // 
            // winFormsLogList1
            // 
            winFormsLogList1.Dock = DockStyle.Bottom;
            winFormsLogList1.Location = new Point(0, 147);
            winFormsLogList1.Margin = new Padding(1, 1, 1, 1);
            winFormsLogList1.Name = "winFormsLogList1";
            winFormsLogList1.Size = new Size(1049, 190);
            winFormsLogList1.TabIndex = 14;
            // 
            // TrackReportGeneratorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1049, 337);
            Controls.Add(winFormsLogList1);
            Controls.Add(cbSkipExistingReports);
            Controls.Add(cbSkipCoordinates);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(tbMaxAltitude);
            Controls.Add(label2);
            Controls.Add(cbCheckMaxAltitude);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(lbStatus);
            Controls.Add(progressBar1);
            Controls.Add(btSelectFiles);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TrackReportGeneratorForm";
            Text = "Track Report Generator";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btSelectFiles;
        private System.Windows.Forms.ProgressBar progressBar1;
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
        private WinFormsLoggerControl.WinFormsLogList winFormsLogList1;
    }
}

