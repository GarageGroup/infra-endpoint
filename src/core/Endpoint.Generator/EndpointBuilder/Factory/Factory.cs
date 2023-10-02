using System.Text;

namespace GarageGroup.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointFactorySource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "System.Text.Json",
            "GarageGroup.Infra.Endpoint",
            "Microsoft.Extensions.Logging")
        .AppendObsoleteAttributeIfNecessary(
            type)
        .AppendEndpointMetadataAttribute(
            type)
        .AppendCodeLine(
            $"public sealed partial class {type.TypeEndpointName} : IEndpoint")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"{type.GetVisibility()} static {type.TypeEndpointName} Resolve(IServiceProvider? serviceProvider, {type.TypeFuncName} endpointFunc)")
        .BeginLambda()
        .AppendCodeLine(
            "new(")
        .BeginArguments()
        .AppendCodeLine(
            $"endpointFunc: {GetNullValidationValue("endpointFunc", type.IsTypeFuncStruct)},",
            $"jsonSerializerOptions: {type.GetSerializerOptionsValue()},",
            $"logger: serviceProvider?.GetEndpointLogger<{type.TypeEndpointName}>());")
        .EndArguments()
        .EndLambda()
        .AppendEmptyLine()
        .AppendCodeLine(
            "private static readonly JsonSerializerOptions DefaultSerializerOptions = EndpointDeserializer.CreateDeafultOptions();")
        .AppendEmptyLine()
        .AppendCodeLine(
            $"private readonly {type.TypeFuncName} endpointFunc;")
        .AppendEmptyLine()
        .AppendCodeLine(
            "private readonly JsonSerializerOptions jsonSerializerOptions;")
        .AppendEmptyLine()
        .AppendCodeLine(
            "private readonly ILogger? logger;")
        .AppendEmptyLine()
        .AppendCodeLine(
            $"private {type.TypeEndpointName}({type.TypeFuncName} endpointFunc, JsonSerializerOptions jsonSerializerOptions, ILogger? logger)")
        .BeginCodeBlock()
        .AppendCodeLine(
            "this.endpointFunc = endpointFunc;",
            "this.logger = logger;",
            "this.jsonSerializerOptions = jsonSerializerOptions;")
        .EndCodeBlock()
        .EndCodeBlock()
        .Build();

    private static string GetVisibility(this EndpointTypeDescription type)
        =>
        type.IsTypePublic ? "public" : "internal";

    private static SourceBuilder AppendObsoleteAttributeIfNecessary(this SourceBuilder builder, EndpointTypeDescription type)
    {
        if (type.ObsoleteData is null)
        {
            return builder;
        }

        var attributeBuilder = new StringBuilder("[Obsolete(").Append(type.ObsoleteData.Message.AsStringValueOrDefault());

        attributeBuilder = type.ObsoleteData.IsError switch
        {
            true => attributeBuilder.Append(", true"),
            false => attributeBuilder.Append(", false"),
            _ => attributeBuilder
        };

        if (string.IsNullOrEmpty(type.ObsoleteData.DiagnosticId) is false)
        {
            attributeBuilder = attributeBuilder.Append(", DiagnosticId = ").Append(type.ObsoleteData.DiagnosticId.AsStringValueOrDefault());
        }

        if (string.IsNullOrEmpty(type.ObsoleteData.UrlFormat) is false)
        {
            attributeBuilder = attributeBuilder.Append(", UrlFormat = ").Append(type.ObsoleteData.UrlFormat.AsStringValueOrDefault());
        }

        attributeBuilder = attributeBuilder.Append(")]");
        return builder.AppendCodeLine(attributeBuilder.ToString());
    }

    private static SourceBuilder AppendEndpointMetadataAttribute(this SourceBuilder builder, EndpointTypeDescription type)
    {
        var method = type.MethodName?.ToUpperInvariant();
        return builder.AppendCodeLine($"[EndpointMetadata({method.AsStringSourceCodeOr()}, {type.Route.AsStringSourceCodeOr()})]");
    }

    private static string GetNullValidationValue(string argumentName, bool isStructType)
        =>
        isStructType switch
        {
            true => argumentName,
            _ => $"{argumentName} ?? throw new ArgumentNullException(nameof({argumentName}))"
        };

    private static string GetSerializerOptionsValue(this EndpointTypeDescription type)
    {
        if (string.IsNullOrEmpty(type.SerializerOptionsPropertyFuncName))
        {
            return "DefaultSerializerOptions";
        }

        return $"{type.TypeFuncName}.{type.SerializerOptionsPropertyFuncName} ?? DefaultSerializerOptions";
    }
}