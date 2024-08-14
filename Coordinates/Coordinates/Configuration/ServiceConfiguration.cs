using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coordinates.Configuration;
public static class ServiceConfiguration
{
    public static IServiceCollection ProviderLoggerFactory(this IServiceCollection services)
    {
        LoggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        return services;
    }

    public static ILoggerFactory LoggerFactory
    {
        get; private set;
    }

}

public class LoggerFactoryActivator : ILoggerFactoryActivator
{
    public void ActivateLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactoryProvider.LoggerFactory = loggerFactory;
    }
}

public interface ILoggerFactoryActivator
{
    public void ActivateLoggerFactory(ILoggerFactory loggerFactory);
}


internal static class LoggerFactoryProvider
{
    internal static ILoggerFactory LoggerFactory
    {
        get; set;
    }

    internal static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }
}

