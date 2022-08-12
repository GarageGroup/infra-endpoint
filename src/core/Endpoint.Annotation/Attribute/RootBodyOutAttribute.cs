using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RootBodyOutAttribute : Attribute
{
    private const string DefaultContentType = MediaTypeNames.Application.Json;

    public RootBodyOutAttribute([AllowNull] string contentType = DefaultContentType)
        =>
        ContentType = string.IsNullOrEmpty(contentType) ? DefaultContentType : contentType;

    public string ContentType { get; }
}