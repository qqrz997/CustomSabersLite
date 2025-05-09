using System;
using static CustomSabersLite.Utilities.Common.RegularExpressions;

namespace CustomSabersLite.Models;

internal record RichTextString : IComparable<RichTextString>
{
    public string FullName { get; }
    private string Value { get; }
    
    private RichTextString(string fullName, string comparableValue)
    {
        FullName = fullName;
        Value = comparableValue;
    }

    public static RichTextString Unknown { get; } = new("", "_");
    public static RichTextString Empty { get; } = new(string.Empty, string.Empty);
    
    public static RichTextString Create(string? fullText)
    {
        if (fullText is null) return Unknown;

        string trimmed = fullText.Trim();
        if (trimmed.Length == 0) return Unknown;

        string replaced = RichTextRegex.Replace(trimmed, string.Empty);
        if (replaced.Length == 0) return Unknown;

        return new(fullText, replaced);
    }

    public int CompareTo(RichTextString other) => 
        string.Compare(Value, other.Value, StringComparison.Ordinal);
    
    public bool Contains(string value, StringComparison comparison = StringComparison.CurrentCulture) => 
        Value.Contains(value, comparison);
}