using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<TimeSpan?, Failure<Unit>> ParseNullableTimeSpan(string? source)
        =>
        ParseNullable(source, ParseTimeSpan);

    public static Result<TimeSpan, Failure<Unit>> ParseTimeSpan(string? source)
        =>
        TimeSpan.TryParse(source, InvariantFormatProvider, out var result) ? result : CreateFailure(source, "TimeSpan");
}