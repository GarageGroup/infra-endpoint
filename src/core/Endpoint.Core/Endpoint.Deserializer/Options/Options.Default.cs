using System.Text.Json;

namespace GGroupp.Infra.Endpoint;

partial class EndpointDeserializer
{
    public static JsonSerializerOptions CreateDeafultOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new EndpointEnumJsonConverterFactory());

        return options;
    }
}