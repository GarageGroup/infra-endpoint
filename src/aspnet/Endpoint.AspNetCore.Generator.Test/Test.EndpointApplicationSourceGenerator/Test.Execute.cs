using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace GarageGroup.Infra.Endpoint.AspNetCore.Generator.Test;

partial class EndpointApplicationSourceGeneratorTest
{
    [Fact]
    public static void Execute_ValidEndpointApplicationExtension_GeneratesConstructorAndEndpointSources()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.AspNetCore
            {
                public sealed class PictureEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class EndpointProvider
                {
                    [EndpointApplicationExtension]
                    public static Dependency<PictureEndpoint> UsePictureEndpoint()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var generatedSources = RunGeneratorAndGetSources(sourceCode, 2);

        var constructor = generatedSources.Single(IsConstructor).SourceText.ToString();
        Assert.Equal(
            NormalizeNewLines(
                """
                // Auto-generated code by PrimeFuncPack
                #nullable enable

                namespace Demo.AspNetCore;

                internal static partial class EndpointProviderEndpointExtensions
                {
                }
                """),
            NormalizeNewLines(constructor));

        var endpoint = generatedSources.Single(IsEndpoint).SourceText.ToString();
        Assert.Equal(
            NormalizeNewLines(
                """
                // Auto-generated code by PrimeFuncPack
                #nullable enable

                using Microsoft.AspNetCore.Builder;

                namespace Demo.AspNetCore;

                partial class EndpointProviderEndpointExtensions
                {
                    internal static TBuilder UsePictureEndpoint<TBuilder>(this TBuilder builder) where TBuilder : IApplicationBuilder
                        =>
                        builder.UseEndpoint(EndpointProvider.UsePictureEndpoint().Resolve);
                }
                """),
            NormalizeNewLines(endpoint));

        static bool IsConstructor(GeneratedSourceResult source)
            =>
            source.HintName.Equals("EndpointProviderEndpointExtensions.g.cs", StringComparison.Ordinal);

        static bool IsEndpoint(GeneratedSourceResult source)
            =>
            source.HintName.Equals("EndpointProviderEndpointExtensions.UsePictureEndpoint.g.cs", StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_ResolverReturnsCustomDependencyWithResolveMethod_GeneratesSources()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;

            namespace Demo.AspNetCore
            {
                public sealed class PictureEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class EndpointProvider
                {
                    [EndpointApplicationExtension]
                    public static Some.Test.CustomDependency<PictureEndpoint> UsePictureEndpoint()
                        =>
                        default!;
                }
            }

            namespace Some.Test
            {
                public sealed class CustomDependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var generatedSources = RunGeneratorAndGetSources(sourceCode, 2);
        var endpoint = generatedSources.Single(IsEndpoint).SourceText.ToString();

        Assert.Contains("builder.UseEndpoint(EndpointProvider.UsePictureEndpoint().Resolve);", endpoint, StringComparison.Ordinal);

        static bool IsEndpoint(GeneratedSourceResult source)
            =>
            source.HintName.Equals("EndpointProviderEndpointExtensions.UsePictureEndpoint.g.cs", StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_MultipleValidEndpointApplicationExtensions_GeneratesOneConstructorAndEndpointSources()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.AspNetCore
            {
                public sealed class FirstEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public sealed class SecondEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                internal static class EndpointProvider
                {
                    [EndpointApplicationExtension]
                    internal static Dependency<FirstEndpoint> UseFirstEndpoint()
                        =>
                        default!;

                    [EndpointApplicationExtension]
                    internal static Dependency<SecondEndpoint> UseSecondEndpoint()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var generatedSources = RunGeneratorAndGetSources(sourceCode, 3);
        var hintNames = generatedSources.Select(GetHintName).ToArray();

        Assert.Contains("EndpointProviderEndpointExtensions.g.cs", hintNames);
        Assert.Contains("EndpointProviderEndpointExtensions.UseFirstEndpoint.g.cs", hintNames);
        Assert.Contains("EndpointProviderEndpointExtensions.UseSecondEndpoint.g.cs", hintNames);

        static string GetHintName(GeneratedSourceResult source)
            =>
            source.HintName;
    }

    [Fact]
    public static void Execute_ResolverIsNotStatic_ThrowsInvalidOperationException()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.Invalid
            {
                public sealed class InvalidEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public sealed class InvalidProvider
                {
                    [EndpointApplicationExtension]
                    public Dependency<InvalidEndpoint> UseInvalidEndpoint()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var exception = Assert.IsType<InvalidOperationException>(result.Results.Single().Exception);

        Assert.Contains("InvalidProvider.UseInvalidEndpoint", exception.Message, StringComparison.Ordinal);
        Assert.Contains("must be static", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_ResolverHasParameter_ThrowsInvalidOperationException()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.Invalid
            {
                public sealed class InvalidEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class InvalidProvider
                {
                    [EndpointApplicationExtension]
                    public static Dependency<InvalidEndpoint> UseInvalidEndpoint(string value)
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var exception = Assert.IsType<InvalidOperationException>(result.Results.Single().Exception);

        Assert.Contains("InvalidProvider.UseInvalidEndpoint", exception.Message, StringComparison.Ordinal);
        Assert.Contains("must have no parameters", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_ResolverReturnsNotDependency_ThrowsInvalidOperationException()
    {
        const string sourceCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;

            namespace Demo.Invalid
            {
                public sealed class InvalidEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class InvalidProvider
                {
                    [EndpointApplicationExtension]
                    public static InvalidEndpoint UseInvalidEndpoint()
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var exception = Assert.IsType<InvalidOperationException>(result.Results.Single().Exception);

        Assert.Contains("InvalidProvider.UseInvalidEndpoint", exception.Message, StringComparison.Ordinal);
        Assert.Contains(
            "return type must contain a public instance Resolve(System.IServiceProvider) method without generic arguments",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_ResolverReturnsDependencyWithNonEndpoint_ThrowsInvalidOperationException()
    {
        const string sourceCode =
            """
            using System;
            using GarageGroup.Infra;
            using PrimeFuncPack;

            namespace Demo.Invalid
            {
                public sealed class NotEndpoint;

                public static class InvalidProvider
                {
                    [EndpointApplicationExtension]
                    public static Dependency<NotEndpoint> UseInvalidEndpoint()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var exception = Assert.IsType<InvalidOperationException>(result.Results.Single().Exception);

        Assert.Contains("InvalidProvider.UseInvalidEndpoint", exception.Message, StringComparison.Ordinal);
        Assert.Contains("must resolve a type that implements GarageGroup.Infra.Endpoint.IEndpoint", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_GenericResolver_ThrowsInvalidOperationException()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.Invalid
            {
                public sealed class InvalidEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class InvalidProvider
                {
                    [EndpointApplicationExtension]
                    public static Dependency<InvalidEndpoint> UseInvalidEndpoint<T>()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var exception = Assert.IsType<InvalidOperationException>(result.Results.Single().Exception);

        Assert.Contains("InvalidProvider.UseInvalidEndpoint", exception.Message, StringComparison.Ordinal);
        Assert.Contains("must have no generic arguments", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_NoEndpointApplicationExtensions_GeneratesNoSources()
    {
        const string sourceCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra.Endpoint;

            namespace Demo.Empty
            {
                public sealed class EmptyEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class EmptyProvider
                {
                    public static EmptyEndpoint UseEmptyEndpoint()
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var generatorResult = result.Results.Single();

        Assert.Null(generatorResult.Exception);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(generatorResult.GeneratedSources);
    }

    [Fact]
    public static void Execute_GenericProviderType_GeneratesNoSources()
    {
        const string sourceCode =
            """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using GarageGroup.Infra;
            using GarageGroup.Infra.Endpoint;
            using PrimeFuncPack;

            namespace Demo.Empty
            {
                public sealed class EmptyEndpoint : IEndpoint
                {
                    public static EndpointMetadata GetEndpointMetadata()
                        =>
                        default!;

                    public Task<EndpointResponse> InvokeAsync(EndpointRequest request, CancellationToken cancellationToken = default)
                        =>
                        default!;
                }

                public static class EmptyProvider<T>
                {
                    [EndpointApplicationExtension]
                    public static Dependency<EmptyEndpoint> UseEmptyEndpoint()
                        =>
                        default!;
                }
            }

            namespace PrimeFuncPack
            {
                public sealed class Dependency<T>
                {
                    public T Resolve(IServiceProvider serviceProvider)
                        =>
                        default!;
                }
            }
            """;

        var result = RunGenerator(sourceCode);
        var generatorResult = result.Results.Single();

        Assert.Null(generatorResult.Exception);
        Assert.Empty(result.Diagnostics);
        Assert.Empty(generatorResult.GeneratedSources);
    }
}