using System;
using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToBadRequestResponse(this Failure<Unit> failure, JsonSerializerOptions? jsonSerializerOptions)
        =>
        new(
            statusCode: 400,
            headers: problemJsonHeaders,
            body: failure.ToBadRequestProblem().SerializeToStream(jsonSerializerOptions));

    private static EndpointProblem ToBadRequestProblem(this Failure<Unit> failure)
        =>
        new(
            type: "BadRequest",
            title: default,
            status: FailureStatusCode.BadRequest,
            detail: failure.FailureMessage);
}