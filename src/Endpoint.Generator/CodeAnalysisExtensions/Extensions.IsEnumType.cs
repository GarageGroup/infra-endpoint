using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEnumType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol switch
        {
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.EnumUnderlyingType is not null,
            _ => false
        };
}