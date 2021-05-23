
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
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btSelectFiles
            // 
            this.btSelectFiles.Location = new System.Drawing.Point(12, 12);
            this.btSelectFiles.Name = "btSelectFiles";
            this.btSelectFiles.Size = new System.Drawing.Size(428, 23);
            this.btSelectFiles.TabIndex = 0;
            this.btSelectFiles.Text = "Select IGC Files";
            this.btSelectFiles.UseVisualStyleBackColor = true;
            this.btSelectFiles.Click += new System.EventHandler(this.btSelectFiles_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 41);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1024, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // logListView1
            // 
            this.logListView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logListView1.Location = new System.Drawing.Point(0, 82);
            this.logListView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.logListView1.Name = "logListView1";
            this.logListView1.Size = new System.Drawing.Size(1048, 200);
            this.logListView1.TabIndex = 2;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(640, 16);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(39, 15);
            this.lbStatus.TabIndex = 3;
            this.lbStatus.Text = "Ready";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFAILoggerParser);
            this.panel1.Controls.Add(this.rbBallonLiveParser);
            this.panel1.Location = new System.Drawing.Point(446, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 23);
            this.panel1.TabIndex = 4;
            // 
            // rbFAILoggerParser
            // 
            this.rbFAILoggerParser.AutoSize = true;
            this.rbFAILoggerParser.Location = new System.Drawing.Point(95, 2);
            this.rbFAILoggerParser.Name = "rbFAILoggerParser";
            this.rbFAILoggerParser.Size = new System.Drawing.Size(81, 19);
            this.rbFAILoggerParser.TabIndex = 1;
            this.rbFAILoggerParser.Text = "FAI Logger";
            this.rbFAILoggerParser.UseVisualStyleBackColor = true;
            // 
            // rbBallonLiveParser
            // 
            this.rbBallonLiveParser.AutoSize = true;
            this.rbBallonLiveParser.Checked = true;
            this.rbBallonLiveParser.Location = new System.Drawing.Point(0, 2);
            this.rbBallonLiveParser.Name = "rbBallonLiveParser";
            this.rbBallonLiveParser.Size = new System.Drawing.Size(89, 19);
            this.rbBallonLiveParser.TabIndex = 0;
            this.rbBallonLiveParser.TabStop = true;
            this.rbBallonLiveParser.Text = "Balloon Live";
            this.rbBallonLiveParser.UseVisualStyleBackColor = true;
            // 
            // TrackReportGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 282);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.logListView1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btSelectFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrackReportGeneratorForm";
            this.Text = "Track Report Generator";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
    }
}

