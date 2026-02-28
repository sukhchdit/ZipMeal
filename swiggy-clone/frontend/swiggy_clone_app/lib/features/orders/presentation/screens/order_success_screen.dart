import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';

class OrderSuccessScreen extends StatelessWidget {
  const OrderSuccessScreen({
    required this.orderId,
    required this.orderNumber,
    super.key,
  });

  final String orderId;
  final String orderNumber;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      body: SafeArea(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: 96,
                  height: 96,
                  decoration: BoxDecoration(
                    color: AppColors.success.withValues(alpha: 0.12),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.check_circle_rounded,
                    size: 64,
                    color: AppColors.success,
                  ),
                ),
                const SizedBox(height: 24),
                Text(
                  'Order Placed!',
                  style: theme.textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Order #$orderNumber',
                  style: theme.textTheme.titleMedium?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Estimated delivery in 45 minutes',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
                ),
                const SizedBox(height: 32),
                FilledButton(
                  onPressed: () => context
                      .push(RouteNames.orderDetailPath(orderId)),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    minimumSize: const Size.fromHeight(48),
                  ),
                  child: const Text('View Order'),
                ),
                const SizedBox(height: 12),
                OutlinedButton(
                  onPressed: () => context.go(RouteNames.home),
                  style: OutlinedButton.styleFrom(
                    minimumSize: const Size.fromHeight(48),
                  ),
                  child: const Text('Back to Home'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
