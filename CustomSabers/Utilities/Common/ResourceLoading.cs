using System.Reflection;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities;

internal class ResourceLoading
{
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    public static byte[] GetResource(string resourcePath) =>
        GetResource(assembly, resourcePath);

    public static async Task<byte[]> GetResourceAsync(string resourcePath) =>
        await GetResourceAsync(assembly, resourcePath);

    private static byte[] GetResource(Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        byte[]? data = new byte[stream.Length];
        stream.Read(data, 0, (int)stream.Length);
        return data;
    }

    private static async Task<byte[]> GetResourceAsync(Assembly assembly, string resourcePath)
    {
        using var stream = assembly.GetManifestResourceStream(resourcePath);
        byte[]? data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, (int)stream.Length);
        return data;
    }
}
