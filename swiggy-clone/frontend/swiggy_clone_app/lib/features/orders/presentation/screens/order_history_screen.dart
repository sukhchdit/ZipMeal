import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/order_summary_model.dart';
import '../providers/my_orders_notifier.dart';
import '../providers/my_orders_state.dart';
import '../providers/reorder_notifier.dart';
import '../providers/reorder_state.dart';

class OrderHistoryScreen extends ConsumerWidget {
  const OrderHistoryScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(myOrdersNotifierProvider);
    final theme = Theme.of(context);

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
      appBar: AppBar(title: const Text('My Orders')),
      body: switch (state) {
        MyOrdersInitial() || MyOrdersLoading() =>
          const AppLoadingWidget(message: 'Loading orders...'),
        MyOrdersError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () =>
                ref.read(myOrdersNotifierProvider.notifier).loadOrders(),
          ),
        MyOrdersLoaded(:final orders, :final hasMore, :final isLoadingMore) =>
          orders.isEmpty
              ? Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.receipt_long_outlined,
                          size: 80, color: AppColors.textTertiaryLight),
                      const SizedBox(height: 16),
                      Text(
                        'No orders yet',
                        style: theme.textTheme.titleMedium?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                  ),
                )
              : RefreshIndicator(
                  onRefresh: () =>
                      ref.read(myOrdersNotifierProvider.notifier).loadOrders(),
                  child: ListView.builder(
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    itemCount: orders.length + (hasMore ? 1 : 0),
                    itemBuilder: (context, index) {
                      if (index >= orders.length) {
                        if (!isLoadingMore) {
                          // Trigger load more
                          WidgetsBinding.instance.addPostFrameCallback((_) {
                            ref
                                .read(myOrdersNotifierProvider.notifier)
                                .loadMore();
                          });
                        }
                        return const Padding(
                          padding: EdgeInsets.all(16),
                          child: Center(
                              child: CircularProgressIndicator(
                                  strokeWidth: 2)),
                        );
                      }
                      return _OrderSummaryCard(
                        order: orders[index],
                        onTap: () => context
                            .push(RouteNames.orderDetailPath(orders[index].id)),
                      );
                    },
                  ),
                ),
      },
    );
  }
}

class _OrderSummaryCard extends ConsumerWidget {
  const _OrderSummaryCard({required this.order, required this.onTap});

  final OrderSummaryModel order;
  final VoidCallback onTap;

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

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[order.status] ?? 'Unknown';
    final statusColor = _statusColors[order.status] ?? AppColors.textSecondaryLight;
    final reorderState = ref.watch(reorderNotifierProvider);
    final isReordering = reorderState is ReorderLoading;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      order.restaurantName,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      statusLabel,
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: statusColor,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 4),
              Text(
                '#${order.orderNumber}',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textTertiaryLight,
                ),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Text(
                    '${order.itemCount} item${order.itemCount > 1 ? 's' : ''}',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                  const Spacer(),
                  Text(
                    '\u20B9${order.totalAmount ~/ 100}',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
              if (order.status == 5) ...[
                const SizedBox(height: 8),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton.icon(
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
                        : const Icon(Icons.refresh, size: 18),
                    label: Text(isReordering ? 'Reordering...' : 'Reorder'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: AppColors.primary,
                      side: const BorderSide(color: AppColors.primary),
                    ),
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}
