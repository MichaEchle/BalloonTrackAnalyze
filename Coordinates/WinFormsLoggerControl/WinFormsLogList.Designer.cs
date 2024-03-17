namespace WinFormsLoggerControl;

partial class WinFormsLogList
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
        logList = new ListView();
        timestamp = new ColumnHeader();
        message = new ColumnHeader();
        source = new ColumnHeader();
        SuspendLayout();
        // 
        // logList
        // 
        logList.Columns.AddRange(new ColumnHeader[] { timestamp, message, source });
        logList.Dock = DockStyle.Fill;
        logList.Location = new Point(0, 0);
        logList.MultiSelect = false;
        logList.Name = "logList";
        logList.Size = new Size(1287, 450);
        logList.TabIndex = 0;
        logList.UseCompatibleStateImageBehavior = false;
        logList.View = View.Details;
        // 
        // timestamp
        // 
        timestamp.Text = "Timestamp";
        timestamp.Width = 240;
        // 
        // message
        // 
        message.Text = "Message";
        message.Width = 800;
        // 
        // source
        // 
        source.Text = "Source";
        source.Width = 240;
        // 
        // WinFormsLogList
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(logList);
        Name = "WinFormsLogList";
        Size = new Size(1287, 450);
        Load += WinFormsLogList_Load;
        ResumeLayout(false);
    }

    #endregion

    private ListView logList;
    private ColumnHeader timestamp;
    private ColumnHeader message;
    private ColumnHeader source;
}
