using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointRequestHelper
{
    public static string? GetClaimValue([AllowNull] this EndpointRequest request, string claimType)
        =>
        request?.User.FindFirst(claimType)?.Value;
}