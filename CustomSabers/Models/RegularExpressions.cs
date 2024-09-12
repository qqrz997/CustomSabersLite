using System.Text.RegularExpressions;

namespace CustomSabersLite.Data;

internal class RegularExpressions
{
    public static Regex RichTextRegex { get; } = new Regex(@"<[^>]*>");
}
