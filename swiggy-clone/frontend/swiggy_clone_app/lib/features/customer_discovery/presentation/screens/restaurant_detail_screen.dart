import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/public_restaurant_detail_model.dart';
import '../../../restaurant_management/data/models/menu_item_model.dart';
import '../providers/public_restaurant_detail_notifier.dart';
import '../providers/public_restaurant_detail_state.dart';
import 'menu_item_detail_sheet.dart';
import '../../../favourite_items/presentation/providers/favourite_item_ids_notifier.dart';
import '../../../reviews/presentation/widgets/restaurant_reviews_section.dart';
import '../../../promotions/presentation/providers/active_promotions_notifier.dart';
import '../../../promotions/presentation/widgets/flash_deal_banner.dart';
import '../../../promotions/presentation/widgets/combo_offer_card.dart';
import '../../../promotions/presentation/widgets/happy_hour_badge.dart';
import '../../../promotions/data/models/promotion_model.dart';

/// Customer-facing restaurant detail page showing banner, info, and full menu.
class RestaurantDetailScreen extends ConsumerWidget {
  const RestaurantDetailScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state =
        ref.watch(publicRestaurantDetailNotifierProvider(restaurantId));

    return Scaffold(
      body: switch (state) {
        PublicRestaurantDetailInitial() ||
        PublicRestaurantDetailLoading() =>
          const AppLoadingWidget(message: 'Loading restaurant...'),
        PublicRestaurantDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(publicRestaurantDetailNotifierProvider(restaurantId)
                    .notifier)
                .loadDetail(),
          ),
        PublicRestaurantDetailLoaded(
          :final restaurant,
          :final isFavourited
        ) =>
          _DetailBody(
            restaurant: restaurant,
            isFavourited: isFavourited,
            onToggleFavourite: () => ref
                .read(publicRestaurantDetailNotifierProvider(restaurantId)
                    .notifier)
                .toggleFavourite(),
          ),
      },
    );
  }
}

class _DetailBody extends ConsumerWidget {
  const _DetailBody({
    required this.restaurant,
    required this.isFavourited,
    required this.onToggleFavourite,
  });

  final PublicRestaurantDetailModel restaurant;
  final bool isFavourited;
  final VoidCallback onToggleFavourite;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final promoState =
        ref.watch(activePromotionsNotifierProvider(restaurant.id));

    // Extract promotions by type
    List<PromotionModel> flashDeals = [];
    List<PromotionModel> comboOffers = [];
    List<PromotionModel> happyHours = [];
    if (promoState is ActivePromotionsLoaded) {
      for (final p in promoState.promotions) {
        switch (p.promotionType) {
          case 0:
            flashDeals.add(p);
          case 1:
            happyHours.add(p);
          case 2:
            comboOffers.add(p);
        }
      }
    }

    return CustomScrollView(
      slivers: [
        // ── Sliver App Bar with banner ──
        SliverAppBar(
          expandedHeight: 200,
          pinned: true,
          actions: [
            IconButton(
              icon: Icon(
                isFavourited ? Icons.favorite : Icons.favorite_border,
                color: isFavourited ? AppColors.error : Colors.white,
              ),
              onPressed: onToggleFavourite,
            ),
          ],
          flexibleSpace: FlexibleSpaceBar(
            title: Text(
              restaurant.name,
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                shadows: [Shadow(blurRadius: 8, color: Colors.black54)],
              ),
            ),
            background: restaurant.bannerUrl != null
                ? Image.network(
                    restaurant.bannerUrl!,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      color: AppColors.primary,
                      child: const Center(
                        child: Icon(Icons.restaurant, size: 64,
                            color: Colors.white70),
                      ),
                    ),
                  )
                : Container(
                    color: AppColors.primary,
                    child: const Center(
                      child: Icon(Icons.restaurant, size: 64,
                          color: Colors.white70),
                    ),
                  ),
          ),
        ),

        // ── Restaurant Info ──
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Cuisines
                if (restaurant.cuisines.isNotEmpty)
                  Text(
                    restaurant.cuisines.join(' \u2022 '),
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                const SizedBox(height: 8),

                // Info chips row
                Row(
                  children: [
                    _InfoChip(
                      icon: Icons.star,
                      label: '${restaurant.averageRating.toStringAsFixed(1)} (${restaurant.totalRatings})',
                      color: AppColors.rating,
                    ),
                    const SizedBox(width: 12),
                    if (restaurant.avgDeliveryTimeMin != null)
                      _InfoChip(
                        icon: Icons.access_time,
                        label: '${restaurant.avgDeliveryTimeMin} min',
                        color: AppColors.info,
                      ),
                    if (restaurant.avgCostForTwo != null) ...[
                      const SizedBox(width: 12),
                      _InfoChip(
                        icon: Icons.currency_rupee,
                        label: '${restaurant.avgCostForTwo! ~/ 100} for two',
                        color: AppColors.textSecondaryLight,
                      ),
                    ],
                  ],
                ),
                const SizedBox(height: 8),

                // Address
                if (restaurant.addressLine1 != null)
                  Text(
                    [
                      restaurant.addressLine1,
                      restaurant.city,
                      restaurant.postalCode,
                    ].where((e) => e != null && e.isNotEmpty).join(', '),
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),

                // Status badges
                const SizedBox(height: 8),
                Wrap(
                  spacing: 8,
                  children: [
                    if (restaurant.isVegOnly)
                      _Badge(label: 'Pure Veg', color: AppColors.success),
                    if (restaurant.isDineInEnabled)
                      _Badge(label: 'Dine-In Available', color: AppColors.info),
                    if (!restaurant.isAcceptingOrders)
                      _Badge(label: 'Currently Closed', color: AppColors.error),
                  ],
                ),

                if (restaurant.description != null &&
                    restaurant.description!.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    restaurant.description!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                ],

                const Divider(height: 32),
              ],
            ),
          ),
        ),

        // ── Active Promotions ──
        if (flashDeals.isNotEmpty)
          SliverToBoxAdapter(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Padding(
                  padding: const EdgeInsets.fromLTRB(16, 0, 16, 8),
                  child: Row(
                    children: [
                      const Icon(Icons.flash_on, color: AppColors.error, size: 20),
                      const SizedBox(width: 4),
                      Text('Flash Deals',
                          style: theme.textTheme.titleMedium
                              ?.copyWith(fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
                SizedBox(
                  height: 160,
                  child: ListView.builder(
                    scrollDirection: Axis.horizontal,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: flashDeals.length,
                    itemBuilder: (context, index) =>
                        FlashDealBanner(promotion: flashDeals[index]),
                  ),
                ),
                const SizedBox(height: 8),
              ],
            ),
          ),

        if (comboOffers.isNotEmpty)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 8),
                    child: Row(
                      children: [
                        const Icon(Icons.local_offer,
                            color: AppColors.info, size: 20),
                        const SizedBox(width: 4),
                        Text('Combo Offers',
                            style: theme.textTheme.titleMedium
                                ?.copyWith(fontWeight: FontWeight.bold)),
                      ],
                    ),
                  ),
                  ...comboOffers
                      .map((c) => ComboOfferCard(promotion: c)),
                ],
              ),
            ),
          ),

        if (happyHours.isNotEmpty)
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 0, 16, 8),
              child: Wrap(
                spacing: 8,
                runSpacing: 4,
                children: happyHours
                    .map((h) => HappyHourBadge(
                          discountValue: h.discountValue,
                          discountType: h.discountType,
                          startTime: h.recurringStartTime,
                          endTime: h.recurringEndTime,
                        ))
                    .toList(),
              ),
            ),
          ),

        // ── Menu Sections ──
        if (restaurant.menuSections.isEmpty)
          const SliverToBoxAdapter(
            child: Center(
              child: Padding(
                padding: EdgeInsets.all(32),
                child: Text('No menu items available.'),
              ),
            ),
          )
        else
          ...restaurant.menuSections.expand((section) => [
                SliverToBoxAdapter(
                  child: Padding(
                    padding: const EdgeInsets.fromLTRB(16, 8, 16, 8),
                    child: Text(
                      section.categoryName,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ),
                SliverList.builder(
                  itemCount: section.items.length,
                  itemBuilder: (context, index) {
                    final item = section.items[index];
                    return _MenuItemCard(
                      item: item,
                      onTap: () => _showItemDetail(context, item),
                    );
                  },
                ),
              ]),

        // ── Reviews Section ──
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.only(top: 16),
            child: RestaurantReviewsSection(
              restaurantId: restaurant.id,
            ),
          ),
        ),

        const SliverToBoxAdapter(child: SizedBox(height: 40)),
      ],
    );
  }

  void _showItemDetail(BuildContext context, MenuItemModel item) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (_) => MenuItemDetailSheet(
        item: item,
        restaurantId: restaurant.id,
      ),
    );
  }
}

class _MenuItemCard extends ConsumerWidget {
  const _MenuItemCard({required this.item, required this.onTap});

  final MenuItemModel item;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final displayPrice = item.discountedPrice ?? item.price;
    final favIds = ref.watch(favouriteItemIdsNotifierProvider);
    final isFav = favIds.contains(item.id);

    return InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Veg/non-veg indicator
            Padding(
              padding: const EdgeInsets.only(top: 3),
              child: Icon(
                item.isVeg ? Icons.circle : Icons.change_history,
                size: 14,
                color: item.isVeg ? AppColors.success : AppColors.error,
              ),
            ),
            const SizedBox(width: 8),

            // Details
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          item.name,
                          style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                      if (item.isBestseller)
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.primary.withValues(alpha: 0.12),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            'BESTSELLER',
                            style: theme.textTheme.labelSmall?.copyWith(
                              color: AppColors.primary,
                              fontWeight: FontWeight.bold,
                              fontSize: 9,
                            ),
                          ),
                        ),
                    ],
                  ),
                  const SizedBox(height: 2),
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
                  if (item.description != null &&
                      item.description!.isNotEmpty) ...[
                    const SizedBox(height: 4),
                    Text(
                      item.description!,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ],
              ),
            ),

            // Image + Heart + Add button
            const SizedBox(width: 12),
            Column(
              children: [
                Stack(
                  children: [
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: SizedBox(
                        width: 80,
                        height: 80,
                        child: item.imageUrl != null
                            ? Image.network(
                                item.imageUrl!,
                                fit: BoxFit.cover,
                                errorBuilder: (_, __, ___) => Container(
                                  color: AppColors.shimmerBase,
                                  child: const Icon(Icons.fastfood_outlined),
                                ),
                              )
                            : Container(
                                color: AppColors.shimmerBase,
                                child: const Icon(Icons.fastfood_outlined,
                                    color: AppColors.textTertiaryLight),
                              ),
                      ),
                    ),
                    Positioned(
                      top: 2,
                      right: 2,
                      child: IconButton(
                        icon: Icon(
                          isFav ? Icons.favorite : Icons.favorite_border,
                          color: isFav
                              ? AppColors.error
                              : AppColors.textTertiaryLight,
                          size: 20,
                        ),
                        onPressed: () => ref
                            .read(favouriteItemIdsNotifierProvider.notifier)
                            .toggle(item.id),
                        visualDensity: VisualDensity.compact,
                        padding: EdgeInsets.zero,
                        constraints: const BoxConstraints(),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                SizedBox(
                  height: 28,
                  child: OutlinedButton(
                    onPressed: onTap,
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(horizontal: 16),
                      foregroundColor: AppColors.primary,
                      side: const BorderSide(color: AppColors.primary),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(6),
                      ),
                      minimumSize: Size.zero,
                      tapTargetSize: MaterialTapTargetSize.shrinkWrap,
                    ),
                    child: const Text('ADD', style: TextStyle(fontSize: 12)),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _InfoChip extends StatelessWidget {
  const _InfoChip({
    required this.icon,
    required this.label,
    required this.color,
  });

  final IconData icon;
  final String label;
  final Color color;

  @override
  Widget build(BuildContext context) => Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: color),
          const SizedBox(width: 3),
          Text(
            label,
            style: Theme.of(context)
                .textTheme
                .bodySmall
                ?.copyWith(fontWeight: FontWeight.w600),
          ),
        ],
      );
}

class _Badge extends StatelessWidget {
  const _Badge({required this.label, required this.color});

  final String label;
  final Color color;

  @override
  Widget build(BuildContext context) => Container(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.12),
          borderRadius: BorderRadius.circular(4),
        ),
        child: Text(
          label,
          style: Theme.of(context).textTheme.labelSmall?.copyWith(
                color: color,
                fontWeight: FontWeight.w600,
              ),
        ),
      );
}
