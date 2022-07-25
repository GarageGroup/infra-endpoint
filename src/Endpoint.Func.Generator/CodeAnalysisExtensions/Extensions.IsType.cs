using System;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsType(this ITypeSymbol typeSymbol, string @namespace, string typeName)
        =>
        string.Equals(typeSymbol.ContainingNamespace.ToString(), @namespace, StringComparison.InvariantCulture) &&
        string.Equals(typeSymbol.Name, typeName, StringComparison.InvariantCulture);
}