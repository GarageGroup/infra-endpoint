using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<long?, Failure<Unit>> ParseNullableInt64(string? source)
        =>
        ParseNullable(source, ParseInt64);

    public static Result<long, Failure<Unit>> ParseInt64(string? source)
        =>
        long.TryParse(source, out var result) ? result : CreateFailure(source, "Int64");
}