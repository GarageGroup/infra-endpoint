using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetRouteValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.RouteValues.GetValueOrAbsent(name).OrDefault();
}