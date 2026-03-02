import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/trending_item_model.dart';
import '../../data/repositories/recommendations_repository.dart';

part 'trending_items_notifier.freezed.dart';
part 'trending_items_notifier.g.dart';

@freezed
sealed class TrendingItemsState with _$TrendingItemsState {
  const factory TrendingItemsState.initial() = TrendingItemsInitial;
  const factory TrendingItemsState.loading() = TrendingItemsLoading;
  const factory TrendingItemsState.loaded({
    required List<TrendingItemModel> items,
  }) = TrendingItemsLoaded;
  const factory TrendingItemsState.error({required Failure failure}) =
      TrendingItemsError;
}

@riverpod
class TrendingItemsNotifier extends _$TrendingItemsNotifier {
  @override
  TrendingItemsState build() {
    _load();
    return const TrendingItemsState.loading();
  }

  Future<void> _load() async {
    final repository = ref.read(recommendationsRepositoryProvider);
    final result = await repository.getTrending();

    if (result.failure != null) {
      state = TrendingItemsState.error(failure: result.failure!);
    } else {
      state = TrendingItemsState.loaded(items: result.data!);
    }
  }

  Future<void> refresh() async {
    state = const TrendingItemsState.loading();
    await _load();
  }
}
