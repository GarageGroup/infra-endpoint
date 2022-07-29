using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<DateTimeOffset?, Failure<Unit>> ParseNullableDateTimeOffset(string? source)
        =>
        ParseNullable(source, ParseDateTimeOffset);

    public static Result<DateTimeOffset, Failure<Unit>> ParseDateTimeOffset(string? source)
        =>
        DateTimeOffset.TryParse(source, InvariantFormatProvider, DefaultDateTimeStyle, out var result)
            ? result
            : CreateFailure(source, "DateTimeOffset");
}