using System.IO;
using System.Xml.Serialization;

namespace GarageGroup.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static Stream? ToXmlStream<T>(this T? value)
    {
        var stream = new MemoryStream();

        var xmlSerializer = new XmlSerializer(typeof(T));
        xmlSerializer.Serialize(stream, value);

        stream.Position = 0;
        return stream;
    }
}