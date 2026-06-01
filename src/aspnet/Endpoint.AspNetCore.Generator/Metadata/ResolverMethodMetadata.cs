namespace GarageGroup.Infra;

internal sealed record class ResolverMethodMetadata
{
    public ResolverMethodMetadata(string methodName, bool isEndpointSet)
    {
        MethodName = methodName ?? string.Empty;
        IsEndpointSet = isEndpointSet;
    }

    public string MethodName { get; }

    public bool IsEndpointSet { get; }
}