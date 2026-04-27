using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

using static EndpointAttributeHelper;

partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<MediaTypeDescription> GetMediaTypes(this Compilation compilation, CancellationToken cancellationToken)
    {
        var visitor = new ExportedTypesCollector(cancellationToken);
        visitor.VisitAssembly(compilation.Assembly);

        return visitor.GetNonStaticTypes().Select(GetMediaTypeDescription).NotNull().ToArray();
    }

    private static MediaTypeDescription? GetMediaTypeDescription(INamedTypeSymbol typeSymbol)
    {
        var mediaTypeMetadataAttributeData = typeSymbol.GetAttributes().FirstOrDefault(IsMediaTypeMetadataAttribute);
        if (mediaTypeMetadataAttributeData is null)
        {
            return null;
        }

        var typeName = mediaTypeMetadataAttributeData.GetAttributePropertyValue("TypeName")?.ToString();
        if (string.IsNullOrWhiteSpace(typeName))
        {
            typeName = typeSymbol.Name + "Metadata";
        }

        return new(
            @namespace: typeSymbol.ContainingNamespace?.ToString(),
            typeName: typeName,
            type: typeSymbol);
    }
}
