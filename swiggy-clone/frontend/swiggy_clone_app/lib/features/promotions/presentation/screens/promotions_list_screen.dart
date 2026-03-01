import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/promotions_notifier.dart';
import '../providers/promotions_state.dart';
import '../widgets/promotion_card.dart';

class PromotionsListScreen extends ConsumerStatefulWidget {
  const PromotionsListScreen({super.key});

  @override
  ConsumerState<PromotionsListScreen> createState() =>
      _PromotionsListScreenState();
}

class _PromotionsListScreenState extends ConsumerState<PromotionsListScreen> {
  int? _selectedTypeFilter;
  bool? _activeFilter;

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(promotionsNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Promotions'),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () async {
          final result = await context.push(RouteNames.createPromotion);
          if (result == true) {
            ref.read(promotionsNotifierProvider.notifier).loadPromotions();
          }
        },
        icon: const Icon(Icons.add),
        label: const Text('New Promotion'),
      ),
      body: Column(
        children: [
          // Filter chips
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: Row(
              children: [
                _FilterChip(
                  label: 'All',
                  selected: _selectedTypeFilter == null,
                  onSelected: (_) => _applyTypeFilter(null),
                ),
                const SizedBox(width: 8),
                _FilterChip(
                  label: 'Flash Deals',
                  selected: _selectedTypeFilter == 0,
                  onSelected: (_) => _applyTypeFilter(0),
                ),
                const SizedBox(width: 8),
                _FilterChip(
                  label: 'Happy Hours',
                  selected: _selectedTypeFilter == 1,
                  onSelected: (_) => _applyTypeFilter(1),
                ),
                const SizedBox(width: 8),
                _FilterChip(
                  label: 'Combos',
                  selected: _selectedTypeFilter == 2,
                  onSelected: (_) => _applyTypeFilter(2),
                ),
                const SizedBox(width: 16),
                FilterChip(
                  label: Text(_activeFilter == true
                      ? 'Active Only'
                      : _activeFilter == false
                          ? 'Inactive Only'
                          : 'All Status'),
                  selected: _activeFilter != null,
                  onSelected: (_) {
                    setState(() {
                      if (_activeFilter == null) {
                        _activeFilter = true;
                      } else if (_activeFilter == true) {
                        _activeFilter = false;
                      } else {
                        _activeFilter = null;
                      }
                    });
                    ref.read(promotionsNotifierProvider.notifier).loadPromotions(
                          promotionType: _selectedTypeFilter,
                          isActive: _activeFilter,
                        );
                  },
                ),
              ],
            ),
          ),

          // List
          Expanded(
            child: switch (state) {
              PromotionsInitial() || PromotionsLoading() =>
                const AppLoadingWidget(message: 'Loading promotions...'),
              PromotionsError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: () => ref
                      .read(promotionsNotifierProvider.notifier)
                      .loadPromotions(),
                ),
              PromotionsLoaded(:final promotions, :final isLoadingMore) =>
                promotions.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.local_offer_outlined,
                                size: 64,
                                color: AppColors.textTertiaryLight),
                            const SizedBox(height: 16),
                            Text(
                              'No promotions yet',
                              style: Theme.of(context)
                                  .textTheme
                                  .titleMedium
                                  ?.copyWith(
                                      color: AppColors.textTertiaryLight),
                            ),
                            const SizedBox(height: 8),
                            const Text('Create your first promotion!'),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: () => ref
                            .read(promotionsNotifierProvider.notifier)
                            .loadPromotions(),
                        child: ListView.builder(
                          itemCount:
                              promotions.length + (isLoadingMore ? 1 : 0),
                          itemBuilder: (context, index) {
                            if (index == promotions.length) {
                              return const Padding(
                                padding: EdgeInsets.all(16),
                                child:
                                    Center(child: CircularProgressIndicator()),
                              );
                            }
                            final promo = promotions[index];
                            return PromotionCard(
                              promotion: promo,
                              onTap: () async {
                                final result = await context.push(
                                  RouteNames.editPromotionPath(promo.id),
                                  extra: promo,
                                );
                                if (result == true) {
                                  ref
                                      .read(
                                          promotionsNotifierProvider.notifier)
                                      .loadPromotions();
                                }
                              },
                              onToggle: (value) => ref
                                  .read(promotionsNotifierProvider.notifier)
                                  .togglePromotion(promo.id,
                                      isActive: value),
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

  void _applyTypeFilter(int? type) {
    setState(() => _selectedTypeFilter = type);
    ref.read(promotionsNotifierProvider.notifier).loadPromotions(
          promotionType: type,
          isActive: _activeFilter,
        );
  }
}

class _FilterChip extends StatelessWidget {
  const _FilterChip({
    required this.label,
    required this.selected,
    required this.onSelected,
  });

  final String label;
  final bool selected;
  final ValueChanged<bool> onSelected;

  @override
  Widget build(BuildContext context) {
    return FilterChip(
      label: Text(label),
      selected: selected,
      onSelected: onSelected,
      selectedColor: AppColors.primary.withValues(alpha: 0.12),
      checkmarkColor: AppColors.primary,
    );
  }
}
