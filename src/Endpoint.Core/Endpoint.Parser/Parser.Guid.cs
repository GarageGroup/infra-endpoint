using System;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointParser
{
    public static Result<Guid?, Failure<Unit>> ParseNullableGuid(string? source)
        =>
        ParseNullable(source, ParseGuid);

    public static Result<Guid, Failure<Unit>> ParseGuid(string? source)
        =>
        Guid.TryParse(source, out var result) ? result : CreateFailure(source, "Guid");
}