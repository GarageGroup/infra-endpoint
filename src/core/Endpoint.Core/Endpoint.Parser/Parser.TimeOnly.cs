using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<TimeOnly?, Failure<Unit>> ParseNullableTimeOnly(string? source)
        =>
        ParseNullable(source, ParseTimeOnly);

    public static Result<TimeOnly, Failure<Unit>> ParseTimeOnly(string? source)
        =>
        TimeOnly.TryParse(source, InvariantFormatProvider, DefaultDateTimeStyle, out var result) ? result : CreateFailure(source, "TimeOnly");
}