namespace GarageGroup.Infra;

internal sealed record class ContentTypeData
{
    private static ContentKind GetContentKind(string? name)
    {
        if (name?.ToLowerInvariant().Contains("json") is true)
        {
            return ContentKind.Json;
        }

        if (name?.ToLowerInvariant().Contains("xml") is true)
        {
            return ContentKind.Xml;
        }

        return ContentKind.Unknown;
    }

    public ContentTypeData(string? name)
    {
        Name = name;
        Kind = GetContentKind(name);
    }

    public string? Name { get; }

    public ContentKind Kind { get; }
}