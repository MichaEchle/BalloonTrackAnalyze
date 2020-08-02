namespace BalloonTrackAnalyze.TaskControls
{
    partial class LandRunControl
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
            this.button1 = new System.Windows.Forms.Button();
            this.tbThirdMarkerNumber = new System.Windows.Forms.TextBox();
            this.tbSecondMarkerNumber = new System.Windows.Forms.TextBox();
            this.tbFirstMarkerNumber = new System.Windows.Forms.TextBox();
            this.tbTaskNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(88, 405);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // tbThirdMarkerNumber
            // 
            this.tbThirdMarkerNumber.Location = new System.Drawing.Point(124, 123);
            this.tbThirdMarkerNumber.Name = "tbThirdMarkerNumber";
            this.tbThirdMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbThirdMarkerNumber.TabIndex = 1;
            // 
            // tbSecondMarkerNumber
            // 
            this.tbSecondMarkerNumber.Location = new System.Drawing.Point(124, 94);
            this.tbSecondMarkerNumber.Name = "tbSecondMarkerNumber";
            this.tbSecondMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbSecondMarkerNumber.TabIndex = 1;
            // 
            // tbFirstMarkerNumber
            // 
            this.tbFirstMarkerNumber.Location = new System.Drawing.Point(124, 65);
            this.tbFirstMarkerNumber.Name = "tbFirstMarkerNumber";
            this.tbFirstMarkerNumber.Size = new System.Drawing.Size(84, 23);
            this.tbFirstMarkerNumber.TabIndex = 1;
            // 
            // tbTaskNumber
            // 
            this.tbTaskNumber.Location = new System.Drawing.Point(124, 36);
            this.tbTaskNumber.Name = "tbTaskNumber";
            this.tbTaskNumber.Size = new System.Drawing.Size(84, 23);
            this.tbTaskNumber.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "3rd Marker No.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "2nd Marker No.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "1st Marker No.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Task No.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(10, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 21);
            this.label5.TabIndex = 0;
            this.label5.Text = "Landrun Task Setup";
            // 
            // LandRunControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbTaskNumber);
            this.Controls.Add(this.tbFirstMarkerNumber);
            this.Controls.Add(this.tbSecondMarkerNumber);
            this.Controls.Add(this.tbThirdMarkerNumber);
            this.Controls.Add(this.button1);
            this.Name = "LandRunControl";
            this.Size = new System.Drawing.Size(557, 593);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbThirdMarkerNumber;
        private System.Windows.Forms.TextBox tbSecondMarkerNumber;
        private System.Windows.Forms.TextBox tbFirstMarkerNumber;
        private System.Windows.Forms.TextBox tbTaskNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
