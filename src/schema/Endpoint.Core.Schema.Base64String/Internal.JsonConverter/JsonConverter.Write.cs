using System.Text.Json;

namespace GGroupp.Infra;

partial class Base64StringJsonConverter
{
    public override void Write(Utf8JsonWriter writer, Base64String value, JsonSerializerOptions options)
        =>
        writer.WriteStringValue(value.Value);
}