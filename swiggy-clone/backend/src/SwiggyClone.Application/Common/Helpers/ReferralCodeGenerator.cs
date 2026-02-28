using System.Security.Cryptography;

namespace SwiggyClone.Application.Common.Helpers;

public static class ReferralCodeGenerator
{
    private const string AlphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CodeLength = 8;
    private const int PrefixLength = 3;

    /// <summary>
    /// Generates an 8-character referral code: first 3 letters from the user's name
    /// (uppercase, padded with random chars if name is too short) + 5 random alphanumeric chars.
    /// </summary>
    public static string Generate(string fullName)
    {
        Span<char> code = stackalloc char[CodeLength];

        var cleaned = fullName.Where(char.IsLetter).Take(PrefixLength).ToArray();
        for (var i = 0; i < PrefixLength; i++)
        {
            code[i] = i < cleaned.Length
                ? char.ToUpperInvariant(cleaned[i])
                : AlphaNumeric[RandomNumberGenerator.GetInt32(26)];
        }

        for (var i = PrefixLength; i < CodeLength; i++)
        {
            code[i] = AlphaNumeric[RandomNumberGenerator.GetInt32(AlphaNumeric.Length)];
        }

        return new string(code);
    }
}
