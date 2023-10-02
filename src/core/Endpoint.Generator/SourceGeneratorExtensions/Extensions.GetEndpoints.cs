using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

using static EndpointAttributeHelper;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointAttributeName = "GarageGroup.Infra.EndpointAttribute";

    internal static IReadOnlyCollection<EndpointTypeDescription> GetEndpointTypes(this GeneratorExecutionContext context)
    {
        var endpointAttributeType = context.Compilation.GetTypeByMetadataNameOrThrow(EndpointAttributeName);

        var visitor = new ExportedTypesCollector(context.CancellationToken);
        visitor.VisitAssembly(context.Compilation.Assembly);

        return visitor.GetNonStaticTypes().Select(InnerGetEndpointType).NotNull().ToArray();

        EndpointTypeDescription? InnerGetEndpointType(INamedTypeSymbol typeSymbol)
            =>
            GetEndpointType(typeSymbol, endpointAttributeType);
    }

    private static EndpointTypeDescription? GetEndpointType(INamedTypeSymbol typeSymbol, INamedTypeSymbol endpointAttributeType)
    {
        var endpointAttributeData = typeSymbol.GetAttributes().FirstOrDefault(IsEndpointAttribute);
        if (endpointAttributeData is null)
        {
            return null;
        }

        if (typeSymbol.TypeParameters.Length > 0)
        {
            throw new NotSupportedException("Generic types are not supported");
        }

        var endpointMethod = typeSymbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(IsEndpointMethod)
            ?? throw new InvalidOperationException($"An endpoint method was not found in the type {typeSymbol.Name}");

        var methodRetrunType = endpointMethod.ReturnType.GetTaskType();
        var failureType = methodRetrunType.IsResultType() ? methodRetrunType?.TypeArguments[1] as INamedTypeSymbol : null;

        var tags = typeSymbol.GetEndpointTags().ToArray();
        if (tags.Length is 0)
        {
            tags = new EndpointTag[]
            {
                new(string.Empty, null)
            };
        }

        return new()
        {
            Namespace = typeSymbol.ContainingNamespace.ToString(),
            IsTypePublic = typeSymbol.DeclaredAccessibility is Accessibility.Public,
            TypeRootName = typeSymbol.Name.GetTypeRootName(),
            TypeFuncName = typeSymbol.Name,
            IsTypeFuncStruct = typeSymbol.IsReferenceType is false,
            MethodFuncName = endpointMethod.Name,
            SerializerOptionsPropertyFuncName = typeSymbol.GetSerializerOptionsPropertyFuncName(),
            MethodName = GetMethodName(endpointAttributeData.ConstructorArguments[0].Value),
            Route = endpointAttributeData.ConstructorArguments[1].Value?.ToString(),
            Summary = endpointAttributeData.GetAttributePropertyValue("Summary")?.ToString(),
            Description = endpointAttributeData.GetAttributePropertyValue("Description")?.ToString(),
            Tags = tags,
            RequestType = endpointMethod.Parameters[0].Type,
            ResponseType = methodRetrunType.IsResultType() ? methodRetrunType?.TypeArguments[0] : methodRetrunType,
            FailureCodeType = failureType?.TypeArguments[0],
            ObsoleteData = typeSymbol.GetObsoleteData()
        };

        bool IsEndpointAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.Equals(endpointAttributeType, SymbolEqualityComparer.Default) is true;
    }

    private static IEnumerable<EndpointTag> GetEndpointTags(this INamedTypeSymbol typeSymbol)
    {
        foreach (var tagAttribute in typeSymbol.GetAttributes().Where(IsEndpointTagAttribute))
        {
            if (tagAttribute.ConstructorArguments.Length is not > 0)
            {
                continue;
            }

            var name = tagAttribute.ConstructorArguments[0].Value?.ToString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            var description = tagAttribute.GetAttributePropertyValue("Description")?.ToString();
            yield return new(name: name!, description: description);
        }
    }

    private static string? GetSerializerOptionsPropertyFuncName(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(IsReturnTypeJsonSerializerOptions)
            .Where(IsGetterAllowed)
            .Where(IsStatic)
            .Where(IsPublicOrInternal)
            .FirstOrDefault()
            ?.Name;

        static bool IsReturnTypeJsonSerializerOptions(IPropertySymbol property)
            =>
            property.Type.IsType("System.Text.Json", "JsonSerializerOptions");

        static bool IsGetterAllowed(IPropertySymbol property)
            =>
            property.GetMethod is not null;

        static bool IsStatic(IPropertySymbol property)
            =>
            property.IsStatic;

        static bool IsPublicOrInternal(IPropertySymbol property)
            =>
            property.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal;
    }

    private static ObsoleteData? GetObsoleteData(this INamedTypeSymbol typeSymbol)
    {
        var obsoleteAttributeData = typeSymbol.GetAttributes().FirstOrDefault(IsObsoleteAttribute);
        if (obsoleteAttributeData is null)
        {
            return null;
        }

        return new(
            message: obsoleteAttributeData.GetAttributeValue(0)?.ToString(),
            isError: obsoleteAttributeData.GetAttributeValue(1) as bool?,
            diagnosticId: obsoleteAttributeData.GetAttributePropertyValue("DiagnosticId")?.ToString(),
            urlFormat: obsoleteAttributeData.GetAttributePropertyValue("UrlFormat")?.ToString());

        static bool IsObsoleteAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsSystemType("ObsoleteAttribute") is true;
    }

    private static string GetMethodName(object? source)
        =>
        source switch
        {
            0 => "Get",
            1 => "Post",
            2 => "Put",
            3 => "Delete",
            4 => "Options",
            5 => "Head",
            6 => "Patch",
            7 => "Trace",
            _ => throw new InvalidOperationException($"An unexpected endpoint method value: {source}")
        };
}