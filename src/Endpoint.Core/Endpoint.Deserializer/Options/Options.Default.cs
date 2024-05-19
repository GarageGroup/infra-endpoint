using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static JsonSerializerOptions CreateDeafultOptions()
        =>
        new(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            Converters =
            {
                new EndpointEnumJsonConverterFactory()
            }
        };
}