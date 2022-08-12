using System;

namespace GGroupp.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<float?, Failure<Unit>> ParseNullableSingle(string? source)
        =>
        ParseNullable(source, ParseSingle);

    public static Result<float, Failure<Unit>> ParseSingle(string? source)
        =>
        float.TryParse(source, FloatNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Single");
}