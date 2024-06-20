using System.Reflection;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities;

internal class ResourceLoading
{
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    public static async Task<byte[]> LoadFromResourceAsync(string resourcePath) =>
        await GetResourceAsync(assembly, resourcePath);

    private static async Task<byte[]> GetResourceAsync(Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        var data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, (int)stream.Length);
        return data;
    }
}
