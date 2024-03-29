using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<byte?, Failure<Unit>> ParseNullableByte(string? source)
        =>
        ParseNullable(source, ParseByte);

    public static Result<byte, Failure<Unit>> ParseByte(string? source)
        =>
        byte.TryParse(source, IntegerNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Byte");
}