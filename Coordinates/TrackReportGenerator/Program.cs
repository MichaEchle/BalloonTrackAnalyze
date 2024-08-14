using Coordinates.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using UILoggingProvider;

namespace TrackReportGenerator
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var uiLoggerProvider = UILoggerProvider.Instance;
            var builder = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddProvider(uiLoggerProvider);
                    })
                .ConfigureServices((_, services) =>
                    {
                        services.AddLogging();
                        services.AddSingleton<TrackReportGeneratorForm>();
                        services.AddSingleton<ExcelTrackReportGenerator>();
                        services.ProviderLoggerFactory();
                    });

            var host = builder.Build();
            //_ = host.Services.GetRequiredService<CoordinatesLoggingConnector>();
            Application.Run(host.Services.GetRequiredService<TrackReportGeneratorForm>());
        }
    }
}
