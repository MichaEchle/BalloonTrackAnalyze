
namespace BLC2021
{
    partial class BLC2021Launch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BLC2021Launch));
            this.btTaskSheet1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbFiddleMode = new System.Windows.Forms.RadioButton();
            this.rbBatchMode = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btChangePilotMapping = new System.Windows.Forms.Button();
            this.btChangeOutputDirectory = new System.Windows.Forms.Button();
            this.lbPilotMapping = new System.Windows.Forms.Label();
            this.lbOutputDirectory = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btTaskSheet1
            // 
            this.btTaskSheet1.Location = new System.Drawing.Point(12, 108);
            this.btTaskSheet1.Name = "btTaskSheet1";
            this.btTaskSheet1.Size = new System.Drawing.Size(102, 23);
            this.btTaskSheet1.TabIndex = 0;
            this.btTaskSheet1.Text = "Task Sheet 1";
            this.btTaskSheet1.UseVisualStyleBackColor = true;
            this.btTaskSheet1.Click += new System.EventHandler(this.btTaskSheet1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFiddleMode);
            this.panel1.Controls.Add(this.rbBatchMode);
            this.panel1.Location = new System.Drawing.Point(12, 68);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(208, 34);
            this.panel1.TabIndex = 1;
            // 
            // rbFiddleMode
            // 
            this.rbFiddleMode.AutoSize = true;
            this.rbFiddleMode.Location = new System.Drawing.Point(103, 10);
            this.rbFiddleMode.Name = "rbFiddleMode";
            this.rbFiddleMode.Size = new System.Drawing.Size(91, 19);
            this.rbFiddleMode.TabIndex = 1;
            this.rbFiddleMode.Text = "Fiddle Mode";
            this.rbFiddleMode.UseVisualStyleBackColor = true;
            // 
            // rbBatchMode
            // 
            this.rbBatchMode.AutoSize = true;
            this.rbBatchMode.Checked = true;
            this.rbBatchMode.Location = new System.Drawing.Point(8, 10);
            this.rbBatchMode.Name = "rbBatchMode";
            this.rbBatchMode.Size = new System.Drawing.Size(89, 19);
            this.rbBatchMode.TabIndex = 0;
            this.rbBatchMode.TabStop = true;
            this.rbBatchMode.Text = "Batch Mode";
            this.rbBatchMode.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Pilot Mapping :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output Directory :";
            // 
            // btChangePilotMapping
            // 
            this.btChangePilotMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btChangePilotMapping.Location = new System.Drawing.Point(713, 12);
            this.btChangePilotMapping.Name = "btChangePilotMapping";
            this.btChangePilotMapping.Size = new System.Drawing.Size(75, 23);
            this.btChangePilotMapping.TabIndex = 4;
            this.btChangePilotMapping.Text = "Change";
            this.btChangePilotMapping.UseVisualStyleBackColor = true;
            this.btChangePilotMapping.Click += new System.EventHandler(this.btChangePilotMapping_Click);
            // 
            // btChangeOutputDirectory
            // 
            this.btChangeOutputDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btChangeOutputDirectory.Location = new System.Drawing.Point(713, 41);
            this.btChangeOutputDirectory.Name = "btChangeOutputDirectory";
            this.btChangeOutputDirectory.Size = new System.Drawing.Size(75, 23);
            this.btChangeOutputDirectory.TabIndex = 5;
            this.btChangeOutputDirectory.Text = "Change";
            this.btChangeOutputDirectory.UseVisualStyleBackColor = true;
            this.btChangeOutputDirectory.Click += new System.EventHandler(this.btChangeOutputDirectory_Click);
            // 
            // lbPilotMapping
            // 
            this.lbPilotMapping.AutoSize = true;
            this.lbPilotMapping.Location = new System.Drawing.Point(121, 16);
            this.lbPilotMapping.Name = "lbPilotMapping";
            this.lbPilotMapping.Size = new System.Drawing.Size(0, 15);
            this.lbPilotMapping.TabIndex = 6;
            // 
            // lbOutputDirectory
            // 
            this.lbOutputDirectory.AutoSize = true;
            this.lbOutputDirectory.Location = new System.Drawing.Point(121, 45);
            this.lbOutputDirectory.Name = "lbOutputDirectory";
            this.lbOutputDirectory.Size = new System.Drawing.Size(0, 15);
            this.lbOutputDirectory.TabIndex = 7;
            // 
            // BLC2021Launch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 205);
            this.Controls.Add(this.lbOutputDirectory);
            this.Controls.Add(this.lbPilotMapping);
            this.Controls.Add(this.btChangeOutputDirectory);
            this.Controls.Add(this.btChangePilotMapping);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btTaskSheet1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BLC2021Launch";
            this.Text = "BLC 2021";
            this.Load += new System.EventHandler(this.BLC2021Launch_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btTaskSheet1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbFiddleMode;
        private System.Windows.Forms.RadioButton rbBatchMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btChangePilotMapping;
        private System.Windows.Forms.Button btChangeOutputDirectory;
        private System.Windows.Forms.Label lbPilotMapping;
        private System.Windows.Forms.Label lbOutputDirectory;
    }
}

