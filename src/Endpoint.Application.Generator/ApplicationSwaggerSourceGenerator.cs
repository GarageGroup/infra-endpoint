using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

[Generator]
internal sealed class ApplicationSwaggerSourceGenerator : ISourceGenerator
{
    private const string ApplicationSwaggerTypeName = "ApplicationSwagger";

    public void Execute(GeneratorExecutionContext context)
    {
        var namespaces = new List<string>
        {
            "System",
            "System.Collections.Generic",
            "GGroupp.Infra.Endpoint",
            "Microsoft.AspNetCore.Builder",
            "Microsoft.Extensions.Configuration",
            "Microsoft.Extensions.DependencyInjection",
            "Microsoft.OpenApi.Models",
            "Swashbuckle.AspNetCore.Swagger"
        };

        var @namespace = context.GetNamespace();

        var endpointTypes = context.GetEndpointTypes();
        var endpointTypeNames = new List<string>();

        foreach (var type in endpointTypes)
        {
            namespaces.Add(type.ContainingNamespace.ToString());
            endpointTypeNames.Add(type.Name);
        }

        _ = namespaces.Remove(@namespace);
        var finalNamespaces = namespaces.Distinct().OrderBy(GetNamespaceOrder).ToArray();

        var codeBuilder = new StringBuilder("// Auto-generated code").AppendLine().Append("#nullable enable");
        codeBuilder = codeBuilder.AppendUsings(finalNamespaces).AppendNamespace(context.GetNamespace()).AppendLine();

        var endpointsCode = endpointTypeNames.Distinct().ToArray().BuildEndpointsCode("        ");

        var classSource = $@"
public static class {ApplicationSwaggerTypeName}
{{
    private static readonly Lazy<OpenApiDocument> lazyTemplate = new(CreateTemplate);

    public static TApplicationBuilder UseEndpointSwagger<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder!!, string swaggerSectionName = ""Swagger"")
        where TApplicationBuilder : IApplicationBuilder
    {{
        _ = applicationBuilder.UseSwagger(ResolveSwaggerProvider);
        return applicationBuilder;

        ISwaggerProvider ResolveSwaggerProvider(IServiceProvider serviceProvider)
            =>
            EndpointSwaggerProvider.Create(
                template: lazyTemplate.Value,
                option: serviceProvider.GetService<IConfiguration>()?.GetSwaggerOption(swaggerSectionName));
    }}

    private static OpenApiDocument CreateTemplate()
        =>
        GetEndpointMetadata().CreateSwaggerTemplate();

    private static IEnumerable<EndpointMetadata> GetEndpointMetadata()
    {{{endpointsCode}
    }}
}}";

        context.AddSource($"{ApplicationSwaggerTypeName}.g.cs", codeBuilder.Append(classSource).ToString());
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }

    private static string GetNamespaceOrder(string ns)
        =>
        ns.StartsWith("System", StringComparison.InvariantCulture) ? "_" + ns : ns;
}