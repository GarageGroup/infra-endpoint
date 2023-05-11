using System;
using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToBadRequestResponse(this Failure<Unit> failure, JsonSerializerOptions? jsonSerializerOptions)
        =>
        new(
            statusCode: BadRequestStatusCode,
            headers: problemJsonHeaders,
            body: failure.ToBadRequestProblem().ToJsonStream(jsonSerializerOptions));

    private static EndpointProblem ToBadRequestProblem(this Failure<Unit> failure)
        =>
        new(
            type: "BadRequest",
            title: default,
            status: BadRequestStatusCode,
            detail: failure.FailureMessage);
}