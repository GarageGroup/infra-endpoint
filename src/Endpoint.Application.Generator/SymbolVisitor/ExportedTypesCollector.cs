using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal sealed class ExportedTypesCollector : SymbolVisitor
{
    private readonly CancellationToken cancellationToken;

    private readonly HashSet<INamedTypeSymbol> exportedTypes;

    internal ExportedTypesCollector(CancellationToken cancellationToken)
    { 
        this.cancellationToken = cancellationToken;
        exportedTypes = new(SymbolEqualityComparer.Default);
    }

    public IReadOnlyCollection<INamedTypeSymbol> GetPublicTypes()
        =>
        exportedTypes.ToImmutableArray();

    public override void VisitAssembly(IAssemblySymbol symbol)
    {
        cancellationToken.ThrowIfCancellationRequested();
        symbol.GlobalNamespace.Accept(this);
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
        foreach (INamespaceOrTypeSymbol namespaceOrType in symbol.GetMembers())
        {
            cancellationToken.ThrowIfCancellationRequested();
            namespaceOrType.Accept(this);
        }
    }

    public override void VisitNamedType(INamedTypeSymbol type)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (type.IsStatic || type.IsAbstract)
        {
            return;
        }

        if (IsAccessibleOutsideOfAssembly(type) is false || exportedTypes.Add(type) is false)
        {
            return;
        }

        var nestedTypes = type.GetTypeMembers();
        if (nestedTypes.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (INamedTypeSymbol nestedType in nestedTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            nestedType.Accept(this);
        }
    }

    private static bool IsAccessibleOutsideOfAssembly(ISymbol symbol)
        =>
        symbol.DeclaredAccessibility switch
        {
            Accessibility.Private => false,
            Accessibility.Internal => false,
            Accessibility.ProtectedAndInternal => false,
            Accessibility.Protected => true,
            Accessibility.ProtectedOrInternal => true,
            Accessibility.Public => true,
            _ => true,
        };
}