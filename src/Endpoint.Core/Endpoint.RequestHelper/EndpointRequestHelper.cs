using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

public static partial class EndpointRequestHelper
{
    private static readonly JsonDocumentOptions DocumentOptions;

    static EndpointRequestHelper()
        =>
        DocumentOptions = new()
        {
            AllowTrailingCommas = true
        };
}