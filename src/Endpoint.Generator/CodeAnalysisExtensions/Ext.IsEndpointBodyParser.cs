using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsEndpointBodyParser(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsAnonymousType || typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        return typeSymbol.Interfaces.Where(IsEndpointBodyParserType).Where(IsParsedTypeEqualToSourceType).Any();

        static bool IsEndpointBodyParserType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.IsType("GarageGroup.Infra", "IEndpointBodyParser");

        bool IsParsedTypeEqualToSourceType(INamedTypeSymbol interfaceType)
            =>
            interfaceType.TypeArguments.Length is 1 &&
            interfaceType.TypeArguments[0].Equals(typeSymbol, SymbolEqualityComparer.Default);
    }
}