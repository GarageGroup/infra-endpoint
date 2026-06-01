namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string GenericEndpointSetTypeSourceCode
        =
        """
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("ProductGet", EndpointMethod.Get, "/products/{id}")]
            public interface IProductGetFunc
            {
                ValueTask<ProductGetOut> InvokeAsync(ProductGetIn input, CancellationToken cancellationToken);
            }

            public sealed record class ProductGetIn([RouteIn] int Id);

            public sealed record class ProductGetOut
            {
                [RootBodyOut]
                public string Name { get; init; }
            }

            [EndpointSet]
            public interface IProductApi<TValue> : IProductGetFunc
            {
            }
        }
        """;

    internal const string EndpointSetWithoutEndpointFunctionsSourceCode
        =
        """
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            public interface IProductContract
            {
                ValueTask<ProductOut> InvokeAsync(ProductIn input, CancellationToken cancellationToken);
            }

            public sealed record class ProductIn([RouteIn] int Id);

            public sealed record class ProductOut
            {
                [RootBodyOut]
                public string Name { get; init; }
            }

            [EndpointSet]
            public interface IProductApi : IProductContract
            {
            }
        }
        """;

    internal const string EndpointSetWithDuplicateOperationIdsSourceCode
        =
        """
        using System.Threading;
        using System.Threading.Tasks;
        using GarageGroup.Infra;

        namespace Demo
        {
            [Endpoint("ProductGet", EndpointMethod.Get, "/products/{id}")]
            public interface IProductGetFunc
            {
                ValueTask<ProductGetOut> InvokeAsync(ProductGetIn input, CancellationToken cancellationToken);
            }

            [Endpoint("ProductGet", EndpointMethod.Delete, "/products/{id}")]
            public interface IProductDeleteFunc
            {
                ValueTask<ProductDeleteOut> InvokeAsync(ProductDeleteIn input, CancellationToken cancellationToken);
            }

            public sealed record class ProductGetIn([RouteIn] int Id);

            public sealed record class ProductGetOut
            {
                [RootBodyOut]
                public string Name { get; init; }
            }

            public sealed record class ProductDeleteIn([RouteIn] int Id);

            public sealed record class ProductDeleteOut;

            [EndpointSet]
            public interface IProductApi : IProductGetFunc, IProductDeleteFunc
            {
            }
        }
        """;
}
