using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

[Generator]
internal sealed class EndpointSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var endpointType in context.GetEndpointTypes())
        {
            var endpointFactorySource = endpointType.BuildEndpointFactorySource();
            context.AddSource(endpointType.TypeEndpointName + ".g.cs", endpointFactorySource);

            var endpointMetadataSource = endpointType.BuildEndpointMetadataSource();
            context.AddSource(endpointType.TypeEndpointName + ".Metadata.g.cs", endpointMetadataSource);

            var endpointIvokeSource = endpointType.BuildEndpointIvokeSource();
            context.AddSource(endpointType.TypeEndpointName + ".Invoke.g.cs", endpointIvokeSource);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}