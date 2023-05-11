using System.Collections.Generic;
using System.Linq;
using GGroupp;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointMetadataSource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "System.Collections.Generic",
            "GarageGroup.Infra",
            "GarageGroup.Infra.Endpoint",
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
            $"method: {type.GetMethodValue()},",
            $"route: {type.Route.ToStringValueOrEmpty()},",
            "summary: default,",
            "description: default,",
            "operation: new()")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"Summary = {type.Summary.ToStringValueOrDefault()},",
            $"Description = {type.Description.ToStringValueOrDefault()},")
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
        var usings = new List<string>();

        var parameterDescriptions = requestConstructor?.Parameters.Select(InnerGetParameterDescription).Where(NotNull).ToArray();
        sourceBuilder.AddUsings(usings);

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
                .AppendCodeLine($"Schema = {parameter.SchemaFunction},")
                .AppendCodeLine($"Description = {parameter.Description.ToStringValueOrDefault()}");

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

        OperationParameterDescription? InnerGetParameterDescription(IParameterSymbol parameterSymbol)
            =>
            GetOperationParameterDescription(parameterSymbol, usings);

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

        var successData = type.GetSuccessData();
        if (successData.Count is 0)
        {
            successData = new[] { new SuccessData(type.GetDefaultStatusCode(), null) };
        }

        var successDataDictionary = new Dictionary<string, SuccessData>();
        foreach (var success in successData)
        {
            var statusCode = success.StatusCode ?? type.GetDefaultStatusCode();
            if (successDataDictionary.ContainsKey(statusCode))
            {
                continue;
            }

            successDataDictionary[statusCode] = success;
        }

        var responseBodyType = type.GetResponseBodyType();
        var responseJsonProperties = type.GetResponseJsonBodyProperties();

        var successes = successDataDictionary.Select(GetValue).ToArray();
        foreach (var success in successes)
        {
            var statusCode = success.StatusCode ?? type.GetDefaultStatusCode();
            var descriptionValue = string.IsNullOrEmpty(success.Description) switch
            {
                true => GetStatusDescription(statusCode).ToStringValueOrDefault(),
                _ => success.Description.ToStringValueOrDefault()
            };

            sourceBuilder
                .AppendCodeLine($"[{statusCode.ToStringValueOrEmpty()}] = new()")
                .BeginCodeBlock()
                .AppendCodeLine($"Description = {descriptionValue},");

            if (responseBodyType is not null)
            {
                sourceBuilder.AppendContent(responseBodyType);
            }
            else if (responseJsonProperties.Count > 0)
            {
                sourceBuilder.AppendJsonPropertiesContent(responseJsonProperties);
            }

            sourceBuilder.EndCodeBlock(',');
        }

        return sourceBuilder;

        static SuccessData GetValue(KeyValuePair<string, SuccessData> kv)
            =>
            kv.Value;
    }

    private static SourceBuilder AppendFailureResponsesBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.FailureCodeType is null)
        {
            return sourceBuilder;
        }

        var problemData = type.FailureCodeType.GetProblemData().OrderBy(GetStatusCode).ToArray();
        if (problemData.Length is not > 0)
        {
            return sourceBuilder;
        }

        var problemDataDictionary = new Dictionary<string, ProblemData>();
        foreach (var problem in problemData)
        {
            var problemCode = problem.StatusCode ?? string.Empty;
            if (problemDataDictionary.ContainsKey(problemCode))
            {
                continue;
            }

            problemDataDictionary[problemCode] = problem;
        }

        var problems = problemDataDictionary.Select(GetValue).ToArray();
        for (var i = 0; i < problems.Length; i++)
        {
            var problem = problems[i];
            var afterSymbol = i < problems.Length - 1 ? "," : null;

            var failureCode = problem.StatusCode;
            var descriptionValue = string.IsNullOrEmpty(problem.Description) switch
            {
                true => GetStatusDescription(failureCode).ToStringValueOrDefault(),
                _ => problem.Description.ToStringValueOrDefault()
            };

            sourceBuilder
                .AppendCodeLine($"[{failureCode.ToStringValueOrEmpty()}] = new()")
                .BeginCodeBlock()
                .AppendCodeLine($"Description = {descriptionValue},")
                .AppendCodeLine("Content = CreateProblemContent()")
                .EndCodeBlock(afterSymbol);
        }

        return sourceBuilder;

        static string? GetStatusCode(ProblemData problem)
            =>
            problem.StatusCode;

        static ProblemData GetValue(KeyValuePair<string, ProblemData> kv)
            =>
            kv.Value;
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
        var usings = new List<string>();

        var exampleValue = bodyType.PropertySymbol.GetExampleValue();
        var description = bodyType.PropertySymbol.GetDescriptionValue();

        var requestBodySchema = bodyType.BodyType.GetSimpleSchemaFunction(usings, exampleValue, description);
        sourceBuilder.AddUsings(usings);

        if (string.IsNullOrEmpty(requestBodySchema) is false)
        {
            return sourceBuilder.AppendCodeLine($"Content = {requestBodySchema}.CreateContent({bodyType.ContentType.ToStringValueOrEmpty()})");
        }

        return sourceBuilder.AppendSchema("Content", bodyType.BodyType, 0, exampleValue, description).AppendCodeLine(
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

            var exampleValue = property.PropertySymbol.GetExampleValue();
            var description = property.PropertySymbol.GetDescriptionValue();

            sourceBuilder.AppendSchema(propertyName, property.PropertyType, 1, exampleValue, description);
        }

        return sourceBuilder
            .EndCodeBlock()
            .EndCodeBlock()
            .AppendCodeLine($".CreateContent(\"application/json\")");
    }

    private static SourceBuilder AppendSchema(
        this SourceBuilder builder, string parameterName, ITypeSymbol type, int level, string? exampleValue, string? description)
    {
        if (level > 0)
        {
            var usings = new List<string>();
            var simpleSchemaFunction = type.GetSimpleSchemaFunction(usings, exampleValue, description);
            builder.AddUsings(usings);

            if (string.IsNullOrEmpty(simpleSchemaFunction) is false)
            {
                return builder.AppendCodeLine($"{parameterName} = {simpleSchemaFunction},");
            }

            if (level >= MaxRecursiveSchemaLevel)
            {
                return builder.AppendCodeLine($"{parameterName} = CreateDefaultSchema({type.IsNullable().ToStringValue()}),");
            }
        }

        var afterSymbol = level > 0 ? "," : null;
        var schemaType = level > 0 ?  "new()" : "new OpenApiSchema";
        level++;

        builder
            .AppendCodeLine($"{parameterName} = {schemaType}")
            .BeginCodeBlock()
            .AppendCodeLine($"Nullable = {type.IsNullable().ToStringValue()},");

        type = type.GetNullableStructType() ?? type;

        var collectionType = type.GetCollectionTypeOrDefault();
        if (collectionType is not null)
        {
            return builder.AppendCodeLine("Type = \"array\",")
                .AppendSchema("Items", collectionType, level, exampleValue, description).EndCodeBlock(afterSymbol);
        }

        builder.AppendCodeLine("Type = \"object\",").AppendCodeLine("Properties = new Dictionary<string, OpenApiSchema>").BeginCodeBlock();

        foreach (var jsonProperty in type.GetJsonProperties())
        {
            var propertyName = "[" + jsonProperty.GetJsonPropertyName().ToStringValueOrEmpty() + "]";

            var jsonExampleValue = jsonProperty.GetExampleValue();
            var jsonDescription = jsonProperty.GetDescriptionValue();

            builder.AppendSchema(propertyName, jsonProperty.Type, level, jsonExampleValue, jsonDescription);
        }

        return builder.EndCodeBlock().EndCodeBlock(afterSymbol);
    }
}