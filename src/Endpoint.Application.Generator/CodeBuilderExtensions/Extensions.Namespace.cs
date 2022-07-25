using System.Text;

namespace GGroupp.Infra;

partial class CodeBuilderExtensions
{
    internal static StringBuilder AppendNamespace(this StringBuilder codeBuilder, string @namespace)
        =>
        codeBuilder.AppendLine().AppendLine().Append("namespace").Append(' ').Append(@namespace).Append(';');
}