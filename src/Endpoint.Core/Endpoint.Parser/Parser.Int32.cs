using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<int?, Failure<Unit>> ParseNullableInt32(string? source)
        =>
        ParseNullable(source, ParseInt32);

    public static Result<int, Failure<Unit>> ParseInt32(string? source)
        =>
        int.TryParse(source, IntegerNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Int32");
}