using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void Execute_PictureSetGetFunc_GeneratesInvokeSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.PictureSetGetSourceCode);
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.PictureSetGetInvokeSourceCode),
            NormalizeNewLines(invokeSource));

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.PictureSetGetInvokeHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_NotificationSubscribeFunc_GeneratesInvokeSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.NotificationSubscribeSourceCode);
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.NotificationSubscribeInvokeSourceCode),
            NormalizeNewLines(invokeSource));

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.NotificationSubscribeInvokeHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_TagSetGetFunc_GeneratesInvokeSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.TagSetGetSourceCode);
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.TagSetGetInvokeSourceCode),
            NormalizeNewLines(invokeSource));

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.TagSetGetInvokeHintName, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_CustomSerializerOptionsFunc_GeneratesInvokeSource()
    {
        var generatedSources = RunGeneratorAndGetSources(EndpointSourceGeneratorData.CustomSerializerOptionsSourceCode);
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Equal(
            NormalizeNewLines(EndpointSourceGeneratorData.CustomSerializerOptionsInvokeSourceCode),
            NormalizeNewLines(invokeSource));

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.Equals(EndpointSourceGeneratorData.CustomSerializerOptionsInvokeHintName, StringComparison.Ordinal);
    }
}
