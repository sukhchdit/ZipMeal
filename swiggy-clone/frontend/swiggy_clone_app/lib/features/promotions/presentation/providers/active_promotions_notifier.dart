import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/promotion_model.dart';
import '../../data/repositories/promotions_repository.dart';

part 'active_promotions_notifier.freezed.dart';
part 'active_promotions_notifier.g.dart';

@freezed
sealed class ActivePromotionsState with _$ActivePromotionsState {
  const factory ActivePromotionsState.initial() = ActivePromotionsInitial;
  const factory ActivePromotionsState.loading() = ActivePromotionsLoading;
  const factory ActivePromotionsState.loaded({
    required List<PromotionModel> promotions,
  }) = ActivePromotionsLoaded;
  const factory ActivePromotionsState.error({required Failure failure}) =
      ActivePromotionsError;
}

@riverpod
class ActivePromotionsNotifier extends _$ActivePromotionsNotifier {
  late PromotionsRepository _repository;

  @override
  ActivePromotionsState build(String restaurantId) {
    _repository = ref.watch(promotionsRepositoryProvider);
    loadPromotions();
    return const ActivePromotionsState.initial();
  }

  Future<void> loadPromotions() async {
    state = const ActivePromotionsState.loading();

    final result = await _repository.getActivePromotions(restaurantId);

    if (result.failure != null) {
      state = ActivePromotionsState.error(failure: result.failure!);
      return;
    }

    state = ActivePromotionsState.loaded(promotions: result.data!);
  }
}
