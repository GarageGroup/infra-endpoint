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

    [Fact]
    public static void Execute_EndpointWithStringResponse_GeneratesMetadataSourceWithSuccessBody()
    {
        const string sourceCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;

            namespace Demo
            {
                [Endpoint("StringGet", EndpointMethod.Get, "/string")]
                public interface IStringGetFunc
                {
                    ValueTask<string> InvokeAsync(Unit input, CancellationToken cancellationToken);
                }

                public readonly record struct Unit;
            }
            """;

        var generatedSources = RunGeneratorAndGetSources(sourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Contains("[\"200\"] = new OpenApiResponse()", metadataSource, StringComparison.Ordinal);
        Assert.Contains("CreateStringSchema", metadataSource, StringComparison.Ordinal);
        Assert.Contains("CreateContent(\"application/json\")", metadataSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"204\"] = new OpenApiResponse()", metadataSource, StringComparison.Ordinal);

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.EndsWith(".Metadata.g.cs", StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_EndpointWithObjectResponse_GeneratesMetadataSourceWithSuccessBody()
    {
        const string sourceCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;

            namespace Demo
            {
                [Endpoint("ObjectGet", EndpointMethod.Get, "/object")]
                public interface IObjectGetFunc
                {
                    ValueTask<object> InvokeAsync(Unit input, CancellationToken cancellationToken);
                }

                public readonly record struct Unit;
            }
            """;

        var generatedSources = RunGeneratorAndGetSources(sourceCode);
        var metadataSource = generatedSources.Single(IsMetadataSource).SourceText.ToString();

        Assert.Contains("[\"200\"] = new OpenApiResponse()", metadataSource, StringComparison.Ordinal);
        Assert.Contains(".CreateContent(\"application/json\")", metadataSource, StringComparison.Ordinal);
        Assert.DoesNotContain("[\"204\"] = new OpenApiResponse()", metadataSource, StringComparison.Ordinal);

        static bool IsMetadataSource(GeneratedSourceResult source)
            =>
            source.HintName.EndsWith(".Metadata.g.cs", StringComparison.Ordinal);
    }
}
