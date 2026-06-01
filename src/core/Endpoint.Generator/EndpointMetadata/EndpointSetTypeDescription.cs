using System.Collections.Generic;

namespace GarageGroup.Infra;

internal sealed record class EndpointSetTypeDescription
{
    public string? Namespace { get; set; }

    public bool IsTypePublic { get; set; }

    public string? TypeRootName { get; set; }

    public string TypeEndpointSetName => TypeRootName + "EndpointSet";

    public string? TypeFuncName { get; set; }

    public bool IsTypeFuncStruct { get; set; }

    public IReadOnlyCollection<EndpointSetEndpointDescription>? Endpoints { get; set; }
}
