import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/dine_in_orders_notifier.dart';
import '../providers/dine_in_orders_state.dart';
import '../widgets/owner_dine_in_order_card.dart';

class DineInOrdersScreen extends ConsumerWidget {
  const DineInOrdersScreen({required this.restaurantId, super.key});

  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return DefaultTabController(
      length: 5,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Dine-In Orders'),
          actions: [
            IconButton(
              icon: const Icon(Icons.refresh),
              onPressed: () => ref
                  .read(dineInOrdersNotifierProvider(restaurantId).notifier)
                  .loadOrders(),
            ),
          ],
          bottom: const TabBar(
            isScrollable: true,
            tabs: [
              Tab(text: 'All'),
              Tab(text: 'New'),
              Tab(text: 'In Progress'),
              Tab(text: 'Ready'),
              Tab(text: 'Served'),
            ],
          ),
        ),
        body: TabBarView(
          children: [
            _OrdersTab(restaurantId: restaurantId),
            _OrdersTab(restaurantId: restaurantId, statusFilter: const [0]),
            _OrdersTab(
                restaurantId: restaurantId, statusFilter: const [1, 2]),
            _OrdersTab(restaurantId: restaurantId, statusFilter: const [3]),
            _OrdersTab(restaurantId: restaurantId, statusFilter: const [4]),
          ],
        ),
      ),
    );
  }
}

class _OrdersTab extends ConsumerWidget {
  const _OrdersTab({required this.restaurantId, this.statusFilter});

  final String restaurantId;
  final List<int>? statusFilter;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(dineInOrdersNotifierProvider(restaurantId));
    final theme = Theme.of(context);

    return switch (state) {
      DineInOrdersInitial() || DineInOrdersLoading() =>
        const AppLoadingWidget(message: 'Loading orders...'),
      DineInOrdersError(:final failure) => AppErrorWidget(
          failure: failure,
          onRetry: () => ref
              .read(dineInOrdersNotifierProvider(restaurantId).notifier)
              .loadOrders(),
        ),
      DineInOrdersLoaded(:final orders) => () {
          final filtered = statusFilter != null
              ? orders
                  .where((o) => statusFilter!.contains(o.status))
                  .toList()
              : orders;

          if (filtered.isEmpty) {
            return Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.receipt_long,
                      size: 48, color: AppColors.textTertiaryLight),
                  const SizedBox(height: 12),
                  Text('No orders', style: theme.textTheme.bodyLarge),
                ],
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () => ref
                .read(dineInOrdersNotifierProvider(restaurantId).notifier)
                .loadOrders(),
            child: ListView.separated(
              padding: const EdgeInsets.all(16),
              itemCount: filtered.length,
              separatorBuilder: (_, __) => const SizedBox(height: 8),
              itemBuilder: (context, index) {
                final order = filtered[index];
                return OwnerDineInOrderCard(
                  order: order,
                  onAdvanceStatus: order.status < 5
                      ? () => ref
                          .read(dineInOrdersNotifierProvider(restaurantId)
                              .notifier)
                          .updateOrderStatus(order.id, order.status + 1)
                      : null,
                );
              },
            ),
          );
        }(),
    };
  }
}
