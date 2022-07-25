namespace GGroupp.Infra;

partial class SourceBuilder
{
    public SourceBuilder AppendCodeLine(string codeLine, params string[] codeLines)
    {
        if (codeBuilder.Length > 0)
        {
            _ = codeBuilder.AppendLine();
        }

        _ = codeBuilder.AppendTab(tabNumber).Append(codeLine);

        foreach (var line in codeLines)
        {
            _ = codeBuilder.AppendLine().AppendTab(tabNumber).Append(line);
        }

        return this;
    }
}