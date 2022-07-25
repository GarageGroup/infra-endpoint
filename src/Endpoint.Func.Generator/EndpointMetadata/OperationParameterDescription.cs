namespace GGroupp.Infra;

internal sealed record class OperationParameterDescription
{
    public OperationParameterDescription(bool required, string location, string name, string schemaFunction)
    {
        Required = required;
        Location = location;
        Name = name;
        SchemaFunction = schemaFunction;
    }

    public bool Required { get; }

    public string Location { get; }

    public string Name { get; }

    public string SchemaFunction { get; }
}