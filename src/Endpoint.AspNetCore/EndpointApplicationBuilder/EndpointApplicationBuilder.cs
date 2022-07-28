using System.Collections.Generic;
using GGroupp.Infra;

namespace Microsoft.AspNetCore.Builder;

public static partial class EndpointApplicationBuilder
{
    private static readonly IReadOnlyCollection<EndpointMethod> standardMethods;

    static EndpointApplicationBuilder()
        =>
        standardMethods = new[]
        {
            EndpointMethod.Get, EndpointMethod.Post, EndpointMethod.Put, EndpointMethod.Delete
        };
}