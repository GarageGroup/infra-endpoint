using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string DefaultNamespace = "GarageGroup.Infra";

    private const string EndpointNamespace = "GarageGroup.Infra.Endpoint";

    private static IEnumerable<RootTypeMetadata> NotNull(this IEnumerable<RootTypeMetadata?> source)
    {
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }

    private static IEnumerable<string> NotEmpty(this IEnumerable<string?> source)
    {
        foreach (var item in source)
        {
            if (string.IsNullOrEmpty(item))
            {
                continue;
            }

            yield return item!;
        }
    }

    private static InvalidOperationException CreateInvalidMethodException(this IMethodSymbol resolverMethod, string message)
        =>
        new($"Endpoint resolver method {resolverMethod.ContainingType?.Name}.{resolverMethod.Name} {message}");
}