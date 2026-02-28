import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../restaurant_management/data/models/menu_item_model.dart';
import '../providers/dine_in_menu_notifier.dart';
import '../providers/dine_in_menu_state.dart';
import '../widgets/dine_in_menu_item_detail_sheet.dart';

class DineInMenuScreen extends ConsumerWidget {
  const DineInMenuScreen({
    required this.sessionId,
    required this.restaurantId,
    super.key,
  });

  final String sessionId;
  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(dineInMenuNotifierProvider(sessionId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Menu')),
      body: switch (state) {
        DineInMenuInitial() || DineInMenuLoading() =>
          const AppLoadingWidget(message: 'Loading menu...'),
        DineInMenuError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(dineInMenuNotifierProvider(sessionId).notifier)
                .loadMenu(),
          ),
        DineInMenuLoaded(:final restaurant) => ListView.builder(
            itemCount: restaurant.menuSections.length,
            itemBuilder: (context, sectionIndex) {
              final section = restaurant.menuSections[sectionIndex];
              return Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(16, 20, 16, 8),
                    child: Text(
                      section.categoryName,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  ...section.items.map(
                    (item) => _MenuItemTile(
                      item: item,
                      onTap: () => _showItemDetail(context, item),
                    ),
                  ),
                  if (sectionIndex < restaurant.menuSections.length - 1)
                    const Divider(indent: 16, endIndent: 16),
                ],
              );
            },
          ),
      },
    );
  }

  void _showItemDetail(BuildContext context, MenuItemModel item) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => DineInMenuItemDetailSheet(
        item: item,
        sessionId: sessionId,
      ),
    );
  }
}

class _MenuItemTile extends StatelessWidget {
  const _MenuItemTile({required this.item, this.onTap});

  final MenuItemModel item;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Veg/Non-veg indicator
            Container(
              margin: const EdgeInsets.only(top: 4),
              padding: const EdgeInsets.all(2),
              decoration: BoxDecoration(
                border: Border.all(
                  color: item.isVeg ? Colors.green : Colors.red,
                  width: 1.5,
                ),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Icon(Icons.circle,
                  size: 8, color: item.isVeg ? Colors.green : Colors.red),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.name,
                    style: theme.textTheme.titleSmall
                        ?.copyWith(fontWeight: FontWeight.w500),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    '\u20B9${(item.price / 100).toStringAsFixed(0)}',
                    style: theme.textTheme.bodyMedium,
                  ),
                  if (item.description != null) ...[
                    const SizedBox(height: 2),
                    Text(
                      item.description!,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            if (item.imageUrl != null) ...[
              const SizedBox(width: 12),
              Stack(
                alignment: Alignment.bottomCenter,
                children: [
                  ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: Image.network(
                      item.imageUrl!,
                      width: 80,
                      height: 80,
                      fit: BoxFit.cover,
                    ),
                  ),
                  Transform.translate(
                    offset: const Offset(0, 12),
                    child: SizedBox(
                      height: 28,
                      child: FilledButton(
                        onPressed: onTap,
                        style: FilledButton.styleFrom(
                          backgroundColor: Colors.white,
                          foregroundColor: AppColors.primary,
                          padding: const EdgeInsets.symmetric(horizontal: 16),
                          side: BorderSide(color: AppColors.primary),
                          minimumSize: Size.zero,
                          textStyle: const TextStyle(
                            fontSize: 13,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        child: const Text('ADD'),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}
