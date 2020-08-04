namespace BalloonTrackAnalyze.TaskControls
{
    partial class PieControl
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
            this.btRemoveTier = new System.Windows.Forms.Button();
            this.btCreate = new System.Windows.Forms.Button();
            this.tbTaskNumber = new System.Windows.Forms.TextBox();
            this.lbPieTiers = new System.Windows.Forms.ListBox();
            this.pieTierControl1 = new BalloonTrackAnalyze.TaskControls.PieTierControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pie Task Setup";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Task No.";
            // 
            // btRemoveTier
            // 
            this.btRemoveTier.Location = new System.Drawing.Point(8, 506);
            this.btRemoveTier.Name = "btRemoveTier";
            this.btRemoveTier.Size = new System.Drawing.Size(196, 23);
            this.btRemoveTier.TabIndex = 3;
            this.btRemoveTier.Text = "Remove Tier";
            this.btRemoveTier.UseVisualStyleBackColor = true;
            this.btRemoveTier.Click += new System.EventHandler(this.btRemoveTier_Click);
            // 
            // btCreate
            // 
            this.btCreate.Location = new System.Drawing.Point(8, 535);
            this.btCreate.Name = "btCreate";
            this.btCreate.Size = new System.Drawing.Size(295, 33);
            this.btCreate.TabIndex = 3;
            this.btCreate.Text = "Create Task";
            this.btCreate.UseVisualStyleBackColor = true;
            this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
            // 
            // tbTaskNumber
            // 
            this.tbTaskNumber.Location = new System.Drawing.Point(124, 36);
            this.tbTaskNumber.Name = "tbTaskNumber";
            this.tbTaskNumber.Size = new System.Drawing.Size(84, 23);
            this.tbTaskNumber.TabIndex = 5;
            // 
            // lbPieTiers
            // 
            this.lbPieTiers.FormattingEnabled = true;
            this.lbPieTiers.ItemHeight = 15;
            this.lbPieTiers.Location = new System.Drawing.Point(8, 406);
            this.lbPieTiers.Name = "lbPieTiers";
            this.lbPieTiers.Size = new System.Drawing.Size(196, 94);
            this.lbPieTiers.TabIndex = 6;
            this.lbPieTiers.SelectedIndexChanged += new System.EventHandler(this.lbPieTiers_SelectedIndexChanged);
            // 
            // pieTierControl1
            // 
            this.pieTierControl1.BackColor = System.Drawing.Color.White;
            this.pieTierControl1.Location = new System.Drawing.Point(0, 65);
            this.pieTierControl1.Name = "pieTierControl1";
            this.pieTierControl1.Size = new System.Drawing.Size(705, 335);
            this.pieTierControl1.TabIndex = 7;
            // 
            // PieControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pieTierControl1);
            this.Controls.Add(this.lbPieTiers);
            this.Controls.Add(this.tbTaskNumber);
            this.Controls.Add(this.btCreate);
            this.Controls.Add(this.btRemoveTier);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PieControl";
            this.Size = new System.Drawing.Size(708, 582);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btRemoveTier;
        private System.Windows.Forms.Button btCreate;
        private System.Windows.Forms.TextBox tbTaskNumber;
        private System.Windows.Forms.ListBox lbPieTiers;
        private PieTierControl pieTierControl1;
    }
}
