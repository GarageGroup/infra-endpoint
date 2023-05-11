using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Endpoint;
using GGroupp.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

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

#if NET7_0_OR_GREATER
        var metadata = TEndpoint.GetEndpointMetadata();
#else
        var metadata = IEndpointMetadataProvider.GetEndpointMetadata<TEndpoint>();
#endif

        var verb = metadata.Method.ToString("F").ToUpperInvariant();
        var template = metadata.Route;

        var route = new RouteBuilder(app).MapVerb(verb, template, InnerInvokeAsync).Build();

        _ = app.UseRouter(route);

        if (app is ISwaggerBuilder swaggerBuilder)
        {
            _ = swaggerBuilder.Use(EndpointSwaggerConfigurator.Configure<TEndpoint>);
        }

        return app;

        Task InnerInvokeAsync(HttpContext context)
            =>
            context.InvokeAsync(endpointResolver);
    }

    private static Task InvokeAsync<TEndpoint>(this HttpContext context, Func<IServiceProvider, TEndpoint> endpointResolver)
        where TEndpoint : class, IEndpoint
    {
        if (context.RequestAborted.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        return InnerInvokeAsync(context, endpointResolver.Invoke(context.RequestServices));
    }

    private static async Task InnerInvokeAsync(HttpContext context, IEndpoint endpoint)
    {
        var request = CreateEndpointRequest(context.Request, context.User);
        var response = await endpoint.InvokeAsync(request, context.RequestAborted).ConfigureAwait(false);

        await context.Response.WriteResponseAsync(response, context.RequestAborted).ConfigureAwait(false);
    }

    private static EndpointRequest CreateEndpointRequest(HttpRequest httpRequest, [AllowNull] ClaimsPrincipal user)
        =>
        new(
            headers: httpRequest.Headers?.Select(MapValue).ToArray(),
            queryParameters: httpRequest.Query?.Select(MapValue).ToArray(),
            routeValues: httpRequest.RouteValues?.Select(MapValue).ToArray(),
            user: user,
            body: httpRequest.Body);

    private static async ValueTask WriteResponseAsync(this HttpResponse httpResponse, EndpointResponse response, CancellationToken cancellationToken)
    {
        httpResponse.StatusCode = response.StatusCode;

        foreach (var header in response.Headers.Where(NotEmpty))
        {
            httpResponse.AddHeader(header!);
        }

        if (response.Body is null)
        {
            return;
        }

        var buffer = new Memory<byte>(new byte[response.Body.Length]);
        await response.Body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

        await httpResponse.BodyWriter.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

        static bool NotEmpty(KeyValuePair<string, string?> pair)
            =>
            string.IsNullOrEmpty(pair.Value) is false;
    }

    private static void AddHeader(this HttpResponse httpResponse, KeyValuePair<string, string> header)
    {
        if (httpResponse.Headers.ContainsKey(header.Key) is false)
        {
            httpResponse.Headers.Add(header.Key, header.Value);
            return;
        }

        var headerValue = httpResponse.Headers[header.Key];
        httpResponse.Headers[header.Key] = StringValues.Concat(headerValue, header.Value);
    }

    private static KeyValuePair<string, string?> MapValue(KeyValuePair<string, StringValues> pair)
        =>
        new(
            pair.Key, pair.Value);

    private static KeyValuePair<string, string?> MapValue(KeyValuePair<string, object?> pair)
        =>
        new(
            pair.Key, pair.Value?.ToString());
}