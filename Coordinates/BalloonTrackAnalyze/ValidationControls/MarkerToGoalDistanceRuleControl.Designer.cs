namespace BalloonTrackAnalyze.ValidationControls
{
    partial class MarkerToGoalDistanceRuleControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMinimumDistance = new System.Windows.Forms.TextBox();
            this.tbMaximumDistance = new System.Windows.Forms.TextBox();
            this.tbGoalNumber = new System.Windows.Forms.TextBox();
            this.rbMinimumDistanceFeet = new System.Windows.Forms.RadioButton();
            this.rbMinimumDistanceMeter = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbMaximumDistanceFeet = new System.Windows.Forms.RadioButton();
            this.rbMaximumDistanceMeter = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btCreate = new System.Windows.Forms.Button();
            this.cbUse3DDistance = new System.Windows.Forms.CheckBox();
            this.cbUseGPSAltitude = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(11, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marker to Goal Distance Rule Setup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Min. Distance";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Max. Distance";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Goal No.";
            // 
            // tbMinimumDistance
            // 
            this.tbMinimumDistance.Location = new System.Drawing.Point(142, 48);
            this.tbMinimumDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbMinimumDistance.Name = "tbMinimumDistance";
            this.tbMinimumDistance.Size = new System.Drawing.Size(95, 27);
            this.tbMinimumDistance.TabIndex = 1;
            // 
            // tbMaximumDistance
            // 
            this.tbMaximumDistance.Location = new System.Drawing.Point(142, 86);
            this.tbMaximumDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbMaximumDistance.Name = "tbMaximumDistance";
            this.tbMaximumDistance.Size = new System.Drawing.Size(95, 27);
            this.tbMaximumDistance.TabIndex = 3;
            // 
            // tbGoalNumber
            // 
            this.tbGoalNumber.Location = new System.Drawing.Point(142, 175);
            this.tbGoalNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbGoalNumber.Name = "tbGoalNumber";
            this.tbGoalNumber.Size = new System.Drawing.Size(95, 27);
            this.tbGoalNumber.TabIndex = 5;
            // 
            // rbMinimumDistanceFeet
            // 
            this.rbMinimumDistanceFeet.AutoSize = true;
            this.rbMinimumDistanceFeet.Location = new System.Drawing.Point(59, 2);
            this.rbMinimumDistanceFeet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbMinimumDistanceFeet.Name = "rbMinimumDistanceFeet";
            this.rbMinimumDistanceFeet.Size = new System.Drawing.Size(40, 24);
            this.rbMinimumDistanceFeet.TabIndex = 2;
            this.rbMinimumDistanceFeet.TabStop = true;
            this.rbMinimumDistanceFeet.Text = "ft";
            this.rbMinimumDistanceFeet.UseVisualStyleBackColor = true;
            // 
            // rbMinimumDistanceMeter
            // 
            this.rbMinimumDistanceMeter.AutoSize = true;
            this.rbMinimumDistanceMeter.Checked = true;
            this.rbMinimumDistanceMeter.Location = new System.Drawing.Point(11, 2);
            this.rbMinimumDistanceMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbMinimumDistanceMeter.Name = "rbMinimumDistanceMeter";
            this.rbMinimumDistanceMeter.Size = new System.Drawing.Size(43, 24);
            this.rbMinimumDistanceMeter.TabIndex = 1;
            this.rbMinimumDistanceMeter.TabStop = true;
            this.rbMinimumDistanceMeter.Text = "m";
            this.rbMinimumDistanceMeter.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.rbMinimumDistanceFeet);
            this.panel1.Controls.Add(this.rbMinimumDistanceMeter);
            this.panel1.Location = new System.Drawing.Point(245, 48);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(107, 30);
            this.panel1.TabIndex = 2;
            // 
            // rbMaximumDistanceFeet
            // 
            this.rbMaximumDistanceFeet.AutoSize = true;
            this.rbMaximumDistanceFeet.Location = new System.Drawing.Point(59, 2);
            this.rbMaximumDistanceFeet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbMaximumDistanceFeet.Name = "rbMaximumDistanceFeet";
            this.rbMaximumDistanceFeet.Size = new System.Drawing.Size(40, 24);
            this.rbMaximumDistanceFeet.TabIndex = 2;
            this.rbMaximumDistanceFeet.TabStop = true;
            this.rbMaximumDistanceFeet.Text = "ft";
            this.rbMaximumDistanceFeet.UseVisualStyleBackColor = true;
            // 
            // rbMaximumDistanceMeter
            // 
            this.rbMaximumDistanceMeter.AutoSize = true;
            this.rbMaximumDistanceMeter.Checked = true;
            this.rbMaximumDistanceMeter.Location = new System.Drawing.Point(11, 2);
            this.rbMaximumDistanceMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbMaximumDistanceMeter.Name = "rbMaximumDistanceMeter";
            this.rbMaximumDistanceMeter.Size = new System.Drawing.Size(43, 24);
            this.rbMaximumDistanceMeter.TabIndex = 1;
            this.rbMaximumDistanceMeter.TabStop = true;
            this.rbMaximumDistanceMeter.Text = "m";
            this.rbMaximumDistanceMeter.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.rbMaximumDistanceFeet);
            this.panel2.Controls.Add(this.rbMaximumDistanceMeter);
            this.panel2.Location = new System.Drawing.Point(245, 86);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(107, 30);
            this.panel2.TabIndex = 4;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(11, 214);
            this.btCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(341, 38);
            this.btCreate.TabIndex = 6;
            this.btCreate.Text = "Create Rule";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // cbUse3DDistance
            // 
            this.cbUse3DDistance.AutoSize = true;
            this.cbUse3DDistance.Checked = true;
            this.cbUse3DDistance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUse3DDistance.Location = new System.Drawing.Point(142, 118);
            this.cbUse3DDistance.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbUse3DDistance.Name = "cbUse3DDistance";
            this.cbUse3DDistance.Size = new System.Drawing.Size(18, 17);
            this.cbUse3DDistance.TabIndex = 7;
            this.cbUse3DDistance.UseVisualStyleBackColor = true;
            // 
            // cbUseGPSAltitude
            // 
            this.cbUseGPSAltitude.AutoSize = true;
            this.cbUseGPSAltitude.Checked = true;
            this.cbUseGPSAltitude.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseGPSAltitude.Location = new System.Drawing.Point(142, 146);
            this.cbUseGPSAltitude.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbUseGPSAltitude.Name = "cbUseGPSAltitude";
            this.cbUseGPSAltitude.Size = new System.Drawing.Size(18, 17);
            this.cbUseGPSAltitude.TabIndex = 8;
            this.cbUseGPSAltitude.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "3D Distance";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "GPS Altitude";
            // 
            // MarkerToGoalDistanceRuleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbUseGPSAltitude);
            this.Controls.Add(this.cbUse3DDistance);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbGoalNumber);
            this.Controls.Add(this.tbMaximumDistance);
            this.Controls.Add(this.tbMinimumDistance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MarkerToGoalDistanceRuleControl";
            this.Size = new System.Drawing.Size(411, 308);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMinimumDistance;
        private System.Windows.Forms.TextBox tbMaximumDistance;
        private System.Windows.Forms.TextBox tbGoalNumber;
        private System.Windows.Forms.RadioButton rbMinimumDistanceFeet;
        private System.Windows.Forms.RadioButton rbMinimumDistanceMeter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbMaximumDistanceFeet;
        private System.Windows.Forms.RadioButton rbMaximumDistanceMeter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.CheckBox cbUse3DDistance;
        private System.Windows.Forms.CheckBox cbUseGPSAltitude;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
