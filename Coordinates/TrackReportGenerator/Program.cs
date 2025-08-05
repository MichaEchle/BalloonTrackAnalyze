using LoggingConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using UILoggingProvider;

namespace TrackReportGenerator;

public static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        UILoggerProvider uiLoggerProvider = UILoggerProvider.Instance;
        IHostBuilder builder = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
                {
                    _ = logging.ClearProviders();
                    _ = logging.AddProvider(uiLoggerProvider);
                })
            .ConfigureServices((_, services) =>
                {
                    services.AddLogging();
                });

        IHost host = builder.Build();
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        LogConnector.LoggerFactory = loggerFactory;
        Application.Run(new TrackReportGeneratorForm());
    }
}
