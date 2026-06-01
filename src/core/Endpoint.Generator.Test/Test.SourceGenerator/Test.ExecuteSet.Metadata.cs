using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void ExecuteSet_ProductSet_GeneratesEndpointSetMetadataSource()
    {
        var generatedSources = RunSetGeneratorAndGetSources(EndpointSourceGeneratorData.ProductSetSourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.ProductSetMetadataSourceCode),
            NormalizeNewLines(metadataSource));

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.ProductSetMetadataHintName, StringComparison.Ordinal);
    }
}