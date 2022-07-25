using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using GGroupp.Infra.Endpoint;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Builder;

partial class EndpointApplicationBuilder
{
    public static TApplicationBuilder UseEndpoint<TApplicationBuilder, TEndpoint>(
        this TApplicationBuilder app!!, Func<IServiceProvider, TEndpoint> endpointResolver!!)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpoint
    {
        var metadata = TEndpoint.GetEndpointMetadata();

        if (standardMethods.Contains(metadata.Method))
        {
            app.UseEndpoints(InnerConfigure);
            return app;
        }

        app.MapWhen(IsMatch, ConfigureWhen);
        return app;

        bool IsMatch(HttpContext context)
            =>
            string.Equals(context.Request.Method, Enum.GetName(metadata.Method), StringComparison.InvariantCultureIgnoreCase);

        void ConfigureWhen(IApplicationBuilder builder)
            =>
            builder.UseEndpoints(InnerConfigure);
        
        void InnerConfigure(IEndpointRouteBuilder builder)
            =>
            builder.ConfigureEndpoint(metadata.Method, metadata.Route, endpointResolver);
    }

    private static void ConfigureEndpoint(
        this IEndpointRouteBuilder builder, EndpointMethod method, string route, Func<IServiceProvider, IEndpoint> endpointResolver)
    {
        if (method is EndpointMethod.Get)
        {
            builder.MapGet(route, InnerInvokeAsync);
        }
        else if (method is EndpointMethod.Post)
        {
            builder.MapPost(route, InnerInvokeAsync);
        }
        else if (method is EndpointMethod.Put)
        {
            builder.MapPut(route, InnerInvokeAsync);
        }
        else if (method is EndpointMethod.Delete)
        {
            builder.MapDelete(route, InnerInvokeAsync);
        }
        else
        {
            builder.Map(route, InnerInvokeAsync);
        }
        
        Task InnerInvokeAsync(HttpContext context)
            =>
            context.InvokeAsync(endpointResolver);
    }

    private static Task InvokeAsync(this HttpContext context, Func<IServiceProvider, IEndpoint> endpointResolver)
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