using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SwiggyClone.Application.Common.Helpers;

public static partial class SlugHelper
{
    public static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant().Trim();
        slug = RemoveDiacritics(slug);
        slug = NonAlphanumericRegex().Replace(slug, "-");
        slug = MultipleDashRegex().Replace(slug, "-");
        slug = slug.Trim('-');
        return slug;
    }

    public static string AppendSuffix(string slug, int suffix) =>
        $"{slug}-{suffix}";

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"[-\s]+")]
    private static partial Regex MultipleDashRegex();
}
