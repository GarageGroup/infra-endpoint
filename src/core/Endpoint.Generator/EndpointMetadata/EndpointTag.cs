namespace GGroupp.Infra;

internal sealed record class EndpointTag
{
    public EndpointTag(string name, string? description)
    {
        Name = name ?? string.Empty;
        Description = description;
    }

    public string Name { get; }

    public string? Description { get; }
}