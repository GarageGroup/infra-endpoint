﻿using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEndpointBodyMetadataProvider(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsAnonymousType is false && typeSymbol.Interfaces.Any(IsProviderType);

        static bool IsProviderType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.IsType("GarageGroup.Infra", "IEndpointBodyMetadataProvider");
    }
}