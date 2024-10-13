using System.Text.RegularExpressions;

namespace CustomSabersLite.Utilities;

internal class RegularExpressions
{
    public static Regex RichTextRegex { get; } = new Regex(@"<[^>]*>");
}
