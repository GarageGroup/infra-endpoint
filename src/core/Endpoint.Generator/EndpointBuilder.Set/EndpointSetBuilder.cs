namespace GarageGroup.Infra;

internal static partial class EndpointSetBuilder
{
    private static string GetVisibility(this EndpointSetTypeDescription type)
        =>
        type.IsTypePublic ? "public" : "internal";

    private static string GetNullValidationValue(string argumentName, bool isStructType)
        =>
        isStructType switch
        {
            true => argumentName,
            _ => $"{argumentName} ?? throw new ArgumentNullException(nameof({argumentName}))"
        };
}
