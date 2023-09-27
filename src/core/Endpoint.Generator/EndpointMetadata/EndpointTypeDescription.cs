using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal sealed record class EndpointTypeDescription
{
    public string? Namespace { get; set; }

    public bool IsTypePublic { get; set; }

    public string? TypeRootName { get; set; }

    public string TypeEndpointName => TypeRootName + "Endpoint";

    public string? TypeFuncName { get; set; }

    public bool IsTypeFuncStruct { get; set; }

    public string? MethodFuncName { get; set; }

    public string? SerializerOptionsPropertyFuncName { get; set; }

    public string? MethodName { get; set; }

    public string? Route { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public IReadOnlyCollection<EndpointTag>? Tags { get; set; }

    public ITypeSymbol? RequestType { get; set; }

    public ITypeSymbol? ResponseType { get; set; }

    public ITypeSymbol? FailureCodeType { get; set; }
}