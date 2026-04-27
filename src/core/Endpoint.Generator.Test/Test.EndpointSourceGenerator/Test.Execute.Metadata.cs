using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void Execute_PictureSetGetFunc_GeneratesMetadataSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.PictureSetGetSourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.PictureSetGetMetadataSourceCode),
            NormalizeNewLines(metadataSource));

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.PictureSetGetMetadataHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_NotificationSubscribeFunc_GeneratesMetadataSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.NotificationSubscribeSourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.NotificationSubscribeMetadataSourceCode),
            NormalizeNewLines(metadataSource));

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.NotificationSubscribeMetadataHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_TagSetGetFunc_GeneratesMetadataSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.TagSetGetSourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.TagSetGetMetadataSourceCode),
            NormalizeNewLines(metadataSource));

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.TagSetGetMetadataHintName, StringComparison.Ordinal);
    }
}
