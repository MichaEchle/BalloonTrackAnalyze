
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
            btSelectFiles = new System.Windows.Forms.Button();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lbStatus = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            rbFAILoggerParser = new System.Windows.Forms.RadioButton();
            rbBallonLiveParser = new System.Windows.Forms.RadioButton();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            rbGPSAltitude = new System.Windows.Forms.RadioButton();
            rbBarometricAltitude = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            cbCheckMaxAltitude = new System.Windows.Forms.CheckBox();
            label2 = new System.Windows.Forms.Label();
            tbMaxAltitude = new System.Windows.Forms.TextBox();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            rbMeter = new System.Windows.Forms.RadioButton();
            rbFeet = new System.Windows.Forms.RadioButton();
            cbSkipCoordinates = new System.Windows.Forms.CheckBox();
            cbSkipExistingReports = new System.Windows.Forms.CheckBox();
            winFormsLogList1 = new WinFormsLoggerControl.WinFormsLogList();
            panel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // btSelectFiles
            // 
            btSelectFiles.Location = new System.Drawing.Point(18, 20);
            btSelectFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btSelectFiles.Name = "btSelectFiles";
            btSelectFiles.Size = new System.Drawing.Size(611, 39);
            btSelectFiles.TabIndex = 0;
            btSelectFiles.Text = "Select IGC Files";
            btSelectFiles.UseVisualStyleBackColor = true;
            btSelectFiles.Click += btSelectFiles_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(18, 159);
            progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(1462, 39);
            progressBar1.TabIndex = 1;
            // 
            // lbStatus
            // 
            lbStatus.AutoSize = true;
            lbStatus.Location = new System.Drawing.Point(914, 26);
            lbStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new System.Drawing.Size(60, 25);
            lbStatus.TabIndex = 3;
            lbStatus.Text = "Ready";
            // 
            // panel1
            // 
            panel1.Controls.Add(rbFAILoggerParser);
            panel1.Controls.Add(rbBallonLiveParser);
            panel1.Location = new System.Drawing.Point(638, 20);
            panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(258, 39);
            panel1.TabIndex = 4;
            // 
            // rbFAILoggerParser
            // 
            rbFAILoggerParser.AutoSize = true;
            rbFAILoggerParser.Location = new System.Drawing.Point(136, 4);
            rbFAILoggerParser.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbFAILoggerParser.Name = "rbFAILoggerParser";
            rbFAILoggerParser.Size = new System.Drawing.Size(123, 29);
            rbFAILoggerParser.TabIndex = 1;
            rbFAILoggerParser.Text = "FAI Logger";
            rbFAILoggerParser.UseVisualStyleBackColor = true;
            // 
            // rbBallonLiveParser
            // 
            rbBallonLiveParser.AutoSize = true;
            rbBallonLiveParser.Checked = true;
            rbBallonLiveParser.Location = new System.Drawing.Point(0, 4);
            rbBallonLiveParser.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            rbBallonLiveParser.Name = "rbBallonLiveParser";
            rbBallonLiveParser.Size = new System.Drawing.Size(131, 29);
            rbBallonLiveParser.TabIndex = 0;
            rbBallonLiveParser.TabStop = true;
            rbBallonLiveParser.Text = "Balloon Live";
            rbBallonLiveParser.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(rbGPSAltitude);
            flowLayoutPanel1.Controls.Add(rbBarometricAltitude);
            flowLayoutPanel1.Location = new System.Drawing.Point(161, 64);
            flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(215, 38);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // rbGPSAltitude
            // 
            rbGPSAltitude.AutoSize = true;
            rbGPSAltitude.Checked = true;
            rbGPSAltitude.Location = new System.Drawing.Point(4, 4);
            rbGPSAltitude.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            rbGPSAltitude.Name = "rbGPSAltitude";
            rbGPSAltitude.Size = new System.Drawing.Size(69, 29);
            rbGPSAltitude.TabIndex = 0;
            rbGPSAltitude.TabStop = true;
            rbGPSAltitude.Text = "GPS";
            rbGPSAltitude.UseVisualStyleBackColor = true;
            rbGPSAltitude.CheckedChanged += rbGPSAltitude_CheckedChanged;
            // 
            // rbBarometricAltitude
            // 
            rbBarometricAltitude.AutoSize = true;
            rbBarometricAltitude.Location = new System.Drawing.Point(81, 4);
            rbBarometricAltitude.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            rbBarometricAltitude.Name = "rbBarometricAltitude";
            rbBarometricAltitude.Size = new System.Drawing.Size(122, 29);
            rbBarometricAltitude.TabIndex = 5;
            rbBarometricAltitude.Text = "Barometric";
            rbBarometricAltitude.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 70);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(131, 25);
            label1.TabIndex = 6;
            label1.Text = "Altitude source";
            // 
            // cbCheckMaxAltitude
            // 
            cbCheckMaxAltitude.AutoSize = true;
            cbCheckMaxAltitude.Checked = true;
            cbCheckMaxAltitude.CheckState = System.Windows.Forms.CheckState.Checked;
            cbCheckMaxAltitude.Location = new System.Drawing.Point(18, 109);
            cbCheckMaxAltitude.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            cbCheckMaxAltitude.Name = "cbCheckMaxAltitude";
            cbCheckMaxAltitude.Size = new System.Drawing.Size(187, 29);
            cbCheckMaxAltitude.TabIndex = 8;
            cbCheckMaxAltitude.Text = "Check max altitude";
            cbCheckMaxAltitude.UseVisualStyleBackColor = true;
            cbCheckMaxAltitude.CheckedChanged += cbCheckMaxAltitude_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(221, 110);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(175, 25);
            label2.TabIndex = 9;
            label2.Text = "Max allowed altitude";
            // 
            // tbMaxAltitude
            // 
            tbMaxAltitude.Location = new System.Drawing.Point(415, 105);
            tbMaxAltitude.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            tbMaxAltitude.Name = "tbMaxAltitude";
            tbMaxAltitude.PlaceholderText = "max Altitude";
            tbMaxAltitude.Size = new System.Drawing.Size(88, 31);
            tbMaxAltitude.TabIndex = 10;
            tbMaxAltitude.Text = "10000";
            tbMaxAltitude.Leave += tbMaxAltitude_Leave;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(rbMeter);
            flowLayoutPanel2.Controls.Add(rbFeet);
            flowLayoutPanel2.Location = new System.Drawing.Point(516, 100);
            flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(146, 40);
            flowLayoutPanel2.TabIndex = 11;
            // 
            // rbMeter
            // 
            rbMeter.AutoSize = true;
            rbMeter.Location = new System.Drawing.Point(4, 4);
            rbMeter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            rbMeter.Name = "rbMeter";
            rbMeter.Size = new System.Drawing.Size(63, 29);
            rbMeter.TabIndex = 0;
            rbMeter.Text = "[m]";
            rbMeter.UseVisualStyleBackColor = true;
            rbMeter.CheckedChanged += rbMeter_CheckedChanged;
            // 
            // rbFeet
            // 
            rbFeet.AutoSize = true;
            rbFeet.Checked = true;
            rbFeet.Location = new System.Drawing.Point(75, 4);
            rbFeet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            rbFeet.Name = "rbFeet";
            rbFeet.Size = new System.Drawing.Size(59, 29);
            rbFeet.TabIndex = 1;
            rbFeet.TabStop = true;
            rbFeet.Text = "[ft]";
            rbFeet.UseVisualStyleBackColor = true;
            // 
            // cbSkipCoordinates
            // 
            cbSkipCoordinates.AutoSize = true;
            cbSkipCoordinates.Checked = true;
            cbSkipCoordinates.CheckState = System.Windows.Forms.CheckState.Checked;
            cbSkipCoordinates.Location = new System.Drawing.Point(638, 68);
            cbSkipCoordinates.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            cbSkipCoordinates.Name = "cbSkipCoordinates";
            cbSkipCoordinates.Size = new System.Drawing.Size(303, 29);
            cbSkipCoordinates.TabIndex = 12;
            cbSkipCoordinates.Text = "Skip coordiantes without location";
            cbSkipCoordinates.UseVisualStyleBackColor = true;
            cbSkipCoordinates.CheckedChanged += cbSkipCoordinates_CheckedChanged;
            // 
            // cbSkipExistingReports
            // 
            cbSkipExistingReports.AutoSize = true;
            cbSkipExistingReports.Checked = true;
            cbSkipExistingReports.CheckState = System.Windows.Forms.CheckState.Checked;
            cbSkipExistingReports.Location = new System.Drawing.Point(961, 68);
            cbSkipExistingReports.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            cbSkipExistingReports.Name = "cbSkipExistingReports";
            cbSkipExistingReports.Size = new System.Drawing.Size(199, 29);
            cbSkipExistingReports.TabIndex = 13;
            cbSkipExistingReports.Text = "Skip existing reports";
            cbSkipExistingReports.UseVisualStyleBackColor = true;
            // 
            // winFormsLogList1
            // 
            winFormsLogList1.Location = new System.Drawing.Point(0, 243);
            winFormsLogList1.Name = "winFormsLogList1";
            winFormsLogList1.Size = new System.Drawing.Size(1498, 317);
            winFormsLogList1.TabIndex = 14;
            // 
            // TrackReportGeneratorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1498, 561);
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
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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

