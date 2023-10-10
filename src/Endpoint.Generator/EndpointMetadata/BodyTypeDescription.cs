using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal sealed record class BodyTypeDescription
{
    public BodyTypeDescription(string propertyName, ISymbol propertySymbol, ContentTypeData contentType, ITypeSymbol bodyType)
    {
        PropertyName = propertyName ?? string.Empty;
        PropertySymbol = propertySymbol;
        ContentType = contentType;
        BodyType = bodyType;
    }

    public string PropertyName { get; }

    public ISymbol PropertySymbol { get; }

    public ContentTypeData ContentType { get; }

    public ITypeSymbol BodyType { get; }
}