using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointMetadataHelper
{
    [return: NotNullIfNotNull(nameof(source))]
    public static string? BuildOperationId(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        return JsonNamingPolicy.KebabCaseLower.ConvertName(source);
    }
}