namespace GarageGroup.Infra;

internal sealed record class EndpointSetEndpointDescription
{
    public string? EndpointNamespace { get; set; }

    public string? EndpointTypeName { get; set; }

    public string? OperationId { get; set; }

    public string? MethodName { get; set; }

    public string? Route { get; set; }
}
