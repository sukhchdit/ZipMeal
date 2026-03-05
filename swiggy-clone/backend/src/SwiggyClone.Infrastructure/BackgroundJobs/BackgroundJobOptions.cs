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
    public string ExpireGroupOrdersCron { get; set; } = "0 0/5 * * * ?";
    public string LoyaltyPointsExpiryCron { get; set; } = "0 0 3 * * ?";
    public int LoyaltyInactivityDays { get; set; } = 180;
    public string EscalateDisputesCron { get; set; } = "0 0/30 * * * ?";
    public int DisputeEscalationHours { get; set; } = 48;
    public string PrecomputeRecommendationsCron { get; set; } = "0 0 0/4 * * ?";
    public string CleanupInteractionsCron { get; set; } = "0 0 4 * * ?";
    public int InteractionRetentionDays { get; set; } = 90;
    public string AutoCompleteExpiredExperimentsCron { get; set; } = "0 0 * * * ?";
    public string ComputeExperimentStatsCron { get; set; } = "0 0/30 * * * ?";
}
