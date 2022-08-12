using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEnumType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.GetEnumUnderlyingType() is not null;

    internal static INamedTypeSymbol? GetEnumUnderlyingType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol switch
        {
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.EnumUnderlyingType,
            _ => null
        };
}