using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<double?, Failure<Unit>> ParseNullableDouble(string? source)
        =>
        ParseNullable(source, ParseDouble);

    public static Result<double, Failure<Unit>> ParseDouble(string? source)
        =>
        double.TryParse(source, FloatNumberStyle, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "Double");
}