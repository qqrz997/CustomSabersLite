﻿using System;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Models;

internal record RichTextSegment : IComparable<RichTextSegment>
{
    public string FullName { get; }
    private string Value { get; }

    private RichTextSegment(string fullName, string comparableValue)
    {
        FullName = fullName;
        Value = comparableValue;
    }

    private static RichTextSegment Unknown { get; } = new("Unknown", "_");

    public static RichTextSegment Create(string? fullText)
    {
        if (fullText is null) return Unknown;

        string trimmed = fullText.Trim();
        if (trimmed.Length == 0) return Unknown;

        string replaced = RegularExpressions.RichTextRegex.Replace(trimmed, string.Empty);
        if (replaced.Length == 0) return Unknown;

        return new(fullText, replaced);
    }

    public int CompareTo(RichTextSegment other) => string.Compare(Value, other.Value, StringComparison.Ordinal);
    
    public bool Contains(string value, StringComparison comparison = StringComparison.CurrentCulture) => 
        Value.Contains(value, comparison);
}