import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/menu_item_search_result_model.dart';
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
                      : AppColors.primaryLight,
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

// ─────────────────────── Responsive Grid Helper ──────────────────

int _responsiveColumnCount(double width) {
  if (width >= 1200) return 4;
  if (width >= 900) return 3;
  if (width >= 600) return 2;
  return 1;
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
      RestaurantSearchLoaded(:final results) => LayoutBuilder(
          builder: (context, constraints) {
            final columns = _responsiveColumnCount(constraints.maxWidth);
            return GridView.builder(
              padding: const EdgeInsets.all(12),
              gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: columns,
                crossAxisSpacing: 12,
                mainAxisSpacing: 12,
                childAspectRatio: 0.85,
              ),
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
      MenuItemSearchLoaded(:final results) => LayoutBuilder(
          builder: (context, constraints) {
            final columns = _responsiveColumnCount(constraints.maxWidth);
            return CustomScrollView(
              slivers: [
                const SliverPadding(padding: EdgeInsets.only(top: 8)),
                for (final group in results) ...[
                  // Restaurant header — full width
                  SliverToBoxAdapter(
                    child: _RestaurantGroupHeader(group: group),
                  ),
                  // Dish items in responsive grid
                  SliverPadding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 12, vertical: 8),
                    sliver: SliverGrid(
                      gridDelegate:
                          SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: columns,
                        crossAxisSpacing: 10,
                        mainAxisSpacing: 10,
                        childAspectRatio: columns == 1 ? 3.2 : 0.78,
                      ),
                      delegate: SliverChildBuilderDelegate(
                        (context, index) {
                          final item = group.items[index];
                          return columns == 1
                              ? _DishListTile(
                                  item: item,
                                  restaurantId: group.restaurantId,
                                )
                              : _DishGridCard(
                                  item: item,
                                  restaurantId: group.restaurantId,
                                  restaurantName: group.restaurantName,
                                );
                        },
                        childCount: group.items.length,
                      ),
                    ),
                  ),
                  const SliverToBoxAdapter(child: Divider(height: 1)),
                ],
                const SliverPadding(padding: EdgeInsets.only(bottom: 24)),
              ],
            );
          },
        ),
    };
  }
}

/// Restaurant header row displayed above each dish group.
class _RestaurantGroupHeader extends StatelessWidget {
  const _RestaurantGroupHeader({required this.group});

  final MenuItemSearchGroupedResultModel group;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: () => context.push(
        RouteNames.restaurantDetailPath(group.restaurantId),
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
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
                const Icon(Icons.star, size: 16, color: Colors.amber),
                const SizedBox(width: 4),
                Text(
                  group.restaurantAverageRating.toStringAsFixed(1),
                  style: theme.textTheme.labelMedium,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

/// Compact horizontal tile used for dish items in single-column (mobile) layout.
class _DishListTile extends StatelessWidget {
  const _DishListTile({required this.item, required this.restaurantId});

  final MenuItemSearchHitModel item;
  final String restaurantId;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: () => context.push(
        RouteNames.restaurantDetailPath(restaurantId),
      ),
      borderRadius: BorderRadius.circular(8),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 4),
        child: Row(
          children: [
            if (item.imageUrl != null)
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: Image.network(
                  item.imageUrl!,
                  width: 56,
                  height: 56,
                  fit: BoxFit.cover,
                  errorBuilder: (_, __, ___) => Container(
                    width: 56,
                    height: 56,
                    color: AppColors.surfaceLight,
                    child: const Icon(Icons.fastfood, size: 24),
                  ),
                ),
              ),
            if (item.imageUrl != null) const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Row(
                    children: [
                      Icon(
                        Icons.circle,
                        size: 12,
                        color: item.isVeg ? Colors.green : Colors.red,
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          item.name,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.titleSmall,
                        ),
                      ),
                    ],
                  ),
                  if (item.description != null) ...[
                    const SizedBox(height: 2),
                    Text(
                      item.description!,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(width: 8),
            Text(
              '\u20B9${(item.discountedPrice ?? item.price) ~/ 100}',
              style: theme.textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Card-style widget used for dish items in multi-column (tablet/desktop) grid.
class _DishGridCard extends StatelessWidget {
  const _DishGridCard({
    required this.item,
    required this.restaurantId,
    required this.restaurantName,
  });

  final MenuItemSearchHitModel item;
  final String restaurantId;
  final String restaurantName;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isDark = theme.brightness == Brightness.dark;
    final placeholderBg =
        isDark ? AppColors.surfaceDark : AppColors.shimmerBase;

    return Card(
      margin: EdgeInsets.zero,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: () => context.push(
          RouteNames.restaurantDetailPath(restaurantId),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // ── Image ──
            Expanded(
              flex: 3,
              child: Container(
                color: placeholderBg,
                child: item.imageUrl != null
                    ? Image.network(
                        item.imageUrl!,
                        fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => const Center(
                          child: Icon(Icons.fastfood, size: 36),
                        ),
                      )
                    : Center(
                        child: Icon(Icons.fastfood, size: 36,
                            color: isDark
                                ? AppColors.textTertiaryDark
                                : AppColors.textTertiaryLight),
                      ),
              ),
            ),
            // ── Info ──
            Expanded(
              flex: 2,
              child: Padding(
                padding: const EdgeInsets.all(10),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Icon(
                          Icons.circle,
                          size: 10,
                          color: item.isVeg ? Colors.green : Colors.red,
                        ),
                        const SizedBox(width: 6),
                        Expanded(
                          child: Text(
                            item.name,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: theme.textTheme.titleSmall?.copyWith(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ],
                    ),
                    if (item.description != null) ...[
                      const SizedBox(height: 2),
                      Text(
                        item.description!,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: isDark
                              ? AppColors.textSecondaryDark
                              : AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                    const Spacer(),
                    Row(
                      children: [
                        Text(
                          '\u20B9${(item.discountedPrice ?? item.price) ~/ 100}',
                          style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        if (item.discountedPrice != null) ...[
                          const SizedBox(width: 4),
                          Text(
                            '\u20B9${item.price ~/ 100}',
                            style: theme.textTheme.bodySmall?.copyWith(
                              decoration: TextDecoration.lineThrough,
                              color: isDark
                                  ? AppColors.textTertiaryDark
                                  : AppColors.textTertiaryLight,
                            ),
                          ),
                        ],
                        if (item.isBestseller) ...[
                          const Spacer(),
                          Container(
                            padding: const EdgeInsets.symmetric(
                                horizontal: 5, vertical: 1),
                            decoration: BoxDecoration(
                              color:
                                  AppColors.warning.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              'BESTSELLER',
                              style: theme.textTheme.labelSmall?.copyWith(
                                color: AppColors.warning,
                                fontWeight: FontWeight.bold,
                                fontSize: 9,
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
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
