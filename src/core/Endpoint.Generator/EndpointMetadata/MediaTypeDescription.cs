using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal sealed record class MediaTypeDescription
{
    public MediaTypeDescription(string? @namespace, string? typeName, ITypeSymbol type)
    {
        Namespace = @namespace ?? string.Empty;
        TypeName = typeName ?? string.Empty;
        Type = type;
    }

    public string Namespace { get; }

    public string TypeName { get; }

    public ITypeSymbol Type { get; }
}