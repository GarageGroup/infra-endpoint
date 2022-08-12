using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<decimal?, Failure<Unit>> ParseNullableDecimal(string? source)
        =>
        ParseNullable(source, ParseDecimal);

    public static Result<decimal, Failure<Unit>> ParseDecimal(string? source)
        =>
        decimal.TryParse(source, DecimalNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Decimal");
}