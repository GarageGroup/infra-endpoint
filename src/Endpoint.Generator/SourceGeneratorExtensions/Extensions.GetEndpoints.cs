using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

using static EndpointAttributeHelper;

internal static partial class SourceGeneratorExtensions
{
    private const string EndpointAttributeName = "GGroupp.Infra.EndpointAttribute";

    internal static IReadOnlyCollection<EndpointTypeDescription> GetEndpointTypes(this GeneratorExecutionContext context)
    {
        var endpointAttributeType = context.Compilation.GetTypeByMetadataNameOrThrow(EndpointAttributeName);

        var visitor = new ExportedTypesCollector(context.CancellationToken);
        visitor.VisitAssembly(context.Compilation.Assembly);

        return visitor.GetNotStaticTypes().Select(InnerGetEndpointType).Where(NotNull).ToArray()!;

        EndpointTypeDescription? InnerGetEndpointType(INamedTypeSymbol typeSymbol)
            =>
            GetEndpointType(typeSymbol, endpointAttributeType);

        static bool NotNull(EndpointTypeDescription? typeDescription)
            =>
            typeDescription is not null;
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

        var endpointMethod = typeSymbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(IsEndpointMethod);
        if (endpointMethod is null)
        {
            throw new InvalidOperationException($"An endpoint method was not found in the type {typeSymbol.Name}");
        }

        var resultType = endpointMethod.ReturnType.GetTaskType() as INamedTypeSymbol;
        var failureType = resultType?.TypeArguments[1] as INamedTypeSymbol;

        var typeRootName = typeSymbol.Name.GetTypeRootName();

        var tags = typeSymbol.GetEndpointTags().ToArray();
        if (tags.Length is 0 && string.IsNullOrEmpty(typeRootName) is false)
        {
            tags = new EndpointTag[]
            {
                new(typeRootName!, null)
            };
        }

        return new()
        {
            Namespace = typeSymbol.ContainingNamespace.ToString(),
            TypeRootName = typeRootName,
            TypeFuncName = typeSymbol.Name,
            MethodFuncName = endpointMethod.Name,
            Method = GetMethodValue(endpointAttributeData.ConstructorArguments[0].Value),
            Route = endpointAttributeData.ConstructorArguments[1].Value?.ToString(),
            Summary = endpointAttributeData.GetAttributePropertyValue("Summary")?.ToString(),
            Description = endpointAttributeData.GetAttributePropertyValue("Description")?.ToString(),
            Tags = tags,
            RequestType = endpointMethod.Parameters[0].Type,
            ResponseType = resultType?.TypeArguments[0],
            FailureCodeType = failureType?.TypeArguments[0]
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

    private static string GetMethodValue(object? source)
        =>
        source switch
        {
            0 => "EndpointMethod.Get",
            1 => "EndpointMethod.Post",
            2 => "EndpointMethod.Put",
            3 => "EndpointMethod.Delete",
            4 => "EndpointMethod.Options",
            5 => "EndpointMethod.Head",
            6 => "EndpointMethod.Patch",
            7 => "EndpointMethod.Trace",
            _ => throw new InvalidOperationException($"An unexpected endpoint method value: {source}")
        };
}