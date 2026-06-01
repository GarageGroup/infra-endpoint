using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Endpoint;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Builder;

public static partial class EndpointApplicationBuilder
{
    private static async Task InvokeAsync(HttpContext context, IEndpointInvokeSupplier endpoint, string? operationId = null)
    {
        var request = new EndpointRequest(
            headers: context.Request.Headers?.Select(MapValue).ToArray(),
            queryParameters: context.Request.Query?.Select(MapValue).ToArray(),
            routeValues: context.Request.RouteValues?.Select(MapValue).ToArray(),
            user: context.User,
            body: context.Request.Body)
        {
            OperationId = operationId
        };

        var response = await endpoint.InvokeAsync(request, context.RequestAborted).ConfigureAwait(false);
        await context.Response.WriteResponseAsync(response, context.RequestAborted).ConfigureAwait(false);
    }

    private static async ValueTask WriteResponseAsync(
        this HttpResponse httpResponse, EndpointResponse response, CancellationToken cancellationToken)
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
            httpResponse.Headers.Append(header.Key, header.Value);
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