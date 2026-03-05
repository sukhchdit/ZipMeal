import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/loyalty_reward_model.dart';
import '../providers/loyalty_notifier.dart';
import '../providers/rewards_notifier.dart';
import '../providers/rewards_state.dart';

class RedeemRewardsScreen extends ConsumerWidget {
  const RedeemRewardsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final rewardsState = ref.watch(rewardsNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.loyaltyRedeemRewards),
      ),
      body: switch (rewardsState) {
        RewardsLoaded(:final rewards, :final isRedeeming) =>
          _LoadedBody(rewards: rewards, isRedeeming: isRedeeming),
        RewardsError(:final failure) => AppErrorWidget(
            message: failure.message,
            onRetry: () =>
                ref.read(rewardsNotifierProvider.notifier).loadRewards(),
          ),
        _ => const AppLoadingWidget(),
      },
    );
  }
}

class _LoadedBody extends ConsumerWidget {
  const _LoadedBody({required this.rewards, required this.isRedeeming});

  final List<LoyaltyRewardModel> rewards;
  final bool isRedeeming;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);

    if (rewards.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.redeem_outlined, size: 64, color: Colors.grey),
            const SizedBox(height: 16),
            Text(
              context.l10n.loyaltyNoRewards,
              style: theme.textTheme.bodyLarge?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
          ],
        ),
      );
    }

    return Stack(
      children: [
        GridView.builder(
          padding: const EdgeInsets.all(16),
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 2,
            mainAxisSpacing: 12,
            crossAxisSpacing: 12,
            childAspectRatio: 0.8,
          ),
          itemCount: rewards.length,
          itemBuilder: (context, index) {
            final reward = rewards[index];
            return _RewardCard(
              reward: reward,
              onRedeem: () => _handleRedeem(context, ref, reward),
            );
          },
        ),
        if (isRedeeming)
          const ColoredBox(
            color: Colors.black26,
            child: Center(child: CircularProgressIndicator()),
          ),
      ],
    );
  }

  Future<void> _handleRedeem(
    BuildContext context,
    WidgetRef ref,
    LoyaltyRewardModel reward,
  ) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(context.l10n.loyaltyRedeemConfirm),
        content: Text(
          '${reward.name}\n${context.l10n.loyaltyRewardPoints}: ${reward.pointsCost}',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: Text(context.l10n.loyaltyFilterAll),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: Text(context.l10n.loyaltyRedeemRewards),
          ),
        ],
      ),
    );

    if ((confirmed ?? false) && context.mounted) {
      final success =
          await ref.read(rewardsNotifierProvider.notifier).redeemReward(reward.id);

      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              success
                  ? context.l10n.loyaltyRedeemSuccess
                  : context.l10n.loyaltyInsufficientPoints,
            ),
          ),
        );
        if (success) {
          // Refresh dashboard
          ref.invalidate(loyaltyNotifierProvider);
        }
      }
    }
  }
}

class _RewardCard extends StatelessWidget {
  const _RewardCard({required this.reward, required this.onRedeem});

  final LoyaltyRewardModel reward;
  final VoidCallback onRedeem;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    final rewardIcon = switch (reward.rewardType) {
      0 => Icons.account_balance_wallet_rounded,
      1 => Icons.percent_rounded,
      2 => Icons.delivery_dining_rounded,
      _ => Icons.redeem_rounded,
    };

    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            CircleAvatar(
              backgroundColor: AppColors.primary.withValues(alpha: 0.12),
              child: Icon(rewardIcon, color: AppColors.primary),
            ),
            const SizedBox(height: 8),
            Text(
              reward.name,
              style: theme.textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
            if (reward.description != null) ...[
              const SizedBox(height: 4),
              Text(
                reward.description!,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
            const Spacer(),
            Row(
              children: [
                const Icon(Icons.stars_rounded, size: 16, color: Colors.amber),
                const SizedBox(width: 4),
                Text(
                  '${reward.pointsCost}',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            SizedBox(
              width: double.infinity,
              child: FilledButton(
                onPressed: onRedeem,
                style: FilledButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                ),
                child: Text(context.l10n.loyaltyRedeemRewards),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
