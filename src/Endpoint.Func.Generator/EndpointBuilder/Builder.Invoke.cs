namespace GGroupp.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointIvokeSource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "System.Collections.Generic",
            "System.Threading",
            "System.Threading.Tasks",
            "GGroupp.Infra",
            "GGroupp.Infra.Endpoint",
            "Microsoft.OpenApi.Models")
        .AddAlias(
            "static EndpointParser")
        .AppendCodeLine(
            "partial class " + type.TypeEndpointName)
        .BeginCodeBlock()
        .AppendCodeLine(
            "public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)")
        .BeginLambda()
        .AppendCodeLine(
            "throw new NotImplementedException();")
        .EndLambda()
        .EndCodeBlock()
        .Build();
}