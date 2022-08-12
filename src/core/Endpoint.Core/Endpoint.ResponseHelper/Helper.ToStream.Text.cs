using System.IO;
using System.Text;

namespace GGroupp.Infra.Endpoint;

partial class EndpointResponseHelper
{
    public static Stream? ToTextStream<T>(this T? value)
    {
        var text = value?.ToString();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return new MemoryStream(Encoding.UTF8.GetBytes(text));
    }
}