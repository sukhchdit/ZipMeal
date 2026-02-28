import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/active_sessions_notifier.dart';
import '../providers/active_sessions_state.dart';

class ActiveSessionsScreen extends ConsumerWidget {
  const ActiveSessionsScreen({required this.restaurantId, super.key});

  final String restaurantId;

  static const _statusLabels = {
    0: 'Active',
    1: 'Bill Requested',
    2: 'Payment Pending',
    3: 'Completed',
    4: 'Cancelled',
  };

  static const _statusColors = {
    0: Colors.green,
    1: Colors.orange,
    2: Colors.amber,
    3: Colors.grey,
    4: Colors.red,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(activeSessionsNotifierProvider(restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Active Sessions'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(activeSessionsNotifierProvider(restaurantId).notifier)
                .loadSessions(),
          ),
        ],
      ),
      body: switch (state) {
        ActiveSessionsInitial() || ActiveSessionsLoading() =>
          const AppLoadingWidget(message: 'Loading sessions...'),
        ActiveSessionsError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(activeSessionsNotifierProvider(restaurantId).notifier)
                .loadSessions(),
          ),
        ActiveSessionsLoaded(:final sessions) => sessions.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.groups_outlined,
                        size: 64, color: AppColors.textTertiaryLight),
                    const SizedBox(height: 16),
                    Text('No active sessions',
                        style: theme.textTheme.titleMedium),
                  ],
                ),
              )
            : RefreshIndicator(
                onRefresh: () => ref
                    .read(
                        activeSessionsNotifierProvider(restaurantId).notifier)
                    .loadSessions(),
                child: ListView.separated(
                  padding: const EdgeInsets.all(16),
                  itemCount: sessions.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final session = sessions[index];
                    final statusLabel =
                        _statusLabels[session.status] ?? 'Unknown';
                    final statusColor =
                        _statusColors[session.status] ?? Colors.grey;

                    return Card(
                      shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12)),
                      child: Padding(
                        padding: const EdgeInsets.all(12),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                Icon(Icons.table_restaurant,
                                    color: AppColors.primary, size: 20),
                                const SizedBox(width: 8),
                                Text(
                                  'Table ${session.tableNumber}',
                                  style: theme.textTheme.titleSmall?.copyWith(
                                      fontWeight: FontWeight.w600),
                                ),
                                if (session.floorSection != null) ...[
                                  const SizedBox(width: 4),
                                  Text(
                                    '\u2022 ${session.floorSection}',
                                    style: theme.textTheme.bodySmall?.copyWith(
                                      color: AppColors.textSecondaryLight,
                                    ),
                                  ),
                                ],
                                const Spacer(),
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                      horizontal: 8, vertical: 3),
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
                            Row(
                              children: [
                                _InfoChip(
                                  icon: Icons.confirmation_number_outlined,
                                  label: session.sessionCode,
                                ),
                                const SizedBox(width: 12),
                                _InfoChip(
                                  icon: Icons.people_outline,
                                  label: '${session.memberCount} members',
                                ),
                              ],
                            ),
                            const Divider(height: 16),
                            Row(
                              mainAxisAlignment:
                                  MainAxisAlignment.spaceBetween,
                              children: [
                                Text(
                                  '${session.orderCount} orders',
                                  style: theme.textTheme.bodyMedium,
                                ),
                                Text(
                                  '\u20B9${(session.totalAmount / 100).toStringAsFixed(0)}',
                                  style: theme.textTheme.titleSmall?.copyWith(
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

class _InfoChip extends StatelessWidget {
  const _InfoChip({required this.icon, required this.label});

  final IconData icon;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 14, color: AppColors.textSecondaryLight),
        const SizedBox(width: 4),
        Text(
          label,
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
        ),
      ],
    );
  }
}
