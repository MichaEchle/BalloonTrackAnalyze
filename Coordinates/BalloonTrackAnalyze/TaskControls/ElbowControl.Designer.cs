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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Elbow Task Setup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Task No.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "1st Marker No.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "2nd Marker No.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "3rd Marker No.";
            // 
            // tbTaskNumber
            // 
            this.tbTaskNumber.Location = new System.Drawing.Point(124, 36);
            this.tbTaskNumber.Name = "tbTaskNumber";
            this.tbTaskNumber.Size = new System.Drawing.Size(84, 23);
            this.tbTaskNumber.TabIndex = 1;
            // 
            // tbFirstMarkerNumber
            // 
            this.tbFirstMarkerNumber.Location = new System.Drawing.Point(124, 67);
            this.tbFirstMarkerNumber.Name = "tbFirstMarkerNumber";
            this.tbFirstMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbFirstMarkerNumber.TabIndex = 1;
            // 
            // tbSecondMarkerNumber
            // 
            this.tbSecondMarkerNumber.Location = new System.Drawing.Point(124, 98);
            this.tbSecondMarkerNumber.Name = "tbSecondMarkerNumber";
            this.tbSecondMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbSecondMarkerNumber.TabIndex = 1;
            // 
            // tbThirdMarkerNumber
            // 
            this.tbThirdMarkerNumber.Location = new System.Drawing.Point(124, 127);
            this.tbThirdMarkerNumber.Name = "tbThirdMarkerNumber";
            this.tbThirdMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbThirdMarkerNumber.TabIndex = 1;
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(76, 400);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(75, 23);
            this.btCreate.TabIndex = 2;
            this.btCreate.Text = "Create";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // ElbowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Name = "ElbowControl";
            this.Size = new System.Drawing.Size(446, 506);
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
    }
}
