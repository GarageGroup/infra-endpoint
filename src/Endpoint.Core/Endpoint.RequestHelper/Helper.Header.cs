using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetHeaderValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.Headers.FirstOrAbsent(header => string.Equals(header.Key, name, System.StringComparison.InvariantCultureIgnoreCase)).OrDefault().Value;
}