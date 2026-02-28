import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/session_orders_notifier.dart';
import '../providers/session_orders_state.dart';

class SessionOrdersScreen extends ConsumerWidget {
  const SessionOrdersScreen({required this.sessionId, super.key});

  final String sessionId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(sessionOrdersNotifierProvider(sessionId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Session Orders')),
      body: switch (state) {
        SessionOrdersInitial() || SessionOrdersLoading() =>
          const AppLoadingWidget(message: 'Loading orders...'),
        SessionOrdersError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(sessionOrdersNotifierProvider(sessionId).notifier)
                .loadOrders(),
          ),
        SessionOrdersLoaded(:final orders) => orders.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.receipt_long,
                        size: 64, color: AppColors.textTertiaryLight),
                    const SizedBox(height: 16),
                    Text(
                      'No orders yet',
                      style: theme.textTheme.titleMedium,
                    ),
                  ],
                ),
              )
            : RefreshIndicator(
                onRefresh: () => ref
                    .read(sessionOrdersNotifierProvider(sessionId).notifier)
                    .loadOrders(),
                child: ListView.separated(
                  padding: const EdgeInsets.all(16),
                  itemCount: orders.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final order = orders[index];
                    return Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Padding(
                        padding: const EdgeInsets.all(12),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              mainAxisAlignment:
                                  MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  order.orderNumber,
                                  style: theme.textTheme.titleSmall
                                      ?.copyWith(
                                          fontWeight: FontWeight.w600),
                                ),
                                _StatusBadge(status: order.status),
                              ],
                            ),
                            const SizedBox(height: 8),
                            ...order.items.map((item) => Padding(
                                  padding:
                                      const EdgeInsets.symmetric(vertical: 2),
                                  child: Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Expanded(
                                        child: Text(
                                          '${item.quantity}x ${item.itemName}',
                                          style: theme.textTheme.bodyMedium,
                                        ),
                                      ),
                                      Text(
                                        '\u20B9${(item.totalPrice / 100).toStringAsFixed(0)}',
                                        style: theme.textTheme.bodyMedium,
                                      ),
                                    ],
                                  ),
                                )),
                            const Divider(height: 16),
                            Row(
                              mainAxisAlignment:
                                  MainAxisAlignment.spaceBetween,
                              children: [
                                Text('Total',
                                    style: theme.textTheme.titleSmall
                                        ?.copyWith(
                                            fontWeight: FontWeight.w600)),
                                Text(
                                  '\u20B9${(order.totalAmount / 100).toStringAsFixed(0)}',
                                  style: theme.textTheme.titleSmall
                                      ?.copyWith(
                                          fontWeight: FontWeight.bold),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
      },
    );
  }
}

class _StatusBadge extends StatelessWidget {
  const _StatusBadge({required this.status});

  final int status;

  static const _labels = {
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready',
    4: 'Served',
    5: 'Completed',
    6: 'Cancelled',
  };

  static const _colors = {
    0: Colors.orange,
    1: Colors.blue,
    2: Colors.purple,
    3: Colors.teal,
    4: Colors.green,
    5: Colors.green,
    6: Colors.red,
  };

  @override
  Widget build(BuildContext context) {
    final label = _labels[status] ?? 'Unknown';
    final color = _colors[status] ?? Colors.grey;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(4),
      ),
      child: Text(
        label,
        style: TextStyle(
          color: color,
          fontSize: 11,
          fontWeight: FontWeight.w600,
        ),
      ),
    );
  }
}
