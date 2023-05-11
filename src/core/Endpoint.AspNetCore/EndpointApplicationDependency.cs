using System;
using GarageGroup.Infra.Endpoint;
using Microsoft.AspNetCore.Builder;
using PrimeFuncPack;

namespace GarageGroup.Infra;

public static class EndpointApplicationDependency
{
    public static TApplicationBuilder MapEndpoint<TApplicationBuilder, TEndpoint>(
        this Dependency<TEndpoint> dependency, TApplicationBuilder applicationBuilder)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpoint
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        return applicationBuilder.UseEndpoint(dependency.Resolve);
    }
}