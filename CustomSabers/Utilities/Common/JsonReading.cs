using System.IO;
using Newtonsoft.Json;

namespace CustomSabersLite.Utilities.Common;

internal class JsonReading
{
    public static T? DeserializeStream<T>(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);
        return new JsonSerializer().Deserialize<T>(jsonTextReader);
    }
}
