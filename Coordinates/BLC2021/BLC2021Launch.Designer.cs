
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
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btTaskSheet1
            // 
            this.btTaskSheet1.Location = new System.Drawing.Point(12, 42);
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
            this.panel1.Location = new System.Drawing.Point(12, 2);
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
            // BLC2021Launch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 205);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btTaskSheet1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BLC2021Launch";
            this.Text = "BLC 2021";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btTaskSheet1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbFiddleMode;
        private System.Windows.Forms.RadioButton rbBatchMode;
    }
}

