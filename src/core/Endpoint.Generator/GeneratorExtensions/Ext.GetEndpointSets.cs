using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<EndpointSetTypeDescription> GetEndpointSetTypes(
        this Compilation compilation, CancellationToken cancellationToken)
    {
        var endpointAttributeType = compilation.GetTypeByMetadataNameOrThrow(EndpointAttributeName);
        var endpointSetAttributeType = compilation.GetTypeByMetadataNameOrThrow(EndpointSetAttributeName);

        var visitor = new ExportedTypesCollector(cancellationToken);
        visitor.VisitAssembly(compilation.Assembly);

        return visitor.GetNonStaticTypes().Select(InnerGetEndpointSetType).NotNull().ToArray();

        EndpointSetTypeDescription? InnerGetEndpointSetType(INamedTypeSymbol typeSymbol)
            =>
            GetEndpointSetType(typeSymbol, endpointAttributeType, endpointSetAttributeType);
    }

    private static EndpointSetTypeDescription? GetEndpointSetType(
        INamedTypeSymbol typeSymbol, INamedTypeSymbol endpointAttributeType, INamedTypeSymbol endpointSetAttributeType)
    {
        if (typeSymbol.GetAttributes().Any(IsEndpointSetAttribute) is false)
        {
            return null;
        }

        if (typeSymbol.TypeParameters.Length > 0)
        {
            throw new NotSupportedException("Generic endpoint set types are not supported");
        }

        var endpoints = typeSymbol.AllInterfaces.Select(GetEndpointInSet).NotNull().ToArray();
        if (endpoints.Length is 0)
        {
            throw new InvalidOperationException(
                $"Endpoint set type {typeSymbol.Name} must inherit at least one interface with {EndpointAttributeName}.");
        }

        var duplicateOperationId = endpoints
            .Where(static endpoint => string.IsNullOrWhiteSpace(endpoint.OperationId) is false)
            .GroupBy(static endpoint => endpoint.OperationId, StringComparer.Ordinal)
            .FirstOrDefault(static group => group.Count() > 1)?
            .Key;

        if (duplicateOperationId is not null)
        {
            throw new InvalidOperationException(
                $"Endpoint set type {typeSymbol.Name} contains duplicate endpoint operationId: {duplicateOperationId}.");
        }

        return new()
        {
            Namespace = typeSymbol.ContainingNamespace.ToString(),
            IsTypePublic = typeSymbol.DeclaredAccessibility is Accessibility.Public,
            TypeRootName = typeSymbol.Name.GetTypeRootName(),
            TypeFuncName = typeSymbol.Name,
            IsTypeFuncStruct = typeSymbol.IsReferenceType is false,
            Endpoints = endpoints
        };

        EndpointSetEndpointDescription? GetEndpointInSet(INamedTypeSymbol interfaceType)
        {
            if (interfaceType.GetAttributes().Any(IsEndpointAttribute) is false)
            {
                return null;
            }

            var endpointType = GetEndpointType(interfaceType, endpointAttributeType, isIncludedInEndpointSet: true)
                ?? throw new InvalidOperationException($"An endpoint type was not found in the interface {interfaceType.Name}");

            return new()
            {
                EndpointNamespace = endpointType.Namespace,
                EndpointTypeName = endpointType.TypeEndpointName,
                OperationId = endpointType.OperationId,
                MethodName = endpointType.MethodName,
                Route = endpointType.Route
            };
        }

        bool IsEndpointAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.Equals(endpointAttributeType, SymbolEqualityComparer.Default) is true;

        bool IsEndpointSetAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.Equals(endpointSetAttributeType, SymbolEqualityComparer.Default) is true;
    }
}
