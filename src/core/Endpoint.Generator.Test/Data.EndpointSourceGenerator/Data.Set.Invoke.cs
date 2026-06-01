namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string ProductSetInvokeHintName
        =
        "ProductEndpointSet.Invoke.g.cs";

    internal const string ProductSetInvokeSourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using Demo.Product.Delete;
        using Demo.Product.Get;
        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;
        using System.Threading;
        using System.Threading.Tasks;

        namespace Demo.Product.Api;

        partial class ProductEndpointSet
        {
            public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
            {
                if (string.Equals(request.OperationId, "ProductGet", StringComparison.Ordinal))
                {
                    return new ProductGetEndpoint(endpointApi, logger).InvokeAsync(request, cancellationToken);
                }

                if (string.Equals(request.OperationId, "ProductDelete", StringComparison.Ordinal))
                {
                    return new ProductDeleteEndpoint(endpointApi, logger).InvokeAsync(request, cancellationToken);
                }

                logger?.LogError("An endpoint operation id is invalid: {operationId}", request.OperationId);
                return Task.FromResult(new EndpointResponse(500, default, default));
            }
        }
        """;
}