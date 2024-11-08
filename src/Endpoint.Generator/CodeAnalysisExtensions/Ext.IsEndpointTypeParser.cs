using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEndpointTypeParser(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAnonymousType)
        {
            return false;
        }

        return typeSymbol.Interfaces.Where(IsEndpointTypeParserType).Where(IsParsedTypeEqualToSourceType).Any();

        static bool IsEndpointTypeParserType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.IsType("GarageGroup.Infra", "IEndpointTypeParser");

        bool IsParsedTypeEqualToSourceType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.TypeArguments.Length is 1 &&
            interfaceType.TypeArguments[0].Equals(typeSymbol, SymbolEqualityComparer.Default);
    }
}