import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/admin_order_detail_model.dart';
import '../providers/admin_order_detail_notifier.dart';

/// Read-only order detail view for admin.
class AdminOrderDetailScreen extends ConsumerWidget {
  const AdminOrderDetailScreen({required this.orderId, super.key});

  final String orderId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(adminOrderDetailNotifierProvider(orderId));

    return Scaffold(
      appBar: AppBar(title: const Text('Order Detail')),
      body: switch (state) {
        AdminOrderDetailInitial() || AdminOrderDetailLoading() =>
          const AppLoadingWidget(message: 'Loading order...'),
        AdminOrderDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(adminOrderDetailNotifierProvider(orderId).notifier)
                .loadDetail(),
          ),
        AdminOrderDetailLoaded(:final order) =>
          _AdminOrderDetailBody(order: order),
      },
    );
  }
}

class _AdminOrderDetailBody extends StatelessWidget {
  const _AdminOrderDetailBody({required this.order});

  final AdminOrderDetailModel order;

  static const _statusLabels = {
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready',
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
    0: 'Pending',
    1: 'Paid',
    2: 'Failed',
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

  static const _orderTypeLabels = {
    0: 'Delivery',
    1: 'Takeaway',
    2: 'Dine-In',
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final dateFormat = DateFormat('dd MMM yyyy, hh:mm a');
    final statusLabel = _statusLabels[order.status] ?? 'Unknown';
    final statusColor =
        _statusColors[order.status] ?? AppColors.textSecondaryLight;
    final paymentLabel =
        _paymentStatusLabels[order.paymentStatus] ?? 'Unknown';
    final paymentColor =
        _paymentStatusColors[order.paymentStatus] ?? Colors.grey;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // ──── Header ────
        Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '#${order.orderNumber}',
                    style: theme.textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    order.restaurantName,
                    style: theme.textTheme.titleSmall?.copyWith(
                      color: AppColors.textSecondaryLight,
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
                    color: paymentColor.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Text(
                    paymentLabel,
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: paymentColor,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),

        const SizedBox(height: 16),

        // ──── Customer Info ────
        Card(
          shape:
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
          child: Padding(
            padding: const EdgeInsets.all(14),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Customer',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                Row(
                  children: [
                    const Icon(Icons.person_outline,
                        size: 18, color: AppColors.textSecondaryLight),
                    const SizedBox(width: 8),
                    Text(order.customerName,
                        style: theme.textTheme.bodyMedium),
                  ],
                ),
                const SizedBox(height: 4),
                Row(
                  children: [
                    const Icon(Icons.phone_outlined,
                        size: 18, color: AppColors.textSecondaryLight),
                    const SizedBox(width: 8),
                    Text(order.customerPhone,
                        style: theme.textTheme.bodyMedium),
                  ],
                ),
              ],
            ),
          ),
        ),

        const SizedBox(height: 16),

        // ──── Items ────
        Text(
          'Items',
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        ...order.items.map(
          (item) => Padding(
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
          ),
        ),

        const Divider(height: 24),

        // ──── Price Breakdown ────
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

        const Divider(height: 32),

        // ──── Additional Info ────
        Text(
          'Additional Info',
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 8),
        _InfoRow(
          label: 'Order Type',
          value: _orderTypeLabels[order.orderType] ?? 'Unknown',
        ),
        _InfoRow(
          label: 'Placed At',
          value: dateFormat.format(order.createdAt),
        ),
        if (order.specialInstructions != null &&
            order.specialInstructions!.isNotEmpty)
          _InfoRow(
            label: 'Instructions',
            value: order.specialInstructions!,
          ),
        if (order.cancellationReason != null &&
            order.cancellationReason!.isNotEmpty)
          _InfoRow(
            label: 'Cancellation',
            value: order.cancellationReason!,
          ),
        if (order.estimatedDeliveryTime != null)
          _InfoRow(
            label: 'Est. Delivery',
            value: dateFormat.format(order.estimatedDeliveryTime!),
          ),
        if (order.actualDeliveryTime != null)
          _InfoRow(
            label: 'Delivered At',
            value: dateFormat.format(order.actualDeliveryTime!),
          ),
        const SizedBox(height: 24),
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
