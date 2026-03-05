using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

internal static class BackgroundJobsExtensions
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new BackgroundJobOptions();
        configuration.GetSection(BackgroundJobOptions.SectionName).Bind(options);

        services.Configure<BackgroundJobOptions>(
            configuration.GetSection(BackgroundJobOptions.SectionName));

        services.AddQuartz(q =>
        {
            AddJob<ExpiredTokenCleanupJob>(q, "expired-token-cleanup", options.ExpiredTokenCleanupCron);
            AddJob<SubscriptionExpiryJob>(q, "subscription-expiry", options.SubscriptionExpiryCron);
            AddJob<AbandonedOrderCleanupJob>(q, "abandoned-order-cleanup", options.AbandonedOrderCleanupCron);
            AddJob<StalledPaymentJob>(q, "stalled-payment", options.StalledPaymentCron);
            AddJob<ExpiredCouponDeactivationJob>(q, "expired-coupon-deactivation", options.ExpiredCouponDeactivationCron);
            AddJob<DineInSessionTimeoutJob>(q, "dinein-session-timeout", options.DineInSessionTimeoutCron);
            AddJob<ScheduledOrderActivationJob>(q, "scheduled-order-activation", options.ScheduledOrderActivationCron);
            AddJob<ExpiredPromotionDeactivationJob>(q, "expired-promotion-deactivation", options.ExpiredPromotionDeactivationCron);
            AddJob<ExpireGroupOrdersJob>(q, "expire-group-orders", options.ExpireGroupOrdersCron);
            AddJob<LoyaltyPointsExpiryJob>(q, "loyalty-points-expiry", options.LoyaltyPointsExpiryCron);
            AddJob<EscalateDisputesJob>(q, "escalate-disputes", options.EscalateDisputesCron);
            AddJob<PrecomputeRecommendationsJob>(q, "precompute-recommendations", options.PrecomputeRecommendationsCron);
            AddJob<CleanupInteractionsJob>(q, "cleanup-interactions", options.CleanupInteractionsCron);
            AddJob<AutoCompleteExpiredExperimentsJob>(q, "auto-complete-experiments", options.AutoCompleteExpiredExperimentsCron);
            AddJob<ComputeExperimentStatsJob>(q, "compute-experiment-stats", options.ComputeExperimentStatsCron);
        });

        services.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

        return services;
    }

    private static void AddJob<TJob>(IServiceCollectionQuartzConfigurator q, string name, string cron)
        where TJob : IJob
    {
        var jobKey = new JobKey(name);
        q.AddJob<TJob>(opts => opts.WithIdentity(jobKey));
        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{name}-trigger")
            .WithCronSchedule(cron));
    }
}
