using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FullBodyInAttribute : Attribute
{
    private const string DefaultContentType = MediaTypeNames.Application.Json;

    public FullBodyInAttribute([AllowNull] string contentType = DefaultContentType)
        =>
        ContentType = string.IsNullOrEmpty(contentType) ? DefaultContentType : contentType;

    public string ContentType { get; }
}