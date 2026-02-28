using System.Globalization;

namespace SwiggyClone.Application.Common.Helpers;

public static class DateBucketHelper
{
    public static DateTimeOffset GetCutoff(int days) =>
        DateTimeOffset.UtcNow.AddDays(-days);

    public static string FormatDailyLabel(int year, int month, int day) =>
        new DateOnly(year, month, day).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string FormatWeeklyLabel(int year, int weekIndex) =>
        $"{year}-W{weekIndex + 1:D2}";

    public static string FormatMonthlyLabel(int year, int month) =>
        $"{year}-{month:D2}";
}
