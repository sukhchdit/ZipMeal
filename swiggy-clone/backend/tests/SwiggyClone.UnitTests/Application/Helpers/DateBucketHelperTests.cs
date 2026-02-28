using FluentAssertions;
using SwiggyClone.Application.Common.Helpers;

namespace SwiggyClone.UnitTests.Application.Helpers;

public sealed class DateBucketHelperTests
{
    [Fact]
    public void GetCutoff_ReturnsDateInPast()
    {
        var cutoff = DateBucketHelper.GetCutoff(7);

        cutoff.Should().BeBefore(DateTimeOffset.UtcNow);
        cutoff.Should().BeCloseTo(DateTimeOffset.UtcNow.AddDays(-7), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void FormatDailyLabel_FormatsCorrectly()
    {
        DateBucketHelper.FormatDailyLabel(2026, 2, 28).Should().Be("2026-02-28");
    }

    [Fact]
    public void FormatDailyLabel_PadsMonthAndDay()
    {
        DateBucketHelper.FormatDailyLabel(2026, 1, 5).Should().Be("2026-01-05");
    }

    [Fact]
    public void FormatWeeklyLabel_FormatsCorrectly()
    {
        DateBucketHelper.FormatWeeklyLabel(2026, 0).Should().Be("2026-W01");
    }

    [Fact]
    public void FormatWeeklyLabel_PadsWeekNumber()
    {
        DateBucketHelper.FormatWeeklyLabel(2026, 9).Should().Be("2026-W10");
    }

    [Fact]
    public void FormatMonthlyLabel_FormatsCorrectly()
    {
        DateBucketHelper.FormatMonthlyLabel(2026, 2).Should().Be("2026-02");
    }
}
