using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void Execute_PictureSetGetFunc_GeneratesFactorySource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.PictureSetGetSourceCode);
        var factorySource = generatedSources.Single(IsFactorySource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.PictureSetGetFactorySourceCode),
            NormalizeNewLines(factorySource));

        static bool IsFactorySource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.PictureSetGetFactoryHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_NotificationSubscribeFunc_GeneratesFactorySource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.NotificationSubscribeSourceCode);
        var factorySource = generatedSources.Single(IsFactorySource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.NotificationSubscribeFactorySourceCode),
            NormalizeNewLines(factorySource));

        static bool IsFactorySource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.NotificationSubscribeFactoryHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_TagSetGetFunc_GeneratesFactorySource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.TagSetGetSourceCode);
        var factorySource = generatedSources.Single(IsFactorySource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.TagSetGetFactorySourceCode),
            NormalizeNewLines(factorySource));

        static bool IsFactorySource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.TagSetGetFactoryHintName, StringComparison.Ordinal);
    }
}