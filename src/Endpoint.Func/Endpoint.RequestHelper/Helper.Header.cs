using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GGroupp.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetHeaderValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.Headers.GetValueOrAbsent(name).OrDefault();
}