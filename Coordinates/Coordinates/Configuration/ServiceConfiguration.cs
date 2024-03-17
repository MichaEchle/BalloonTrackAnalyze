using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coordinates.Configuration;
public static class ServiceConfiguration
{
    public static IServiceCollection AddCoordinateServices(this IServiceCollection services)
    {
        services.AddSingleton<CoordinatesLoggingConnector>(service =>
        {
            return new CoordinatesLoggingConnector(service.GetRequiredService<ILoggerFactory>());
        });
        return services;
    }
}
