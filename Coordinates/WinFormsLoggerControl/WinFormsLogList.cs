using Microsoft.Extensions.Logging;
using UILoggingProvider;

namespace WinFormsLoggerControl;

public partial class WinFormsLogList : UserControl
{
    public WinFormsLogList()
    {
        InitializeComponent();
    }

    private void WinFormsLogList_Load(object sender, EventArgs e)
    {
        logList.SmallImageList = new ImageList();
        logList.SmallImageList.Images.Add("Information", Resources.StatusInformation_exp_16x);
        logList.SmallImageList.Images.Add("Warning", Resources.StatusWarning_exp_16x);
        logList.SmallImageList.Images.Add("Error", Resources.StatusCriticalError_exp_16x);
        logList.FullRowSelect = true;
        UILogger.LogEvent += UILogger_LogEvent;
    }

    private void UILogger_LogEvent(object? sender, UILogEventArgs e)
    {
        ListViewItem item = new();
        item.Text=e.Timestamp.ToString("dd-MMM-yyyy HH:mm:ss");
        item.SubItems.Add(e.Message);
        item.SubItems.Add(e.Source);
        item.ForeColor = Color.Black;
        switch (e.LogLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
            case LogLevel.Information:
            case LogLevel.None:
            default:
                item.BackColor = Color.White;
                item.ImageIndex = 0;
                break;
            case LogLevel.Warning:
                item.BackColor = Color.Yellow;
                item.ImageIndex = 1;
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                item.BackColor = Color.Red;
                item.ImageIndex = 2;
                break;
        }
        if (InvokeRequired)
        {
            Invoke(new Action(() => logList.Items.Add(item)));
        }
        else
        {
            logList.Items.Add(item);
        }
    }
}
