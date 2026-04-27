using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

public static partial class EndpointSourceGeneratorTest
{
    private static readonly IReadOnlyList<MetadataReference> MetadataReferences
        =
        [
            ..
            ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).OrEmpty().Split(Path.PathSeparator).Select(CreateFromFile),
            CreateFromType<EndpointAttribute>()
        ];

    private static GeneratedSourceResult[] RunGeneratorAndGetSources(string sourceCode)
    {
        var result = RunGenerator(sourceCode);
        var generatorResult = result.Results.Single();

        Assert.Null(generatorResult.Exception);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(3, generatorResult.GeneratedSources.Length);

        return generatorResult.GeneratedSources.ToArray();
    }

    private static GeneratorDriverRunResult RunGenerator(string sourceCode)
    {
        var compilation = CreateCompilation(sourceCode);
        var compilationDiagnostics = compilation.GetDiagnostics().Where(IsError).ToArray();

        Assert.Empty(compilationDiagnostics);

        return RunGenerator(compilation);

        static bool IsError(Diagnostic diagnostic)
            =>
            diagnostic.Severity is DiagnosticSeverity.Error;
    }

    private static MetadataReference CreateFromFile(string path)
        =>
        MetadataReference.CreateFromFile(path);

    private static MetadataReference CreateFromType<T>()
        =>
        MetadataReference.CreateFromFile(typeof(T).Assembly.Location);

    private static CSharpCompilation CreateCompilation(string sourceCode)
    {
        return CSharpCompilation.Create(
            assemblyName: "Endpoint.Generator.DynamicTests",
            syntaxTrees:
            [
                CSharpSyntaxTree.ParseText(sourceCode, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))
            ],
            references: MetadataReferences,
            options: new(OutputKind.DynamicallyLinkedLibrary));
    }

    private static GeneratorDriverRunResult RunGenerator(CSharpCompilation compilation)
    {

        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(CreateGenerator());
        generatorDriver = generatorDriver.RunGenerators(compilation);

        return generatorDriver.GetRunResult();
    }

    private static ISourceGenerator CreateGenerator()
    {
        var assembly = Assembly.Load("GarageGroup.Infra.Endpoint.Generator");
        var generatorType = assembly.GetType("GarageGroup.Infra.EndpointSourceGenerator", throwOnError: true)!;
        var generator = Activator.CreateInstance(generatorType, nonPublic: true)!;

        return generator switch
        {
            ISourceGenerator sourceGenerator => sourceGenerator,
            IIncrementalGenerator incrementalGenerator => incrementalGenerator.AsSourceGenerator(),
            _ => throw new InvalidOperationException($"Unsupported generator type: {generatorType.FullName}")
        };
    }

    private static string GetExceptionMessageChain(Exception exception)
    {
        var messages = new List<string>();

        for (var current = exception; current is not null; current = current.InnerException)
        {
            messages.Add(current.Message);
        }

        return string.Join(" | ", messages);
    }

    private static string NormalizeNewLines(string source)
        =>
        source.Replace("\r\n", "\n").Trim();

    private static string OrEmpty(this string? value)
        =>
        value ?? string.Empty;
}