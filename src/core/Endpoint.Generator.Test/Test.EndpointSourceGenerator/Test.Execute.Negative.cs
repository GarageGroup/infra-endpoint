using System;
using System.Linq;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void Execute_GenericEndpointType_ThrowsNotSupportedException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.GenericEndpointTypeSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;
        Assert.Contains("Generic types are not supported", GetExceptionMessageChain(exception), StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_EndpointWithoutMethod_ThrowsInvalidOperationException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.EndpointWithoutMethodSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;
        Assert.Contains("An endpoint method was not found in the type IInvalidFunc", GetExceptionMessageChain(exception), StringComparison.Ordinal);
    }

    [Fact]
    public static void Execute_EndpointWithWhiteSpaceName_ThrowsInvalidOperationException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.EndpointWithWhiteSpaceNameSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;
        Assert.Contains(
            "Endpoint operationId for type IInvalidNameFunc must be specified and cannot be null or whitespace.",
            GetExceptionMessageChain(exception),
            StringComparison.Ordinal);
    }
}