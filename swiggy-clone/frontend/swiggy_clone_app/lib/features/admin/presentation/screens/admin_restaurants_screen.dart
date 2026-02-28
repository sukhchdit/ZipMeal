import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/admin_restaurant_model.dart';
import '../providers/admin_restaurants_notifier.dart';
import '../providers/admin_restaurants_state.dart';

/// Paginated list of all platform restaurants with status filter and search.
class AdminRestaurantsScreen extends ConsumerStatefulWidget {
  const AdminRestaurantsScreen({super.key});

  @override
  ConsumerState<AdminRestaurantsScreen> createState() =>
      _AdminRestaurantsScreenState();
}

class _AdminRestaurantsScreenState
    extends ConsumerState<AdminRestaurantsScreen> {
  bool _isSearching = false;
  final _searchController = TextEditingController();
  int? _selectedStatus;

  static const _statusLabels = {
    0: 'Pending',
    1: 'Approved',
    2: 'Suspended',
    3: 'Rejected',
  };

  static const _statusColors = {
    0: Colors.orange,
    1: AppColors.success,
    2: AppColors.error,
    3: AppColors.textTertiaryLight,
  };

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _applyFilters() {
    ref.read(adminRestaurantsNotifierProvider.notifier).loadRestaurants(
          statusFilter: _selectedStatus,
          search:
              _searchController.text.isEmpty ? null : _searchController.text,
        );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(adminRestaurantsNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: _isSearching
            ? TextField(
                controller: _searchController,
                autofocus: true,
                decoration: const InputDecoration(
                  hintText: 'Search restaurants...',
                  border: InputBorder.none,
                ),
                onSubmitted: (_) => _applyFilters(),
              )
            : const Text('Restaurants'),
        actions: [
          IconButton(
            icon: Icon(_isSearching ? Icons.close : Icons.search),
            onPressed: () {
              setState(() {
                _isSearching = !_isSearching;
                if (!_isSearching) {
                  _searchController.clear();
                  _applyFilters();
                }
              });
            },
          ),
        ],
      ),
      body: Column(
        children: [
          // ──── Status Filter Chips ────
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: [
                Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: FilterChip(
                    label: const Text('All'),
                    selected: _selectedStatus == null,
                    onSelected: (_) {
                      setState(() => _selectedStatus = null);
                      _applyFilters();
                    },
                  ),
                ),
                ..._statusLabels.entries.map(
                  (entry) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: FilterChip(
                      label: Text(entry.value),
                      selected: _selectedStatus == entry.key,
                      selectedColor:
                          _statusColors[entry.key]?.withValues(alpha: 0.2),
                      onSelected: (_) {
                        setState(() => _selectedStatus = entry.key);
                        _applyFilters();
                      },
                    ),
                  ),
                ),
              ],
            ),
          ),

          // ──── Restaurant List ────
          Expanded(
            child: switch (state) {
              AdminRestaurantsInitial() || AdminRestaurantsLoading() =>
                const AppLoadingWidget(message: 'Loading restaurants...'),
              AdminRestaurantsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: _applyFilters,
                ),
              AdminRestaurantsLoaded(
                :final restaurants,
                :final page,
                :final totalPages,
                :final isLoadingMore,
              ) =>
                restaurants.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.store_outlined,
                                size: 80,
                                color: AppColors.textTertiaryLight),
                            const SizedBox(height: 16),
                            Text(
                              'No restaurants found',
                              style: theme.textTheme.titleMedium?.copyWith(
                                color: AppColors.textSecondaryLight,
                              ),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        color: AppColors.primary,
                        onRefresh: () async => _applyFilters(),
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          itemCount: restaurants.length +
                              (page < totalPages ? 1 : 0),
                          itemBuilder: (context, index) {
                            if (index >= restaurants.length) {
                              if (!isLoadingMore) {
                                WidgetsBinding.instance
                                    .addPostFrameCallback((_) {
                                  ref
                                      .read(adminRestaurantsNotifierProvider
                                          .notifier)
                                      .loadRestaurants(
                                        statusFilter: _selectedStatus,
                                        search: _searchController.text.isEmpty
                                            ? null
                                            : _searchController.text,
                                        page: page + 1,
                                      );
                                });
                              }
                              return const Padding(
                                padding: EdgeInsets.all(16),
                                child: Center(
                                  child: CircularProgressIndicator(
                                      strokeWidth: 2),
                                ),
                              );
                            }
                            return _RestaurantCard(
                              restaurant: restaurants[index],
                              onTap: () => context.push(
                                RouteNames.adminRestaurantDetailPath(
                                    restaurants[index].id),
                              ),
                            );
                          },
                        ),
                      ),
            },
          ),
        ],
      ),
    );
  }
}

class _RestaurantCard extends StatelessWidget {
  const _RestaurantCard({required this.restaurant, required this.onTap});

  final AdminRestaurantModel restaurant;
  final VoidCallback onTap;

  static const _statusLabels = {
    0: 'Pending',
    1: 'Approved',
    2: 'Suspended',
    3: 'Rejected',
  };

  static const _statusColors = {
    0: Colors.orange,
    1: AppColors.success,
    2: AppColors.error,
    3: AppColors.textTertiaryLight,
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final statusLabel = _statusLabels[restaurant.status] ?? 'Unknown';
    final statusColor =
        _statusColors[restaurant.status] ?? AppColors.textSecondaryLight;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Logo or icon
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: restaurant.logoUrl != null
                    ? Image.network(
                        restaurant.logoUrl!,
                        width: 56,
                        height: 56,
                        fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => Container(
                          width: 56,
                          height: 56,
                          color: AppColors.primaryLight,
                          child: const Icon(Icons.store,
                              color: AppColors.primary),
                        ),
                      )
                    : Container(
                        width: 56,
                        height: 56,
                        color: AppColors.primaryLight,
                        child:
                            const Icon(Icons.store, color: AppColors.primary),
                      ),
              ),
              const SizedBox(width: 12),

              // Details
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      restaurant.name,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    if (restaurant.city != null)
                      Text(
                        restaurant.city!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    const SizedBox(height: 2),
                    Text(
                      'Owner: ${restaurant.ownerName}',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),

              // Status + Rating
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: statusColor.withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      statusLabel,
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: statusColor,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Icon(Icons.star, size: 14,
                          color: AppColors.rating),
                      const SizedBox(width: 2),
                      Text(
                        restaurant.averageRating.toStringAsFixed(1),
                        style: theme.textTheme.labelMedium?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
