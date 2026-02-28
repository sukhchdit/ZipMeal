import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../../favourite_items/presentation/screens/favourite_items_tab.dart';
import '../providers/favourites_notifier.dart';
import '../providers/favourites_state.dart';
import '../widgets/restaurant_card.dart';

/// Screen displaying the user's favourited restaurants and dishes in tabs.
class FavouritesScreen extends StatelessWidget {
  const FavouritesScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Favourites'),
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Restaurants'),
              Tab(text: 'Dishes'),
            ],
          ),
        ),
        body: const TabBarView(
          children: [
            _RestaurantsTab(),
            FavouriteItemsTab(),
          ],
        ),
      ),
    );
  }
}

class _RestaurantsTab extends ConsumerWidget {
  const _RestaurantsTab();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(favouritesNotifierProvider);
    final theme = Theme.of(context);

    return switch (state) {
      FavouritesInitial() || FavouritesLoading() =>
        const AppLoadingWidget(message: 'Loading favourites...'),
      FavouritesError(:final failure) => AppErrorWidget(
          failure: failure,
          onRetry: () => ref
              .read(favouritesNotifierProvider.notifier)
              .loadFavourites(),
        ),
      FavouritesLoaded(:final restaurants) => restaurants.isEmpty
          ? Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(
                    Icons.favorite_border,
                    size: 64,
                    color: AppColors.textTertiaryLight,
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'No favourites yet',
                    style: theme.textTheme.titleMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Start adding restaurants you love!',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ],
              ),
            )
          : RefreshIndicator(
              color: AppColors.primary,
              onRefresh: () => ref
                  .read(favouritesNotifierProvider.notifier)
                  .loadFavourites(),
              child: ListView.builder(
                padding: const EdgeInsets.only(top: 8, bottom: 24),
                itemCount: restaurants.length,
                itemBuilder: (context, index) {
                  final restaurant = restaurants[index];
                  return Dismissible(
                    key: ValueKey(restaurant.id),
                    direction: DismissDirection.endToStart,
                    background: Container(
                      alignment: Alignment.centerRight,
                      padding: const EdgeInsets.only(right: 24),
                      color: AppColors.error,
                      child: const Icon(Icons.favorite_border,
                          color: Colors.white),
                    ),
                    confirmDismiss: (_) async {
                      return await ref
                          .read(favouritesNotifierProvider.notifier)
                          .removeFavourite(restaurant.id);
                    },
                    child: CustomerRestaurantCard(
                      restaurant: restaurant,
                      onTap: () => context.push(
                        RouteNames.restaurantDetailPath(restaurant.id),
                      ),
                    ),
                  );
                },
              ),
            ),
    };
  }
}
