using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static TypeData GetTypeData(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol || namedTypeSymbol.TypeArguments.Length is not > 0)
        {
            return new(
                namespaces: new[]
                {
                    typeSymbol.ContainingNamespace.ToString()
                },
                name: typeSymbol.Name);
        }

        var argumentTypes = namedTypeSymbol.TypeArguments.Select(GetTypeData);

        return new(
            namespaces: new List<string>(argumentTypes.SelectMany(GetNamespaces))
            {
                typeSymbol.ContainingNamespace.ToString()
            },
            name: $"{typeSymbol.Name}<{string.Join(",", argumentTypes.Select(GetName))}>");

        static IEnumerable<string> GetNamespaces(TypeData typeData)
            =>
            typeData.Namespaces;

        static string GetName(TypeData typeData)
            =>
            typeData.Name;
    }
}