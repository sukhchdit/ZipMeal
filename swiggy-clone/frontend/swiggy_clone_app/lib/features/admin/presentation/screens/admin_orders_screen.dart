import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/admin_dashboard_model.dart';
import '../providers/admin_orders_notifier.dart';
import '../providers/admin_orders_state.dart';

/// Paginated order list with status and date filters.
class AdminOrdersScreen extends ConsumerStatefulWidget {
  const AdminOrdersScreen({super.key});

  @override
  ConsumerState<AdminOrdersScreen> createState() => _AdminOrdersScreenState();
}

class _AdminOrdersScreenState extends ConsumerState<AdminOrdersScreen> {
  int? _selectedStatus;

  static const _statusFilterLabels = <int?, String>{
    null: 'All',
    0: 'Placed',
    1: 'Confirmed',
    2: 'Preparing',
    3: 'Ready',
    4: 'Out for Delivery',
    5: 'Delivered',
    6: 'Cancelled',
  };

  void _applyFilters() {
    ref.read(adminOrdersNotifierProvider.notifier).loadOrders(
          statusFilter: _selectedStatus,
        );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(adminOrdersNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Orders')),
      body: Column(
        children: [
          // ──── Status Filter Chips ────
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: _statusFilterLabels.entries
                  .map(
                    (entry) => Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: FilterChip(
                        label: Text(entry.value),
                        selected: _selectedStatus == entry.key,
                        onSelected: (_) {
                          setState(() => _selectedStatus = entry.key);
                          _applyFilters();
                        },
                      ),
                    ),
                  )
                  .toList(),
            ),
          ),

          // ──── Order List ────
          Expanded(
            child: switch (state) {
              AdminOrdersInitial() || AdminOrdersLoading() =>
                const AppLoadingWidget(message: 'Loading orders...'),
              AdminOrdersError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: _applyFilters,
                ),
              AdminOrdersLoaded(
                :final orders,
                :final page,
                :final totalPages,
                :final isLoadingMore,
              ) =>
                orders.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.receipt_long_outlined,
                                size: 80,
                                color: AppColors.textTertiaryLight),
                            const SizedBox(height: 16),
                            Text(
                              'No orders found',
                              style: theme.textTheme.titleMedium?.copyWith(
                                color: AppColors.textSecondaryLight,
                              ),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        color: AppColors.primary,
                        onRefresh: () async => _applyFilters(),
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          itemCount:
                              orders.length + (page < totalPages ? 1 : 0),
                          itemBuilder: (context, index) {
                            if (index >= orders.length) {
                              if (!isLoadingMore) {
                                WidgetsBinding.instance
                                    .addPostFrameCallback((_) {
                                  ref
                                      .read(
                                          adminOrdersNotifierProvider.notifier)
                                      .loadOrders(
                                        statusFilter: _selectedStatus,
                                        page: page + 1,
                                      );
                                });
                              }
                              return const Padding(
                                padding: EdgeInsets.all(16),
                                child: Center(
                                  child: CircularProgressIndicator(
                                      strokeWidth: 2),
                                ),
                              );
                            }
                            return _OrderCard(
                              order: orders[index],
                              onTap: () => context.push(
                                RouteNames.adminOrderDetailPath(
                                    orders[index].id),
                              ),
                            );
                          },
                        ),
                      ),
            },
          ),
        ],
      ),
    );
  }
}

class _OrderCard extends StatelessWidget {
  const _OrderCard({required this.order, required this.onTap});

  final AdminOrderSummaryModel order;
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
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[order.status] ?? 'Unknown';
    final statusColor =
        _statusColors[order.status] ?? AppColors.textSecondaryLight;
    final dateFormat = DateFormat('dd MMM, hh:mm a');

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(14),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      '#${order.orderNumber}',
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
              const SizedBox(height: 6),
              Text(
                '${order.customerName} - ${order.restaurantName}',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
              const SizedBox(height: 6),
              Row(
                children: [
                  Text(
                    dateFormat.format(order.createdAt),
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
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
            ],
          ),
        ),
      ),
    );
  }
}
