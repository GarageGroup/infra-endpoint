using System;
using System.Collections.Generic;

namespace GGroupp.Infra;

internal sealed record class TypeData
{
    public TypeData(IReadOnlyCollection<string> namespaces, string name)
    {
        Namespaces = namespaces ?? Array.Empty<string>();
        Name = name ?? string.Empty;
    }

    public IReadOnlyCollection<string> Namespaces { get; }

    public string Name { get; }
}