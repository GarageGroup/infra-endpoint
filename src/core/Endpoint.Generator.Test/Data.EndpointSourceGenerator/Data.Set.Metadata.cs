namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorData
{
    internal const string ProductSetMetadataHintName
        =
        "ProductEndpointSet.Metadata.g.cs";

    internal const string ProductSetMetadataSourceCode
        =
        """
        // Auto-generated code by PrimeFuncPack
        #nullable enable

        using Demo.Product.Delete;
        using Demo.Product.Get;
        using GarageGroup.Infra.Endpoint;
        using System.Collections.Generic;

        namespace Demo.Product.Api;

        partial class ProductEndpointSet
        {
            public static IReadOnlyCollection<EndpointMetadata> Metadata { get; }
                =
                [
                    ProductGetEndpoint.GetEndpointMetadata(),
                    ProductDeleteEndpoint.GetEndpointMetadata()
                ];
        }
        """;
}