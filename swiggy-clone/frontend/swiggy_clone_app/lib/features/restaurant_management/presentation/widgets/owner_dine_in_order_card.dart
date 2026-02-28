import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/owner_dine_in_order_model.dart';

class OwnerDineInOrderCard extends StatelessWidget {
  const OwnerDineInOrderCard({
    required this.order,
    required this.onAdvanceStatus,
    super.key,
  });

  final OwnerDineInOrderModel order;
  final VoidCallback? onAdvanceStatus;

  static const _statusLabels = {
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready',
    4: 'Served',
    5: 'Completed',
    6: 'Cancelled',
  };

  static const _statusColors = {
    0: Colors.orange,
    1: Colors.blue,
    2: Colors.purple,
    3: Colors.teal,
    4: Colors.green,
    5: Colors.green,
    6: Colors.red,
  };

  static const _actionLabels = {
    0: 'Accept',
    1: 'Start Preparing',
    2: 'Mark Ready',
    3: 'Mark Served',
    4: 'Complete',
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[order.status] ?? 'Unknown';
    final statusColor = _statusColors[order.status] ?? Colors.grey;
    final actionLabel = _actionLabels[order.status];

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        order.orderNumber,
                        style: theme.textTheme.titleSmall
                            ?.copyWith(fontWeight: FontWeight.w600),
                      ),
                      Text(
                        'Table ${order.tableNumber} \u2022 ${order.customerName}',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                  decoration: BoxDecoration(
                    color: statusColor.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: Text(
                    statusLabel,
                    style: TextStyle(
                      color: statusColor,
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),

            // Items summary
            ...order.items.map((item) => Padding(
                  padding: const EdgeInsets.symmetric(vertical: 1),
                  child: Row(
                    children: [
                      Expanded(
                        child: Text(
                          '${item.quantity}x ${item.itemName}'
                          '${item.variantName != null ? ' (${item.variantName})' : ''}',
                          style: theme.textTheme.bodySmall,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      Text(
                        '\u20B9${(item.price / 100).toStringAsFixed(0)}',
                        style: theme.textTheme.bodySmall,
                      ),
                    ],
                  ),
                )),

            if (order.specialInstructions != null) ...[
              const SizedBox(height: 4),
              Text(
                'Note: ${order.specialInstructions}',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textTertiaryLight,
                  fontStyle: FontStyle.italic,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],

            const Divider(height: 16),

            // Footer: total + action
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  '${order.itemCount} items \u2022 \u20B9${(order.totalAmount / 100).toStringAsFixed(0)}',
                  style: theme.textTheme.titleSmall
                      ?.copyWith(fontWeight: FontWeight.bold),
                ),
                if (actionLabel != null && onAdvanceStatus != null)
                  SizedBox(
                    height: 32,
                    child: FilledButton(
                      onPressed: onAdvanceStatus,
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(horizontal: 12),
                        textStyle: const TextStyle(fontSize: 12),
                      ),
                      child: Text(actionLabel),
                    ),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
