using System.Text.RegularExpressions;

namespace CustomSabersLite.Utilities.Common;

internal class RegularExpressions
{
    public static Regex RichTextRegex { get; } = new Regex(@"<[^>]*>");
}
