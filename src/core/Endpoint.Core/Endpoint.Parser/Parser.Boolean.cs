using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<bool?, Failure<Unit>> ParseNullableBoolean(string? source)
        =>
        ParseNullable(source, ParseBoolean);

    public static Result<bool, Failure<Unit>> ParseBoolean(string? source)
        =>
        bool.TryParse(source, out var result) ? result : CreateFailure(source, "Boolean");
}