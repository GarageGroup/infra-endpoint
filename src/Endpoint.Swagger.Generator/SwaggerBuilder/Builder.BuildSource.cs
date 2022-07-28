using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class SwaggerBuilder
{
    internal static string BuildSource(this GeneratorExecutionContext context, string typeName)
        =>
        new SourceBuilder(
            context.GetNamespace())
        .AddUsing(
            "System",
            "System.Collections.Generic",
            "GGroupp.Infra.Endpoint",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            "Microsoft.OpenApi.Models",
            "Swashbuckle.AspNetCore.Swagger")
        .AppendCodeLine(
            $"public static class {typeName}")
        .BeginCodeBlock()
        .AppendCodeLine(
            "private static readonly Lazy<OpenApiDocument> lazyTemplate = new(CreateTemplate);")
        .AppendEmptyLine()
        .AppendCodeLine(
            "public static TApplicationBuilder UseEndpointSwagger<TApplicationBuilder>(")
        .BeginArguments()
        .AppendCodeLine(
            "this TApplicationBuilder applicationBuilder!!, string swaggerSectionName = \"Swagger\")",
            "where TApplicationBuilder : class, IApplicationBuilder")
        .EndArguments()
        .BeginCodeBlock()
        .AppendCodeLine(
            "return applicationBuilder.UseSwagger(ResolveSwaggerProvider);")
        .AppendEmptyLine()
        .AppendCodeLine(
            "ISwaggerProvider ResolveSwaggerProvider(IServiceProvider serviceProvider)")
        .BeginLambda()
        .AppendCodeLine(
            "EndpointSwaggerProvider.Create(")
        .BeginArguments()
        .AppendCodeLine(
            "template: lazyTemplate.Value,",
            "option: serviceProvider.GetService<IConfiguration>()?.GetSwaggerOption(swaggerSectionName));")
        .EndArguments()
        .EndLambda()
        .EndCodeBlock()
        .AppendEmptyLine()
        .AppendCreateTemplate(context)
        .AppendEmptyLine()
        .AppendGetEndpointMetadata(context)
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendCreateTemplate(this SourceBuilder sourceBuilder, GeneratorExecutionContext context)
    {
        sourceBuilder.AppendCodeLine("private static OpenApiDocument CreateTemplate()");

        var configurators = context.GetConfigurators();
        if (configurators.Count is not > 0)
        {
            return sourceBuilder.BeginLambda().AppendCodeLine("GetEndpointMetadata().CreateSwaggerTemplate();").EndLambda();
        }

        sourceBuilder.BeginCodeBlock().AppendCodeLine("var template = GetEndpointMetadata().CreateSwaggerTemplate();").AppendEmptyLine();

        foreach (var configurator in configurators)
        {
            sourceBuilder.AddUsing(
                configurator.ContainingNamespace.ToString())
            .AppendCodeLine(
                $"{configurator.Name}.Configure(template);");
        }

        return sourceBuilder.AppendEmptyLine().AppendCodeLine("return template;").EndCodeBlock();
    }

    private static SourceBuilder AppendGetEndpointMetadata(this SourceBuilder sourceBuilder, GeneratorExecutionContext context)
    {
        sourceBuilder.AppendCodeLine("private static IEnumerable<EndpointMetadata> GetEndpointMetadata()");

        var endpointTypes = context.GetEndpointTypes();
        if (endpointTypes.Count is not > 0)
        {
            return sourceBuilder.AddUsing("System.Linq").BeginLambda().AppendCodeLine("Enumerable.Empty<EndpointMetadata>();").EndLambda();
        }

        sourceBuilder.BeginCodeBlock();

        foreach (var endpointType in endpointTypes)
        {
            sourceBuilder.AddUsing(
                endpointType.ContainingNamespace.ToString())
            .AppendCodeLine(
                $"yield return {endpointType.Name}.GetEndpointMetadata();");
        }

        return sourceBuilder.EndCodeBlock();
    }
}