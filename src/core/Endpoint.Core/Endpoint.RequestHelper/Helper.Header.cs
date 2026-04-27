using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetHeaderValue([AllowNull] this EndpointRequest request, string name)
    {
        return request?.Headers.FirstOrDefault(CompareHeaderWithName).Value;

        bool CompareHeaderWithName(KeyValuePair<string, string?> header)
            =>
            string.Equals(header.Key, name, System.StringComparison.InvariantCultureIgnoreCase);
    }
}