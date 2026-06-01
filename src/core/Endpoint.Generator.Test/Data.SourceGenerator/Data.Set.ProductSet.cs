namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string ProductSetSourceCode
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

        namespace Demo.Product.Common
        {
            public interface INotEndpointFunc
            {
                ValueTask<Unit> InvokeAsync(Demo.Product.Delete.ProductDeleteIn input, CancellationToken cancellationToken);
            }
        }

        namespace Demo.Product.Api
        {
            using Demo.Product.Common;
            using Demo.Product.Delete;
            using Demo.Product.Get;

            [EndpointSet]
            public interface IProductApi : IProductGetFunc, IProductDeleteFunc, INotEndpointFunc
            {
            }
        }

        namespace System
        {
            public readonly record struct Unit;
        }
        """;
}