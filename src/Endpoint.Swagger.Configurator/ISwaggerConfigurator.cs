using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

public interface ISwaggerConfigurator
{
    static abstract void Configure(OpenApiDocument openApiDocument);
}