using System;
using System.Threading.Tasks;
using GarageGroup.Infra;
using GarageGroup.Infra.Endpoint;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder;

partial class EndpointApplicationBuilder
{
    public static TApplicationBuilder UseEndpoint<TApplicationBuilder, TEndpoint>(
        this TApplicationBuilder app, Func<IServiceProvider, TEndpoint> endpointResolver)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpoint
    {
        ArgumentNullException.ThrowIfNull(app);
        ArgumentNullException.ThrowIfNull(endpointResolver);

        return app.InternalUseEndpoint(endpointResolver);
    }

    internal static TApplicationBuilder InternalUseEndpoint<TApplicationBuilder, TEndpoint>(
        this TApplicationBuilder app, Func<IServiceProvider, TEndpoint> endpointResolver)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpoint
    {
        var metadata = TEndpoint.GetEndpointMetadata();

        var verb = metadata.Method.ToString("F").ToUpperInvariant();
        var template = metadata.Route;

        var route = new RouteBuilder(app).MapVerb(verb, template, InnerInvokeAsync).Build();
        _ = app.UseRouter(route);

        if (app is ISwaggerBuilder swaggerBuilder)
        {
            _ = swaggerBuilder.Use(TEndpoint.GetEndpointMetadata().Configure);
        }

        return app;

        Task InnerInvokeAsync(HttpContext context)
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                return Task.FromCanceled(context.RequestAborted);
            }

            return InvokeAsync(context, endpointResolver.Invoke(context.RequestServices));
        }
    }
}