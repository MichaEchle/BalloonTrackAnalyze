namespace LoggerComponent
{
    public partial class LogFileViewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogFileViewer));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.labelToFocus = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WrapTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenLogfileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 24);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1070, 575);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 26);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // labelToFocus
            // 
            this.labelToFocus.AutoSize = true;
            this.labelToFocus.Location = new System.Drawing.Point(1015, 31);
            this.labelToFocus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelToFocus.Name = "labelToFocus";
            this.labelToFocus.Size = new System.Drawing.Size(38, 15);
            this.labelToFocus.TabIndex = 2;
            this.labelToFocus.Text = "label1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyToolStripMenuItem,
            this.WrapTextToolStripMenuItem,
            this.OpenLogfileLocationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1070, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CopyToolStripMenuItem.Image")));
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.CopyToolStripMenuItem.Text = "Copy";
            this.CopyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // WrapTextToolStripMenuItem
            // 
            this.WrapTextToolStripMenuItem.CheckOnClick = true;
            this.WrapTextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("WrapTextToolStripMenuItem.Image")));
            this.WrapTextToolStripMenuItem.Name = "WrapTextToolStripMenuItem";
            this.WrapTextToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.WrapTextToolStripMenuItem.Text = "Wrap Text";
            this.WrapTextToolStripMenuItem.CheckedChanged += new System.EventHandler(this.WrapTextToolStripMenuItem_CheckedChanged);
            this.WrapTextToolStripMenuItem.Click += new System.EventHandler(this.WrapTextToolStripMenuItem_Click);
            // 
            // OpenLogfileLocationToolStripMenuItem
            // 
            this.OpenLogfileLocationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenLogfileLocationToolStripMenuItem.Image")));
            this.OpenLogfileLocationToolStripMenuItem.Name = "OpenLogfileLocationToolStripMenuItem";
            this.OpenLogfileLocationToolStripMenuItem.Size = new System.Drawing.Size(152, 20);
            this.OpenLogfileLocationToolStripMenuItem.Text = "Open Logfile Location";
            this.OpenLogfileLocationToolStripMenuItem.Click += new System.EventHandler(this.OpenLogfileLocationToolStripMenuItem_Click);
            // 
            // LogFileViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 599);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelToFocus);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(257, 104);
            this.Name = "LogFileViewer";
            this.Text = "Logfile Viewer";
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labelToFocus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem CopyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenLogfileLocationToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem WrapTextToolStripMenuItem;
    }
}