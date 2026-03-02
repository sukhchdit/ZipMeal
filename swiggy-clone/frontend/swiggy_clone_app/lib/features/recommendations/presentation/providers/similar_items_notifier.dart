import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/recommended_menu_item_model.dart';
import '../../data/repositories/recommendations_repository.dart';

part 'similar_items_notifier.freezed.dart';
part 'similar_items_notifier.g.dart';

@freezed
sealed class SimilarItemsState with _$SimilarItemsState {
  const factory SimilarItemsState.initial() = SimilarItemsInitial;
  const factory SimilarItemsState.loading() = SimilarItemsLoading;
  const factory SimilarItemsState.loaded({
    required List<RecommendedMenuItemModel> items,
  }) = SimilarItemsLoaded;
  const factory SimilarItemsState.error({required Failure failure}) =
      SimilarItemsError;
}

@riverpod
class SimilarItemsNotifier extends _$SimilarItemsNotifier {
  @override
  SimilarItemsState build(String menuItemId) {
    _load();
    return const SimilarItemsState.loading();
  }

  Future<void> _load() async {
    final repository = ref.read(recommendationsRepositoryProvider);
    final result = await repository.getSimilarItems(menuItemId: menuItemId);

    if (result.failure != null) {
      state = SimilarItemsState.error(failure: result.failure!);
    } else {
      state = SimilarItemsState.loaded(items: result.data!);
    }
  }

  Future<void> refresh() async {
    state = const SimilarItemsState.loading();
    await _load();
  }
}
