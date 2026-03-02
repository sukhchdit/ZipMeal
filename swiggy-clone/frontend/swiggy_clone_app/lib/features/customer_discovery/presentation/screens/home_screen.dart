import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../../auth/presentation/providers/auth_notifier.dart';
import '../../../auth/presentation/providers/auth_state.dart';
import '../../../notifications/presentation/providers/unread_count_notifier.dart';
import '../../../recommendations/presentation/providers/personalized_recommendations_notifier.dart';
import '../../../recommendations/presentation/providers/trending_items_notifier.dart';
import '../../data/models/home_feed_model.dart';
import '../providers/home_feed_notifier.dart';
import '../providers/home_feed_state.dart';
import '../widgets/restaurant_card.dart';

/// Main home screen showing banners, cuisine chips, and restaurant sections.
class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(homeFeedNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Row(
          children: [
            Icon(Icons.location_on, color: AppColors.primary, size: 20),
            const SizedBox(width: 4),
            Expanded(
              child: Text(
                context.l10n.deliverToHome,
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
                overflow: TextOverflow.ellipsis,
              ),
            ),
          ],
        ),
        actions: [
          _NotificationBell(),
          IconButton(
            icon: const Icon(Icons.search),
            onPressed: () => context.go(RouteNames.search),
          ),
        ],
      ),
      body: switch (state) {
        HomeFeedInitial() || HomeFeedLoading() =>
          AppLoadingWidget(message: context.l10n.loading),
        HomeFeedError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () =>
                ref.read(homeFeedNotifierProvider.notifier).loadFeed(),
          ),
        HomeFeedLoaded(:final feed) => RefreshIndicator(
            color: AppColors.primary,
            onRefresh: () =>
                ref.read(homeFeedNotifierProvider.notifier).loadFeed(),
            child: _HomeFeedBody(feed: feed),
          ),
      },
    );
  }
}

class _HomeFeedBody extends ConsumerWidget {
  const _HomeFeedBody({required this.feed});

  final HomeFeedModel feed;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final authState = ref.watch(authNotifierProvider);
    final isAuthenticated = authState is AuthAuthenticated;

    return ListView(
      children: [
        // ── Banners ──
        if (feed.banners.isNotEmpty)
          SizedBox(
            height: 160,
            child: PageView.builder(
              itemCount: feed.banners.length,
              itemBuilder: (context, index) {
                final banner = feed.banners[index];
                return GestureDetector(
                  onTap: banner.deepLink != null
                      ? () => context.push(banner.deepLink!)
                      : null,
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 8),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(12),
                      child: Container(
                        color: AppColors.primaryLight,
                        child: Image.network(
                          banner.imageUrl,
                          fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => Center(
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Icon(Icons.local_offer,
                                    size: 40, color: AppColors.primary),
                                const SizedBox(height: 4),
                                Text(context.l10n.specialOffer,
                                    style: theme.textTheme.labelLarge
                                        ?.copyWith(color: AppColors.primary)),
                              ],
                            ),
                          ),
                        ),
                      ),
                    ),
                  ),
                );
              },
            ),
          ),

        // ── Cuisine Chips ──
        if (feed.cuisineChips.isNotEmpty) ...[
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
            child: Text(
              context.l10n.whatsOnYourMind,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          SizedBox(
            height: 48,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 16),
              itemCount: feed.cuisineChips.length,
              separatorBuilder: (_, __) => const SizedBox(width: 8),
              itemBuilder: (context, index) {
                final chip = feed.cuisineChips[index];
                return ActionChip(
                  avatar: chip.iconUrl != null
                      ? CircleAvatar(
                          backgroundImage: NetworkImage(chip.iconUrl!),
                          radius: 12,
                        )
                      : null,
                  label: Text(chip.name),
                  onPressed: () {
                    // Navigate to browse with cuisine filter
                    context.go(
                      '${RouteNames.search}?cuisineId=${chip.id}&cuisineName=${chip.name}',
                    );
                  },
                  backgroundColor: AppColors.primaryLight,
                  side: BorderSide.none,
                );
              },
            ),
          ),
        ],

        // ── Restaurant Sections ──
        ...feed.sections.map((section) => _RestaurantSection(section: section)),

        // ── Personalized Recommendations (auth users only) ──
        if (isAuthenticated) ...[
          _PersonalizedRecommendationsSection(),
          _TrendingNearYouSection(),
        ],

        // ── Trending for everyone ──
        if (!isAuthenticated) _TrendingNearYouSection(),

        const SizedBox(height: 24),
      ],
    );
  }
}

class _PersonalizedRecommendationsSection extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(personalizedRecommendationsNotifierProvider);
    final theme = Theme.of(context);

    return state.map(
      initial: (_) => const SizedBox.shrink(),
      loading: (_) => Padding(
        padding: const EdgeInsets.symmetric(vertical: 16),
        child: Center(
          child: SizedBox(
            height: 24,
            width: 24,
            child: CircularProgressIndicator(strokeWidth: 2),
          ),
        ),
      ),
      error: (_) => const SizedBox.shrink(),
      loaded: (loaded) {
        final recs = loaded.recommendations;
        if (recs.recommendedRestaurants.isEmpty &&
            recs.recommendedDishes.isEmpty) {
          return const SizedBox.shrink();
        }

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Recommended Restaurants
            if (recs.recommendedRestaurants.isNotEmpty) ...[
              Padding(
                padding: const EdgeInsets.fromLTRB(16, 24, 16, 8),
                child: Text(
                  context.l10n.recommendedForYou,
                  style: theme.textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              SizedBox(
                height: 180,
                child: ListView.separated(
                  scrollDirection: Axis.horizontal,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemCount: recs.recommendedRestaurants.length,
                  separatorBuilder: (_, __) => const SizedBox(width: 12),
                  itemBuilder: (context, index) {
                    final r = recs.recommendedRestaurants[index];
                    return _RecommendedRestaurantCard(
                      name: r.name,
                      cuisines: r.cuisines.take(2).join(', '),
                      rating: r.averageRating,
                      deliveryTime: r.avgDeliveryTimeMin,
                      reason: r.recommendationReason,
                      imageUrl: r.logoUrl,
                      onTap: () => context.push(
                        RouteNames.restaurantDetailPath(r.id),
                      ),
                    );
                  },
                ),
              ),
            ],

            // Recommended Dishes
            if (recs.recommendedDishes.isNotEmpty) ...[
              Padding(
                padding: const EdgeInsets.fromLTRB(16, 24, 16, 8),
                child: Text(
                  context.l10n.dishesYoullLove,
                  style: theme.textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              SizedBox(
                height: 140,
                child: ListView.separated(
                  scrollDirection: Axis.horizontal,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemCount: recs.recommendedDishes.length,
                  separatorBuilder: (_, __) => const SizedBox(width: 12),
                  itemBuilder: (context, index) {
                    final d = recs.recommendedDishes[index];
                    return _RecommendedDishCard(
                      name: d.name,
                      restaurantName: d.restaurantName,
                      price: d.discountedPrice ?? d.price,
                      reason: d.recommendationReason,
                      imageUrl: d.imageUrl,
                      isVeg: d.isVeg,
                      onTap: () => context.push(
                        RouteNames.restaurantDetailPath(d.restaurantId),
                      ),
                    );
                  },
                ),
              ),
            ],
          ],
        );
      },
    );
  }
}

class _TrendingNearYouSection extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(trendingItemsNotifierProvider);
    final theme = Theme.of(context);

    return state.map(
      initial: (_) => const SizedBox.shrink(),
      loading: (_) => const SizedBox.shrink(),
      error: (_) => const SizedBox.shrink(),
      loaded: (loaded) {
        if (loaded.items.isEmpty) return const SizedBox.shrink();

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 24, 16, 8),
              child: Text(
                context.l10n.trendingNearYou,
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            SizedBox(
              height: 140,
              child: ListView.separated(
                scrollDirection: Axis.horizontal,
                padding: const EdgeInsets.symmetric(horizontal: 16),
                itemCount: loaded.items.length,
                separatorBuilder: (_, __) => const SizedBox(width: 12),
                itemBuilder: (context, index) {
                  final item = loaded.items[index];
                  return _TrendingItemCard(
                    name: item.name,
                    restaurantName: item.restaurantName,
                    price: item.price,
                    rank: item.trendRank,
                    orderCount: item.orderCount,
                    imageUrl: item.imageUrl,
                    isVeg: item.isVeg,
                    onTap: () => context.push(
                      RouteNames.restaurantDetailPath(item.restaurantId),
                    ),
                  );
                },
              ),
            ),
          ],
        );
      },
    );
  }
}

class _RecommendedRestaurantCard extends StatelessWidget {
  const _RecommendedRestaurantCard({
    required this.name,
    required this.cuisines,
    required this.rating,
    this.deliveryTime,
    this.reason,
    this.imageUrl,
    this.onTap,
  });

  final String name;
  final String cuisines;
  final double rating;
  final int? deliveryTime;
  final String? reason;
  final String? imageUrl;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return GestureDetector(
      onTap: onTap,
      child: SizedBox(
        width: 160,
        child: Card(
          clipBehavior: Clip.antiAlias,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                height: 80,
                width: double.infinity,
                color: AppColors.primaryLight,
                child: imageUrl != null
                    ? Image.network(imageUrl!, fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => Icon(
                              Icons.restaurant,
                              size: 32,
                              color: AppColors.primary,
                            ))
                    : Icon(Icons.restaurant,
                        size: 32, color: AppColors.primary),
              ),
              Padding(
                padding: const EdgeInsets.all(8),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(name,
                        style: theme.textTheme.bodySmall
                            ?.copyWith(fontWeight: FontWeight.bold),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis),
                    const SizedBox(height: 2),
                    Text(cuisines,
                        style: theme.textTheme.labelSmall
                            ?.copyWith(color: Colors.grey),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Icon(Icons.star, size: 12, color: Colors.amber),
                        const SizedBox(width: 2),
                        Text(rating.toStringAsFixed(1),
                            style: theme.textTheme.labelSmall),
                        if (deliveryTime != null) ...[
                          const SizedBox(width: 8),
                          Icon(Icons.access_time,
                              size: 12, color: Colors.grey),
                          const SizedBox(width: 2),
                          Text('${deliveryTime}m',
                              style: theme.textTheme.labelSmall),
                        ],
                      ],
                    ),
                    if (reason != null) ...[
                      const SizedBox(height: 4),
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 6, vertical: 2),
                        decoration: BoxDecoration(
                          color: AppColors.primaryLight,
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Text(reason!,
                            style: theme.textTheme.labelSmall
                                ?.copyWith(color: AppColors.primary),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis),
                      ),
                    ],
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _RecommendedDishCard extends StatelessWidget {
  const _RecommendedDishCard({
    required this.name,
    required this.restaurantName,
    required this.price,
    this.reason,
    this.imageUrl,
    required this.isVeg,
    this.onTap,
  });

  final String name;
  final String restaurantName;
  final int price;
  final String? reason;
  final String? imageUrl;
  final bool isVeg;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return GestureDetector(
      onTap: onTap,
      child: SizedBox(
        width: 140,
        child: Card(
          clipBehavior: Clip.antiAlias,
          child: Row(
            children: [
              Container(
                width: 60,
                height: double.infinity,
                color: AppColors.primaryLight,
                child: imageUrl != null
                    ? Image.network(imageUrl!, fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => Icon(
                              Icons.fastfood,
                              size: 24,
                              color: AppColors.primary,
                            ))
                    : Icon(Icons.fastfood, size: 24, color: AppColors.primary),
              ),
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.all(8),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Row(
                        children: [
                          Icon(
                            Icons.circle,
                            size: 10,
                            color: isVeg ? Colors.green : Colors.red,
                          ),
                          const SizedBox(width: 4),
                          Expanded(
                            child: Text(name,
                                style: theme.textTheme.bodySmall
                                    ?.copyWith(fontWeight: FontWeight.bold),
                                maxLines: 1,
                                overflow: TextOverflow.ellipsis),
                          ),
                        ],
                      ),
                      const SizedBox(height: 2),
                      Text(restaurantName,
                          style: theme.textTheme.labelSmall
                              ?.copyWith(color: Colors.grey),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis),
                      const SizedBox(height: 2),
                      Text(
                        '₹${(price / 100).toStringAsFixed(0)}',
                        style: theme.textTheme.bodySmall
                            ?.copyWith(fontWeight: FontWeight.bold),
                      ),
                      if (reason != null) ...[
                        const SizedBox(height: 2),
                        Text(reason!,
                            style: theme.textTheme.labelSmall
                                ?.copyWith(color: AppColors.primary),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis),
                      ],
                    ],
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

class _TrendingItemCard extends StatelessWidget {
  const _TrendingItemCard({
    required this.name,
    required this.restaurantName,
    required this.price,
    required this.rank,
    required this.orderCount,
    this.imageUrl,
    required this.isVeg,
    this.onTap,
  });

  final String name;
  final String restaurantName;
  final int price;
  final int rank;
  final int orderCount;
  final String? imageUrl;
  final bool isVeg;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return GestureDetector(
      onTap: onTap,
      child: SizedBox(
        width: 140,
        child: Card(
          clipBehavior: Clip.antiAlias,
          child: Stack(
            children: [
              Row(
                children: [
                  Container(
                    width: 60,
                    height: double.infinity,
                    color: AppColors.primaryLight,
                    child: imageUrl != null
                        ? Image.network(imageUrl!, fit: BoxFit.cover,
                            errorBuilder: (_, __, ___) => Icon(
                                  Icons.local_fire_department,
                                  size: 24,
                                  color: Colors.orange,
                                ))
                        : Icon(Icons.local_fire_department,
                            size: 24, color: Colors.orange),
                  ),
                  Expanded(
                    child: Padding(
                      padding: const EdgeInsets.all(8),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Row(
                            children: [
                              Icon(Icons.circle,
                                  size: 10,
                                  color: isVeg ? Colors.green : Colors.red),
                              const SizedBox(width: 4),
                              Expanded(
                                child: Text(name,
                                    style: theme.textTheme.bodySmall
                                        ?.copyWith(
                                            fontWeight: FontWeight.bold),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis),
                              ),
                            ],
                          ),
                          const SizedBox(height: 2),
                          Text(restaurantName,
                              style: theme.textTheme.labelSmall
                                  ?.copyWith(color: Colors.grey),
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis),
                          const SizedBox(height: 2),
                          Text(
                            '₹${(price / 100).toStringAsFixed(0)}',
                            style: theme.textTheme.bodySmall
                                ?.copyWith(fontWeight: FontWeight.bold),
                          ),
                          const SizedBox(height: 2),
                          Text(
                            context.l10n.ordersToday(orderCount),
                            style: theme.textTheme.labelSmall
                                ?.copyWith(color: Colors.orange),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
              Positioned(
                top: 4,
                left: 4,
                child: Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                  decoration: BoxDecoration(
                    color: Colors.orange,
                    borderRadius: BorderRadius.circular(4),
                  ),
                  child: Text(
                    '#$rank',
                    style: theme.textTheme.labelSmall
                        ?.copyWith(color: Colors.white),
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

class _RestaurantSection extends StatelessWidget {
  const _RestaurantSection({required this.section});

  final RestaurantSectionModel section;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 24, 16, 8),
          child: Text(
            section.title,
            style: theme.textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        ...section.restaurants.map(
          (restaurant) => CustomerRestaurantCard(
            restaurant: restaurant,
            onTap: () => context.push(
              RouteNames.restaurantDetailPath(restaurant.id),
            ),
          ),
        ),
      ],
    );
  }
}

class _NotificationBell extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final unreadCount = ref.watch(unreadCountNotifierProvider);

    return IconButton(
      icon: Badge(
        isLabelVisible: unreadCount > 0,
        label: Text(
          unreadCount > 99 ? '99+' : unreadCount.toString(),
          style: const TextStyle(fontSize: 10),
        ),
        child: const Icon(Icons.notifications_outlined),
      ),
      onPressed: () => context.push(RouteNames.notifications),
    );
  }
}
