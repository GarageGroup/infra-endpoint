using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

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
        .AddUsings(
            type.RequestType?.GetDisplayedData().AllNamespaces)
        .AddUsings(
            type.ResponseType?.GetDisplayedData().AllNamespaces)
        .AddUsings(
            type.FailureCodeType?.GetDisplayedData().AllNamespaces)
        .AppendCodeLine(
            "partial class " + type.TypeEndpointName)
        .BeginCodeBlock()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLine(
            "public async Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)")
        .BeginCodeBlock()
        .AppendInvokeAsyncBlock(type)
        .EndCodeBlock()
        .AppendEmptyLine()
        .AppendMapRequestFunction(type)
        .AppendEmptyLine()
        .AppendObsoleteAttributeIfNecessary(type)
        .AppendCodeLine(
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
            sourceBuilder.AppendCodeLine(
                "var inputResult = MapRequest(request);");
        }
        else
        {
            sourceBuilder.AppendCodeLine(
                "var inputResult = await MapRequestAsync(request, cancellationToken).ConfigureAwait(false);");
        }

        _ = sourceBuilder
            .AppendCodeLine(
                "if (inputResult.IsFailure)")
            .BeginCodeBlock()
            .AppendCodeLine(
                "var inputFailure = inputResult.FailureOrThrow();")
            .AppendEmptyLine()
            .AppendCodeLine(
                "logger?.LogError(inputFailure.SourceException, \"Request is incorrect: {failureMessage}\", inputFailure.FailureMessage);")
            .AppendCodeLine(
                "return inputResult.FailureOrThrow().ToBadRequestResponse(jsonSerializerOptions);")
            .EndCodeBlock()
            .AppendEmptyLine()
            .AppendCodeLine(
                "var input = inputResult.SuccessOrThrow();")
            .AppendCodeLine(
                $"var endpointResult = await endpointFunc.{type.GetMethodFuncName()}(input, cancellationToken).ConfigureAwait(false);")
            .AppendEmptyLine();

        if (type.FailureCodeType is null)
        {
            return sourceBuilder.AppendCodeLine("return MapSuccess(endpointResult);");
        }

        return sourceBuilder.AppendCodeLine("return endpointResult.Fold(MapSuccess, MapFailure);");
    }

    private static SourceBuilder AppendMapRequestFunction(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var inTypeName = type.GetRequestTypeName();

        sourceBuilder = sourceBuilder.AppendObsoleteAttributeIfNecessary(type);

        if (type.HasRequestBody() is false)
        {
            sourceBuilder.AppendCodeLine(
                $"private static Result<{inTypeName}, Failure<Unit>> MapRequest(EndpointRequest request)");
        }
        else
        {
            sourceBuilder.AppendCodeLine(
                $"private async ValueTask<Result<{inTypeName}, Failure<Unit>>> MapRequestAsync(EndpointRequest request, CancellationToken token)");
        }

        var requestConstructor = type.RequestType?.GetConstructor();
        if (requestConstructor?.Parameters.Length is not > 0)
        {
            return sourceBuilder.BeginLambda().AppendCodeLine($"new {inTypeName}();").EndLambda();
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
                    .AppendCodeLine("await Task.Yield();")
                    .AppendCodeLine($"var {parameter.Name} = request.Body{nullable};")
                    .AppendEmptyLine();

                continue;
            }

            if (parameter.Type.IsSystemType("String"))
            {
                sourceBuilder.AppendCodeLine(
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
            sourceBuilder = sourceBuilder.AddUsings(requestBodyData.AllNamespaces);

            var requestType = requestBodyData.DisplayedTypeName;

            if (requestBody.BodyType.IsEndpointBodyParser())
            {
                sourceBuilder = sourceBuilder.AppendCodeLine(
                    $"var {bodyResult} = await {requestType}.ParseAsync(request, token).ConfigureAwait(false);");
            }
            else
            {
                if (requestBody.ContentType.Kind is ContentKind.Xml)
                {
                    sourceBuilder = sourceBuilder.AppendCodeLine(
                        $"var {bodyResult} = await request.DeserializeXmlBodyAsync<{requestType}>(token).ConfigureAwait(false);");
                }
                else
                {
                    sourceBuilder = sourceBuilder.AppendCodeLine(
                        $"var {bodyResult} = await request.DeserializeBodyAsync<{requestType}>(jsonSerializerOptions, logger, token)" +
                        ".ConfigureAwait(false);");
                }
            }

            sourceBuilder = sourceBuilder
                .AppendCodeLine(
                    $"if ({bodyResult}.IsFailure)")
                .BeginCodeBlock()
                .AppendCodeLine(
                    $"return {bodyResult}.FailureOrThrow();")
                .EndCodeBlock()
                .AppendEmptyLine();
        }

        sourceBuilder = sourceBuilder.AppendCodeLine($"return new {inTypeName}(").BeginArguments();

        for (var i = 0; i < requestConstructor.Parameters.Length; i++)
        {
            var parameter = requestConstructor.Parameters[i];
            var lastSymbol = i < requestConstructor.Parameters.Length - 1 ? "," : ");";

            if (resultParameters.Contains(parameter.Name))
            {
                sourceBuilder.AppendCodeLine($"{parameter.Name}: {parameter.Name}Result.SuccessOrThrow(){lastSymbol}");
            }
            else
            {
                sourceBuilder.AppendCodeLine($"{parameter.Name}: {parameter.Name}{lastSymbol}");
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
        sourceBuilder.AppendCodeLine(
            $"var bodyDocumentResult = await request.{parseAsyncFunctionValue}.ConfigureAwait(false);")
        .AppendCodeLine(
            "if (bodyDocumentResult.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLine(
            "return bodyDocumentResult.FailureOrThrow();")
        .EndCodeBlock()
        .AppendEmptyLine()
        .AppendCodeLine(
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

        return sourceBuilder.AppendCodeLine(
            $"var {parameterName}Result = bodyDocument.{GetDeserializeFunctionValue()};")
        .AppendCodeLine(
            $"if ({parameterName}Result.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLine(
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
            sourceBuilder.AddUsings(typeData.AllNamespaces);

            if (type.GetEnumUnderlyingTypeOrDefault() is not null)
            {
                return $"Get{nullableValue}EnumOrFailure<{typeData.DisplayedTypeName}>({jsonPropertyValue})";
            }

            var nullableSign = isNullable ? "?" : string.Empty;
            return $"DeserializeOrFailure<{typeData.DisplayedTypeName}{nullableSign}>({jsonPropertyValue}, jsonSerializerOptions, logger)";
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
        sourceBuilder.AppendCodeLine("return new(").BeginArguments();

        var successStatusCodeType = type.GetSuccessStatusCodeType();

        if (successStatusCodeType is not null)
        {
            sourceBuilder.AppendMapSuccessStatusCodeBlock(successStatusCodeType);
        }
        else
        {
            var statusCodeName = type.GetSuccessStatusCodeValue();
            var statusCodeValue = string.IsNullOrEmpty(statusCodeName) ? type.GetDefaultStatusCode() : statusCodeName;

            sourceBuilder.AppendCodeLine($"statusCode: {statusCodeValue},");
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
            sourceBuilder.AppendCodeLine("headers: default,");
        }
        else
        {
            sourceBuilder.AppendCodeLine("headers: new KeyValuePair<string, string?>[]").BeginCodeBlock();

            for (var i = 0; i < headers.Count; i++)
            {
                var header = headers[i];
                var lastSymbol = i < headers.Count - 1 ? "," : string.Empty;

                sourceBuilder.AppendCodeLine($"new({header.Key}, {header.Value}){lastSymbol}");
            }

            sourceBuilder.EndCodeBlock(',');
        }

        if (responseBodyType is not null)
        {
            if (responseBodyType.BodyType.IsStreamType())
            {
                sourceBuilder = sourceBuilder.AppendCodeLine($"body: success.{responseBodyType.PropertyName});");
            }
            else if (responseBodyType.ContentType.Kind is ContentKind.Json)
            {
                sourceBuilder = sourceBuilder.AppendCodeLine($"body: success.{responseBodyType.PropertyName}.ToJsonStream(jsonSerializerOptions));");
            }
            else if (responseBodyType.ContentType.Kind is ContentKind.Xml)
            {
                sourceBuilder = sourceBuilder.AppendCodeLine($"body: success.{responseBodyType.PropertyName}.ToXmlStream());");
            }
            else
            {
                sourceBuilder = sourceBuilder.AppendCodeLine($"body: success.{responseBodyType.PropertyName}.ToTextStream());");
            }

            return sourceBuilder.EndArguments();
        }

        if (responseBodyProperties.Count is not > 0)
        {
            return sourceBuilder.AppendCodeLine("body: default);").EndArguments();
        }

        const string localFunctionName = "InnerGetBody";

        sourceBuilder
            .AppendCodeLine($"body: {localFunctionName}());")
            .EndArguments()
            .AddUsing("System.IO", "System.Text.Json")
            .AppendEmptyLine()
            .AppendCodeLine($"Stream {localFunctionName}()")
            .BeginCodeBlock()
            .AppendCodeLine("var stream = new MemoryStream();")
            .AppendCodeLine("using var writer = new Utf8JsonWriter(stream);")
            .AppendEmptyLine()
            .AppendCodeLine("writer.WriteStartObject();")
            .AppendEmptyLine();

        foreach (var jsonBodyProperty in responseBodyProperties)
        {
            sourceBuilder.AppendWriteJsonProperty(jsonBodyProperty);
        }

        return sourceBuilder
            .AppendCodeLine("writer.WriteEndObject();")
            .AppendCodeLine("writer.Flush();")
            .AppendEmptyLine()
            .AppendCodeLine("stream.Position = 0;")
            .AppendCodeLine("return stream;")
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
                    .AppendCodeLine($"if ({conditionValue} is not null)");
            }
            else
            {
                var typeDisplayedData = type.GetDisplayedData();

                sourceBuilder = sourceBuilder
                    .AddUsings(typeDisplayedData.AllNamespaces)
                    .AppendCodeLine(
                        $"if ({conditionValue} != default({typeDisplayedData.DisplayedTypeName}))");
            }

            sourceBuilder = sourceBuilder.BeginCodeBlock();
        }

        var jsonNameValue = jsonBodyProperty.BodyParameterName.AsStringSourceCodeOrStringEmpty();

        if (type.IsSystemType(nameof(String)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLine($"writer.WriteString({jsonNameValue}, {propertyValue});");
        }
        else if (type.IsSystemType(nameof(Guid)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLine($"writer.WriteString({jsonNameValue}, {propertyValue}.ToString());");
        }
        else if (type.IsSystemType(nameof(Boolean)))
        {
            sourceBuilder = sourceBuilder.AppendCodeLine($"writer.WriteBoolean({jsonNameValue}, {propertyValue});");
        }
        else if (type.IsAnySystemType(GetJsonNumberSystemTypes()))
        {
            sourceBuilder = sourceBuilder.AppendCodeLine($"writer.WriteNumber({jsonNameValue}, {propertyValue});");
        }
        else
        {
            sourceBuilder = sourceBuilder.AppendCodeLine(
                $"writer.WritePropertyName({jsonNameValue});")
            .AppendCodeLine(
                $"JsonSerializer.Serialize(writer, {propertyValue}, jsonSerializerOptions);");
        }

        if (hasCheck)
        {
            sourceBuilder = sourceBuilder.EndCodeBlock();
        }

        if (hasNullCheck && ignoreCondition is JsonIgnoreCondition.Never)
        {
            sourceBuilder = sourceBuilder
                .AppendCodeLine("else")
                .BeginCodeBlock()
                .AppendCodeLine($"writer.WriteNull({jsonNameValue});")
                .EndCodeBlock();
        }

        return sourceBuilder.AppendEmptyLine();
    }

    private static SourceBuilder AppendMapSuccessStatusCodeBlock(this SourceBuilder sourceBuilder, ITypeSymbol successStatusCodeType)
    {
        var successStatusCodeNames = successStatusCodeType.GetSuccessStatusCodeNames();
        if (successStatusCodeNames.Count is not > 0)
        {
            return sourceBuilder.AppendCodeLine($"statusCode: {DefaultSuccessStatusCodeValue},");
        }

        sourceBuilder.AppendCodeLine(
            $"statusCode: ((ISuccessStatusCodeProvider<{successStatusCodeType.Name}>)success).StatusCode switch")
        .BeginCodeBlock();

        foreach (var successStatusCodeName in successStatusCodeNames)
        {
            sourceBuilder.AppendCodeLine(
                $"{successStatusCodeType.Name}.{successStatusCodeName.Key} => {successStatusCodeName.Value},");
        }

        return sourceBuilder.AppendCodeLine("_ => default").EndCodeBlock(',');
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
            .AppendCodeLine(
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

            sourceBuilder.AppendCodeLine(
                $"if (failure.FailureCode is {failureCodeTypeName}.{problem.StatusFieldName})")
            .BeginCodeBlock()
            .AppendCodeLine(
                $"LogUnexpectedStatusCode({code}, failure.SourceException, failure.FailureMessage);")
            .AppendEmptyLine()
            .AppendCodeLine(
                "return new EndpointProblem(")
            .BeginArguments()
            .AppendCodeLine(
                $"type: {GetStatusDescription(problem.StatusCode).AsStringValueOrDefault()},")
            .AppendCodeLine(
                $"title: {problem.Title.AsStringValueOrDefault()},")
            .AppendCodeLine(
                $"status: {code},");

            if (problem.DetailFromFailureMessage)
            {
                sourceBuilder.AppendCodeLine("detail: failure.FailureMessage)");
            }
            else
            {
                sourceBuilder.AppendCodeLine($"detail: {problem.Detail.AsStringValueOrDefault()})");
            }

            sourceBuilder.EndArguments()
            .AppendCodeLine(".ToFailureResponse(jsonSerializerOptions);")
            .EndCodeBlock()
            .AppendEmptyLine();
        }

        sourceBuilder.AppendCodeLine(
            "logger?.LogError(" + 
            "failure.SourceException, " +
            "\"An unexpected http error: {errorCode}. Message: {message}\", " +
            "failure.FailureCode, " +
            "failure.FailureMessage);")
        .AppendCodeLine(
            "return new(500, default, default);");

        if (problems.Count is not > 0)
        {
            return sourceBuilder;
        }

        return sourceBuilder.AppendEmptyLine().AppendCodeLine(
            "void LogUnexpectedStatusCode(int code, Exception? sourceException, string failureMessage)")
        .BeginLambda()
        .AppendCodeLine(
            "logger?.LogInformation(sourceException, \"An unsuccessful status code: {statusCode}. Message: {message}\", code, failureMessage);")
        .EndLambda();
    }

    private static SourceBuilder AppendParseRequestParameter(
        this SourceBuilder builder, ITypeSymbol parameterType, string parameterName, string requestFunctionValue, List<string> resultParameters)
    {
        if (parameterType.IsSystemType(nameof(String)))
        {
            return builder.AppendCodeLine($"var {parameterName} = {requestFunctionValue} ?? string.Empty;");
        }

        var nullableValue = parameterType.IsNullable() ? "Nullable" : string.Empty;
        var type = parameterType.GetNullableStructType() ?? parameterType;

        resultParameters.Add(parameterName);

        return builder.AppendCodeLine(
            $"var {parameterName}Result = {GetParserFunctionName()}({requestFunctionValue});")
        .AppendCodeLine(
            $"if ({parameterName}Result.IsFailure)")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"return {parameterName}Result.FailureOrThrow();")
        .EndCodeBlock();

        string GetParserFunctionName()
        {
            if (type.IsAnySystemType(GetParserSystemTypes()))
            {
                return "EndpointParser.Parse" + nullableValue + type.Name;
            }

            var typeData = type.GetDisplayedData();
            builder.AddUsings(typeData.AllNamespaces);

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