using System.IO;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static bool IsStreamType(this ITypeSymbol typeSymbol)
        =>
        typeSymbol.IsType("System.IO", nameof(Stream));
}