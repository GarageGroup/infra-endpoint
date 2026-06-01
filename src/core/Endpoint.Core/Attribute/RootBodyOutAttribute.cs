using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RootBodyOutAttribute([AllowNull] string contentType = RootBodyOutAttribute.DefaultContentType) : Attribute
{
    private const string DefaultContentType = MediaTypeNames.Application.Json;

    public string ContentType { get; } = string.IsNullOrEmpty(contentType) ? DefaultContentType : contentType;
}