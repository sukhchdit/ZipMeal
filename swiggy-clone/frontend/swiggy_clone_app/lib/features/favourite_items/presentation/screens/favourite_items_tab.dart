import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/favourite_item_model.dart';
import '../providers/favourite_items_notifier.dart';
import '../providers/favourite_items_state.dart';

class FavouriteItemsTab extends ConsumerWidget {
  const FavouriteItemsTab({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(favouriteItemsNotifierProvider);
    final theme = Theme.of(context);

    return switch (state) {
      FavouriteItemsInitial() || FavouriteItemsLoading() =>
        const AppLoadingWidget(message: 'Loading favourite dishes...'),
      FavouriteItemsError(:final failure) => AppErrorWidget(
          failure: failure,
          onRetry: () =>
              ref.read(favouriteItemsNotifierProvider.notifier).loadItems(),
        ),
      FavouriteItemsLoaded(:final items) => items.isEmpty
          ? Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Icon(
                    Icons.fastfood_outlined,
                    size: 64,
                    color: AppColors.textTertiaryLight,
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'No favourite dishes yet',
                    style: theme.textTheme.titleMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Tap the heart icon on dishes you love!',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ],
              ),
            )
          : RefreshIndicator(
              color: AppColors.primary,
              onRefresh: () =>
                  ref.read(favouriteItemsNotifierProvider.notifier).loadItems(),
              child: ListView.builder(
                padding: const EdgeInsets.only(top: 8, bottom: 24),
                itemCount: items.length,
                itemBuilder: (context, index) {
                  final item = items[index];
                  return Dismissible(
                    key: ValueKey(item.menuItemId),
                    direction: DismissDirection.endToStart,
                    background: Container(
                      alignment: Alignment.centerRight,
                      padding: const EdgeInsets.only(right: 24),
                      color: AppColors.error,
                      child: const Icon(Icons.favorite_border,
                          color: Colors.white),
                    ),
                    confirmDismiss: (_) =>
                        ref
                            .read(favouriteItemsNotifierProvider.notifier)
                            .removeItem(item.menuItemId),
                    child: _FavouriteItemCard(
                      item: item,
                      onTap: () => context.push(
                        RouteNames.restaurantDetailPath(item.restaurantId),
                      ),
                    ),
                  );
                },
              ),
            ),
    };
  }
}

class _FavouriteItemCard extends StatelessWidget {
  const _FavouriteItemCard({required this.item, required this.onTap});

  final FavouriteItemModel item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final displayPrice = item.discountedPrice ?? item.price;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Veg indicator
              Icon(
                item.isVeg ? Icons.circle : Icons.change_history,
                size: 14,
                color: item.isVeg ? AppColors.success : AppColors.error,
              ),
              const SizedBox(width: 8),

              // Item info
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.itemName,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      item.restaurantName,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Text(
                          '\u20B9${displayPrice ~/ 100}',
                          style: theme.textTheme.bodyMedium?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        if (item.discountedPrice != null) ...[
                          const SizedBox(width: 6),
                          Text(
                            '\u20B9${item.price ~/ 100}',
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: AppColors.textTertiaryLight,
                              decoration: TextDecoration.lineThrough,
                            ),
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              ),

              // Image
              if (item.imageUrl != null)
                ClipRRect(
                  borderRadius: BorderRadius.circular(8),
                  child: Image.network(
                    item.imageUrl!,
                    width: 60,
                    height: 60,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      width: 60,
                      height: 60,
                      color: AppColors.shimmerBase,
                      child: const Icon(Icons.fastfood_outlined, size: 24),
                    ),
                  ),
                ),

              // Availability indicator
              if (!item.isAvailable)
                Padding(
                  padding: const EdgeInsets.only(left: 8),
                  child: Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: AppColors.error.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      'N/A',
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: AppColors.error,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
