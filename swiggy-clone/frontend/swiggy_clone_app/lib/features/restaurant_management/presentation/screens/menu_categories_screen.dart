import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/menu_category_model.dart';
import '../providers/menu_categories_notifier.dart';
import '../providers/menu_categories_state.dart';
import '../widgets/menu_category_tile.dart';

/// Lists menu categories for a restaurant. Provides add, edit, and delete
/// capabilities via bottom sheet dialogs.
class MenuCategoriesScreen extends ConsumerStatefulWidget {
  const MenuCategoriesScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  ConsumerState<MenuCategoriesScreen> createState() =>
      _MenuCategoriesScreenState();
}

class _MenuCategoriesScreenState extends ConsumerState<MenuCategoriesScreen> {
  Future<void> _showCategoryDialog({MenuCategoryModel? existing}) async {
    final nameController =
        TextEditingController(text: existing?.name ?? '');
    final descController =
        TextEditingController(text: existing?.description ?? '');
    final sortController = TextEditingController(
        text: existing?.sortOrder.toString() ?? '0');
    final formKey = GlobalKey<FormState>();

    final confirmed = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => Padding(
        padding: EdgeInsets.only(
          left: 20,
          right: 20,
          top: 20,
          bottom: MediaQuery.of(ctx).viewInsets.bottom + 20,
        ),
        child: Form(
          key: formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                existing != null ? 'Edit Category' : 'New Category',
                style: Theme.of(ctx).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: nameController,
                decoration: const InputDecoration(
                  labelText: 'Category Name *',
                  prefixIcon: Icon(Icons.label_outlined),
                ),
                textCapitalization: TextCapitalization.words,
                autofocus: true,
                validator: (v) => v == null || v.trim().isEmpty
                    ? 'Name is required'
                    : null,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: descController,
                decoration: const InputDecoration(
                  labelText: 'Description',
                  prefixIcon: Icon(Icons.notes_outlined),
                ),
                maxLines: 2,
                minLines: 1,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: sortController,
                decoration: const InputDecoration(
                  labelText: 'Sort Order',
                  prefixIcon: Icon(Icons.sort_outlined),
                ),
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
              ),
              const SizedBox(height: 20),
              FilledButton(
                onPressed: () {
                  if (formKey.currentState!.validate()) {
                    Navigator.of(ctx).pop(true);
                  }
                },
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  minimumSize: const Size.fromHeight(48),
                ),
                child: Text(existing != null ? 'Update' : 'Create'),
              ),
            ],
          ),
        ),
      ),
    );

    if (confirmed != true || !mounted) return;

    final data = <String, dynamic>{
      'name': nameController.text.trim(),
      'description': descController.text.trim(),
      'sortOrder': int.tryParse(sortController.text.trim()) ?? 0,
    };

    final notifier = ref.read(
        menuCategoriesNotifierProvider(widget.restaurantId).notifier);

    bool success;
    if (existing != null) {
      success = await notifier.updateCategory(existing.id, data);
    } else {
      success = await notifier.createCategory(data);
    }

    if (!mounted) return;
    if (!success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Operation failed. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    }

    nameController.dispose();
    descController.dispose();
    sortController.dispose();
  }

  Future<void> _confirmDelete(MenuCategoryModel category) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Category'),
        content: Text(
            'Are you sure you want to delete "${category.name}"? This action cannot be undone.'),
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
        .read(menuCategoriesNotifierProvider(widget.restaurantId).notifier)
        .deleteCategory(category.id);

    if (!mounted) return;
    if (!success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to delete category.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final state =
        ref.watch(menuCategoriesNotifierProvider(widget.restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Menu Categories'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => _showCategoryDialog(),
            tooltip: 'Add Category',
          ),
        ],
      ),
      body: switch (state) {
        MenuCategoriesInitial() || MenuCategoriesLoading() =>
          const AppLoadingWidget(message: 'Loading categories...'),
        MenuCategoriesError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(menuCategoriesNotifierProvider(widget.restaurantId)
                    .notifier)
                .loadCategories(),
          ),
        MenuCategoriesLoaded(:final categories) => categories.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.category_outlined,
                      size: 64,
                      color: AppColors.textTertiaryLight,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'No categories yet',
                      style: theme.textTheme.titleMedium?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                    const SizedBox(height: 8),
                    FilledButton.icon(
                      onPressed: () => _showCategoryDialog(),
                      icon: const Icon(Icons.add),
                      label: const Text('Add Category'),
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
                    .read(menuCategoriesNotifierProvider(widget.restaurantId)
                        .notifier)
                    .loadCategories(),
                child: ListView.builder(
                  padding: const EdgeInsets.only(top: 8, bottom: 16),
                  itemCount: categories.length,
                  itemBuilder: (context, index) {
                    final category = categories[index];
                    return MenuCategoryTile(
                      category: category,
                      onTap: () => context.push(
                        RouteNames.menuItemsPath(
                          widget.restaurantId,
                          category.id,
                        ),
                      ),
                      onEdit: () =>
                          _showCategoryDialog(existing: category),
                      onDelete: () => _confirmDelete(category),
                    );
                  },
                ),
              ),
      },
    );
  }
}
