using System;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Endpoint;

partial class EndpointServiceProviderExtensions
{
    public static ILogger<TEndpoint>? GetEndpointLogger<TEndpoint>(this IServiceProvider? serviceProvider)
        where TEndpoint : IEndpoint
    {
        var loggerFactoryValue = serviceProvider?.GetService(typeof(ILoggerFactory));

        if (loggerFactoryValue is not ILoggerFactory loggerFactory)
        {
            return null;
        }

        return loggerFactory.CreateLogger<TEndpoint>();
    }
}