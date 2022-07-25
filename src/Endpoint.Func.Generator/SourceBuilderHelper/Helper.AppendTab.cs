using System.Text;

namespace GGroupp.Infra;

partial class SourceBuilderHelper
{
    internal static StringBuilder AppendTab(this StringBuilder builder, int tabNumber)
    {
        if (tabNumber is not > 0)
        {
            return builder;
        }

        var tab = new string(' ', TabInterval * tabNumber);
        return builder.Append(tab);
    }
}