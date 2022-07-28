using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsSystemType(this ITypeSymbol typeSymbol, string typeName)
        =>
        typeSymbol.IsType(SystemNamespace, typeName);
}