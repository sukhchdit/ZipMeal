import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/loyalty_dashboard_model.dart';
import '../providers/loyalty_notifier.dart';
import '../providers/loyalty_state.dart';

class LoyaltyDashboardScreen extends ConsumerWidget {
  const LoyaltyDashboardScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final loyaltyState = ref.watch(loyaltyNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.loyaltyRewards),
      ),
      body: switch (loyaltyState) {
        LoyaltyLoaded(:final dashboard) => _LoadedBody(dashboard: dashboard),
        LoyaltyError(:final failure) => AppErrorWidget(
            message: failure.message,
            onRetry: () =>
                ref.read(loyaltyNotifierProvider.notifier).loadDashboard(),
          ),
        _ => const AppLoadingWidget(),
      },
    );
  }
}

class _LoadedBody extends StatelessWidget {
  const _LoadedBody({required this.dashboard});

  final LoyaltyDashboardModel dashboard;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Tier card
        _TierCard(dashboard: dashboard),
        const SizedBox(height: 16),

        // Progress bar to next tier
        if (dashboard.nextTier != null) ...[
          _TierProgressBar(dashboard: dashboard),
          const SizedBox(height: 16),
        ],

        // Stats row
        Row(
          children: [
            Expanded(
              child: _StatTile(
                icon: Icons.star_rounded,
                label: context.l10n.loyaltyLifetimeEarned,
                value: '${dashboard.lifetimePointsEarned}',
                color: Colors.amber,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: _StatTile(
                icon: Icons.account_balance_wallet_rounded,
                label: context.l10n.loyaltyBalance,
                value: '${dashboard.pointsBalance}',
                color: AppColors.primary,
              ),
            ),
          ],
        ),
        const SizedBox(height: 20),

        // Redeem CTA
        FilledButton.icon(
          onPressed: () => context.push(RouteNames.loyaltyRewards),
          icon: const Icon(Icons.redeem_rounded),
          label: Text(context.l10n.loyaltyRedeemRewards),
          style: FilledButton.styleFrom(
            minimumSize: const Size.fromHeight(52),
          ),
        ),
        const SizedBox(height: 24),

        // Recent activity
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              context.l10n.loyaltyRecentActivity,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            TextButton(
              onPressed: () => context.push(RouteNames.loyaltyHistory),
              child: Text(context.l10n.loyaltyPointsHistory),
            ),
          ],
        ),
        const SizedBox(height: 8),

        if (dashboard.recentTransactions.isEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 32),
            child: Center(
              child: Text(
                context.l10n.loyaltyNoTransactions,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
              ),
            ),
          )
        else
          ...dashboard.recentTransactions.map(
            (txn) => _TransactionTile(transaction: txn),
          ),
      ],
    );
  }
}

class _TierCard extends StatelessWidget {
  const _TierCard({required this.dashboard});

  final LoyaltyDashboardModel dashboard;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = _tierGradient(dashboard.currentTier.level);

    return Container(
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: colors,
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: colors.first.withValues(alpha: 0.4),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                _tierIcon(dashboard.currentTier.level),
                color: Colors.white,
                size: 32,
              ),
              const SizedBox(width: 12),
              Text(
                dashboard.currentTier.name,
                style: theme.textTheme.headlineSmall?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const Spacer(),
              Text(
                '${dashboard.currentTier.multiplier}x',
                style: theme.textTheme.titleLarge?.copyWith(
                  color: Colors.white.withValues(alpha: 0.9),
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Text(
            '${dashboard.pointsBalance}',
            style: theme.textTheme.displaySmall?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            context.l10n.loyaltyPoints,
            style: theme.textTheme.bodyLarge?.copyWith(
              color: Colors.white.withValues(alpha: 0.85),
            ),
          ),
        ],
      ),
    );
  }

  List<Color> _tierGradient(int level) => switch (level) {
        0 => const [Color(0xFFCD7F32), Color(0xFFA0522D)],
        1 => const [Color(0xFFC0C0C0), Color(0xFF808080)],
        2 => const [Color(0xFFFFD700), Color(0xFFDAA520)],
        3 => const [Color(0xFFE5E4E2), Color(0xFF8E8E8E)],
        _ => const [Color(0xFFCD7F32), Color(0xFFA0522D)],
      };

  IconData _tierIcon(int level) => switch (level) {
        0 => Icons.shield_outlined,
        1 => Icons.shield_rounded,
        2 => Icons.workspace_premium_rounded,
        3 => Icons.diamond_rounded,
        _ => Icons.shield_outlined,
      };
}

class _TierProgressBar extends StatelessWidget {
  const _TierProgressBar({required this.dashboard});

  final LoyaltyDashboardModel dashboard;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final nextTier = dashboard.nextTier!;
    final earned = dashboard.lifetimePointsEarned;
    final currentMin = dashboard.currentTier.minLifetimePoints;
    final nextMin = nextTier.minLifetimePoints;
    final range = nextMin - currentMin;
    final progress = range > 0
        ? ((earned - currentMin) / range).clamp(0.0, 1.0)
        : 0.0;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(8),
          child: LinearProgressIndicator(
            value: progress,
            minHeight: 10,
            backgroundColor: AppColors.surfaceLight,
          ),
        ),
        const SizedBox(height: 6),
        Text(
          '${dashboard.pointsToNextTier} ${context.l10n.loyaltyPointsToNext} ${nextTier.name}',
          style: theme.textTheme.bodySmall?.copyWith(
            color: AppColors.textSecondaryLight,
          ),
        ),
      ],
    );
  }
}

class _StatTile extends StatelessWidget {
  const _StatTile({
    required this.icon,
    required this.label,
    required this.value,
    required this.color,
  });

  final IconData icon;
  final String label;
  final String value;
  final Color color;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, color: color, size: 24),
          const SizedBox(height: 8),
          Text(
            value,
            style: theme.textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 2),
          Text(
            label,
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
        ],
      ),
    );
  }
}

class _TransactionTile extends StatelessWidget {
  const _TransactionTile({required this.transaction});

  final LoyaltyTransactionModel transaction;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isEarn = transaction.type == 0;
    final isExpire = transaction.type == 2;

    final icon = isEarn
        ? Icons.arrow_upward_rounded
        : isExpire
            ? Icons.access_time_rounded
            : Icons.arrow_downward_rounded;

    final color = isEarn
        ? Colors.green
        : isExpire
            ? Colors.orange
            : Colors.red;

    final prefix = isEarn ? '+' : '-';

    return ListTile(
      contentPadding: EdgeInsets.zero,
      leading: CircleAvatar(
        backgroundColor: color.withValues(alpha: 0.12),
        child: Icon(icon, color: color, size: 20),
      ),
      title: Text(
        transaction.description,
        style: theme.textTheme.bodyMedium,
        maxLines: 1,
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text(
        _formatDate(transaction.createdAt),
        style: theme.textTheme.bodySmall?.copyWith(
          color: AppColors.textSecondaryLight,
        ),
      ),
      trailing: Text(
        '$prefix${transaction.points}',
        style: theme.textTheme.titleMedium?.copyWith(
          color: color,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }

  String _formatDate(DateTime date) =>
      '${date.day}/${date.month}/${date.year}';
}
