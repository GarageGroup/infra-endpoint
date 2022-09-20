using System.Text.Json.Serialization;

namespace GGroupp.Infra;

internal sealed partial class Base64StringJsonConverter : JsonConverter<Base64String>
{
}