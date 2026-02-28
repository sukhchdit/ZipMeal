import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/restaurant_table_model.dart';
import '../providers/table_management_notifier.dart';
import '../providers/table_management_state.dart';
import '../widgets/table_form_dialog.dart';

class TableManagementScreen extends ConsumerWidget {
  const TableManagementScreen({required this.restaurantId, super.key});

  final String restaurantId;

  static const _statusLabels = {
    0: 'Available',
    1: 'Occupied',
    2: 'Reserved',
    3: 'Maintenance',
  };

  static const _statusColors = {
    0: Colors.green,
    1: Colors.orange,
    2: Colors.blue,
    3: Colors.grey,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(tableManagementNotifierProvider(restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Table Management'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(tableManagementNotifierProvider(restaurantId).notifier)
                .loadTables(),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        backgroundColor: AppColors.primary,
        onPressed: () => _showCreateDialog(context, ref),
        child: const Icon(Icons.add),
      ),
      body: switch (state) {
        TableManagementInitial() || TableManagementLoading() =>
          const AppLoadingWidget(message: 'Loading tables...'),
        TableManagementError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(tableManagementNotifierProvider(restaurantId).notifier)
                .loadTables(),
          ),
        TableManagementLoaded(:final tables) => tables.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.table_restaurant,
                        size: 64, color: AppColors.textTertiaryLight),
                    const SizedBox(height: 16),
                    Text('No tables yet', style: theme.textTheme.titleMedium),
                    const SizedBox(height: 8),
                    Text(
                      'Tap + to add your first table',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                  ],
                ),
              )
            : RefreshIndicator(
                onRefresh: () => ref
                    .read(
                        tableManagementNotifierProvider(restaurantId).notifier)
                    .loadTables(),
                child: ListView.separated(
                  padding: const EdgeInsets.all(16),
                  itemCount: tables.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final table = tables[index];
                    return _TableCard(
                      table: table,
                      theme: theme,
                      statusLabel:
                          _statusLabels[table.status] ?? 'Unknown',
                      statusColor:
                          _statusColors[table.status] ?? Colors.grey,
                      onEdit: () =>
                          _showEditDialog(context, ref, table),
                      onDelete: () =>
                          _confirmDelete(context, ref, table),
                      onToggleActive: (value) => ref
                          .read(tableManagementNotifierProvider(restaurantId)
                              .notifier)
                          .updateTable(table.id, {'isActive': value}),
                    );
                  },
                ),
              ),
      },
    );
  }

  void _showCreateDialog(BuildContext context, WidgetRef ref) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (_) => TableFormDialog(
        onSubmit: (data) async {
          final success = await ref
              .read(tableManagementNotifierProvider(restaurantId).notifier)
              .createTable(data);
          if (context.mounted && success) Navigator.of(context).pop();
        },
      ),
    );
  }

  void _showEditDialog(
      BuildContext context, WidgetRef ref, RestaurantTableModel table) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (_) => TableFormDialog(
        initialTableNumber: table.tableNumber,
        initialCapacity: table.capacity,
        initialFloorSection: table.floorSection,
        onSubmit: (data) async {
          final success = await ref
              .read(tableManagementNotifierProvider(restaurantId).notifier)
              .updateTable(table.id, data);
          if (context.mounted && success) Navigator.of(context).pop();
        },
      ),
    );
  }

  void _confirmDelete(
      BuildContext context, WidgetRef ref, RestaurantTableModel table) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Table'),
        content:
            Text('Are you sure you want to deactivate table "${table.tableNumber}"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              ref
                  .read(tableManagementNotifierProvider(restaurantId).notifier)
                  .deleteTable(table.id);
            },
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }
}

class _TableCard extends StatelessWidget {
  const _TableCard({
    required this.table,
    required this.theme,
    required this.statusLabel,
    required this.statusColor,
    required this.onEdit,
    required this.onDelete,
    required this.onToggleActive,
  });

  final RestaurantTableModel table;
  final ThemeData theme;
  final String statusLabel;
  final Color statusColor;
  final VoidCallback onEdit;
  final VoidCallback onDelete;
  final ValueChanged<bool> onToggleActive;

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppColors.primaryLight,
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Icon(Icons.table_restaurant,
                      color: AppColors.primary, size: 24),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Table ${table.tableNumber}',
                        style: theme.textTheme.titleSmall
                            ?.copyWith(fontWeight: FontWeight.w600),
                      ),
                      Text(
                        'Seats ${table.capacity}'
                        '${table.floorSection != null ? ' \u2022 ${table.floorSection}' : ''}',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                  ),
                ),
                // Status badge
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
            const Divider(height: 16),
            Row(
              children: [
                // Active toggle
                Text('Active', style: theme.textTheme.bodySmall),
                const SizedBox(width: 4),
                SizedBox(
                  height: 24,
                  child: Switch(
                    value: table.isActive,
                    activeColor: AppColors.success,
                    onChanged: onToggleActive,
                  ),
                ),
                const Spacer(),
                if (table.activeSessionCount > 0)
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: Colors.orange.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      'Session active',
                      style: TextStyle(
                        color: Colors.orange,
                        fontSize: 10,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                const SizedBox(width: 8),
                IconButton(
                  icon: const Icon(Icons.edit_outlined, size: 20),
                  onPressed: onEdit,
                  visualDensity: VisualDensity.compact,
                ),
                IconButton(
                  icon: Icon(Icons.delete_outline,
                      size: 20, color: AppColors.error),
                  onPressed: onDelete,
                  visualDensity: VisualDensity.compact,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
