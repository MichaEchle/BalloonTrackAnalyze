using LoggingConnector;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;
using UILoggingProvider;

namespace BLC2021
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
                });

            var host = builder.Build();
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            LogConnector.LoggerFactory = loggerFactory;
            Application.Run(new BLC2021Launch());
        }
    }
}
