using GGroupp.Infra.Endpoint;
using Microsoft.AspNetCore.Builder;
using PrimeFuncPack;

namespace GGroupp.Infra;

public static class EndpointApplicationDependency
{
    public static TApplicationBuilder MapEndpoint<TApplicationBuilder, TEndpoint>(
        this Dependency<TEndpoint> dependency!!, TApplicationBuilder applicationBuilder!!)
        where TApplicationBuilder : IApplicationBuilder
        where TEndpoint : class, IEndpoint
        =>
        applicationBuilder.UseEndpoint(dependency.Resolve);
}