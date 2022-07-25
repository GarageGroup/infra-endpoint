using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<byte?, Failure<Unit>> ParseNullableByte(string? source)
        =>
        ParseNullable(source, ParseByte);

    public static Result<byte, Failure<Unit>> ParseByte(string? source)
        =>
        byte.TryParse(source, out var result) ? result : CreateFailure(source, "Byte");
}