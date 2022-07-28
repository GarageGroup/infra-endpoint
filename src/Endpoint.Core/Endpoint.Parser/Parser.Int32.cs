using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<int?, Failure<Unit>> ParseNullableInt32(string? source)
        =>
        ParseNullable(source, ParseInt32);

    public static Result<int, Failure<Unit>> ParseInt32(string? source)
        =>
        int.TryParse(source, out var result) ? result : CreateFailure(source, "Int32");
}