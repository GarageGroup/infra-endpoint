using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace GGroupp.Infra.Endpoint;

public sealed partial class EndpointSwaggerProvider : ISwaggerProvider
{
    public static EndpointSwaggerProvider Create(OpenApiDocument template!!, SwaggerOption? option)
        =>
        new(template, option);

    private const string DefaultDocumentTitle = "HTTP Endpoints API";

    private readonly OpenApiDocument template;

    private readonly SwaggerOption? option;

    private EndpointSwaggerProvider(OpenApiDocument template, SwaggerOption? option)
    {
        this.template = template;
        this.option = option;
    }
}