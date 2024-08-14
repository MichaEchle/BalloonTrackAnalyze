using BalloonTrackAnalyze.TaskControls;
using Competition.Configuration;
using Coordinates.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using UILoggingProvider;

namespace BalloonTrackAnalyze
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
                    services.AddCompetitionServices();
                    services.AddSingleton<Form1>();
                    services.ProviderLoggerFactory();
                });

            var host = builder.Build();
            //_ = host.Services.GetRequiredService<CoordinatesLoggingConnector>();
            //_ = host.Services.GetRequiredService<CompetitionLoggingConnector>();
            Application.Run(host.Services.GetRequiredService<Form1>());
        }
    }
}
