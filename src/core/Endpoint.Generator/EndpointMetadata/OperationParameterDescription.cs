namespace GGroupp.Infra;

internal sealed record class OperationParameterDescription
{
    public OperationParameterDescription(bool required, string location, string name, string schemaFunction, string? description)
    {
        Required = required;
        Location = location ?? string.Empty;
        Name = name ?? string.Empty;
        SchemaFunction = schemaFunction ?? string.Empty;
        Description = description;
    }

    public bool Required { get; }

    public string Location { get; }

    public string Name { get; }

    public string SchemaFunction { get; }

    public string? Description { get; }
}