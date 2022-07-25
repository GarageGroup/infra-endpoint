using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsAnyType(this ITypeSymbol typeSymbol, string @namespace, params string[] types)
    {
        if (string.Equals(typeSymbol.ContainingNamespace?.ToString(), @namespace, StringComparison.InvariantCulture) is false)
        {
            return false;
        }

        return types.Length is 0 || types.Any(IsEqualToType);

        bool IsEqualToType(string type)
            =>
            string.Equals(typeSymbol.Name, type, StringComparison.InvariantCulture);
    }
}