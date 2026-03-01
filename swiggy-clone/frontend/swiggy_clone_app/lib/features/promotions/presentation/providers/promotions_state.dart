import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/promotion_model.dart';

part 'promotions_state.freezed.dart';

@freezed
sealed class PromotionsState with _$PromotionsState {
  const factory PromotionsState.initial() = PromotionsInitial;
  const factory PromotionsState.loading() = PromotionsLoading;
  const factory PromotionsState.loaded({
    required List<PromotionModel> promotions,
    required int totalCount,
    required int page,
    required int totalPages,
    @Default(false) bool isLoadingMore,
  }) = PromotionsLoaded;
  const factory PromotionsState.error({required Failure failure}) =
      PromotionsError;
}
