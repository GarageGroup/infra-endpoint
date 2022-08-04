using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointMetadataSource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System.Collections.Generic",
            "GGroupp.Infra",
            "GGroupp.Infra.Endpoint",
            "Microsoft.OpenApi.Models")
        .AddAlias(
            "static EndpointMetadataHelper")
        .AppendCodeLine(
            "partial class " + type.TypeEndpointName)
        .BeginCodeBlock()
        .AppendCodeLine(
            "public static EndpointMetadata GetEndpointMetadata()")
        .BeginLambda()
        .AppendCodeLine(
            "new(")
        .BeginArguments()
        .AppendCodeLine(
            $"method: {type.Method},",
            $"route: {type.Route.ToStringValueOrEmpty()},",
            $"summary: {type.Summary.ToStringValueOrDefault()},",
            $"description: {type.Description.ToStringValueOrDefault()},",
            "operation: new()")
        .BeginCodeBlock()
        .AppendTags(type)
        .AppendOperationParameters(type)
        .AppendRequestBody(type)
        .AppendCodeLine(
            "Responses = new()")
        .BeginCodeBlock()
        .AppendSuccessResponsesBody(type)
        .AppendFailureResponsesBody(type)
        .EndCodeBlock()
        .EndCodeBlock(',')
        .AppendCodeLine(
            "schemas: new Dictionary<string, OpenApiSchema>()")
        .BeginCodeBlock()
        .AppendSchemasBody(type)
        .EndCodeBlock(");")
        .EndArguments()
        .EndLambda()
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendTags(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.Tags?.Count is not > 0)
        {
            return sourceBuilder;
        }

        var tags = type.Tags.ToArray();
        sourceBuilder.AppendCodeLine("Tags = new List<OpenApiTag>").BeginCodeBlock();

        for (var i = 0; i < tags.Length; i++)
        {
            var tag = tags[i];

            sourceBuilder
                .AppendCodeLine("new()")
                .BeginCodeBlock()
                .AppendCodeLine("Name = " + tag.Name.ToStringValueOrEmpty() + ",")
                .AppendCodeLine("Description = " + tag.Description.ToStringValueOrDefault());

            if (i < tags.Length - 1)
            {
                sourceBuilder.EndCodeBlock(',');
            }
            else
            {
                sourceBuilder.EndCodeBlock();
            }
        }

        return sourceBuilder.EndCodeBlock(',');
    }

    private static SourceBuilder AppendOperationParameters(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var requestConstructor = type.RequestType?.GetConstructor();
        var parameterDescriptions = requestConstructor?.Parameters.Select(GetOperationParameterDescription).Where(NotNull).ToArray();

        if (parameterDescriptions?.Length is not > 0)
        {
            return sourceBuilder;
        }

        sourceBuilder.AppendCodeLine("Parameters = new OpenApiParameter[]").BeginCodeBlock();

        for (var i = 0; i < parameterDescriptions.Length; i++)
        {
            var parameter = parameterDescriptions[i]!;

            sourceBuilder.AppendCodeLine("new()")
                .BeginCodeBlock()
                .AppendCodeLine($"Required = {parameter.Required.ToStringValue()},")
                .AppendCodeLine($"In = ParameterLocation.{parameter.Location},")
                .AppendCodeLine($"Name = {parameter.Name.ToStringValueOrEmpty()},")
                .AppendCodeLine($"Schema = {parameter.SchemaFunction}");

            if (i < parameterDescriptions.Length - 1)
            {
                sourceBuilder.EndCodeBlock(',');
            }
            else
            {
                sourceBuilder.EndCodeBlock();
            }
        }

        return sourceBuilder.EndCodeBlock(',');

        static bool NotNull(OperationParameterDescription? description)
            =>
            description is not null;
    }

    private static SourceBuilder AppendRequestBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var requestBodyType = type.GetRequestBodyType();
        if (requestBodyType is not null)
        {
            return sourceBuilder.AppendCodeLine("RequestBody = new()").BeginCodeBlock().AppendContent(requestBodyType).EndCodeBlock(',');
        }

        var requestBodyProperties = type.GetRequestJsonBodyProperties();
        if (requestBodyProperties.Count is not > 0)
        {
            return sourceBuilder;
        }

        return sourceBuilder
            .AppendCodeLine("RequestBody = new()")
            .BeginCodeBlock()
            .AppendJsonPropertiesContent(requestBodyProperties)
            .EndCodeBlock(',');
    }

    private static SourceBuilder AppendSuccessResponsesBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.ResponseType is null)
        {
            return sourceBuilder;
        }

        var successStatusCodes = type.GetSuccessStatusCodes();
        if (successStatusCodes.Count is 0)
        {
            successStatusCodes = new[] { type.GetDefaultStatusCode() };
        }

        var responseBodyType = type.GetResponseBodyType();
        var responseJsonProperties = type.GetResponseJsonBodyProperties();

        foreach (var successStatusCode in successStatusCodes)
        {
            sourceBuilder
                .AppendCodeLine($"[{successStatusCode.ToStringValueOrEmpty()}] = new()")
                .BeginCodeBlock()
                .AppendCodeLine($"Description = {GetStatusDescription(successStatusCode).ToStringValueOrDefault()},");

            if (responseBodyType is not null)
            {
                sourceBuilder.AppendContent(responseBodyType);
            }
            else  if (responseJsonProperties.Count > 0)
            {
                sourceBuilder.AppendJsonPropertiesContent(responseJsonProperties);
            }

            sourceBuilder.EndCodeBlock(',');
        }

        return sourceBuilder;
    }

    private static SourceBuilder AppendFailureResponsesBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.FailureCodeType is null)
        {
            return sourceBuilder;
        }

        var failureCodes = type.FailureCodeType.GetProblemData().OrderBy(GetStatusCode).Select(GetStatusCode).Distinct().ToArray();
        if (failureCodes.Length is not > 0)
        {
            return sourceBuilder;
        }

        for (var i = 0; i < failureCodes.Length; i++)
        {
            var failureCode = failureCodes[i];
            var afterSymbol = i < failureCodes.Length - 1 ? "," : null;

            sourceBuilder
                .AppendCodeLine($"[{failureCode.ToStringValueOrEmpty()}] = new()")
                .BeginCodeBlock()
                .AppendCodeLine($"Description = {GetStatusDescription(failureCode).ToStringValueOrDefault()},")
                .AppendCodeLine("Content = CreateProblemContent()")
                .EndCodeBlock(afterSymbol);
        }

        return sourceBuilder;

        static string? GetStatusCode(ProblemData problemData)
            =>
            problemData.StatusCode;
    }

    private static SourceBuilder AppendSchemasBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.FailureCodeType?.GetProblemData().Any() is not true)
        {
            return sourceBuilder;
        }

        return sourceBuilder.AppendCodeLine("[\"ProblemDetails\"] = CreateProblemSchema()");
    }

    private static SourceBuilder AppendContent(this SourceBuilder sourceBuilder, BodyTypeDescription bodyType)
    {
        var requestBodySchema = bodyType.BodyType.GetSimpleSchemaFunction();
        if (string.IsNullOrEmpty(requestBodySchema) is false)
        {
            return sourceBuilder.AppendCodeLine($"Content = {requestBodySchema}.CreateContent({bodyType.ContentType.ToStringValueOrEmpty()})");
        }

        return sourceBuilder.AppendSchema("Content", bodyType.BodyType, 0).AppendCodeLine(
            $".CreateContent({bodyType.ContentType.ToStringValueOrEmpty()})");
    }

    private static SourceBuilder AppendJsonPropertiesContent(
        this SourceBuilder sourceBuilder, IReadOnlyCollection<JsonBodyPropertyDescription> jsonProperties)
    {
        sourceBuilder
            .AppendCodeLine("Content = new OpenApiSchema")
            .BeginCodeBlock()
            .AppendCodeLine("Type = \"object\",")
            .AppendCodeLine("Properties = new Dictionary<string, OpenApiSchema>")
            .BeginCodeBlock();

        foreach (var property in jsonProperties)
        {
            var propertyName = "[" + property.JsonPropertyName.ToStringValueOrEmpty() + "]";
            sourceBuilder.AppendSchema(propertyName, property.PropertyType, 1);
        }

        return sourceBuilder
            .EndCodeBlock()
            .EndCodeBlock()
            .AppendCodeLine($".CreateContent(\"application/json\")");
    }

    private static SourceBuilder AppendSchema(this SourceBuilder sourceBuilder, string parameterName, ITypeSymbol type, int level)
    {
        if (level > 0)
        {
            var simpleSchemaFunction = type.GetSimpleSchemaFunction();
            if (string.IsNullOrEmpty(simpleSchemaFunction) is false)
            {
                return sourceBuilder.AppendCodeLine($"{parameterName} = {simpleSchemaFunction},");
            }

            if (level >= MaxSchemaLevel)
            {
                return sourceBuilder.AppendCodeLine($"{parameterName} = CreateDefaultSchema({type.IsNullable().ToStringValue()}),");
            }
        }

        var afterSymbol = level > 0 ? "," : null;
        var schemaType = level > 0 ?  "new()" : "new OpenApiSchema";
        level++;

        sourceBuilder
            .AppendCodeLine($"{parameterName} = {schemaType}")
            .BeginCodeBlock()
            .AppendCodeLine($"Nullable = {type.IsNullable().ToStringValue()},");

        var collectionType = type.GetCollectionType();
        if (collectionType is not null)
        {
            return sourceBuilder.AppendCodeLine("Type = \"array\",").AppendSchema("Items", collectionType, level).EndCodeBlock(afterSymbol);
        }

        sourceBuilder.AppendCodeLine("Type = \"object\",").AppendCodeLine("Properties = new Dictionary<string, OpenApiSchema>").BeginCodeBlock();

        foreach (var jsonProperty in type.GetJsonProperties())
        {
            var propertyName = "[" + jsonProperty.GetJsonPropertyName().ToStringValueOrEmpty() + "]";
            sourceBuilder.AppendSchema(propertyName, jsonProperty.Type, level);
        }

        return sourceBuilder.EndCodeBlock().EndCodeBlock(afterSymbol);
    }
}