namespace BalloonTrackAnalyze.ValidationControls
{
    partial class DeclarationToGoalDistanceRuleControl
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
            this.tbMinimumDistance = new System.Windows.Forms.TextBox();
            this.tbMaximumDistance = new System.Windows.Forms.TextBox();
            this.btCreate = new System.Windows.Forms.Button();
            this.rbMinimumDistanceFeet = new System.Windows.Forms.RadioButton();
            this.rbMinimumDistanceMeter = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbMaximumDistanceFeet = new System.Windows.Forms.RadioButton();
            this.rbMaximumDistanceMeter = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(314, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Declaration to Goal Distance Rule Setup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Min. Distance";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Max. Distance";
            // 
            // tbMinimumDistance
            // 
            this.tbMinimumDistance.Location = new System.Drawing.Point(124, 39);
            this.tbMinimumDistance.Name = "tbMinimumDistance";
            this.tbMinimumDistance.Size = new System.Drawing.Size(84, 23);
            this.tbMinimumDistance.TabIndex = 1;
            // 
            // tbMaximumDistance
            // 
            this.tbMaximumDistance.Location = new System.Drawing.Point(124, 68);
            this.tbMaximumDistance.Name = "tbMaximumDistance";
            this.tbMaximumDistance.Size = new System.Drawing.Size(84, 23);
            this.tbMaximumDistance.TabIndex = 3;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(10, 97);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(298, 28);
            this.btCreate.TabIndex = 5;
            this.btCreate.Text = "Create Rule";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // rbMinimumDistanceFeet
            // 
            this.rbMinimumDistanceFeet.AutoSize = true;
            this.rbMinimumDistanceFeet.Location = new System.Drawing.Point(52, 1);
            this.rbMinimumDistanceFeet.Name = "rbMinimumDistanceFeet";
            this.rbMinimumDistanceFeet.Size = new System.Drawing.Size(33, 19);
            this.rbMinimumDistanceFeet.TabIndex = 2;
            this.rbMinimumDistanceFeet.TabStop = true;
            this.rbMinimumDistanceFeet.Text = "ft";
            this.rbMinimumDistanceFeet.UseVisualStyleBackColor = true;
            // 
            // rbMinimumDistanceMeter
            // 
            this.rbMinimumDistanceMeter.AutoSize = true;
            this.rbMinimumDistanceMeter.Checked = true;
            this.rbMinimumDistanceMeter.Location = new System.Drawing.Point(10, 1);
            this.rbMinimumDistanceMeter.Name = "rbMinimumDistanceMeter";
            this.rbMinimumDistanceMeter.Size = new System.Drawing.Size(36, 19);
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
            this.panel1.Location = new System.Drawing.Point(214, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(94, 23);
            this.panel1.TabIndex = 2;
            // 
            // rbMaximumDistanceFeet
            // 
            this.rbMaximumDistanceFeet.AutoSize = true;
            this.rbMaximumDistanceFeet.Location = new System.Drawing.Point(52, 1);
            this.rbMaximumDistanceFeet.Name = "rbMaximumDistanceFeet";
            this.rbMaximumDistanceFeet.Size = new System.Drawing.Size(33, 19);
            this.rbMaximumDistanceFeet.TabIndex = 2;
            this.rbMaximumDistanceFeet.TabStop = true;
            this.rbMaximumDistanceFeet.Text = "ft";
            this.rbMaximumDistanceFeet.UseVisualStyleBackColor = true;
            // 
            // rbMaximumDistanceMeter
            // 
            this.rbMaximumDistanceMeter.AutoSize = true;
            this.rbMaximumDistanceMeter.Checked = true;
            this.rbMaximumDistanceMeter.Location = new System.Drawing.Point(10, 1);
            this.rbMaximumDistanceMeter.Name = "rbMaximumDistanceMeter";
            this.rbMaximumDistanceMeter.Size = new System.Drawing.Size(36, 19);
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
            this.panel2.Location = new System.Drawing.Point(214, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(94, 23);
            this.panel2.TabIndex = 4;
            // 
            // DeclarationToGoalDistanceRuleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.tbMaximumDistance);
            this.Controls.Add(this.tbMinimumDistance);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "DeclarationToGoalDistanceRuleControl";
            this.Size = new System.Drawing.Size(360, 160);
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
        private System.Windows.Forms.TextBox tbMinimumDistance;
        private System.Windows.Forms.TextBox tbMaximumDistance;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.RadioButton rbMinimumDistanceFeet;
        private System.Windows.Forms.RadioButton rbMinimumDistanceMeter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbMaximumDistanceFeet;
        private System.Windows.Forms.RadioButton rbMaximumDistanceMeter;
        private System.Windows.Forms.Panel panel2;
    }
}
