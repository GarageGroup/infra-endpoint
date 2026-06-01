using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void ExecuteSet_ProductSet_GeneratesEndpointSetInvokeSource()
    {
        var generatedSources = RunSetGeneratorAndGetSources(EndpointSourceGeneratorData.ProductSetSourceCode);
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.ProductSetInvokeSourceCode),
            NormalizeNewLines(invokeSource));

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.ProductSetInvokeHintName, StringComparison.Ordinal);
    }
}
