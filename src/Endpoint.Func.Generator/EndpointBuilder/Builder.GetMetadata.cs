using System;
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
            "System",
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
        var requestConstructor = type.RequestType?.GetConstructor();
        var bodyParameters = requestConstructor?.Parameters.Where(IsBodyParameter).ToArray();

        if (bodyParameters?.Length is not > 0)
        {
            return sourceBuilder;
        }

        if (bodyParameters.Length > 1)
        {
            throw new InvalidOperationException("There must be only one request body parameter");
        }

        return sourceBuilder
                    .AppendCodeLine(
                        "RequestBody = new()")
                    .BeginCodeBlock()
                    .AppendRequestBodyContent(bodyParameters[0].Type)
                    .EndCodeBlock(',');
    }

    private static SourceBuilder AppendRequestBodyContent(this SourceBuilder sourceBuilder, ITypeSymbol bodyType)
    {
        var requestBodySchema = bodyType.GetSimpleSchemaFunction();
        if (string.IsNullOrEmpty(requestBodySchema) is false)
        {
            return sourceBuilder.AppendCodeLine($"Content = {requestBodySchema}.CreateTextContent()");
        }

        return sourceBuilder.AppendSchema("Content", bodyType, false).AppendCodeLine(".CreateJsonContent()");
    }

    private static SourceBuilder AppendSchema(this SourceBuilder sourceBuilder, string parameterName, ITypeSymbol type, bool inner)
    {
        if (inner)
        {
            var simpleSchemaFunction = type.GetSimpleSchemaFunction();
            if (string.IsNullOrEmpty(simpleSchemaFunction) is false)
            {
                return sourceBuilder.AppendCodeLine($"{parameterName} = {simpleSchemaFunction},");
            }
        }

        var afterSymbol = inner ? "," : null;
        var schemaType = inner ?  "new()" : "new OpenApiSchema";

        sourceBuilder
            .AppendCodeLine($"{parameterName} = {schemaType}")
            .BeginCodeBlock()
            .AppendCodeLine($"Nullable = {type.IsNullable().ToStringValue()},");

        var collectionType = type.GetCollectionType();
        if (collectionType is not null)
        {
            return sourceBuilder.AppendCodeLine("Type = \"array\",").AppendSchema("Items", collectionType, true).EndCodeBlock(afterSymbol);
        }

        sourceBuilder.AppendCodeLine("Type = \"object\",").AppendCodeLine("Properties = new Dictionary<string, OpenApiSchema>").BeginCodeBlock();

        foreach (var jsonProperty in type.GetJsonProperties())
        {
            var propertyName = "[" + jsonProperty.GetJsonPropertyName().ToStringValueOrEmpty() + "]";
            sourceBuilder.AppendSchema(propertyName, jsonProperty.Type, true);
        }

        return sourceBuilder.EndCodeBlock().EndCodeBlock(afterSymbol);
    }

    private static SourceBuilder AppendSchemasBody(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        return sourceBuilder;
    }
}