import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/order_model.dart';
import '../providers/order_detail_notifier.dart';
import '../providers/order_detail_state.dart';
import '../providers/order_tracking_notifier.dart';
import '../providers/reorder_notifier.dart';
import '../providers/reorder_state.dart';
import '../../../reviews/presentation/widgets/review_prompt_banner.dart';
import '../widgets/tip_delivery_sheet.dart';

class OrderDetailScreen extends ConsumerWidget {
  const OrderDetailScreen({required this.orderId, super.key});

  final String orderId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    // Subscribe to real-time SignalR updates for this order
    ref.watch(orderTrackingNotifierProvider(orderId));
    final state = ref.watch(orderDetailNotifierProvider(orderId));

    ref.listen(reorderNotifierProvider, (_, next) {
      switch (next) {
        case ReorderSuccess(:final result):
          final unavailableCount = result.unavailableItems.length;
          final msg = unavailableCount > 0
              ? 'Cart updated. $unavailableCount item${unavailableCount > 1 ? 's were' : ' was'} unavailable.'
              : 'Cart updated with your previous order!';
          ScaffoldMessenger.of(context)
              .showSnackBar(SnackBar(content: Text(msg)));
          context.push(RouteNames.cart);
        case ReorderError(:final failure):
          ScaffoldMessenger.of(context)
              .showSnackBar(SnackBar(content: Text(failure.message)));
        default:
          break;
      }
    });

    return Scaffold(
      appBar: AppBar(title: const Text('Order Details')),
      body: switch (state) {
        OrderDetailInitial() || OrderDetailLoading() =>
          const AppLoadingWidget(message: 'Loading order...'),
        OrderDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(orderDetailNotifierProvider(orderId).notifier)
                .loadDetail(),
          ),
        OrderDetailLoaded(:final order) => _OrderDetailBody(
            order: order,
            onCancel: () => _showCancelDialog(context, ref),
          ),
      },
    );
  }

  void _showCancelDialog(BuildContext context, WidgetRef ref) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Cancel Order'),
        content: const Text('Are you sure you want to cancel this order?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('No'),
          ),
          TextButton(
            onPressed: () async {
              Navigator.of(ctx).pop();
              await ref
                  .read(orderDetailNotifierProvider(orderId).notifier)
                  .cancelOrder(reason: 'Cancelled by customer');
            },
            child: Text('Yes, Cancel',
                style: TextStyle(color: AppColors.error)),
          ),
        ],
      ),
    );
  }
}

class _OrderDetailBody extends ConsumerWidget {
  const _OrderDetailBody({required this.order, required this.onCancel});

  final OrderModel order;
  final VoidCallback onCancel;

  static const _statusLabels = {
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready for Pickup',
    4: 'Out for Delivery',
    5: 'Delivered',
    6: 'Cancelled',
  };

  static const _statusColors = {
    0: AppColors.info,
    1: AppColors.info,
    2: AppColors.primary,
    3: AppColors.primary,
    4: AppColors.primary,
    5: AppColors.success,
    6: AppColors.error,
  };

  static const _paymentStatusLabels = {
    0: 'Payment Pending',
    1: 'Paid',
    2: 'Payment Failed',
    3: 'Refunded',
    4: 'Partial Refund',
  };

  static const _paymentStatusColors = {
    0: Colors.orange,
    1: Colors.green,
    2: Colors.red,
    3: Colors.blue,
    4: Colors.amber,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[order.status] ?? 'Unknown';
    final statusColor = _statusColors[order.status] ?? AppColors.textSecondaryLight;
    final canCancel = order.status == 0 || order.status == 1;
    final reorderState = ref.watch(reorderNotifierProvider);
    final isReordering = reorderState is ReorderLoading;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Order header
        Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    order.restaurantName,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    '#${order.orderNumber}',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ],
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
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
                    style: theme.textTheme.labelMedium?.copyWith(
                      color: statusColor,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(height: 4),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                  decoration: BoxDecoration(
                    color: (_paymentStatusColors[order.paymentStatus] ??
                            Colors.grey)
                        .withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Text(
                    _paymentStatusLabels[order.paymentStatus] ?? 'Unknown',
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: _paymentStatusColors[order.paymentStatus] ??
                          Colors.grey,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),

        if (order.estimatedDeliveryTime != null) ...[
          const SizedBox(height: 8),
          Row(
            children: [
              const Icon(Icons.access_time, size: 16, color: AppColors.info),
              const SizedBox(width: 4),
              Text(
                'Estimated delivery: ~45 min',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
              ),
            ],
          ),
        ],

        const Divider(height: 32),

        // Items
        Text(
          'Items',
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        ...order.items.map((item) => Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          '${item.quantity}x ${item.itemName}',
                          style: theme.textTheme.bodyMedium,
                        ),
                        if (item.addons.isNotEmpty)
                          Text(
                            item.addons.map((a) => a.addonName).join(', '),
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: AppColors.textTertiaryLight,
                            ),
                          ),
                      ],
                    ),
                  ),
                  Text(
                    '\u20B9${item.totalPrice ~/ 100}',
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
            )),

        const Divider(height: 24),

        // Price breakdown
        _PriceRow(label: 'Subtotal', amount: order.subtotal),
        const SizedBox(height: 4),
        _PriceRow(label: 'Tax', amount: order.taxAmount),
        const SizedBox(height: 4),
        _PriceRow(label: 'Delivery Fee', amount: order.deliveryFee),
        const SizedBox(height: 4),
        _PriceRow(label: 'Packaging', amount: order.packagingCharge),
        if (order.discountAmount > 0) ...[
          const SizedBox(height: 4),
          _PriceRow(label: 'Discount', amount: -order.discountAmount),
        ],
        const Divider(height: 24),
        _PriceRow(label: 'Total', amount: order.totalAmount, isBold: true),

        // Track order button (visible for Out for Delivery status)
        if (order.status == 4) ...[
          const SizedBox(height: 24),
          FilledButton.icon(
            onPressed: () =>
                context.push(RouteNames.orderTrackingPath(order.id)),
            icon: const Icon(Icons.delivery_dining),
            label: const Text('Track Order'),
            style: FilledButton.styleFrom(
              minimumSize: const Size.fromHeight(48),
            ),
          ),
        ],

        // Tip prompt for delivered delivery orders without a tip
        if (order.status == 5 && order.orderType == 0 && !order.hasTipped)
          _TipPromptBanner(orderId: order.id),

        // Review prompt for delivered orders without a review
        if (order.status == 5 && !order.hasReview)
          ReviewPromptBanner(
            orderId: order.id,
            restaurantName: order.restaurantName,
          ),

        // Reorder button for delivered orders
        if (order.status == 5) ...[
          const SizedBox(height: 16),
          OutlinedButton.icon(
            onPressed: isReordering
                ? null
                : () => ref
                    .read(reorderNotifierProvider.notifier)
                    .reorder(order.id),
            icon: isReordering
                ? const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.refresh),
            label: Text(isReordering ? 'Reordering...' : 'Reorder'),
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.primary,
              side: const BorderSide(color: AppColors.primary),
              minimumSize: const Size.fromHeight(48),
            ),
          ),
        ],

        // Cancel button
        if (canCancel) ...[
          const SizedBox(height: 24),
          OutlinedButton(
            onPressed: onCancel,
            style: OutlinedButton.styleFrom(
              foregroundColor: AppColors.error,
              side: const BorderSide(color: AppColors.error),
              minimumSize: const Size.fromHeight(48),
            ),
            child: const Text('Cancel Order'),
          ),
        ],
      ],
    );
  }
}

class _PriceRow extends StatelessWidget {
  const _PriceRow({
    required this.label,
    required this.amount,
    this.isBold = false,
  });

  final String label;
  final int amount;
  final bool isBold;

  @override
  Widget build(BuildContext context) {
    final style = isBold
        ? Theme.of(context)
            .textTheme
            .titleMedium
            ?.copyWith(fontWeight: FontWeight.bold)
        : Theme.of(context).textTheme.bodyMedium;

    final prefix = amount < 0 ? '-' : '';
    final absAmount = amount.abs();

    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: style),
        Text('$prefix\u20B9${absAmount ~/ 100}', style: style),
      ],
    );
  }
}

class _TipPromptBanner extends StatelessWidget {
  const _TipPromptBanner({required this.orderId});

  final String orderId;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      margin: const EdgeInsets.only(top: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.primary.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.primary.withValues(alpha: 0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.volunteer_activism,
                  color: AppColors.primary, size: 24),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Tip your delivery partner',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            'Show appreciation for their service with a tip.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton(
              onPressed: () => showModalBottomSheet<void>(
                context: context,
                isScrollControlled: true,
                shape: const RoundedRectangleBorder(
                  borderRadius:
                      BorderRadius.vertical(top: Radius.circular(16)),
                ),
                builder: (_) => TipDeliverySheet(orderId: orderId),
              ),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppColors.primary,
                side: const BorderSide(color: AppColors.primary),
              ),
              child: const Text('Tip Now'),
            ),
          ),
        ],
      ),
    );
  }
}
