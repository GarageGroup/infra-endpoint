using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal sealed record class JsonBodyPropertyDescription
{
    public JsonBodyPropertyDescription(string propertyName, string jsonPropertyName, ITypeSymbol propertyType)
    {
        PropertyName = propertyName ?? string.Empty;
        JsonPropertyName = jsonPropertyName ?? string.Empty;
        PropertyType = propertyType;
    }

    public string PropertyName { get; }

    public string JsonPropertyName { get; }

    public ITypeSymbol PropertyType { get; }
}