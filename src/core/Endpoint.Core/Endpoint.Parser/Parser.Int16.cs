using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<short?, Failure<Unit>> ParseNullableInt16(string? source)
        =>
        ParseNullable(source, ParseInt16);

    public static Result<short, Failure<Unit>> ParseInt16(string? source)
        =>
        short.TryParse(source, IntegerNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Int16");
}