using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CustomSabersLite.Utilities;

internal class Hashing
{
    public static string? MD5Checksum(string filePath, string format)
    {
        using var fileStream = File.OpenRead(filePath);
        return MD5Checksum(fileStream, format);
    }

    public static string? MD5Checksum(Stream stream, string format) =>
        stream.Length == 0 || !stream.CanRead || !stream.CanSeek ? null
        : GetMD5String(stream, format);

    private static string? GetMD5String(Stream stream, string format)
    {
        using var md5 = MD5.Create();
        md5.ComputeHash(stream);
        return HashToString(md5.Hash, format) switch
        {
            { Length: > 0 } hashString => hashString,
            _ => null
        };
    }

    public static string HashToString(byte[] hashBytes, string format)
    {
        var sb = new StringBuilder();
        hashBytes.Select(b => b.ToString(format))
            .ForEach(s => sb.Append(s));
        return sb.ToString();
    }
}
