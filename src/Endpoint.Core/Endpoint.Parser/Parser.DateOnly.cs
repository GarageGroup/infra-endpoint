using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<DateOnly?, Failure<Unit>> ParseNullableDateOnly(string? source)
        =>
        ParseNullable(source, ParseDateOnly);

    public static Result<DateOnly, Failure<Unit>> ParseDateOnly(string? source)
        =>
        DateOnly.TryParse(source, InvariantFormatProvider, DefaultDateTimeStyle, out var result) ? result : CreateFailure(source, "DateOnly");
}