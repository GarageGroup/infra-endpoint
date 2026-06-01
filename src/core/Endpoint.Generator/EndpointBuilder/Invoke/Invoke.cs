using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using PrimeFuncPack;

namespace GarageGroup.Infra;

using static EndpointAttributeHelper;

partial class EndpointBuilder
{
    internal static string BuildEndpointInvokeSource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System",
            "System.Collections.Generic",
            "System.Threading",
            "System.Threading.Tasks",
            "GarageGroup.Infra",
            "GarageGroup.Infra.Endpoint",
            "Microsoft.Extensions.Logging")
        .AddUsing(
            type.RequestType?.GetDisplayedData().AllNamespaces.ToArray() ?? [])
        .AddUsing(
            type.ResponseType?.GetDisplayedData().AllNamespaces.ToArray() ?? [])
        .AddUsing(
            type.FailureCodeType?.GetDisplayedData().AllNamespaces.ToArray() ?? [])
        .AppendCodeLines(
            "partial class " + type.TypeEndpointName)
        .BeginCodeBlock()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLines(
            "public async Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)")
        .BeginCodeBlock()
        .AppendInvokeAsyncBlock(type)
        .EndCodeBlock()
        .AppendEmptyLine()
        .AppendMapRequestFunction(type)
        .AppendEmptyLine()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLines(
            $"private EndpointResponse MapSuccess({type.GetResponseTypeName()} success)")
        .BeginCodeBlock()
        .AppendMapSuccessBlock(type)
        .EndCodeBlock()
        .AppendMapFailureMetod(type)
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendInvokeAsyncBlock(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var requestBody = type.GetRequestBodyType();
        var requestBodyProperties = type.GetRequestBodyProperties();

        if (requestBody is null && requestBodyProperties.Count is not > 0)
        {
            sourceBuilder.AppendCodeLines(
                "var inputResult = MapRequest(request);");
        }
        else
        {
            sourceBuilder.AppendCodeLines(
                "var inputResult = await MapRequestAsync(request, cancellationToken).ConfigureAwait(false);");
        }

        _ = sourceBuilder
            .AppendCodeLines(
                "if (inputResult.IsFailure)")
            .BeginCodeBlock()
            .AppendCodeLines(
                "var inputFailure = inputResult.FailureOrThrow();")
            .AppendEmptyLine()
            .AppendCodeLines(
                "logger?.LogError(inputFailure.SourceException, \"Request is incorrect: {failureMessage}\", inputFailure.FailureMessage);")
            .AppendCodeLines(
                "return inputResult.FailureOrThrow().ToBadRequestResponse(SerializerOptions);")
            .EndCodeBlock()
            .AppendEmptyLine()
            .AppendCodeLines(
                "var input = inputResult.SuccessOrThrow();")
            .AppendCodeLines(
                $"var endpointResult = await endpointFunc.{type.GetMethodFuncName()}(input, cancellationToken).ConfigureAwait(false);")
            .AppendEmptyLine();

        if (type.FailureCodeType is null)
        {
            return sourceBuilder.AppendCodeLines("return MapSuccess(endpointResult);");
        }

        return sourceBuilder.AppendCodeLines("return endpointResult.Fold(MapSuccess, MapFailure);");
    }

    private static SourceBuilder AppendMapRequestFunction(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var inTypeName = type.GetRequestTypeName();

        sourceBuilder = sourceBuilder.AppendObsoleteAttributeIfNecessary(type);

        if (type.HasRequestBody() is false)
        {
            sourceBuilder.AppendCodeLines(
                $"private static Result<{inTypeName}, Failure<Unit>> MapRequest(EndpointRequest request)");
        }
        else
        {
            sourceBuilder.AppendCodeLines(
                $"private async ValueTask<Result<{inTypeName}, Failure<Unit>>> MapRequestAsync(EndpointRequest request, CancellationToken token)");
        }

        var requestConstructor = type.RequestType?.GetConstructor();
        if (requestConstructor?.Parameters.Length is not > 0)
        {
            return sourceBuilder.BeginLambda().AppendCodeLines($"new {inTypeName}();").EndLambda();
        }

        sourceBuilder.BeginCodeBlock();
        var resultParameters = new List<string>();

        var requestBody = type.GetRequestBodyType();
        var requestProperties = type.GetRequestBodyProperties();

        var useBodyDocument = false;

        foreach (var parameter in requestConstructor.Parameters)
        {
            if (parameter.GetAttributes().Any(IsRootBodyAttribute) is false)
            {
                var requestFunctionValue = parameter.GetRequestFunctionValue();
                sourceBuilder.AppendParseRequestParameter(parameter.Type, parameter.Name, requestFunctionValue, resultParameters).AppendEmptyLine();
                continue;
            }

            var nullable = parameter.NullableAnnotation is NullableAnnotation.Annotated ? string.Empty : "!";

            var bodyProperty = requestProperties.FirstOrDefault(p => IsBodyPropertyMatched(p, parameter));
            if (bodyProperty is not null)
            {
                if (useBodyDocument is false)
                {
                    var parseAsyncFunctionValue = bodyProperty.PropertyKind switch
                    {
                        BodyPropertyKind.Form => "ReadFormDataAsync(token)",
                        _ => "ParseDocumentAsync(logger, token)"
                    };

                    sourceBuilder.AppendParseBodyDocument(parseAsyncFunctionValue).AppendEmptyLine();
                    useBodyDocument = true;
                }

                if (bodyProperty.PropertyKind is BodyPropertyKind.Form)
                {
                    sourceBuilder.AppendParseFormDocumentParameter(bodyProperty, resultParameters).AppendEmptyLine();
                }
                else
                {
                    sourceBuilder.AppendParseJsonDocumentParameter(bodyProperty, resultParameters).AppendEmptyLine();
                }

                continue;
            }

            if (parameter.Type.IsStreamType())
            {
                sourceBuilder
                    .AppendCodeLines("await Task.Yield();")
                    .AppendCodeLines($"var {parameter.Name} = request.Body{nullable};")
                    .AppendEmptyLine();

                continue;
            }

            if (parameter.Type.IsSystemType("String"))
            {
                sourceBuilder.AppendCodeLines(
                    $"var {parameter.Name} = await request.ReadStringAsync(token).ConfigureAwait(false);")
                .AppendEmptyLine();

                continue;
            }

            if (requestBody is null)
            {
                continue;
            }

            var bodyResult = $"{parameter.Name}Result";
            resultParameters.Add(parameter.Name);

            var requestBodyData = requestBody.BodyType.GetDisplayedData();
            sourceBuilder = sourceBuilder.AddUsing(requestBodyData.AllNamespaces.ToArray());

            var requestType = requestBodyData.DisplayedTypeName;

            if (requestBody.BodyType.IsEndpointBodyParser())
            {
                sourceBuilder = sourceBuilder.AppendCodeLines(
                    $"var {bodyResult} = await {requestType}.ParseAsync(request, token).ConfigureAwait(false);");
            }
            else
            {
                if (requestBody.ContentType.Kind is ContentKind.Xml)
                {
                    sourceBuilder = sourceBuilder.AppendCodeLines(
                        $"var {bodyResult} = await request.DeserializeXmlBodyAsync<{requestType}>(token).ConfigureAwait(false);");
                }
                else
                {
                    sourceBuilder = sourceBuilder.AppendCodeLines(
                        $"var {bodyResult} = await request.DeserializeBodyAsync<{requestType}>(SerializerOptions, logger, token)" +
                        ".ConfigureAwait(false);");
                }
            }

            sourceBuilder = sourceBuilder
                .AppendCodeLines(
                    $"if ({bodyResult}.IsFailure)")
                .BeginCodeBlock()
                .AppendCodeLines(
                    $"return {bodyResult}.FailureOrThrow();")
                .EndCodeBlock()
                .AppendEmptyLine();
        }

        sourceBuilder = sourceBuilder.AppendCodeLines($"return new {inTypeName}(").BeginArguments();

        for (var i = 0; i < requestConstructor.Parameters.Length; i++)
        {
            var parameter = requestConstructor.Parameters[i];
            var lastSymbol = i < requestConstructor.Parameters.Length - 1 ? "," : ");";

            if (resultParameters.Contains(parameter.Name))
            {
                sourceBuilder.AppendCodeLines($"{parameter.Name}: {parameter.Name}Result.SuccessOrThrow(){lastSymbol}");
            }
            else
            {
                sourceBuilder.AppendCodeLines($"{parameter.Name}: {parameter.Name}{lastSymbol}");
            }
        }

        return sourceBuilder.EndArguments().EndCodeBlock();

        static bool IsRootBodyAttribute(AttributeData attributeData)
            =>
            IsRootBodyInAttribute(attributeData) || IsJsonBodyInAttribute(attributeData) || IsFormBodyInAttribute(attributeData);

        static bool IsBodyPropertyMatched(BodyPropertyDescription bodyProperty, IParameterSymbol parameter)
            =>
            string.Equals(bodyProperty.PropertyName, parameter.Name, StringComparison.InvariantCulture);
    }

    private static SourceBuilder AppendParseBodyDocument(this SourceBuilder sourceBuilder, string parseAsyncFunctionValue)
        =>
        sourceBuilder.AppendCodeLines(
            $"var bodyDocumentResult = await request.{parseAsyncFunctionValue}.ConfigureAwait(false);")
        .AppendCodeLines(
            "if (bodyDocumentResult.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLines(
            "return bodyDocumentResult.FailureOrThrow();")
        .EndCodeBlock()
        .AppendEmptyLine()
        .AppendCodeLines(
            "var bodyDocument = bodyDocumentResult.SuccessOrThrow();");

    private static SourceBuilder AppendParseJsonDocumentParameter(
        this SourceBuilder sourceBuilder, BodyPropertyDescription jsonBodyProperty, List<string> resultParameters)
    {
        var parameterName = jsonBodyProperty.PropertyName;
        resultParameters.Add(parameterName);

        var jsonPropertyValue = jsonBodyProperty.BodyParameterName.AsStringSourceCodeOrStringEmpty();
        var isNullable = jsonBodyProperty.PropertyType.IsNullable();

        var nullableValue = isNullable ? "Nullable" : string.Empty;
        var type = jsonBodyProperty.PropertyType.GetNullableStructType() ?? jsonBodyProperty.PropertyType;

        return sourceBuilder.AppendCodeLines(
            $"var {parameterName}Result = bodyDocument.{GetDeserializeFunctionValue()};")
        .AppendCodeLines(
            $"if ({parameterName}Result.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLines(
            $"return {parameterName}Result.FailureOrThrow();")
        .EndCodeBlock();

        string GetDeserializeFunctionValue()
        {
            if (type.IsAnySystemType(GetJsonDeserializerSystemTypes()))
            {
                return $"Get{nullableValue}{type.Name}OrFailure({jsonPropertyValue})";
            }

            if (type.IsSystemType(nameof(String)))
            {
                return $"GetStringOrFailure({jsonPropertyValue})";
            }

            var typeData = type.GetDisplayedData();
            sourceBuilder.AddUsing(typeData.AllNamespaces.ToArray());

            if (type.GetEnumUnderlyingTypeOrDefault() is not null)
            {
                return $"Get{nullableValue}EnumOrFailure<{typeData.DisplayedTypeName}>({jsonPropertyValue})";
            }

            var nullableSign = isNullable ? "?" : string.Empty;
            return $"DeserializeOrFailure<{typeData.DisplayedTypeName}{nullableSign}>({jsonPropertyValue}, SerializerOptions, logger)";
        }
    }

    private static SourceBuilder AppendParseFormDocumentParameter(
        this SourceBuilder sourceBuilder, BodyPropertyDescription formProperty, List<string> resultParameters)
    {
        var propertyName = formProperty.PropertyName;
        var propertyType = formProperty.PropertyType;

        var requestFunctionValue = $"bodyDocument.Get({formProperty.BodyParameterName.AsStringSourceCodeOrStringEmpty()})";
        return sourceBuilder.AppendParseRequestParameter(propertyType, propertyName, requestFunctionValue, resultParameters);
    }

    private static SourceBuilder AppendMapSuccessBlock(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        sourceBuilder.AppendCodeLines("return new(").BeginArguments();

        var successStatusCodeType = type.GetSuccessStatusCodeType();

        if (successStatusCodeType is not null)
        {
            sourceBuilder.AppendMapSuccessStatusCodeBlock(successStatusCodeType);
        }
        else
        {
            var statusCodeName = type.GetSuccessStatusCodeValue();
            var statusCodeValue = string.IsNullOrEmpty(statusCodeName) ? type.GetDefaultStatusCode() : statusCodeName;

            sourceBuilder.AppendCodeLines($"statusCode: {statusCodeValue},");
        }

        var headers = type.GetHeaderOutProperties().Select(GetPropertyValue).ToList();
        var responseBodyType = type.GetResponseBodyType();
        var responseBodyProperties = type.GetResponseBodyProperties();

        if (string.IsNullOrEmpty(responseBodyType?.ContentType.Name) is false)
        {
            headers.Add(new("\"Content-Type\"", $"\"{responseBodyType?.ContentType.Name}\""));
        }
        else if (responseBodyProperties.Count > 0)
        {
            headers.Add(new("\"Content-Type\"", "\"application/json; charset=utf-8\""));
        }

        if (headers.Count is not > 0)
        {
            sourceBuilder.AppendCodeLines("headers: default,");
        }
        else
        {
            sourceBuilder.AppendCodeLines("headers: new KeyValuePair<string, string?>[]").BeginCodeBlock();

            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                var lastSymbol = i < headers.Count - 1 ? "," : string.Empty;

                sourceBuilder.AppendCodeLines($"new({header.Key}, {header.Value}){lastSymbol}");
            }

            sourceBuilder.EndCodeBlock(",");
        }

        if (responseBodyType is not null)
        {
            if (responseBodyType.BodyType.IsStreamType())
            {
                sourceBuilder = sourceBuilder.AppendCodeLines($"body: success.{responseBodyType.PropertyName});");
            }
            else if (responseBodyType.ContentType.Kind is ContentKind.Json)
            {
                sourceBuilder = sourceBuilder.AppendCodeLines($"body: success.{responseBodyType.PropertyName}.ToJsonStream(SerializerOptions));");
            }
            else if (responseBodyType.ContentType.Kind is ContentKind.Xml)
            {
                sourceBuilder = sourceBuilder.AppendCodeLines($"body: success.{responseBodyType.PropertyName}.ToXmlStream());");
            }
            else
            {
                sourceBuilder = sourceBuilder.AppendCodeLines($"body: success.{responseBodyType.PropertyName}.ToTextStream());");
            }

            return sourceBuilder.EndArguments();
        }

        if (responseBodyProperties.Count is not > 0)
        {
            return sourceBuilder.AppendCodeLines("body: default);").EndArguments();
        }

        const string localFunctionName = "InnerGetBody";

        sourceBuilder
            .AppendCodeLines($"body: {localFunctionName}());")
            .EndArguments()
            .AddUsing("System.IO", "System.Text.Json")
            .AppendEmptyLine()
            .AppendCodeLines($"Stream {localFunctionName}()")
            .BeginCodeBlock()
            .AppendCodeLines("var stream = new MemoryStream();")
            .AppendCodeLines("using var writer = new Utf8JsonWriter(stream);")
            .AppendEmptyLine()
            .AppendCodeLines("writer.WriteStartObject();")
            .AppendEmptyLine();

        foreach (var jsonBodyProperty in responseBodyProperties)
        {
            sourceBuilder.AppendWriteJsonProperty(jsonBodyProperty);
        }

        return sourceBuilder
            .AppendCodeLines("writer.WriteEndObject();")
            .AppendCodeLines("writer.Flush();")
            .AppendEmptyLine()
            .AppendCodeLines("stream.Position = 0;")
            .AppendCodeLines("return stream;")
            .EndCodeBlock();

        static KeyValuePair<string, string> GetPropertyValue(KeyValuePair<string, IPropertySymbol> headerProperty)
        {
            var key = headerProperty.Key.AsStringSourceCodeOrStringEmpty();
            var propertyName = headerProperty.Value.Name;

            if (headerProperty.Value.Type.IsSystemType("String") is false)
            {
                if (headerProperty.Value.Type.IsValueType && headerProperty.Value.Type.IsNullable() is false)
                {
                    propertyName += ".ToString()";
                }
                else
                {
                    propertyName += "?.ToString()";
                }
            }

            return new(headerProperty.Key.AsStringSourceCodeOrStringEmpty(), $"success.{propertyName}");
        }
    }

    private static SourceBuilder AppendWriteJsonProperty(this SourceBuilder sourceBuilder, BodyPropertyDescription jsonBodyProperty)
    {
        var ignoreCondition = (jsonBodyProperty.PropertySymbol as IPropertySymbol).GetJsonIgnoreCondition();
        if (ignoreCondition is JsonIgnoreCondition.Always)
        {
            return sourceBuilder;
        }

        var type = jsonBodyProperty.PropertyType;
        var nullableStruct = false;

        if (type.GetNullableStructType() is ITypeSymbol typeSymbol)
        {
            type = typeSymbol;
            nullableStruct = true;
        }

        var conditionValue = $"success.{jsonBodyProperty.PropertyName}";
        var propertyValue = nullableStruct ? conditionValue + ".Value" : conditionValue;

        var hasNullCheck = nullableStruct || jsonBodyProperty.PropertyType.IsReferenceType;
        var hasDefaultCheck = hasNullCheck is false && type.IsValueType && ignoreCondition is JsonIgnoreCondition.WhenWritingDefault;

        var hasCheck = hasNullCheck || hasDefaultCheck;
        if (hasCheck)
        {
            if (hasNullCheck)
            {
                sourceBuilder = sourceBuilder
                    .AppendCodeLines($"if ({conditionValue} is not null)");
            }
            else
            {
                var typeDisplayedData = type.GetDisplayedData();

                sourceBuilder = sourceBuilder.AddUsing(typeDisplayedData.AllNamespaces.ToArray()).AppendCodeLines(
                    $"if ({conditionValue} != default({typeDisplayedData.DisplayedTypeName}))");
            }

            sourceBuilder = sourceBuilder.BeginCodeBlock();
        }

        var jsonNameValue = jsonBodyProperty.BodyParameterName.AsStringSourceCodeOrStringEmpty();

        if (type.IsSystemType(nameof(String)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLines($"writer.WriteString({jsonNameValue}, {propertyValue});");
        }
        else if (type.IsSystemType(nameof(Guid)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLines($"writer.WriteString({jsonNameValue}, {propertyValue}.ToString());");
        }
        else if (type.IsSystemType(nameof(Boolean)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLines($"writer.WriteBoolean({jsonNameValue}, {propertyValue});");
        }
        else if (type.IsAnySystemType(GetJsonNumberSystemTypes()))
        {
            sourceBuilder = sourceBuilder.AppendCodeLines($"writer.WriteNumber({jsonNameValue}, {propertyValue});");
        }
        else
        {
            sourceBuilder = sourceBuilder.AppendCodeLines(
                $"writer.WritePropertyName({jsonNameValue});")
            .AppendCodeLines(
                $"JsonSerializer.Serialize(writer, {propertyValue}, SerializerOptions);");
        }

        if (hasCheck)
        {
            sourceBuilder = sourceBuilder.EndCodeBlock();
        }

        if (hasNullCheck && ignoreCondition is JsonIgnoreCondition.Never)
        {
            sourceBuilder = sourceBuilder
                .AppendCodeLines("else")
                .BeginCodeBlock()
                .AppendCodeLines($"writer.WriteNull({jsonNameValue});")
                .EndCodeBlock();
        }

        return sourceBuilder.AppendEmptyLine();
    }

    private static SourceBuilder AppendMapSuccessStatusCodeBlock(this SourceBuilder sourceBuilder, ITypeSymbol successStatusCodeType)
    {
        var successStatusCodeNames = successStatusCodeType.GetSuccessStatusCodeNames();
        if (successStatusCodeNames.Count is not > 0)
        {
            return sourceBuilder.AppendCodeLines($"statusCode: {DefaultSuccessStatusCodeValue},");
        }

        sourceBuilder.AppendCodeLines(
            $"statusCode: ((ISuccessStatusCodeProvider<{successStatusCodeType.Name}>)success).StatusCode switch")
        .BeginCodeBlock();

        foreach (var successStatusCodeName in successStatusCodeNames)
        {
            sourceBuilder.AppendCodeLines(
                $"{successStatusCodeType.Name}.{successStatusCodeName.Key} => {successStatusCodeName.Value},");
        }

        return sourceBuilder.AppendCodeLines("_ => default").EndCodeBlock(",");
    }

    private static SourceBuilder AppendMapFailureMetod(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.FailureCodeType is null)
        {
            return sourceBuilder;
        }

        return sourceBuilder
            .AppendEmptyLine()
            .AppendObsoleteAttributeIfNecessary(type)
            .AppendCodeLines(
                $"private EndpointResponse MapFailure(Failure<{type.GetFailureCodeTypeName()}> failure)")
            .BeginCodeBlock()
            .AppendMapFailureBlock(type)
            .EndCodeBlock();
    }

    private static SourceBuilder AppendMapFailureBlock(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var problems = type.FailureCodeType?.GetProblemData() ?? Array.Empty<ProblemData>();
        var failureCodeTypeName = type.GetFailureCodeTypeName();

        foreach (var problem in problems)
        {
            var code = problem.StatusCode;

            sourceBuilder.AppendCodeLines(
                $"if (failure.FailureCode is {failureCodeTypeName}.{problem.StatusFieldName})")
            .BeginCodeBlock()
            .AppendCodeLines(
                $"LogUnexpectedStatusCode({code}, failure.SourceException, failure.FailureMessage);")
            .AppendEmptyLine()
            .AppendCodeLines(
                "return new EndpointProblem(")
            .BeginArguments()
            .AppendCodeLines(
                $"type: {GetStatusDescription(problem.StatusCode).AsStringValueOrDefault()},")
            .AppendCodeLines(
                $"title: {problem.Title.AsStringValueOrDefault()},")
            .AppendCodeLines(
                $"status: {code},");

            if (problem.DetailFromFailureMessage)
            {
                sourceBuilder.AppendCodeLines("detail: failure.FailureMessage)");
            }
            else
            {
                sourceBuilder.AppendCodeLines($"detail: {problem.Detail.AsStringValueOrDefault()})");
            }

            sourceBuilder.EndArguments()
            .AppendCodeLines(".ToFailureResponse(SerializerOptions);")
            .EndCodeBlock()
            .AppendEmptyLine();
        }

        sourceBuilder.AppendCodeLines(
            "logger?.LogError(" + 
            "failure.SourceException, " +
            "\"An unexpected http error: {errorCode}. Message: {message}\", " +
            "failure.FailureCode, " +
            "failure.FailureMessage);")
        .AppendCodeLines(
            "return new(500, default, default);");

        if (problems.Count is not > 0)
        {
            return sourceBuilder;
        }

        return sourceBuilder.AppendEmptyLine().AppendCodeLines(
            "void LogUnexpectedStatusCode(int code, Exception? sourceException, string failureMessage)")
        .BeginLambda()
        .AppendCodeLines(
            "logger?.LogInformation(sourceException, \"An unsuccessful status code: {statusCode}. Message: {message}\", code, failureMessage);")
        .EndLambda();
    }

    private static SourceBuilder AppendParseRequestParameter(
        this SourceBuilder builder, ITypeSymbol parameterType, string parameterName, string requestFunctionValue, List<string> resultParameters)
    {
        if (parameterType.IsSystemType(nameof(String)))
        {
            return builder.AppendCodeLines($"var {parameterName} = {requestFunctionValue} ?? string.Empty;");
        }

        var nullableValue = parameterType.IsNullable() ? "Nullable" : string.Empty;
        var type = parameterType.GetNullableStructType() ?? parameterType;

        resultParameters.Add(parameterName);

        return builder.AppendCodeLines(
            $"var {parameterName}Result = {GetParserFunctionName()}({requestFunctionValue});")
        .AppendCodeLines(
            $"if ({parameterName}Result.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLines(
            $"return {parameterName}Result.FailureOrThrow();")
        .EndCodeBlock();

        string GetParserFunctionName()
        {
            if (type.IsAnySystemType(GetParserSystemTypes()))
            {
                return "EndpointParser.Parse" + nullableValue + type.Name;
            }

            var typeData = type.GetDisplayedData();
            builder.AddUsing(typeData.AllNamespaces.ToArray());

            if (type.GetEnumUnderlyingTypeOrDefault() is not null)
            {
                return "EndpointParser.Parse" + nullableValue + $"Enum<{typeData.DisplayedTypeName}>";
            }

            if (type.IsEndpointTypeParser())
            {
                return $"{typeData.DisplayedTypeName}.Parse";
            }

            throw new NotSupportedException($"Type {type.Name} is not supported as a request parameter type");
        }
    }
}
