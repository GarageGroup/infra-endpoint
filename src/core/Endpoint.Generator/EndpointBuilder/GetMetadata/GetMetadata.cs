using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class EndpointBuilder
{
    internal static string BuildEndpointMetadataSource(this EndpointTypeDescription type)
        =>
        new SourceBuilder(
            type.Namespace)
        .AddUsing(
            "GarageGroup.Infra",
            "GarageGroup.Infra.Endpoint",
            "Microsoft.OpenApi",
            "System.Collections.Generic")
        .AddAlias(
            "static EndpointMetadataHelper")
        .AppendCodeLines(
            "partial class " + type.TypeEndpointName)
        .BeginCodeBlock()
        .AppendCodeLines(
            "public static EndpointMetadata GetEndpointMetadata()")
        .BeginLambda()
        .AppendCodeLines(
            "new(")
        .BeginArguments()
        .AppendCodeLines(
            $"method: {type.GetMethodValue()},",
            $"route: {type.Route.AsStringSourceCodeOrStringEmpty()},",
            "summary: default,",
            "description: default,",
            "operation: new()")
        .BeginCodeBlock()
        .AppendDeprecatedTagIfNecessary(
            type)
        .AppendCodeLines(
            $"Summary = {type.Summary.AsStringValueOrDefault()},",
            $"Description = {type.Description.AsStringValueOrDefault()},")
        .AppendTags(type)
        .AppendOperationParameters(type)
        .AppendRequestBody(type)
        .AppendCodeLines(
            "Responses = new()")
        .BeginCodeBlock()
        .AppendSuccessResponsesBody(type)
        .AppendFailureResponsesBody(type)
        .EndCodeBlock()
        .EndCodeBlock(",")
        .AppendCodeLines(
            "schemas: new Dictionary<string, IOpenApiSchema>()")
        .BeginCodeBlock()
        .AppendSchemasBody(type)
        .EndCodeBlock(")")
        .EndArguments()
        .BeginCodeBlock()
        .AppendCodeLines(
            $"OperationId = {type.OperationId.AsStringSourceCodeOrStringEmpty()}")
        .EndCodeBlock(";")
        .EndLambda()
        .EndCodeBlock()
        .Build();

    private static SourceBuilder AppendDeprecatedTagIfNecessary(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.ObsoleteData is null)
        {
            return sourceBuilder;
        }

        return sourceBuilder.AppendCodeLines(
            "Deprecated = true,");
    }

    private static SourceBuilder AppendTags(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        if (type.Tags?.Count is not > 0)
        {
            return sourceBuilder;
        }

        var tags = type.Tags.ToArray();
        sourceBuilder.AppendCodeLines("Tags = new HashSet<OpenApiTagReference>").BeginCodeBlock();

        for (var i = 0; i < tags.Length; i++)
        {
            var tag = tags[i];

            sourceBuilder
                .AppendCodeLines("new(")
                .BeginArguments()
                .AppendCodeLines(
                    tag.Name.AsStringSourceCodeOrStringEmpty() + ",",
                    "new()")
                .BeginCodeBlock()
                .AppendCodeLines("Tags = new HashSet<OpenApiTag>")
                .BeginCodeBlock()
                .AppendCodeLines("new()")
                .BeginCodeBlock()
                .AppendCodeLines(
                    "Name = " + tag.Name.AsStringSourceCodeOrStringEmpty() + ",",
                    "Description = " + tag.Description.AsStringValueOrDefault())
                .EndCodeBlock()
                .EndCodeBlock(",")
                .EndCodeBlock(",")
                .AppendCodeLines(i < tags.Length - 1 ? "null)," : "null)")
                .EndArguments();
        }

        return sourceBuilder.EndCodeBlock(",");
    }

    private static SourceBuilder AppendOperationParameters(this SourceBuilder sourceBuilder, EndpointTypeDescription type)
    {
        var requestConstructor = type.RequestType?.GetConstructor();
        var usings = new List<string>();

        var parameterDescriptions = requestConstructor?.Parameters.Select(InnerGetParameterDescription).Where(NotNull).ToArray();
        sourceBuilder.AddUsing(usings.ToArray());

        if (parameterDescriptions?.Length is not > 0)
        {
            return sourceBuilder;
        }

        sourceBuilder.AppendCodeLines("Parameters =").BeginCollectionExpression();

        for (var i = 0; i < parameterDescriptions.Length; i++)
        {
            var parameter = parameterDescriptions[i]!;

            sourceBuilder.AppendCodeLines("new OpenApiParameter()")
                .BeginCodeBlock()
                .AppendCodeLines($"Required = {parameter.Required.ToStringValue()},")
                .AppendCodeLines($"In = ParameterLocation.{parameter.Location},")
                .AppendCodeLines($"Name = {parameter.Name.AsStringSourceCodeOrStringEmpty()},")
                .AppendCodeLines($"Schema = {parameter.SchemaFunction},")
                .AppendCodeLines($"Description = {parameter.Description.AsStringValueOrDefault()}");

            if (i < parameterDescriptions.Length - 1)
            {
                sourceBuilder.EndCodeBlock(",");
            }
            else
            {
                sourceBuilder.EndCodeBlock();
            }
        }

        return sourceBuilder.EndCollectionExpression(",");

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
            if (requestBodyType.BodyType.IsEndpointBodyMetadataProvider() is false)
            {
                return sourceBuilder.AppendCodeLines("RequestBody = new OpenApiRequestBody")
                    .BeginCodeBlock().AppendContent(requestBodyType).EndCodeBlock(",");
            }

            var requestBodyData = requestBodyType.BodyType.GetDisplayedData();
            sourceBuilder = sourceBuilder.AddUsing(requestBodyData.AllNamespaces.ToArray());

            var requestType = requestBodyData.DisplayedTypeName;
            return sourceBuilder.AppendCodeLines($"RequestBody = {requestType}.GetEndpointBodyMetadata(),");
        }

        var requestBodyProperties = type.GetRequestBodyProperties();
        if (requestBodyProperties.Count is not > 0)
        {
            return sourceBuilder;
        }

        return sourceBuilder
            .AppendCodeLines("RequestBody = new OpenApiRequestBody")
            .BeginCodeBlock()
            .AppendBodyPropertiesContent(requestBodyProperties)
            .EndCodeBlock(",");
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
            successData = [new SuccessData(type.GetDefaultStatusCode(), null)];
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
        var responseBodyProperties = type.GetResponseBodyProperties();

        var successes = successDataDictionary.Select(GetValue).ToArray();
        foreach (var success in successes)
        {
            var statusCode = success.StatusCode ?? type.GetDefaultStatusCode();
            var descriptionValue = string.IsNullOrEmpty(success.Description) switch
            {
                true => GetStatusDescription(statusCode).AsStringValueOrDefault(),
                _ => success.Description.AsStringValueOrDefault()
            };

            sourceBuilder
                .AppendCodeLines($"[{statusCode.AsStringSourceCodeOrStringEmpty()}] = new OpenApiResponse()")
                .BeginCodeBlock()
                .AppendCodeLines($"Description = {descriptionValue},");

            if (responseBodyType is not null)
            {
                sourceBuilder.AppendContent(responseBodyType);
            }
            else if (responseBodyProperties.Count > 0)
            {
                sourceBuilder.AppendBodyPropertiesContent(responseBodyProperties);
            }
            else if (type.HasImplicitResponseBody())
            {
                sourceBuilder.AppendContent(type.ResponseType, "application/json");
            }

            sourceBuilder.EndCodeBlock(",");
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
                true => GetStatusDescription(failureCode).AsStringValueOrDefault(),
                _ => problem.Description.AsStringValueOrDefault()
            };

            sourceBuilder
                .AppendCodeLines($"[{failureCode.AsStringSourceCodeOrStringEmpty()}] = new OpenApiResponse()")
                .BeginCodeBlock()
                .AppendCodeLines($"Description = {descriptionValue},")
                .AppendCodeLines("Content = CreateProblemContent()")
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

        return sourceBuilder.AppendCodeLines("[\"ProblemDetails\"] = CreateProblemSchema()");
    }

    private static SourceBuilder AppendContent(this SourceBuilder sourceBuilder, BodyTypeDescription bodyType)
    {
        var usings = new List<string>();

        var exampleValue = bodyType.PropertySymbol.GetExampleValue();
        var description = bodyType.PropertySymbol.GetDescriptionValue();

        var requestBodySchema = bodyType.BodyType.GetSimpleSchemaFunction(usings, exampleValue, description);
        sourceBuilder = sourceBuilder.AddUsing(usings.ToArray());

        if (string.IsNullOrEmpty(requestBodySchema) is false)
        {
            return sourceBuilder.AppendCodeLines(
                $"Content = {requestBodySchema}.CreateContent({bodyType.ContentType.Name.AsStringSourceCodeOrStringEmpty()})");
        }

        return sourceBuilder
            .AppendSchema(
                "Content", bodyType.BodyType, 0, exampleValue, description, default)
            .AppendRootXmlSchemaIfNecessary(
                bodyType.BodyType)
            .AppendCodeLines(
                $".CreateContent({bodyType.ContentType.Name.AsStringSourceCodeOrStringEmpty()})");
    }

    private static SourceBuilder AppendContent(this SourceBuilder sourceBuilder, ITypeSymbol bodyType, string contentType)
    {
        var usings = new List<string>();

        var requestBodySchema = bodyType.GetSimpleSchemaFunction(usings, null, null);
        sourceBuilder = sourceBuilder.AddUsing(usings.ToArray());

        if (string.IsNullOrEmpty(requestBodySchema) is false)
        {
            return sourceBuilder.AppendCodeLines(
                $"Content = {requestBodySchema}.CreateContent({contentType.AsStringSourceCodeOrStringEmpty()})");
        }

        return sourceBuilder
            .AppendSchema(
                "Content", bodyType, 0, null, null, default)
            .AppendRootXmlSchemaIfNecessary(
                bodyType)
            .AppendCodeLines(
                $".CreateContent({contentType.AsStringSourceCodeOrStringEmpty()})");
    }

    private static SourceBuilder AppendBodyPropertiesContent(
        this SourceBuilder sourceBuilder, IReadOnlyCollection<BodyPropertyDescription> bodyProperties)
    {
        sourceBuilder = sourceBuilder
            .AppendCodeLines("Content = new OpenApiSchema")
            .BeginCodeBlock()
            .AppendCodeLines("Type = JsonSchemaType.Object,")
            .AppendCodeLines("Properties = new Dictionary<string, IOpenApiSchema>")
            .BeginCodeBlock();

        foreach (var property in bodyProperties)
        {
            var propertyName = "[" + property.BodyParameterName.AsStringSourceCodeOrStringEmpty() + "]";

            var exampleValue = property.PropertySymbol.GetExampleValue();
            var description = property.PropertySymbol.GetDescriptionValue();

            sourceBuilder
                .AppendSchema(propertyName, property.PropertyType, 1, exampleValue, description, property.PropertySymbol as IPropertySymbol);
        }

        var contentType = bodyProperties.FirstOrDefault()?.PropertyKind switch
        {
            BodyPropertyKind.Form => "application/x-www-form-urlencoded",
            _ => "application/json"
        };

        return sourceBuilder
            .EndCodeBlock()
            .EndCodeBlock()
            .AppendCodeLines($".CreateContent(\"{contentType}\")");
    }

    private static SourceBuilder AppendSchema(
        this SourceBuilder builder,
        string parameterName,
        ITypeSymbol type,
        int level,
        string? exampleValue,
        string? description,
        IPropertySymbol? property)
    {
        if (property.IsXmlIgnored())
        {
            return builder;
        }

        if (level > 0)
        {
            var usings = new List<string>();
            var simpleSchemaFunction = type.GetSimpleSchemaFunction(usings, exampleValue, description);
            builder = builder.AddUsing(usings.ToArray());

            if (string.IsNullOrEmpty(simpleSchemaFunction) is false)
            {
                return builder.AppendSchemaIfNecessary(
                    $"{parameterName} = {simpleSchemaFunction}", property);
            }

            if (level >= MaxRecursiveSchemaLevel)
            {
                return builder.AppendSchemaIfNecessary(
                    $"{parameterName} = CreateDefaultSchema({type.IsNullable().ToStringValue()})", property);
            }
        }

        var afterSymbol = level > 0 ? "," : null;
        var schemaType = "new OpenApiSchema";
        level++;

        builder = builder
            .AppendCodeLines($"{parameterName} = {schemaType}")
            .BeginCodeBlock();

        var isNullable = type.IsNullable();

        type = type.GetNullableStructType() ?? type;

        var collectionType = type.GetCollectionTypeOrDefault();
        if (collectionType is not null)
        {
            return builder
                .AppendCodeLines($"Type = {GetSchemaTypeValue("Array", isNullable)},")
                .AppendSchema("Items", collectionType, level, exampleValue, description, property)
                .AppendXmlSchemaAsPropertyIfNecessary(property)
                .EndCodeBlock(afterSymbol);
        }

        builder = builder.AppendCodeLines($"Type = {GetSchemaTypeValue("Object", isNullable)},");
        if (string.IsNullOrEmpty(exampleValue) is false)
        {
            builder = builder.AppendCodeLines($"Example = {exampleValue},");
        }

        builder = builder
            .AppendCodeLines("Properties = new Dictionary<string, IOpenApiSchema>")
            .BeginCodeBlock();

        foreach (var jsonProperty in type.GetJsonProperties())
        {
            var propertyName = "[" + jsonProperty.GetJsonPropertyName().AsStringSourceCodeOrStringEmpty() + "]";

            var jsonExampleValue = jsonProperty.GetExampleValue();
            var jsonDescription = jsonProperty.GetDescriptionValue();

            builder.AppendSchema(propertyName, jsonProperty.Type, level, jsonExampleValue, jsonDescription, jsonProperty);
        }

        return builder.EndCodeBlock().EndCodeBlock(afterSymbol);
    }

    private static string GetSchemaTypeValue(string typeName, bool nullable)
        =>
        nullable ? $"JsonSchemaType.{typeName} | JsonSchemaType.Null" : $"JsonSchemaType.{typeName}";

    private static SourceBuilder AppendRootXmlSchemaIfNecessary(
        this SourceBuilder builder, ITypeSymbol typeSymbol)
    {
        var xmlRootAttribute = typeSymbol.GetXmlRootAttribute();
        if (xmlRootAttribute is null)
        {
            return builder;
        }

        return builder
            .AppendCodeLines(".WithXml(")
            .BeginArguments()
            .AppendCodeLines("xml: new()")
            .BeginCodeBlock()
            .AppendXmlName(xmlRootAttribute)
            .AppendXmlNamespace(xmlRootAttribute)
            .AppendCodeLines("Wrapped = true")
            .EndCodeBlock(")")
            .EndArguments();
    }

    private static SourceBuilder AppendXmlSchemaAsPropertyIfNecessary(
        this SourceBuilder builder, IPropertySymbol? property, string? afterSymbol = null)
    {
        if (property is null || property.ContainsXmlAttribute() is false)
        {
            return builder;
        }

        return builder
            .AppendCodeLines("Xml = new()")
            .BeginCodeBlock()
            .AppendXmlSchemaPropertiesIfNecessary(property, true)
            .EndCodeBlock(afterSymbol);
    }

    private static SourceBuilder AppendSchemaIfNecessary(
        this SourceBuilder builder, string codeLine, IPropertySymbol? property)
    {
        var isDeprecated = property?.GetObsoleteData() is not null;
        var isXmlSchema = property?.ContainsXmlAttribute() is true;

        if (isDeprecated is false && isXmlSchema is false)
        {
            return builder.AppendCodeLines($"{codeLine},");
        }

        builder = builder.AppendCodeLines(codeLine);

        if (isDeprecated && isXmlSchema is false)
        {
            return builder.AppendCodeLines(".Deprecate(),");
        }

        return builder
            .AppendCodeLines(".Deprecate()")
            .AppendCodeLines(".WithXml(")
            .BeginArguments()
            .AppendCodeLines("xml: new()")
            .BeginCodeBlock()
            .AppendXmlSchemaPropertiesIfNecessary(property!, false)
            .EndCodeBlock("),")
            .EndArguments();
    }

    private static SourceBuilder AppendXmlSchemaPropertiesIfNecessary(
        this SourceBuilder builder, IPropertySymbol property, bool isObject)
    {
        var xmlElementAttribute = property.GetXmlElementAttribute();
        if (xmlElementAttribute is not null)
        {
            return builder
                .AppendXmlName(xmlElementAttribute)
                .AppendXmlNamespace(xmlElementAttribute);
        }

        var xmlAttributeAttribute = property.GetXmlAttributeAttribute();
        if (xmlAttributeAttribute is not null)
        {
            return builder
                .AppendXmlName(xmlAttributeAttribute)
                .AppendXmlNamespace(xmlAttributeAttribute)
                .AppendCodeLines("Attribute = true");
        }

        if (isObject)
        {
            var xmlArrayAttribute = property.GetXmlArrayAttribute();
            if (xmlArrayAttribute is not null)
            {
                return builder
                    .AppendXmlName(xmlArrayAttribute)
                    .AppendXmlNamespace(xmlArrayAttribute)
                    .AppendCodeLines("Wrapped = true");
            }
        }
        else
        {
            var xmlArrayItemAttribute = property.GetXmlArrayItemAttribute();
            if (xmlArrayItemAttribute is not null)
            {
                return builder
                    .AppendXmlName(xmlArrayItemAttribute)
                    .AppendXmlNamespace(xmlArrayItemAttribute);
            }
        }

        return builder;
    }

    private static SourceBuilder AppendXmlName(this SourceBuilder builder, AttributeData xmlAttribute)
    {
        var xmlElementName = xmlAttribute.GetAttributeValue(0, "ElementName")?.ToString();

        if (string.IsNullOrEmpty(xmlElementName))
        {
            return builder;
        }

        return builder.AppendCodeLines($"Name = {xmlElementName.AsStringSourceCodeOrStringEmpty()},");
    }

    private static SourceBuilder AppendXmlNamespace(this SourceBuilder builder, AttributeData xmlAttribute)
    {
        var xmlNamespace = xmlAttribute.GetAttributePropertyValue("Namespace")?.ToString();

        if (Uri.TryCreate(xmlNamespace, UriKind.Absolute, out var _) is false)
        {
            return builder;
        }

        return builder.AppendCodeLines($"Namespace = new({xmlNamespace.AsStringSourceCodeOrStringEmpty()})");
    }
}
