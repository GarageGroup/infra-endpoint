using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GGroupp.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetRouteValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.RouteValues.GetValueOrAbsent(name).OrDefault();
}