using System.Net;
using System.Text.RegularExpressions;

namespace SwiggyClone.Application.Common.Helpers;

/// <summary>
/// Static utility to strip HTML tags from user-supplied strings.
/// Uses source-generated regex for zero-allocation matching.
/// Performs a double-pass to defend against double-encoded payloads.
/// </summary>
public static partial class HtmlSanitizer
{
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // First pass: strip HTML tags
        var result = HtmlTagRegex().Replace(input, string.Empty);

        // Decode HTML entities (e.g., &lt;script&gt;)
        result = WebUtility.HtmlDecode(result);

        // Second pass: strip any tags that were hidden in encoded form
        result = HtmlTagRegex().Replace(result, string.Empty);

        return result.Trim();
    }

    [GeneratedRegex("<[^>]*>")]
    private static partial Regex HtmlTagRegex();
}
