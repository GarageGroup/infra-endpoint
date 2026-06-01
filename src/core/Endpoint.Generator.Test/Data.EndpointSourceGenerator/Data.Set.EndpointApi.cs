namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string ProductEndpointApiSetSourceCode
        =
        """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo.Product.Get
        {
            [Endpoint("ProductGet", EndpointMethod.Get, "/products/{id}")]
            public interface IProductGetFunc
            {
                ValueTask<ProductGetOut> InvokeAsync(ProductGetIn input, CancellationToken cancellationToken);
            }

            public sealed record class ProductGetIn
            {
                public ProductGetIn([RouteIn] int id)
                    =>
                    Id = id;

                public int Id { get; }
            }

            public sealed record class ProductGetOut
            {
                [RootBodyOut]
                public string Name { get; init; }
            }
        }

        namespace Demo.Product.Delete
        {
            [Endpoint("ProductDelete", EndpointMethod.Delete, "/products/{id}")]
            public interface IProductDeleteFunc
            {
                ValueTask<Unit> InvokeAsync(ProductDeleteIn input, CancellationToken cancellationToken);
            }

            public sealed record class ProductDeleteIn
            {
                public ProductDeleteIn([RouteIn] int id)
                    =>
                    Id = id;

                public int Id { get; }
            }
        }

        namespace Demo.Product.Api
        {
            using Demo.Product.Delete;
            using Demo.Product.Get;

            [EndpointSet]
            public interface IProductEndpointApi : IProductGetFunc, IProductDeleteFunc
            {
            }
        }

        namespace System
        {
            public readonly record struct Unit;
        }
        """;

    internal const string ProductEndpointApiSetFactoryHintName
        =
        "ProductEndpointSet.g.cs";

    internal const string ProductEndpointApiSetFactorySourceCode
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
            public static ProductEndpointSet Resolve(IServiceProvider? serviceProvider, IProductEndpointApi endpointApi)
                =>
                new(
                    endpointApi: endpointApi ?? throw new ArgumentNullException(nameof(endpointApi)),
                    logger: serviceProvider?.GetEndpointLogger<ProductEndpointSet>());

            private readonly IProductEndpointApi endpointApi;

            private readonly ILogger? logger;

            private ProductEndpointSet(IProductEndpointApi endpointApi, ILogger? logger)
            {
                this.endpointApi = endpointApi;
                this.logger = logger;
            }
        }
        """;
}
