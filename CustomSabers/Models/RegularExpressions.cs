using System.Text.RegularExpressions;

namespace CustomSabersLite.Models;

internal class RegularExpressions
{
    public static Regex RichTextRegex { get; } = new Regex(@"<[^>]*>");
}
