using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

[Generator(LanguageNames.CSharp)]
internal sealed class EndpointSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var endpointTypes = context.CompilationProvider.SelectMany(SourceGeneratorExtensions.GetEndpointTypes);
        context.RegisterSourceOutput(endpointTypes, AddEndpointSources);

        var mediaTypes = context.CompilationProvider.SelectMany(SourceGeneratorExtensions.GetMediaTypes);
        context.RegisterSourceOutput(mediaTypes, AddMediaTypeSource);
    }

    private static void AddEndpointSources(SourceProductionContext context, EndpointTypeDescription endpointType)
    {
        var endpointFactorySource = endpointType.BuildEndpointFactorySource();
        context.AddSource(endpointType.TypeEndpointName + ".g.cs", endpointFactorySource);

        var endpointMetadataSource = endpointType.BuildEndpointMetadataSource();
        context.AddSource(endpointType.TypeEndpointName + ".Metadata.g.cs", endpointMetadataSource);

        var endpointIvokeSource = endpointType.BuildEndpointInvokeSource();
        context.AddSource(endpointType.TypeEndpointName + ".Invoke.g.cs", endpointIvokeSource);
    }

    private static void AddMediaTypeSource(SourceProductionContext context, MediaTypeDescription mediaType)
        =>
        context.AddSource(mediaType.TypeName + ".g.cs", mediaType.BuildSource());
}