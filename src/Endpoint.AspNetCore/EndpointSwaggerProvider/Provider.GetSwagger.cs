using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra.Endpoint;

partial class EndpointSwaggerProvider
{
    public OpenApiDocument GetSwagger(string documentName, string? host = null, string? basePath = null)
        =>
        new()
        {
            Info = new()
            {
                Title = GetDocumentTitle(documentName),
                Version = string.IsNullOrEmpty(option?.ApiVersion) ? template.Info?.Version : option.ApiVersion,
                Description = string.IsNullOrEmpty(option?.Description) ? template.Info?.Description : option.Description
            },
            Workspace = template.Workspace,
            Servers = template.Servers,
            Paths = template.Paths,
            Components = template.Components,
            SecurityRequirements = template.SecurityRequirements,
            Tags = template.Tags,
            ExternalDocs = template.ExternalDocs,
            Extensions = template.Extensions
        };

    private string GetDocumentTitle([AllowNull] string documentName)
    {
        if (string.IsNullOrEmpty(option?.ApiName) is false)
        {
            return option.ApiName;
        }

        if (string.IsNullOrEmpty(documentName) is false)
        {
            return documentName;
        }

        return DefaultDocumentTitle;
    }
}