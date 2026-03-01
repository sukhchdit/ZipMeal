import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/promotion_model.dart';
import '../../data/repositories/promotions_repository.dart';
import 'promotions_state.dart';

part 'promotions_notifier.g.dart';

@riverpod
class PromotionsNotifier extends _$PromotionsNotifier {
  late PromotionsRepository _repository;

  @override
  PromotionsState build() {
    _repository = ref.watch(promotionsRepositoryProvider);
    loadPromotions();
    return const PromotionsState.initial();
  }

  int? _currentTypeFilter;
  bool? _currentActiveFilter;
  String? _currentSearch;

  Future<void> loadPromotions({
    int? promotionType,
    bool? isActive,
    String? search,
    int page = 1,
  }) async {
    if (page == 1) {
      state = const PromotionsState.loading();
      _currentTypeFilter = promotionType;
      _currentActiveFilter = isActive;
      _currentSearch = search;
    }

    final result = await _repository.getPromotions(
      promotionType: promotionType ?? _currentTypeFilter,
      isActive: isActive ?? _currentActiveFilter,
      search: search ?? _currentSearch,
      page: page,
    );

    if (result.failure != null) {
      state = PromotionsState.error(failure: result.failure!);
      return;
    }

    final data = result.data!;
    final currentState = state;
    final existingPromotions = page > 1 && currentState is PromotionsLoaded
        ? currentState.promotions
        : <PromotionModel>[];

    state = PromotionsState.loaded(
      promotions: [...existingPromotions, ...data.items],
      totalCount: data.totalCount,
      page: data.page,
      totalPages: data.totalPages,
    );
  }

  Future<void> loadMore() async {
    final currentState = state;
    if (currentState is! PromotionsLoaded || currentState.isLoadingMore) return;
    if (currentState.page >= currentState.totalPages) return;

    state = currentState.copyWith(isLoadingMore: true);
    await loadPromotions(page: currentState.page + 1);
  }

  Future<bool> togglePromotion(String id, {required bool isActive}) async {
    final failure = await _repository.togglePromotion(id, isActive: isActive);
    if (failure != null) return false;

    final currentState = state;
    if (currentState is PromotionsLoaded) {
      final updated = currentState.promotions.map((p) {
        if (p.id == id) {
          return p.copyWith(isActive: isActive);
        }
        return p;
      }).toList();
      state = currentState.copyWith(promotions: updated);
    }
    return true;
  }

  Future<bool> deletePromotion(String id) async {
    final failure = await _repository.deletePromotion(id);
    if (failure != null) return false;

    final currentState = state;
    if (currentState is PromotionsLoaded) {
      final updated =
          currentState.promotions.where((p) => p.id != id).toList();
      state = currentState.copyWith(
        promotions: updated,
        totalCount: currentState.totalCount - 1,
      );
    }
    return true;
  }
}
