namespace SwiggyClone.Infrastructure.BackgroundJobs;

internal sealed class BackgroundJobOptions
{
    public const string SectionName = "BackgroundJobs";

    public string ExpiredTokenCleanupCron { get; set; } = "0 0 2 * * ?";
    public string SubscriptionExpiryCron { get; set; } = "0 0 * * * ?";
    public string AbandonedOrderCleanupCron { get; set; } = "0 0/15 * * * ?";
    public string StalledPaymentCron { get; set; } = "0 0/10 * * * ?";
    public string ExpiredCouponDeactivationCron { get; set; } = "0 5 * * * ?";
    public string DineInSessionTimeoutCron { get; set; } = "0 0/10 * * * ?";
    public string ScheduledOrderActivationCron { get; set; } = "0 0/5 * * * ?";
    public string ExpiredPromotionDeactivationCron { get; set; } = "0 5 * * * ?";

    public int TokenRetentionDays { get; set; } = 7;
    public int AbandonedOrderMinutes { get; set; } = 60;
    public int StalledPaymentMinutes { get; set; } = 30;
    public int DineInPaymentPendingMinutes { get; set; } = 30;
}
