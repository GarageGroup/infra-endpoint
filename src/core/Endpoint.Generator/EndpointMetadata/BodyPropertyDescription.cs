using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal sealed record class BodyPropertyDescription
{
    public BodyPropertyDescription(
        string propertyName,
        string bodyParameterName,
        ISymbol propertySymbol,
        ITypeSymbol propertyType,
        BodyPropertyKind propertyKind)
    {
        PropertyName = propertyName ?? string.Empty;
        BodyParameterName = bodyParameterName ?? string.Empty;
        PropertySymbol = propertySymbol;
        PropertyType = propertyType;
        PropertyKind = propertyKind;
    }

    public string PropertyName { get; }

    public string BodyParameterName { get; }

    public ISymbol PropertySymbol { get; }

    public ITypeSymbol PropertyType { get; }

    public BodyPropertyKind PropertyKind { get; }
}