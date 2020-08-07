namespace BalloonTrackAnalyze.TaskControls
{
    partial class ElbowControl
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
            this.tbTaskNumber = new System.Windows.Forms.TextBox();
            this.tbFirstMarkerNumber = new System.Windows.Forms.TextBox();
            this.tbSecondMarkerNumber = new System.Windows.Forms.TextBox();
            this.tbThirdMarkerNumber = new System.Windows.Forms.TextBox();
            this.btCreate = new System.Windows.Forms.Button();
            this.lbRules = new System.Windows.Forms.ListBox();
            this.cbRuleList = new System.Windows.Forms.ComboBox();
            this.btRemoveRule = new System.Windows.Forms.Button();
            this.plRuleControl = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Elbow Task Setup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Task No.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "1st Marker No.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "2nd Marker No.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "3rd Marker No.";
            // 
            // tbTaskNumber
            // 
            this.tbTaskNumber.Location = new System.Drawing.Point(142, 48);
            this.tbTaskNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbTaskNumber.Name = "tbTaskNumber";
            this.tbTaskNumber.Size = new System.Drawing.Size(95, 27);
            this.tbTaskNumber.TabIndex = 1;
            // 
            // tbFirstMarkerNumber
            // 
            this.tbFirstMarkerNumber.Location = new System.Drawing.Point(142, 87);
            this.tbFirstMarkerNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbFirstMarkerNumber.Name = "tbFirstMarkerNumber";
            this.tbFirstMarkerNumber.Size = new System.Drawing.Size(95, 27);
            this.tbFirstMarkerNumber.TabIndex = 2;
            // 
            // tbSecondMarkerNumber
            // 
            this.tbSecondMarkerNumber.Location = new System.Drawing.Point(142, 125);
            this.tbSecondMarkerNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbSecondMarkerNumber.Name = "tbSecondMarkerNumber";
            this.tbSecondMarkerNumber.Size = new System.Drawing.Size(95, 27);
            this.tbSecondMarkerNumber.TabIndex = 3;
            // 
            // tbThirdMarkerNumber
            // 
            this.tbThirdMarkerNumber.Location = new System.Drawing.Point(142, 164);
            this.tbThirdMarkerNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbThirdMarkerNumber.Name = "tbThirdMarkerNumber";
            this.tbThirdMarkerNumber.Size = new System.Drawing.Size(95, 27);
            this.tbThirdMarkerNumber.TabIndex = 4;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(11, 203);
            this.btCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(341, 44);
            this.btCreate.TabIndex = 8;
            this.btCreate.Text = "Create Task";
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
            this.lbRules.TabIndex = 7;
            this.lbRules.SelectedIndexChanged += new System.EventHandler(this.lbRules_SelectedIndexChanged);
            // 
            // cbRuleList
            // 
            this.cbRuleList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRuleList.FormattingEnabled = true;
            this.cbRuleList.Items.AddRange(new object[] {
            "",
            "Marker Timing",
            "Marker to other Markers Distance",
            "Marker to Goal Distance"});
            this.cbRuleList.Location = new System.Drawing.Point(371, 185);
            this.cbRuleList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbRuleList.Name = "cbRuleList";
            this.cbRuleList.Size = new System.Drawing.Size(228, 28);
            this.cbRuleList.TabIndex = 5;
            this.cbRuleList.SelectedIndexChanged += new System.EventHandler(this.cbRuleList_SelectedIndexChanged);
            // 
            // btRemoveRule
            // 
            this.btRemoveRule.Location = new System.Drawing.Point(607, 184);
            this.btRemoveRule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btRemoveRule.Name = "btRemoveRule";
            this.btRemoveRule.Size = new System.Drawing.Size(105, 31);
            this.btRemoveRule.TabIndex = 9;
            this.btRemoveRule.Text = "Remove Rule";
            this.btRemoveRule.UseVisualStyleBackColor = true;
            this.btRemoveRule.Click += new System.EventHandler(this.btRemoveRule_Click);
            // 
            // plRuleControl
            // 
            this.plRuleControl.Location = new System.Drawing.Point(358, 221);
            this.plRuleControl.Name = "plRuleControl";
            this.plRuleControl.Size = new System.Drawing.Size(451, 555);
            this.plRuleControl.TabIndex = 10;
            // 
            // ElbowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.plRuleControl);
            this.Controls.Add(this.btRemoveRule);
            this.Controls.Add(this.cbRuleList);
            this.Controls.Add(this.lbRules);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.tbThirdMarkerNumber);
            this.Controls.Add(this.tbSecondMarkerNumber);
            this.Controls.Add(this.tbFirstMarkerNumber);
            this.Controls.Add(this.tbTaskNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ElbowControl";
            this.Size = new System.Drawing.Size(809, 776);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbTaskNumber;
        private System.Windows.Forms.TextBox tbFirstMarkerNumber;
        private System.Windows.Forms.TextBox tbSecondMarkerNumber;
        private System.Windows.Forms.TextBox tbThirdMarkerNumber;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.ListBox lbRules;
        private System.Windows.Forms.ComboBox cbRuleList;
        private System.Windows.Forms.Button btRemoveRule;
        private System.Windows.Forms.Panel plRuleControl;
    }
}
