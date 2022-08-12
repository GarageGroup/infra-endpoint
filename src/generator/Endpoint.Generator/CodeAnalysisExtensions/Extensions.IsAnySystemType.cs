using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsAnySystemType(this ITypeSymbol typeSymbol, params string[] types)
        =>
        typeSymbol.IsAnyType(SystemNamespace, types);
}