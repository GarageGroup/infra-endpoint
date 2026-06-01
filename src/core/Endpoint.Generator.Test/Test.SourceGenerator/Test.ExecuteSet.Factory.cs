using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void ExecuteSet_ProductSet_GeneratesEndpointSetFactorySource()
    {
        var generatedSources = RunSetGeneratorAndGetSources(EndpointSourceGeneratorData.ProductSetSourceCode);
        var factorySource = generatedSources.Single(IsFactorySource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.ProductSetFactorySourceCode),
            NormalizeNewLines(factorySource));

        static bool IsFactorySource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.ProductSetFactoryHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void ExecuteSet_ProductSet_RelatedEndpointsHaveInternalConstructors()
    {
        var generatedSources = RunSetGeneratorAndGetSources(EndpointSourceGeneratorData.ProductSetSourceCode);

        var productGetFactorySource = generatedSources.Single(IsProductGetFactory).SourceText.ToString();
        Assert.Contains(
            "internal ProductGetEndpoint(IProductGetFunc endpointFunc, ILogger? logger)",
            productGetFactorySource,
            StringComparison.Ordinal);

        var productDeleteFactorySource = generatedSources.Single(IsProductDeleteFactory).SourceText.ToString();
        Assert.Contains(
            "internal ProductDeleteEndpoint(IProductDeleteFunc endpointFunc, ILogger? logger)",
            productDeleteFactorySource,
            StringComparison.Ordinal);

        static bool IsProductGetFactory(GeneratedSourceResult source)
            =>
            source.HintName.Equals("ProductGetEndpoint.g.cs", StringComparison.Ordinal);

        static bool IsProductDeleteFactory(GeneratedSourceResult source)
            =>
            source.HintName.Equals("ProductDeleteEndpoint.g.cs", StringComparison.Ordinal);
    }

    [Fact]
    public static void ExecuteSet_ProductEndpointApiSet_GeneratesEndpointSetFactorySource()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.ProductEndpointApiSetSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.Null(generatorResult.Exception);
        Assert.Empty(runResult.Diagnostics);

        var generatedSources = generatorResult.GeneratedSources.ToArray();
        var factorySource = generatedSources.Single(IsFactorySource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.ProductEndpointApiSetFactorySourceCode),
            NormalizeNewLines(factorySource));

        static bool IsFactorySource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.ProductEndpointApiSetFactoryHintName, StringComparison.Ordinal);
    }

    private static GeneratedSourceResult[] RunSetGeneratorAndGetSources(string sourceCode)
    {
        var result = RunGenerator(sourceCode);
        var generatorResult = result.Results.Single();

        Assert.Null(generatorResult.Exception);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(9, generatorResult.GeneratedSources.Length);

        return generatorResult.GeneratedSources.ToArray();
    }
}
