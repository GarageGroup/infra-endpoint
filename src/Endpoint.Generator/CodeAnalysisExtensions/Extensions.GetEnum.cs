using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class CodeAnalysisExtensions
{
    internal static IEnumerable<IFieldSymbol> GetEnumFields(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IFieldSymbol>().Where(IsPublic).Where(NotEmptyName);

        static bool IsPublic(IFieldSymbol fieldSymbol)
            =>
            fieldSymbol.DeclaredAccessibility is Accessibility.Public;

        static bool NotEmptyName(IFieldSymbol fieldSymbol)
            =>
            string.IsNullOrEmpty(fieldSymbol.Name) is false;
    }
}