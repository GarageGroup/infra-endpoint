using System.Text.Json;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static JsonSerializerOptions CreateDeafultOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true
        };

        options.Converters.Add(new EndpointEnumJsonConverterFactory());
        return options;
    }
}