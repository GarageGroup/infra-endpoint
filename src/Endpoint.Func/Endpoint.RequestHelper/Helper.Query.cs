using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GGroupp.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetQueryParameterValue([AllowNull] this EndpointRequest request, string name)
        =>
        request?.QueryParameters.GetValueOrAbsent(name).OrDefault();
}