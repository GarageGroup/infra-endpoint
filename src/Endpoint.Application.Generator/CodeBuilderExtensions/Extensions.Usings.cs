using System.Collections.Generic;
using System.Text;

namespace GGroupp.Infra;

partial class CodeBuilderExtensions
{
    internal static StringBuilder AppendUsings(this StringBuilder codeBuilder, IReadOnlyCollection<string> namespaces)
    {
        if (namespaces.Count is not > 0)
        {
            return codeBuilder;
        }

        foreach (var @namespace in namespaces)
        {
            codeBuilder.AppendLine().Append("using").Append(' ').Append(@namespace).Append(';');
        }

        return codeBuilder;
    }
}