namespace LoggerComponent
{
    partial class LogListView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogListView));
            this.LogListViewer = new System.Windows.Forms.ListView();
            this.ColumnHeaderTimeStamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeaderSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LogListViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenLogfileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CopyLogLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LogListViewContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogListView
            // 
            this.LogListViewer.AllowColumnReorder = true;
            this.LogListViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogListViewer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderTimeStamp,
            this.ColumnHeaderText,
            this.ColumnHeaderSource});
            this.LogListViewer.ContextMenuStrip = this.LogListViewContextMenuStrip;
            this.LogListViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogListViewer.FullRowSelect = true;
            this.LogListViewer.HideSelection = false;
            this.LogListViewer.Location = new System.Drawing.Point(0, 0);
            this.LogListViewer.MultiSelect = false;
            this.LogListViewer.Name = "LogListView";
            this.LogListViewer.ShowItemToolTips = true;
            this.LogListViewer.Size = new System.Drawing.Size(919, 431);
            this.LogListViewer.TabIndex = 5;
            this.LogListViewer.UseCompatibleStateImageBehavior = false;
            this.LogListViewer.View = System.Windows.Forms.View.Details;
            this.LogListViewer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LogListView_MouseDoubleClick);
            // 
            // ColumnHeaderTimeStamp
            // 
            this.ColumnHeaderTimeStamp.Text = "Timestamp";
            this.ColumnHeaderTimeStamp.Width = 140;
            // 
            // ColumnHeaderText
            // 
            this.ColumnHeaderText.Text = "Text";
            this.ColumnHeaderText.Width = 468;
            // 
            // ColumnHeaderSource
            // 
            this.ColumnHeaderSource.Text = "Source";
            this.ColumnHeaderSource.Width = 150;
            // 
            // LogListViewContextMenuStrip
            // 
            this.LogListViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenLogfileToolStripMenuItem,
            this.OpenLogfileLocationToolStripMenuItem,
            this.ClearLogsToolStripMenuItem,
            this.toolStripSeparator3,
            this.CopyLogLineToolStripMenuItem});
            this.LogListViewContextMenuStrip.Name = "logItemStrip";
            this.LogListViewContextMenuStrip.Size = new System.Drawing.Size(205, 98);
            this.LogListViewContextMenuStrip.Text = "Log File";
            this.LogListViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.LogListViewContextMenuStrip_Opening);
            // 
            // OpenLogfileToolStripMenuItem
            // 
            this.OpenLogfileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.OpenLogfileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenLogfileToolStripMenuItem.Image")));
            this.OpenLogfileToolStripMenuItem.Name = "OpenLogfileToolStripMenuItem";
            this.OpenLogfileToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.OpenLogfileToolStripMenuItem.Text = "Open Logfile";
            this.OpenLogfileToolStripMenuItem.ToolTipText = "Open logfile (and highlight current selected log line)";
            this.OpenLogfileToolStripMenuItem.Click += new System.EventHandler(this.OpenLogfileToolStripMenuItem_Click);
            // 
            // OpenLogfileLocationToolStripMenuItem
            // 
            this.OpenLogfileLocationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenLogfileLocationToolStripMenuItem.Image")));
            this.OpenLogfileLocationToolStripMenuItem.Name = "OpenLogfileLocationToolStripMenuItem";
            this.OpenLogfileLocationToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.OpenLogfileLocationToolStripMenuItem.Text = "Open Logfile Location";
            this.OpenLogfileLocationToolStripMenuItem.ToolTipText = "Open explorer at logfile\'s location";
            this.OpenLogfileLocationToolStripMenuItem.Click += new System.EventHandler(this.OpenLogfileLocationToolStripMenuItem_Click);
            // 
            // ClearLogsToolStripMenuItem
            // 
            this.ClearLogsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ClearLogsToolStripMenuItem.Image")));
            this.ClearLogsToolStripMenuItem.Name = "ClearLogsToolStripMenuItem";
            this.ClearLogsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.ClearLogsToolStripMenuItem.Text = "Clear Logs";
            this.ClearLogsToolStripMenuItem.ToolTipText = "Clear log list view";
            this.ClearLogsToolStripMenuItem.Click += new System.EventHandler(this.ClearLogsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(201, 6);
            // 
            // CopyLogLineToolStripMenuItem
            // 
            this.CopyLogLineToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CopyLogLineToolStripMenuItem.Image")));
            this.CopyLogLineToolStripMenuItem.Name = "CopyLogLineToolStripMenuItem";
            this.CopyLogLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyLogLineToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.CopyLogLineToolStripMenuItem.Text = "Copy Log Line";
            this.CopyLogLineToolStripMenuItem.ToolTipText = "Copy current selected log line to clipboard";
            this.CopyLogLineToolStripMenuItem.Click += new System.EventHandler(this.CopyLogLineToolStripMenuItem_Click);
            // 
            // BFLogListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LogListViewer);
            this.Name = "BFLogListView";
            this.Size = new System.Drawing.Size(919, 431);
            this.Load += new System.EventHandler(this.BFLogListView_Load);
            this.LogListViewContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LogListViewer;
        private System.Windows.Forms.ColumnHeader ColumnHeaderTimeStamp;
        private System.Windows.Forms.ColumnHeader ColumnHeaderText;
        private System.Windows.Forms.ColumnHeader ColumnHeaderSource;
        private System.Windows.Forms.ContextMenuStrip LogListViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenLogfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenLogfileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem CopyLogLineToolStripMenuItem;
    }
}
