import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/admin_restaurant_model.dart';
import '../providers/admin_restaurant_detail_notifier.dart';

/// Restaurant detail screen with admin actions (approve, reject, suspend, reactivate).
class AdminRestaurantDetailScreen extends ConsumerWidget {
  const AdminRestaurantDetailScreen({required this.restaurantId, super.key});

  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state =
        ref.watch(adminRestaurantDetailNotifierProvider(restaurantId));

    return Scaffold(
      appBar: AppBar(title: const Text('Restaurant Detail')),
      body: switch (state) {
        AdminRestaurantDetailInitial() ||
        AdminRestaurantDetailLoading() =>
          const AppLoadingWidget(message: 'Loading restaurant...'),
        AdminRestaurantDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(adminRestaurantDetailNotifierProvider(restaurantId)
                    .notifier)
                .loadDetail(),
          ),
        AdminRestaurantDetailLoaded(:final restaurant) =>
          _RestaurantDetailBody(
            restaurant: restaurant,
            restaurantId: restaurantId,
          ),
      },
    );
  }
}

class _RestaurantDetailBody extends ConsumerWidget {
  const _RestaurantDetailBody({
    required this.restaurant,
    required this.restaurantId,
  });

  final AdminRestaurantModel restaurant;
  final String restaurantId;

  static const _statusLabels = {
    0: 'Pending',
    1: 'Approved',
    2: 'Suspended',
    3: 'Rejected',
  };

  static const _statusColors = {
    0: Colors.orange,
    1: AppColors.success,
    2: AppColors.error,
    3: AppColors.textTertiaryLight,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[restaurant.status] ?? 'Unknown';
    final statusColor =
        _statusColors[restaurant.status] ?? AppColors.textSecondaryLight;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // ──── Logo & Name ────
        Center(
          child: ClipRRect(
            borderRadius: BorderRadius.circular(12),
            child: restaurant.logoUrl != null
                ? Image.network(
                    restaurant.logoUrl!,
                    width: 80,
                    height: 80,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      width: 80,
                      height: 80,
                      color: AppColors.primaryLight,
                      child: const Icon(Icons.store,
                          size: 40, color: AppColors.primary),
                    ),
                  )
                : Container(
                    width: 80,
                    height: 80,
                    color: AppColors.primaryLight,
                    child: const Icon(Icons.store,
                        size: 40, color: AppColors.primary),
                  ),
          ),
        ),
        const SizedBox(height: 12),
        Center(
          child: Text(
            restaurant.name,
            style: theme.textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
            textAlign: TextAlign.center,
          ),
        ),
        if (restaurant.slug.isNotEmpty) ...[
          const SizedBox(height: 2),
          Center(
            child: Text(
              restaurant.slug,
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textTertiaryLight,
              ),
            ),
          ),
        ],
        const SizedBox(height: 12),

        // ──── Status & Accepting Orders Badges ────
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                color: statusColor.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                statusLabel,
                style: theme.textTheme.labelLarge?.copyWith(
                  color: statusColor,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(width: 8),
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
              decoration: BoxDecoration(
                color: (restaurant.isAcceptingOrders
                        ? AppColors.success
                        : AppColors.textTertiaryLight)
                    .withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                restaurant.isAcceptingOrders
                    ? 'Accepting Orders'
                    : 'Not Accepting',
                style: theme.textTheme.labelMedium?.copyWith(
                  color: restaurant.isAcceptingOrders
                      ? AppColors.success
                      : AppColors.textTertiaryLight,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),

        if (restaurant.statusReason != null &&
            restaurant.statusReason!.isNotEmpty) ...[
          const SizedBox(height: 8),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: AppColors.warning.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(8),
              border: Border.all(
                  color: AppColors.warning.withValues(alpha: 0.3)),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.info_outline,
                    size: 18, color: AppColors.warning),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'Reason: ${restaurant.statusReason}',
                    style: theme.textTheme.bodySmall,
                  ),
                ),
              ],
            ),
          ),
        ],

        const Divider(height: 32),

        // ──── Info Section ────
        if (restaurant.description != null &&
            restaurant.description!.isNotEmpty) ...[
          _InfoRow(label: 'Description', value: restaurant.description!),
          const SizedBox(height: 4),
        ],
        if (restaurant.city != null)
          _InfoRow(
            label: 'Location',
            value:
                '${restaurant.city}${restaurant.state != null ? ', ${restaurant.state}' : ''}',
          ),
        _InfoRow(label: 'Owner', value: restaurant.ownerName),
        _InfoRow(label: 'Owner Phone', value: restaurant.ownerPhone),
        if (restaurant.fssaiLicense != null)
          _InfoRow(label: 'FSSAI', value: restaurant.fssaiLicense!),
        if (restaurant.gstNumber != null)
          _InfoRow(label: 'GST', value: restaurant.gstNumber!),
        _InfoRow(
          label: 'Rating',
          value:
              '${restaurant.averageRating.toStringAsFixed(1)} (${restaurant.totalRatings} ratings)',
        ),

        const Divider(height: 32),

        // ──── Admin Actions ────
        Text(
          'Actions',
          style: theme.textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),

        ..._buildActions(context, ref),
      ],
    );
  }

  List<Widget> _buildActions(BuildContext context, WidgetRef ref) {
    final notifier =
        ref.read(adminRestaurantDetailNotifierProvider(restaurantId).notifier);

    switch (restaurant.status) {
      // Pending
      case 0:
        return [
          FilledButton.icon(
            onPressed: () async {
              final confirmed = await _confirmAction(
                context,
                title: 'Approve Restaurant',
                message: 'Approve "${restaurant.name}"?',
              );
              if (confirmed) await notifier.approve();
            },
            icon: const Icon(Icons.check),
            label: const Text('Approve'),
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.success,
              minimumSize: const Size.fromHeight(48),
            ),
          ),
          const SizedBox(height: 8),
          OutlinedButton.icon(
            onPressed: () => _showReasonDialog(
              context,
              title: 'Reject Restaurant',
              hint: 'Reason for rejection',
              onSubmit: (reason) async => notifier.reject(reason),
            ),
            icon: const Icon(Icons.close),
            label: const Text('Reject'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.error,
              side: const BorderSide(color: AppColors.error),
              minimumSize: const Size.fromHeight(48),
            ),
          ),
        ];

      // Approved
      case 1:
        return [
          OutlinedButton.icon(
            onPressed: () => _showReasonDialog(
              context,
              title: 'Suspend Restaurant',
              hint: 'Reason for suspension',
              onSubmit: (reason) async => notifier.suspend(reason),
            ),
            icon: const Icon(Icons.block),
            label: const Text('Suspend'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.error,
              side: const BorderSide(color: AppColors.error),
              minimumSize: const Size.fromHeight(48),
            ),
          ),
        ];

      // Suspended
      case 2:
        return [
          FilledButton.icon(
            onPressed: () async {
              final confirmed = await _confirmAction(
                context,
                title: 'Reactivate Restaurant',
                message: 'Reactivate "${restaurant.name}"?',
              );
              if (confirmed) await notifier.reactivate();
            },
            icon: const Icon(Icons.restart_alt),
            label: const Text('Reactivate'),
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.success,
              minimumSize: const Size.fromHeight(48),
            ),
          ),
        ];

      // Rejected
      case 3:
        return [
          Center(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Text(
                'No actions available for rejected restaurants.',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                textAlign: TextAlign.center,
              ),
            ),
          ),
        ];

      default:
        return [];
    }
  }

  Future<bool> _confirmAction(
    BuildContext context, {
    required String title,
    required String message,
  }) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            child: const Text('Confirm'),
          ),
        ],
      ),
    );
    return result ?? false;
  }

  void _showReasonDialog(
    BuildContext context, {
    required String title,
    required String hint,
    required Future<bool> Function(String reason) onSubmit,
  }) {
    final controller = TextEditingController();

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(title),
        content: TextField(
          controller: controller,
          maxLines: 3,
          decoration: InputDecoration(
            hintText: hint,
            border: const OutlineInputBorder(),
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              final reason = controller.text.trim();
              if (reason.isEmpty) return;
              Navigator.of(ctx).pop();
              onSubmit(reason);
            },
            child: const Text('Submit'),
          ),
        ],
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  const _InfoRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 110,
            child: Text(
              label,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
          ),
          Expanded(
            child: Text(value, style: theme.textTheme.bodyMedium),
          ),
        ],
      ),
    );
  }
}
