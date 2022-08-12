using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

public interface IOpenApiSchemaProvider
{
    static abstract OpenApiSchema GetSchema(bool nullable, IOpenApiAny? example = null);
}