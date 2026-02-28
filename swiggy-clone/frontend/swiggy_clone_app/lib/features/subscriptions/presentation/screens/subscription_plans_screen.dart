import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/subscription_notifier.dart';
import '../providers/subscription_state.dart';
import '../widgets/active_subscription_card.dart';
import '../widgets/plan_card.dart';

class SubscriptionPlansScreen extends ConsumerWidget {
  const SubscriptionPlansScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final subscriptionState = ref.watch(subscriptionNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Subscription Plans')),
      body: switch (subscriptionState) {
        SubscriptionInitial() || SubscriptionLoading() =>
          const AppLoadingWidget(message: 'Loading plans...'),
        SubscriptionError(:final failure) => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.error_outline, size: 48, color: AppColors.error),
                const SizedBox(height: 12),
                Text(
                  failure.message,
                  textAlign: TextAlign.center,
                  style: theme.textTheme.bodyMedium,
                ),
                const SizedBox(height: 16),
                FilledButton.icon(
                  onPressed: () => ref
                      .read(subscriptionNotifierProvider.notifier)
                      .loadData(),
                  icon: const Icon(Icons.refresh),
                  label: const Text('Retry'),
                ),
              ],
            ),
          ),
        SubscriptionLoaded(
          :final plans,
          :final activeSubscription,
          :final isSubscribing,
          :final isCancelling,
        ) =>
          RefreshIndicator(
            onRefresh: () =>
                ref.read(subscriptionNotifierProvider.notifier).loadData(),
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                // Active subscription card
                if (activeSubscription != null) ...[
                  Text(
                    'Your Subscription',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),
                  ActiveSubscriptionCard(
                    subscription: activeSubscription,
                    isCancelling: isCancelling,
                    onCancel: () async {
                      final success = await ref
                          .read(subscriptionNotifierProvider.notifier)
                          .cancelSubscription();
                      if (context.mounted) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            content: Text(success
                                ? 'Subscription cancelled'
                                : 'Failed to cancel subscription'),
                          ),
                        );
                      }
                    },
                  ),
                  const SizedBox(height: 8),
                ],

                // Available plans
                Text(
                  'Available Plans',
                  style: theme.textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 12),
                if (plans.isEmpty)
                  Center(
                    child: Padding(
                      padding: const EdgeInsets.all(32),
                      child: Text(
                        'No plans available right now',
                        style: theme.textTheme.bodyLarge?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ),
                  )
                else
                  ...plans.map(
                    (plan) => PlanCard(
                      plan: plan,
                      hasActiveSub: activeSubscription != null,
                      isSubscribing: isSubscribing,
                      onSubscribe: () async {
                        final success = await ref
                            .read(subscriptionNotifierProvider.notifier)
                            .subscribe(plan.id);
                        if (context.mounted) {
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text(success
                                  ? 'Subscribed to ${plan.name}!'
                                  : 'Failed to subscribe'),
                            ),
                          );
                        }
                      },
                    ),
                  ),
              ],
            ),
          ),
      },
    );
  }
}
