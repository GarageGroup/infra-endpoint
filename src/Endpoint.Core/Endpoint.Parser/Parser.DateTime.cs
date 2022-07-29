using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<DateTime?, Failure<Unit>> ParseNullableDateTime(string? source)
        =>
        ParseNullable(source, ParseDateTime);

    public static Result<DateTime, Failure<Unit>> ParseDateTime(string? source)
        =>
        DateTime.TryParse(source, InvariantFormatProvider, DefaultDateTimeStyle, out var result) ? result : CreateFailure(source, "DateTime");
}