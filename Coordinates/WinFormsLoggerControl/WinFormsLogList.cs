using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using UILoggingProvider;

namespace WinFormsLoggerControl;

public partial class WinFormsLogList : UserControl
{
    public WinFormsLogList()
    {
        InitializeComponent();
    }

    private ChannelReader<LogItem> _reader;

    private void WinFormsLogList_Load(object sender, EventArgs e)
    {
        logList.SmallImageList = new ImageList();
        logList.SmallImageList.Images.Add("Information", Resources.StatusInformation_exp_16x);
        logList.SmallImageList.Images.Add("Warning", Resources.StatusWarning_exp_16x);
        logList.SmallImageList.Images.Add("Error", Resources.StatusCriticalError_exp_16x);
        logList.FullRowSelect = true;
        _reader = UILoggerProvider.Instance.LogItemReader;

        Task.Run(ReadLogItemsAsync);
    }

    private async Task ReadLogItemsAsync()
    {
        await foreach (LogItem item in _reader.ReadAllAsync())
        {

            ListViewItem listViewItem = new()
            {
                Text = item.Timestamp.ToString("dd-MMM-yyyy HH:mm:ss")
            };
            listViewItem.SubItems.Add(item.Message);
            listViewItem.SubItems.Add(item.Source);
            listViewItem.ForeColor = Color.Black;
            switch (item.LogLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.None:
                default:
                    listViewItem.BackColor = Color.White;
                    listViewItem.ImageIndex = 0;
                    break;
                case LogLevel.Warning:
                    listViewItem.BackColor = Color.Yellow;
                    listViewItem.ImageIndex = 1;
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    listViewItem.BackColor = Color.Red;
                    listViewItem.ImageIndex = 2;
                    break;
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() => logList.Items.Add(listViewItem)));
            }
            else
            {
                logList.Items.Add(listViewItem);
            }
        }
    }
}
