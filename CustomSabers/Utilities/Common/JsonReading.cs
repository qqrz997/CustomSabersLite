using Newtonsoft.Json;
using System.IO;

namespace CustomSabersLite.Utilities;

internal class JsonReading
{
    public static T? DeserializeStream<T>(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var jsonTextReader = new JsonTextReader(streamReader);
        T? obj = new JsonSerializer().Deserialize<T>(jsonTextReader);
        return obj;
    }
}
