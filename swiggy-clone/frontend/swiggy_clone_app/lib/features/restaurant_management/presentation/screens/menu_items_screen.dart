import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/menu_item_model.dart';
import '../providers/menu_items_notifier.dart';
import '../providers/menu_items_state.dart';
import '../widgets/menu_item_tile.dart';

/// Lists all menu items in a given category. Supports adding new items,
/// editing existing items, and swipe-to-delete.
class MenuItemsScreen extends ConsumerStatefulWidget {
  const MenuItemsScreen({
    required this.restaurantId,
    required this.categoryId,
    super.key,
  });

  final String restaurantId;
  final String categoryId;

  @override
  ConsumerState<MenuItemsScreen> createState() => _MenuItemsScreenState();
}

class _MenuItemsScreenState extends ConsumerState<MenuItemsScreen> {
  Future<void> _confirmDelete(MenuItemModel item) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Item'),
        content: Text(
            'Are you sure you want to delete "${item.name}"? This cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: TextButton.styleFrom(foregroundColor: AppColors.error),
            child: const Text('Delete'),
          ),
        ],
      ),
    );

    if (confirmed != true || !mounted) return;

    final success = await ref
        .read(menuItemsNotifierProvider(widget.restaurantId, widget.categoryId)
            .notifier)
        .deleteItem(item.id);

    if (!mounted) return;
    if (!success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to delete item.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  void _navigateToForm({String? itemId}) {
    context.push(
      RouteNames.menuItemFormPath(widget.restaurantId),
      extra: {
        'categoryId': widget.categoryId,
        if (itemId != null) 'itemId': itemId,
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
        menuItemsNotifierProvider(widget.restaurantId, widget.categoryId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Menu Items'),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _navigateToForm(),
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
        child: const Icon(Icons.add),
      ),
      body: switch (state) {
        MenuItemsInitial() || MenuItemsLoading() =>
          const AppLoadingWidget(message: 'Loading items...'),
        MenuItemsError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(menuItemsNotifierProvider(
                        widget.restaurantId, widget.categoryId)
                    .notifier)
                .loadItems(),
          ),
        MenuItemsLoaded(:final items) => items.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.fastfood_outlined,
                      size: 64,
                      color: AppColors.textTertiaryLight,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'No items in this category',
                      style: theme.textTheme.titleMedium?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                    const SizedBox(height: 8),
                    FilledButton.icon(
                      onPressed: () => _navigateToForm(),
                      icon: const Icon(Icons.add),
                      label: const Text('Add Item'),
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                      ),
                    ),
                  ],
                ),
              )
            : RefreshIndicator(
                color: AppColors.primary,
                onRefresh: () => ref
                    .read(menuItemsNotifierProvider(
                            widget.restaurantId, widget.categoryId)
                        .notifier)
                    .loadItems(),
                child: ListView.builder(
                  padding: const EdgeInsets.only(top: 8, bottom: 80),
                  itemCount: items.length,
                  itemBuilder: (context, index) {
                    final item = items[index];
                    return Dismissible(
                      key: ValueKey(item.id),
                      direction: DismissDirection.endToStart,
                      background: Container(
                        alignment: Alignment.centerRight,
                        padding: const EdgeInsets.only(right: 24),
                        color: AppColors.error,
                        child: const Icon(Icons.delete_outline,
                            color: Colors.white),
                      ),
                      confirmDismiss: (_) async {
                        await _confirmDelete(item);
                        return false; // Handled by provider reload
                      },
                      child: MenuItemTile(
                        item: item,
                        onTap: () => _navigateToForm(itemId: item.id),
                        onAvailabilityChanged: (value) async {
                          await ref
                              .read(menuItemsNotifierProvider(
                                      widget.restaurantId, widget.categoryId)
                                  .notifier)
                              .updateItem(item.id, {'isAvailable': value});
                        },
                      ),
                    );
                  },
                ),
              ),
      },
    );
  }
}
