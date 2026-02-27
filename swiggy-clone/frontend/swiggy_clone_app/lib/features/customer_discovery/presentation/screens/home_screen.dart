import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../../notifications/presentation/providers/unread_count_notifier.dart';
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
                'Deliver to Home',
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
          const AppLoadingWidget(message: 'Loading...'),
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

class _HomeFeedBody extends StatelessWidget {
  const _HomeFeedBody({required this.feed});

  final HomeFeedModel feed;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

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
                                Text('Special Offer',
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
              'What\'s on your mind?',
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

        const SizedBox(height: 24),
      ],
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
