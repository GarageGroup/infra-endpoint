namespace GGroupp.Infra;

internal sealed record class OperationParameterDescription
{
    public OperationParameterDescription(bool required, string location, string name, string schemaFunction)
    {
        Required = required;
        Location = location ?? string.Empty;
        Name = name ?? string.Empty;
        SchemaFunction = schemaFunction ?? string.Empty;
    }

    public bool Required { get; }

    public string Location { get; }

    public string Name { get; }

    public string SchemaFunction { get; }
}