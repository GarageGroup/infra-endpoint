using System;
using System.Threading.Tasks;
using GarageGroup.Infra;
using GarageGroup.Infra.Endpoint;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder;

partial class EndpointApplicationBuilder
{
    public static TApplicationBuilder UseEndpointSet<TApplicationBuilder, TEndpoint>(
        this TApplicationBuilder app, Func<IServiceProvider, TEndpoint> endpointResolver)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpointSet
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(endpointResolver);

        return app.InternalUseEndpointSet(endpointResolver);
    }

    internal static TApplicationBuilder InternalUseEndpointSet<TApplicationBuilder, TEndpoint>(
        this TApplicationBuilder app, Func<IServiceProvider, TEndpoint> endpointResolver)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpointSet
    {
        var metadata = TEndpoint.Metadata;
        if (metadata?.Count is not > 0)
        {
            return app;
        }

        var routeBuilder = new RouteBuilder(app);
        foreach (var operation in metadata)
        {
            var verb = operation.Method.ToString("F").ToUpperInvariant();
            _ = routeBuilder.MapVerb(verb, operation.Route, InnerInvokeAsync);

            Task InnerInvokeAsync(HttpContext context)
                =>
                InvokeAsync(context, endpointResolver.Invoke(context.RequestServices), operation.OperationId);
        }

        _ = app.UseRouter(routeBuilder.Build());

        if (app is not ISwaggerBuilder swaggerBuilder)
        {
            return app;
        }

        foreach (var operation in metadata)
        {
            _ = swaggerBuilder.Use(operation.Configure);
        }

        return app;
    }
}