using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToFailureResponse(this EndpointProblem problem, JsonSerializerOptions? jsonSerializerOptions)
        =>
        new(
            statusCode: problem.Status,
            headers: problemJsonHeaders,
            body: problem.ToJsonStream(jsonSerializerOptions));
}