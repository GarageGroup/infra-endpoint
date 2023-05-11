using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

public interface IOpenApiSchemaProvider
{
    static abstract OpenApiSchema GetSchema(bool nullable, IOpenApiAny? example = null, string? description = null);
}