namespace GGroupp.Infra;

partial class SourceBuilder
{
    public SourceBuilder BeginArguments()
    {
        tabNumber++;
        return this;
    }
}