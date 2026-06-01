using PrimeFuncPack;

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
        .AppendEndpointMetadataAttribute(
            type)
        .AppendCodeLines(
            $"public sealed partial class {type.TypeEndpointName} : IEndpoint")
        .BeginCodeBlock()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLines(
            $"{type.GetVisibility()} static {type.TypeEndpointName} Resolve(IServiceProvider? serviceProvider, {type.TypeFuncName} endpointFunc)")
        .BeginLambda()
        .AppendCodeLines(
            "new(")
        .BeginArguments()
        .AppendCodeLines(
            $"endpointFunc: {GetNullValidationValue("endpointFunc", type.IsTypeFuncStruct)},",
            $"logger: serviceProvider?.GetEndpointLogger<{type.TypeEndpointName}>());")
        .EndArguments()
        .EndLambda()
        .AppendEmptyLine()
        .AppendCodeLines(
            $"private static readonly JsonSerializerOptions SerializerOptions = {type.GetSerializerOptionsValue()};")
        .AppendEmptyLine()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLines(
            $"private readonly {type.TypeFuncName} endpointFunc;")
        .AppendEmptyLine()
        .AppendCodeLines(
            "private readonly ILogger? logger;")
        .AppendEmptyLine()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLines(
            $"{type.GetConstructorVisibility()} {type.TypeEndpointName}({type.TypeFuncName} endpointFunc, ILogger? logger)")
        .BeginCodeBlock()
        .AppendCodeLines(
            "this.endpointFunc = endpointFunc;",
            "this.logger = logger;")
        .EndCodeBlock()
        .EndCodeBlock()
        .Build();

    private static string GetVisibility(this EndpointTypeDescription type)
        =>
        type.IsTypePublic ? "public" : "internal";

    private static string GetConstructorVisibility(this EndpointTypeDescription type)
        =>
        type.IsIncludedInEndpointSet ? "internal" : "private";

    private static SourceBuilder AppendEndpointMetadataAttribute(this SourceBuilder builder, EndpointTypeDescription type)
    {
        var method = type.MethodName?.ToUpperInvariant();
        return builder.AppendCodeLines(
            $"[EndpointMetadata({method.AsStringSourceCodeOr()}, {type.Route.AsStringSourceCodeOr()})]",
            $"[EndpointOperationMetadata({type.OperationId.AsStringSourceCodeOrStringEmpty()}, {method.AsStringSourceCodeOr()}, {type.Route.AsStringSourceCodeOr()})]");
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
            return "EndpointDeserializer.CreateDeafultOptions()";
        }

        return $"{type.TypeFuncName}.{type.SerializerOptionsPropertyFuncName} ?? EndpointDeserializer.CreateDeafultOptions()";
    }
}
