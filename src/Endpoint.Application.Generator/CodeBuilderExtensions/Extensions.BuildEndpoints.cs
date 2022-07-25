using System.Collections.Generic;
using System.Text;

namespace GGroupp.Infra;

partial class CodeBuilderExtensions
{
    internal static string BuildEndpointsCode(this IReadOnlyCollection<string> endpointTypeNames, string tab)
    {
        if (endpointTypeNames.Count is not > 0)
        {
            return new StringBuilder().AppendLine().Append(tab).Append("yield break;").ToString();
        }

        var codeBuilder = new StringBuilder();

        foreach (var endpointTypeName in endpointTypeNames)
        {
            codeBuilder.AppendLine().Append(tab).Append("yield return").Append(' ').Append(endpointTypeName).Append(".GetEndpointMetadata();");
        }

        return codeBuilder.ToString();
    }
}