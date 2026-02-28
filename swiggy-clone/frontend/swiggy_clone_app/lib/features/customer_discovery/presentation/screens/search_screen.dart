import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/search_suggestion_model.dart';
import '../providers/menu_item_search_notifier.dart';
import '../providers/menu_item_search_state.dart';
import '../providers/restaurant_search_notifier.dart';
import '../providers/restaurant_search_state.dart';
import '../providers/search_suggestions_notifier.dart';
import '../providers/search_suggestions_state.dart';
import '../widgets/restaurant_card.dart';

/// Search screen with debounced text input, autocomplete suggestions,
/// and tabs for Restaurants / Dishes.
class SearchScreen extends ConsumerStatefulWidget {
  const SearchScreen({super.key});

  @override
  ConsumerState<SearchScreen> createState() => _SearchScreenState();
}

class _SearchScreenState extends ConsumerState<SearchScreen>
    with SingleTickerProviderStateMixin {
  final _searchController = TextEditingController();
  Timer? _debounce;
  late TabController _tabController;
  bool _showSuggestions = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _tabController.addListener(_onTabChanged);
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _searchController.dispose();
    _tabController.removeListener(_onTabChanged);
    _tabController.dispose();
    super.dispose();
  }

  void _onTabChanged() {
    if (!_tabController.indexIsChanging) {
      _performSearch(_searchController.text);
    }
  }

  void _onSearchChanged(String value) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 400), () {
      // Fetch suggestions for 1+ chars
      if (value.trim().isNotEmpty) {
        ref
            .read(searchSuggestionsNotifierProvider.notifier)
            .fetchSuggestions(value);
        setState(() => _showSuggestions = true);
      } else {
        ref.read(searchSuggestionsNotifierProvider.notifier).clear();
        setState(() => _showSuggestions = false);
      }

      // Perform full search for 2+ chars
      _performSearch(value);
    });
  }

  void _performSearch(String value) {
    if (value.trim().length < 2) return;

    if (_tabController.index == 0) {
      ref.read(restaurantSearchNotifierProvider.notifier).search(value);
    } else {
      ref.read(menuItemSearchNotifierProvider.notifier).search(value);
    }
  }

  void _onSuggestionTapped(SearchSuggestionModel suggestion) {
    setState(() => _showSuggestions = false);

    if (suggestion.type == 'restaurant') {
      context.push(RouteNames.restaurantDetailPath(suggestion.id));
    } else {
      // Dish suggestion: fill the search box and search for dishes
      _searchController.text = suggestion.text;
      _tabController.animateTo(1);
      ref
          .read(menuItemSearchNotifierProvider.notifier)
          .search(suggestion.text);
    }
  }

  void _clearSearch() {
    _searchController.clear();
    ref.read(restaurantSearchNotifierProvider.notifier).clear();
    ref.read(menuItemSearchNotifierProvider.notifier).clear();
    ref.read(searchSuggestionsNotifierProvider.notifier).clear();
    setState(() => _showSuggestions = false);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: TextField(
          controller: _searchController,
          autofocus: true,
          decoration: InputDecoration(
            hintText: 'Search restaurants, dishes...',
            border: InputBorder.none,
            hintStyle: theme.textTheme.bodyLarge?.copyWith(
              color: AppColors.textTertiaryLight,
            ),
          ),
          style: theme.textTheme.bodyLarge,
          onChanged: _onSearchChanged,
          onSubmitted: (value) {
            setState(() => _showSuggestions = false);
            _performSearch(value);
          },
        ),
        actions: [
          if (_searchController.text.isNotEmpty)
            IconButton(
              icon: const Icon(Icons.clear),
              onPressed: _clearSearch,
            ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Restaurants'),
            Tab(text: 'Dishes'),
          ],
        ),
      ),
      body: Stack(
        children: [
          TabBarView(
            controller: _tabController,
            children: [
              _RestaurantResults(
                searchController: _searchController,
              ),
              _DishResults(
                searchController: _searchController,
              ),
            ],
          ),
          if (_showSuggestions) _SuggestionsOverlay(
            onTap: _onSuggestionTapped,
          ),
        ],
      ),
    );
  }
}

// ─────────────────────── Suggestions Overlay ──────────────────────

class _SuggestionsOverlay extends ConsumerWidget {
  const _SuggestionsOverlay({required this.onTap});

  final ValueChanged<SearchSuggestionModel> onTap;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(searchSuggestionsNotifierProvider);
    final theme = Theme.of(context);

    return switch (state) {
      SearchSuggestionsLoaded(:final suggestions) => Material(
          elevation: 4,
          color: theme.scaffoldBackgroundColor,
          child: ListView.builder(
            shrinkWrap: true,
            padding: EdgeInsets.zero,
            itemCount: suggestions.length,
            itemBuilder: (context, index) {
              final s = suggestions[index];
              return ListTile(
                leading: Icon(
                  s.type == 'restaurant'
                      ? Icons.restaurant
                      : Icons.fastfood,
                  color: s.type == 'restaurant'
                      ? AppColors.primaryLight
                      : AppColors.accentLight,
                ),
                title: Text(s.text),
                subtitle: s.type == 'dish' && s.restaurantName != null
                    ? Text(
                        s.restaurantName!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      )
                    : null,
                trailing: Text(
                  s.type == 'restaurant' ? 'Restaurant' : 'Dish',
                  style: theme.textTheme.labelSmall?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
                ),
                onTap: () => onTap(s),
              );
            },
          ),
        ),
      _ => const SizedBox.shrink(),
    };
  }
}

// ─────────────────────── Restaurant Results Tab ──────────────────

class _RestaurantResults extends ConsumerWidget {
  const _RestaurantResults({required this.searchController});

  final TextEditingController searchController;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(restaurantSearchNotifierProvider);

    return switch (state) {
      RestaurantSearchInitial() => const _EmptyState(
          icon: Icons.search,
          message: 'Search for restaurants by name or cuisine',
        ),
      RestaurantSearchLoading() =>
        const AppLoadingWidget(message: 'Searching...'),
      RestaurantSearchEmpty() => const _EmptyState(
          icon: Icons.search_off,
          message: 'No restaurants found. Try a different search.',
        ),
      RestaurantSearchError(:final failure) => AppErrorWidget(
          failure: failure,
          onRetry: () => ref
              .read(restaurantSearchNotifierProvider.notifier)
              .search(searchController.text),
        ),
      RestaurantSearchLoaded(:final results) => ListView.builder(
          padding: const EdgeInsets.only(top: 8, bottom: 24),
          itemCount: results.length,
          itemBuilder: (context, index) {
            final restaurant = results[index];
            return CustomerRestaurantCard(
              restaurant: restaurant,
              onTap: () => context.push(
                RouteNames.restaurantDetailPath(restaurant.id),
              ),
            );
          },
        ),
    };
  }
}

// ─────────────────────── Dish Results Tab ────────────────────────

class _DishResults extends ConsumerWidget {
  const _DishResults({required this.searchController});

  final TextEditingController searchController;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(menuItemSearchNotifierProvider);
    final theme = Theme.of(context);

    return switch (state) {
      MenuItemSearchInitial() => const _EmptyState(
          icon: Icons.fastfood_outlined,
          message: 'Search for your favourite dishes',
        ),
      MenuItemSearchLoading() =>
        const AppLoadingWidget(message: 'Searching dishes...'),
      MenuItemSearchEmpty() => const _EmptyState(
          icon: Icons.search_off,
          message: 'No dishes found. Try a different search.',
        ),
      MenuItemSearchError(:final failure) => AppErrorWidget(
          failure: failure,
          onRetry: () => ref
              .read(menuItemSearchNotifierProvider.notifier)
              .search(searchController.text),
        ),
      MenuItemSearchLoaded(:final results) => ListView.builder(
          padding: const EdgeInsets.only(top: 8, bottom: 24),
          itemCount: results.length,
          itemBuilder: (context, index) {
            final group = results[index];
            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Restaurant header
                InkWell(
                  onTap: () => context.push(
                    RouteNames.restaurantDetailPath(group.restaurantId),
                  ),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 12),
                    child: Row(
                      children: [
                        if (group.restaurantLogoUrl != null)
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.network(
                              group.restaurantLogoUrl!,
                              width: 40,
                              height: 40,
                              fit: BoxFit.cover,
                              errorBuilder: (_, __, ___) => Container(
                                width: 40,
                                height: 40,
                                color: AppColors.surfaceLight,
                                child: const Icon(Icons.restaurant, size: 20),
                              ),
                            ),
                          ),
                        if (group.restaurantLogoUrl != null)
                          const SizedBox(width: 12),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                group.restaurantName,
                                style: theme.textTheme.titleSmall,
                              ),
                              if (group.restaurantCity != null)
                                Text(
                                  group.restaurantCity!,
                                  style: theme.textTheme.bodySmall?.copyWith(
                                    color: AppColors.textSecondaryLight,
                                  ),
                                ),
                            ],
                          ),
                        ),
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const Icon(Icons.star, size: 16,
                                color: Colors.amber),
                            const SizedBox(width: 4),
                            Text(
                              group.restaurantAverageRating
                                  .toStringAsFixed(1),
                              style: theme.textTheme.labelMedium,
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),
                // Menu items
                ...group.items.map((item) => ListTile(
                      leading: item.imageUrl != null
                          ? ClipRRect(
                              borderRadius: BorderRadius.circular(8),
                              child: Image.network(
                                item.imageUrl!,
                                width: 48,
                                height: 48,
                                fit: BoxFit.cover,
                                errorBuilder: (_, __, ___) => Container(
                                  width: 48,
                                  height: 48,
                                  color: AppColors.surfaceLight,
                                  child: const Icon(Icons.fastfood, size: 24),
                                ),
                              ),
                            )
                          : null,
                      title: Row(
                        children: [
                          Icon(
                            Icons.circle,
                            size: 12,
                            color: item.isVeg ? Colors.green : Colors.red,
                          ),
                          const SizedBox(width: 8),
                          Expanded(child: Text(item.name)),
                        ],
                      ),
                      subtitle: item.description != null
                          ? Text(
                              item.description!,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                            )
                          : null,
                      trailing: Text(
                        '\u20B9${(item.discountedPrice ?? item.price) ~/ 100}',
                        style: theme.textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      onTap: () => context.push(
                        RouteNames.restaurantDetailPath(group.restaurantId),
                      ),
                    )),
                const Divider(height: 1),
              ],
            );
          },
        ),
    };
  }
}

// ─────────────────────── Empty State ─────────────────────────────

class _EmptyState extends StatelessWidget {
  const _EmptyState({required this.icon, required this.message});

  final IconData icon;
  final String message;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 64, color: AppColors.textTertiaryLight),
          const SizedBox(height: 16),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 32),
            child: Text(
              message,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
              textAlign: TextAlign.center,
            ),
          ),
        ],
      ),
    );
  }
}
