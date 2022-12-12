using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEndpointTypeParser(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAnonymousType)
        {
            return false;
        }

        return typeSymbol.Interfaces.Where(IsEndpointTypeParserType).Where(IsParedTypeEqualToSourceType).Any();

        static bool IsEndpointTypeParserType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.IsType("GGroupp.Infra", "IEndpointTypeParser");

        bool IsParedTypeEqualToSourceType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.TypeArguments.Length is 1 &&
            interfaceType.TypeArguments[0].Equals(typeSymbol, SymbolEqualityComparer.Default);
    }
}