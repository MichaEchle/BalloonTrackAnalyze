﻿namespace BalloonTrackAnalyze.TaskControls
{
    partial class PieTierControl
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
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbGoalNumber = new System.Windows.Forms.TextBox();
            this.tbRadius = new System.Windows.Forms.TextBox();
            this.tbMultiplier = new System.Windows.Forms.TextBox();
            this.cbIsReetranceAllowed = new System.Windows.Forms.CheckBox();
            this.tbLowerBoundary = new System.Windows.Forms.TextBox();
            this.tbUpperBoundary = new System.Windows.Forms.TextBox();
            this.rbRadiusFeet = new System.Windows.Forms.RadioButton();
            this.rbRadiusMeter = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbLowerBoundaryFeet = new System.Windows.Forms.RadioButton();
            this.rbLowerBoundaryMeter = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbUpperBoundaryFeet = new System.Windows.Forms.RadioButton();
            this.rbUpperBoundaryMeter = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btCreate = new System.Windows.Forms.Button();
            this.lbRules = new System.Windows.Forms.ListBox();
            this.btRemoveRule = new System.Windows.Forms.Button();
            this.cbRuleList = new System.Windows.Forms.ComboBox();
            this.plRuleControl = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Goal No.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Radius";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Reentrance";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Multiplier";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Lower Boundary";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 240);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Upper Boundary";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(11, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 28);
            this.label7.TabIndex = 0;
            this.label7.Text = "Pie Tier Setup";
            // 
            // tbGoalNumber
            // 
            this.tbGoalNumber.Location = new System.Drawing.Point(142, 48);
            this.tbGoalNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbGoalNumber.Name = "tbGoalNumber";
            this.tbGoalNumber.Size = new System.Drawing.Size(95, 27);
            this.tbGoalNumber.TabIndex = 1;
            // 
            // tbRadius
            // 
            this.tbRadius.Location = new System.Drawing.Point(142, 87);
            this.tbRadius.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(95, 27);
            this.tbRadius.TabIndex = 2;
            // 
            // tbMultiplier
            // 
            this.tbMultiplier.Location = new System.Drawing.Point(142, 159);
            this.tbMultiplier.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbMultiplier.Name = "tbMultiplier";
            this.tbMultiplier.Size = new System.Drawing.Size(95, 27);
            this.tbMultiplier.TabIndex = 5;
            // 
            // cbIsReetranceAllowed
            // 
            this.cbIsReetranceAllowed.AutoSize = true;
            this.cbIsReetranceAllowed.Checked = true;
            this.cbIsReetranceAllowed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsReetranceAllowed.Location = new System.Drawing.Point(142, 125);
            this.cbIsReetranceAllowed.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbIsReetranceAllowed.Name = "cbIsReetranceAllowed";
            this.cbIsReetranceAllowed.Size = new System.Drawing.Size(86, 24);
            this.cbIsReetranceAllowed.TabIndex = 4;
            this.cbIsReetranceAllowed.Text = "Allowed";
            this.cbIsReetranceAllowed.UseVisualStyleBackColor = true;
            // 
            // tbLowerBoundary
            // 
            this.tbLowerBoundary.Location = new System.Drawing.Point(142, 197);
            this.tbLowerBoundary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbLowerBoundary.Name = "tbLowerBoundary";
            this.tbLowerBoundary.Size = new System.Drawing.Size(95, 27);
            this.tbLowerBoundary.TabIndex = 6;
            // 
            // tbUpperBoundary
            // 
            this.tbUpperBoundary.Location = new System.Drawing.Point(142, 236);
            this.tbUpperBoundary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbUpperBoundary.Name = "tbUpperBoundary";
            this.tbUpperBoundary.Size = new System.Drawing.Size(95, 27);
            this.tbUpperBoundary.TabIndex = 8;
            // 
            // rbRadiusFeet
            // 
            this.rbRadiusFeet.AutoSize = true;
            this.rbRadiusFeet.Location = new System.Drawing.Point(59, 1);
            this.rbRadiusFeet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRadiusFeet.Name = "rbRadiusFeet";
            this.rbRadiusFeet.Size = new System.Drawing.Size(40, 24);
            this.rbRadiusFeet.TabIndex = 2;
            this.rbRadiusFeet.TabStop = true;
            this.rbRadiusFeet.Text = "ft";
            this.rbRadiusFeet.UseVisualStyleBackColor = true;
            // 
            // rbRadiusMeter
            // 
            this.rbRadiusMeter.AutoSize = true;
            this.rbRadiusMeter.Checked = true;
            this.rbRadiusMeter.Location = new System.Drawing.Point(11, 1);
            this.rbRadiusMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbRadiusMeter.Name = "rbRadiusMeter";
            this.rbRadiusMeter.Size = new System.Drawing.Size(43, 24);
            this.rbRadiusMeter.TabIndex = 1;
            this.rbRadiusMeter.TabStop = true;
            this.rbRadiusMeter.Text = "m";
            this.rbRadiusMeter.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.rbRadiusFeet);
            this.panel1.Controls.Add(this.rbRadiusMeter);
            this.panel1.Location = new System.Drawing.Point(245, 87);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(107, 31);
            this.panel1.TabIndex = 3;
            // 
            // rbLowerBoundaryFeet
            // 
            this.rbLowerBoundaryFeet.AutoSize = true;
            this.rbLowerBoundaryFeet.Location = new System.Drawing.Point(59, 1);
            this.rbLowerBoundaryFeet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbLowerBoundaryFeet.Name = "rbLowerBoundaryFeet";
            this.rbLowerBoundaryFeet.Size = new System.Drawing.Size(40, 24);
            this.rbLowerBoundaryFeet.TabIndex = 2;
            this.rbLowerBoundaryFeet.TabStop = true;
            this.rbLowerBoundaryFeet.Text = "ft";
            this.rbLowerBoundaryFeet.UseVisualStyleBackColor = true;
            // 
            // rbLowerBoundaryMeter
            // 
            this.rbLowerBoundaryMeter.AutoSize = true;
            this.rbLowerBoundaryMeter.Checked = true;
            this.rbLowerBoundaryMeter.Location = new System.Drawing.Point(11, 1);
            this.rbLowerBoundaryMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbLowerBoundaryMeter.Name = "rbLowerBoundaryMeter";
            this.rbLowerBoundaryMeter.Size = new System.Drawing.Size(43, 24);
            this.rbLowerBoundaryMeter.TabIndex = 1;
            this.rbLowerBoundaryMeter.TabStop = true;
            this.rbLowerBoundaryMeter.Text = "m";
            this.rbLowerBoundaryMeter.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.rbLowerBoundaryFeet);
            this.panel2.Controls.Add(this.rbLowerBoundaryMeter);
            this.panel2.Location = new System.Drawing.Point(245, 197);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(107, 31);
            this.panel2.TabIndex = 7;
            // 
            // rbUpperBoundaryFeet
            // 
            this.rbUpperBoundaryFeet.AutoSize = true;
            this.rbUpperBoundaryFeet.Location = new System.Drawing.Point(59, 1);
            this.rbUpperBoundaryFeet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbUpperBoundaryFeet.Name = "rbUpperBoundaryFeet";
            this.rbUpperBoundaryFeet.Size = new System.Drawing.Size(40, 24);
            this.rbUpperBoundaryFeet.TabIndex = 2;
            this.rbUpperBoundaryFeet.TabStop = true;
            this.rbUpperBoundaryFeet.Text = "ft";
            this.rbUpperBoundaryFeet.UseVisualStyleBackColor = true;
            // 
            // rbUpperBoundaryMeter
            // 
            this.rbUpperBoundaryMeter.AutoSize = true;
            this.rbUpperBoundaryMeter.Checked = true;
            this.rbUpperBoundaryMeter.Location = new System.Drawing.Point(11, 1);
            this.rbUpperBoundaryMeter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbUpperBoundaryMeter.Name = "rbUpperBoundaryMeter";
            this.rbUpperBoundaryMeter.Size = new System.Drawing.Size(43, 24);
            this.rbUpperBoundaryMeter.TabIndex = 1;
            this.rbUpperBoundaryMeter.TabStop = true;
            this.rbUpperBoundaryMeter.Text = "m";
            this.rbUpperBoundaryMeter.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.rbUpperBoundaryFeet);
            this.panel3.Controls.Add(this.rbUpperBoundaryMeter);
            this.panel3.Location = new System.Drawing.Point(245, 236);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(107, 31);
            this.panel3.TabIndex = 9;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(11, 275);
            this.btCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(341, 44);
            this.btCreate.TabIndex = 13;
            this.btCreate.Text = "Create Tier";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // lbRules
            // 
            this.lbRules.FormattingEnabled = true;
            this.lbRules.ItemHeight = 20;
            this.lbRules.Location = new System.Drawing.Point(371, 52);
            this.lbRules.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbRules.Name = "lbRules";
            this.lbRules.Size = new System.Drawing.Size(340, 124);
            this.lbRules.TabIndex = 12;
            this.lbRules.SelectedIndexChanged += new System.EventHandler(this.lbRules_SelectedIndexChanged);
            // 
            // btRemoveRule
            // 
            this.btRemoveRule.Location = new System.Drawing.Point(602, 185);
            this.btRemoveRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btRemoveRule.Name = "btRemoveRule";
            this.btRemoveRule.Size = new System.Drawing.Size(110, 32);
            this.btRemoveRule.TabIndex = 14;
            this.btRemoveRule.Text = "Remove Rule";
            this.btRemoveRule.UseVisualStyleBackColor = true;
            this.btRemoveRule.Click += new System.EventHandler(this.btRemoveRule_Click);
            // 
            // cbRuleList
            // 
            this.cbRuleList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRuleList.FormattingEnabled = true;
            this.cbRuleList.Items.AddRange(new object[] {
            "",
            "Declaration to Goal Distance",
            "Declaration to Goal Height",
            "Goal to other Goals Distance"});
            this.cbRuleList.Location = new System.Drawing.Point(371, 185);
            this.cbRuleList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRuleList.Name = "cbRuleList";
            this.cbRuleList.Size = new System.Drawing.Size(223, 28);
            this.cbRuleList.TabIndex = 10;
            this.cbRuleList.SelectedIndexChanged += new System.EventHandler(this.cbRuleList_SelectedIndexChanged);
            // 
            // plRuleControl
            // 
            this.plRuleControl.Location = new System.Drawing.Point(359, 221);
            this.plRuleControl.Name = "plRuleControl";
            this.plRuleControl.Size = new System.Drawing.Size(447, 226);
            this.plRuleControl.TabIndex = 15;
            // 
            // PieTierControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.plRuleControl);
            this.Controls.Add(this.cbRuleList);
            this.Controls.Add(this.btRemoveRule);
            this.Controls.Add(this.lbRules);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tbUpperBoundary);
            this.Controls.Add(this.tbLowerBoundary);
            this.Controls.Add(this.cbIsReetranceAllowed);
            this.Controls.Add(this.tbMultiplier);
            this.Controls.Add(this.tbRadius);
            this.Controls.Add(this.tbGoalNumber);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PieTierControl";
            this.Size = new System.Drawing.Size(806, 447);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbGoalNumber;
        private System.Windows.Forms.TextBox tbRadius;
        private System.Windows.Forms.TextBox tbMultiplier;
        private System.Windows.Forms.CheckBox cbIsReetranceAllowed;
        private System.Windows.Forms.TextBox tbLowerBoundary;
        private System.Windows.Forms.TextBox tbUpperBoundary;
        private System.Windows.Forms.RadioButton rbRadiusFeet;
        private System.Windows.Forms.RadioButton rbRadiusMeter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbLowerBoundaryFeet;
        private System.Windows.Forms.RadioButton rbLowerBoundaryMeter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbUpperBoundaryFeet;
        private System.Windows.Forms.RadioButton rbUpperBoundaryMeter;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.ListBox lbRules;
        private System.Windows.Forms.Button btRemoveRule;
        private System.Windows.Forms.ComboBox cbRuleList;
        private System.Windows.Forms.Panel plRuleControl;
    }
}
