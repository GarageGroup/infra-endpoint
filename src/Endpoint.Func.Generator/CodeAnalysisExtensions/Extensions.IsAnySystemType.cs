using System;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsAnySystemType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.IsAnyType(SystemNamespace);
}