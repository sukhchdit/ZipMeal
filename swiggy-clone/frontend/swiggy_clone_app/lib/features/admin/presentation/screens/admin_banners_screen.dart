import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/admin_banner_model.dart';
import '../providers/admin_banners_notifier.dart';
import '../providers/admin_banners_state.dart';

/// Paginated list of all banners with search and active/inactive filter.
class AdminBannersScreen extends ConsumerStatefulWidget {
  const AdminBannersScreen({super.key});

  @override
  ConsumerState<AdminBannersScreen> createState() =>
      _AdminBannersScreenState();
}

class _AdminBannersScreenState extends ConsumerState<AdminBannersScreen> {
  bool _isSearching = false;
  final _searchController = TextEditingController();
  bool? _activeFilter;

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _applyFilters() {
    ref.read(adminBannersNotifierProvider.notifier).loadBanners(
          isActive: _activeFilter,
          search:
              _searchController.text.isEmpty ? null : _searchController.text,
        );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(adminBannersNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: _isSearching
            ? TextField(
                controller: _searchController,
                autofocus: true,
                decoration: const InputDecoration(
                  hintText: 'Search banners...',
                  border: InputBorder.none,
                ),
                onSubmitted: (_) => _applyFilters(),
              )
            : const Text('Banners'),
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
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _applyFilters,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () async {
          final created = await context.push<bool>(
            RouteNames.adminBannerForm,
          );
          if (created == true) _applyFilters();
        },
        child: const Icon(Icons.add),
      ),
      body: Column(
        children: [
          // ──── Active/Inactive Filter Chips ────
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
                    selected: _activeFilter == null,
                    onSelected: (_) {
                      setState(() => _activeFilter = null);
                      _applyFilters();
                    },
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: FilterChip(
                    label: const Text('Active'),
                    selected: _activeFilter == true,
                    selectedColor: AppColors.success.withValues(alpha: 0.2),
                    onSelected: (_) {
                      setState(() => _activeFilter = true);
                      _applyFilters();
                    },
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: FilterChip(
                    label: const Text('Inactive'),
                    selected: _activeFilter == false,
                    selectedColor: AppColors.error.withValues(alpha: 0.2),
                    onSelected: (_) {
                      setState(() => _activeFilter = false);
                      _applyFilters();
                    },
                  ),
                ),
              ],
            ),
          ),

          // ──── Banner List ────
          Expanded(
            child: switch (state) {
              AdminBannersInitial() || AdminBannersLoading() =>
                const AppLoadingWidget(message: 'Loading banners...'),
              AdminBannersError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: _applyFilters,
                ),
              AdminBannersLoaded(
                :final banners,
                :final page,
                :final totalPages,
                :final isLoadingMore,
              ) =>
                banners.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.image_outlined,
                                size: 80,
                                color: AppColors.textTertiaryLight),
                            const SizedBox(height: 16),
                            Text(
                              'No banners found',
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
                          itemCount:
                              banners.length + (page < totalPages ? 1 : 0),
                          itemBuilder: (context, index) {
                            if (index >= banners.length) {
                              if (!isLoadingMore) {
                                WidgetsBinding.instance
                                    .addPostFrameCallback((_) {
                                  ref
                                      .read(adminBannersNotifierProvider
                                          .notifier)
                                      .loadBanners(
                                        isActive: _activeFilter,
                                        search:
                                            _searchController.text.isEmpty
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
                            return _BannerCard(
                              banner: banners[index],
                              onTap: () async {
                                final updated = await context.push<bool>(
                                  RouteNames.adminBannerForm,
                                  extra: banners[index],
                                );
                                if (updated == true) _applyFilters();
                              },
                              onToggle: () => _showToggleDialog(
                                banners[index],
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

  void _showToggleDialog(AdminBannerModel banner) {
    final newStatus = !banner.isActive;
    final action = newStatus ? 'activate' : 'deactivate';

    showDialog<void>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('${action[0].toUpperCase()}${action.substring(1)} Banner'),
        content: Text(
          'Are you sure you want to $action "${banner.title}"?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () async {
              Navigator.of(ctx).pop();
              final success = await ref
                  .read(adminBannersNotifierProvider.notifier)
                  .toggleBanner(banner.id, newStatus);
              if (mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text(
                      success
                          ? 'Banner ${newStatus ? 'activated' : 'deactivated'}'
                          : 'Failed to update banner status',
                    ),
                  ),
                );
              }
            },
            child: Text(action[0].toUpperCase() + action.substring(1)),
          ),
        ],
      ),
    );
  }
}

class _BannerCard extends StatelessWidget {
  const _BannerCard({
    required this.banner,
    required this.onTap,
    required this.onToggle,
  });

  final AdminBannerModel banner;
  final VoidCallback onTap;
  final VoidCallback onToggle;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isExpired = banner.validUntil.isBefore(DateTime.now());

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: onTap,
        onLongPress: onToggle,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Thumbnail
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: Image.network(
                  banner.imageUrl,
                  width: 60,
                  height: 60,
                  fit: BoxFit.cover,
                  errorBuilder: (_, __, ___) => Container(
                    width: 60,
                    height: 60,
                    color: AppColors.primaryLight,
                    child:
                        const Icon(Icons.image, color: AppColors.primary),
                  ),
                ),
              ),
              const SizedBox(width: 12),

              // Details
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      banner.title,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    if (banner.deepLink != null)
                      Text(
                        banner.deepLink!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textTertiaryLight,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    const SizedBox(height: 2),
                    Text(
                      '${_formatDate(banner.validFrom)} – ${_formatDate(banner.validUntil)}',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: isExpired
                            ? AppColors.error
                            : AppColors.textSecondaryLight,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),

              // Status + Order
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: (banner.isActive
                              ? AppColors.success
                              : AppColors.error)
                          .withValues(alpha: 0.12),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      banner.isActive ? 'Active' : 'Inactive',
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: banner.isActive
                            ? AppColors.success
                            : AppColors.error,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  const SizedBox(height: 6),
                  Container(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: AppColors.primaryLight,
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      '#${banner.displayOrder}',
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: AppColors.primary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  const SizedBox(height: 4),
                  InkWell(
                    onTap: onToggle,
                    child: Icon(
                      banner.isActive
                          ? Icons.toggle_on
                          : Icons.toggle_off_outlined,
                      color: banner.isActive
                          ? AppColors.success
                          : AppColors.textTertiaryLight,
                      size: 28,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _formatDate(DateTime dt) {
    return '${dt.day.toString().padLeft(2, '0')}/${dt.month.toString().padLeft(2, '0')}/${dt.year}';
  }
}
