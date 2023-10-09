using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal sealed record class JsonBodyPropertyDescription
{
    public JsonBodyPropertyDescription(string propertyName, ISymbol propertySymbol, string jsonPropertyName, ITypeSymbol propertyType)
    {
        PropertyName = propertyName ?? string.Empty;
        PropertySymbol = propertySymbol;
        JsonPropertyName = jsonPropertyName ?? string.Empty;
        PropertyType = propertyType;
    }

    public string PropertyName { get; }

    public ISymbol PropertySymbol { get; }

    public string JsonPropertyName { get; }

    public ITypeSymbol PropertyType { get; }
}