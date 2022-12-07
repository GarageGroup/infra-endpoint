namespace GGroupp.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointFactorySource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "System.Text.Json",
            "GGroupp.Infra.Endpoint",
            "Microsoft.Extensions.Logging")
        .AppendCodeLine(
            $"public sealed partial class {type.TypeEndpointName} : IEndpoint")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"internal static {type.TypeEndpointName} Resolve(IServiceProvider? serviceProvider, {type.TypeFuncName} endpointFunc)")
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
            "private static readonly JsonSerializerOptions DefaultSerializerOptions = new(JsonSerializerDefaults.Web);")
        .AppendEmptyLine()
        .AppendCodeLine(
            $"private readonly {type.TypeFuncName} endpointFunc;")
        .AppendEmptyLine()
        .AppendCodeLine(
            "private readonly JsonSerializerOptions jsonSerializerOptions;")
        .AppendEmptyLine()
        .AppendCodeLine(
            "private readonly ILogger? logger;")
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