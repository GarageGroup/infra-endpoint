using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetHeaderValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.Headers.FirstOrDefault(header => CompareHeaderWithName(header, name)).Value;

    private static bool CompareHeaderWithName(KeyValuePair<string, string?> header, string name)
        =>
        string.Equals(header.Key, name, System.StringComparison.InvariantCultureIgnoreCase)
}