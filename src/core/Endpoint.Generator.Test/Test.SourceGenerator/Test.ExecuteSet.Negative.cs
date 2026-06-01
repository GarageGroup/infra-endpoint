using System;
using System.Linq;
using Xunit;

namespace GarageGroup.Infra.Endpoint.Generator.Test;

partial class EndpointSourceGeneratorTest
{
    [Fact]
    public static void ExecuteSet_GenericEndpointSetType_ThrowsNotSupportedException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.GenericEndpointSetTypeSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;

        Assert.Contains(
            "Generic endpoint set types are not supported",
            GetExceptionMessageChain(exception),
            StringComparison.Ordinal);
    }

    [Fact]
    public static void ExecuteSet_EndpointSetWithoutEndpointFunctions_ThrowsInvalidOperationException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.EndpointSetWithoutEndpointFunctionsSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;

        Assert.Contains(
            "Endpoint set type IProductApi must inherit at least one interface with GarageGroup.Infra.EndpointAttribute.",
            GetExceptionMessageChain(exception),
            StringComparison.Ordinal);
    }

    [Fact]
    public static void ExecuteSet_EndpointSetWithDuplicateOperationIds_ThrowsInvalidOperationException()
    {
        var runResult = RunGenerator(EndpointSourceGeneratorData.EndpointSetWithDuplicateOperationIdsSourceCode);
        var generatorResult = runResult.Results.Single();

        Assert.NotNull(generatorResult.Exception);
        var exception = generatorResult.Exception!;

        Assert.Contains(
            "Endpoint set type IProductApi contains duplicate endpoint operationId: ProductGet.",
            GetExceptionMessageChain(exception),
            StringComparison.Ordinal);
    }
}
