using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetQueryParameterValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.QueryParameters.GetValueOrAbsent(name).OrDefault();
}