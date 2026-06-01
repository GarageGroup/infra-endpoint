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

    [Fact]
    public static void Execute_EndpointWithStringResponse_GeneratesInvokeSourceWithJsonBody()
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
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Contains("statusCode: 200,", invokeSource, StringComparison.Ordinal);
        Assert.Contains("new(\"Content-Type\", \"application/json\")", invokeSource, StringComparison.Ordinal);
        Assert.Contains("body: success.ToJsonStream(SerializerOptions));", invokeSource, StringComparison.Ordinal);

        Assert.DoesNotContain("statusCode: 204,", invokeSource, StringComparison.Ordinal);
        Assert.DoesNotContain("body: default);", invokeSource, StringComparison.Ordinal);

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.EndsWith(".Invoke.g.cs", StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_EndpointWithObjectResponse_GeneratesInvokeSourceWithJsonBody()
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
        var invokeSource = generatedSources.Single(IsInvokeSource).SourceText.ToString();

        Assert.Contains("statusCode: 200,", invokeSource, StringComparison.Ordinal);
        Assert.Contains("new(\"Content-Type\", \"application/json\")", invokeSource, StringComparison.Ordinal);
        Assert.Contains("body: success.ToJsonStream(SerializerOptions));", invokeSource, StringComparison.Ordinal);

        Assert.DoesNotContain("statusCode: 204,", invokeSource, StringComparison.Ordinal);
        Assert.DoesNotContain("body: default);", invokeSource, StringComparison.Ordinal);

        static bool IsInvokeSource(GeneratedSourceResult source)
            =>
            source.HintName.EndsWith(".Invoke.g.cs", StringComparison.Ordinal);
    }
}
