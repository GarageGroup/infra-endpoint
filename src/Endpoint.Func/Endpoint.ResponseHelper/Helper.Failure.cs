using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static EndpointResponse ToFailureResponse(this EndpointProblem problem, JsonSerializerOptions? jsonSerializerOptions)
        =>
        new(
            statusCode: (int)problem.Status,
            headers: problemJsonHeaders,
            body: problem.SerializeToStream(jsonSerializerOptions));
}