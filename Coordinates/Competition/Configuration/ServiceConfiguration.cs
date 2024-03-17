using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Competition.Configuration;
public static class ServiceConfiguration
{
    public static IServiceCollection AddCompetitionServices(this IServiceCollection services)
    {
        services.AddSingleton<CompetitionLoggingConnector>(service =>
        {
            return new CompetitionLoggingConnector(service.GetRequiredService<ILoggerFactory>());
        });
        return services;
    }
}
