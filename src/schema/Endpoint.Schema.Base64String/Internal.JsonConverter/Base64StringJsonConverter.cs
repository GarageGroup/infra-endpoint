using System.Text.Json.Serialization;

namespace GarageGroup.Infra;

internal sealed partial class Base64StringJsonConverter : JsonConverter<Base64String>
{
}