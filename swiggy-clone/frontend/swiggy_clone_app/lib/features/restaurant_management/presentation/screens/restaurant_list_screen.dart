import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/restaurant_list_notifier.dart';
import '../providers/restaurant_list_state.dart';
import '../widgets/restaurant_card.dart';

/// Screen showing a list of the authenticated owner's restaurants.
///
/// Includes a FAB to register a new restaurant and pull-to-refresh.
class RestaurantListScreen extends ConsumerStatefulWidget {
  const RestaurantListScreen({super.key});

  @override
  ConsumerState<RestaurantListScreen> createState() =>
      _RestaurantListScreenState();
}

class _RestaurantListScreenState extends ConsumerState<RestaurantListScreen> {
  @override
  Widget build(BuildContext context) {
    final state = ref.watch(restaurantListNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Restaurants'),
        centerTitle: false,
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => context.push(RouteNames.registerRestaurant),
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
        icon: const Icon(Icons.add),
        label: const Text('Add Restaurant'),
      ),
      body: switch (state) {
        RestaurantListInitial() || RestaurantListLoading() =>
          const AppLoadingWidget(message: 'Loading restaurants...'),
        RestaurantListError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () =>
                ref.read(restaurantListNotifierProvider.notifier).loadRestaurants(),
          ),
        RestaurantListLoaded(:final restaurants) => restaurants.isEmpty
            ? Center(
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.store_outlined,
                      size: 80,
                      color: AppColors.textTertiaryLight,
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'No restaurants yet',
                      style: theme.textTheme.titleLarge?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Tap the button below to register your first restaurant.',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  ],
                ),
              )
            : RefreshIndicator(
                color: AppColors.primary,
                onRefresh: () => ref
                    .read(restaurantListNotifierProvider.notifier)
                    .loadRestaurants(),
                child: ListView.builder(
                  padding: const EdgeInsets.only(top: 8, bottom: 80),
                  itemCount: restaurants.length,
                  itemBuilder: (context, index) {
                    final restaurant = restaurants[index];
                    return RestaurantCard(
                      restaurant: restaurant,
                      onTap: () => context.push(
                        RouteNames.restaurantDashboardPath(restaurant.id),
                      ),
                    );
                  },
                ),
              ),
      },
    );
  }
}
