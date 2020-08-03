namespace BalloonTrackAnalyze.ValidationControls
{
    partial class MarkerTimingRuleControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbOpenAtMinute = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbCloseAtMinute = new System.Windows.Forms.TextBox();
            this.btCreate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marker Timing Rule Setup";
            // 
            // tbOpenAtMinute
            // 
            this.tbOpenAtMinute.AutoSize = true;
            this.tbOpenAtMinute.Location = new System.Drawing.Point(10, 39);
            this.tbOpenAtMinute.Name = "tbOpenAtMinute";
            this.tbOpenAtMinute.Size = new System.Drawing.Size(92, 15);
            this.tbOpenAtMinute.TabIndex = 0;
            this.tbOpenAtMinute.Text = "Open At Minute";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 39);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(84, 23);
            this.textBox1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Close at Minute";
            // 
            // tbCloseAtMinute
            // 
            this.tbCloseAtMinute.Location = new System.Drawing.Point(124, 66);
            this.tbCloseAtMinute.Name = "tbCloseAtMinute";
            this.tbCloseAtMinute.Size = new System.Drawing.Size(84, 23);
            this.tbCloseAtMinute.TabIndex = 1;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(10, 95);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(298, 28);
            this.btCreate.TabIndex = 2;
            this.btCreate.Text = "Create";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // MarkerTimingRuleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.tbCloseAtMinute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tbOpenAtMinute);
            this.Controls.Add(this.label1);
            this.Name = "MarkerTimingRuleControl";
            this.Size = new System.Drawing.Size(360, 160);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label tbOpenAtMinute;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbCloseAtMinute;
        private System.Windows.Forms.Button btCreate;
    }
}
