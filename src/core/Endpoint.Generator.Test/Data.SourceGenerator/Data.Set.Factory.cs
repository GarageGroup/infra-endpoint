namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string ProductSetFactoryHintName
        =
        "ProductEndpointSet.g.cs";

    internal const string ProductSetFactorySourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using GarageGroup.Infra.Endpoint;
        using Microsoft.Extensions.Logging;
        using System;

        namespace Demo.Product.Api;

        [EndpointOperationMetadata("ProductGet", "GET", "/products/{id}")]
        [EndpointOperationMetadata("ProductDelete", "DELETE", "/products/{id}")]
        public sealed partial class ProductEndpointSet : IEndpointSet
        {
            public static ProductEndpointSet Resolve(IServiceProvider? serviceProvider, IProductApi endpointApi)
                =>
                new(
                    endpointApi: endpointApi ?? throw new ArgumentNullException(nameof(endpointApi)),
                    logger: serviceProvider?.GetEndpointLogger<ProductEndpointSet>());

            private readonly IProductApi endpointApi;

            private readonly ILogger? logger;

            private ProductEndpointSet(IProductApi endpointApi, ILogger? logger)
            {
                this.endpointApi = endpointApi;
                this.logger = logger;
            }
        }
        """;
}