using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal sealed record class BodyTypeDescription
{
    public BodyTypeDescription(string propertyName, string? contentType, ITypeSymbol bodyType)
    {
        PropertyName = propertyName ?? string.Empty;
        ContentType = contentType;
        BodyType = bodyType;
        IsJsonType = contentType?.ToLowerInvariant().Contains("json") is true;
    }

    public string PropertyName { get; }

    public string? ContentType { get; }

    public ITypeSymbol BodyType { get; }

    public bool IsJsonType { get; }
}