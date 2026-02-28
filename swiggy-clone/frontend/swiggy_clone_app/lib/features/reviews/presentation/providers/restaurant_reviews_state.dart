import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/review_model.dart';

part 'restaurant_reviews_state.freezed.dart';

@freezed
sealed class RestaurantReviewsState with _$RestaurantReviewsState {
  const factory RestaurantReviewsState.initial() = RestaurantReviewsInitial;
  const factory RestaurantReviewsState.loading() = RestaurantReviewsLoading;
  const factory RestaurantReviewsState.loaded({
    required List<ReviewModel> reviews,
    required int totalCount,
    required int currentPage,
  }) = RestaurantReviewsLoaded;
  const factory RestaurantReviewsState.error({required Failure failure}) =
      RestaurantReviewsError;
}
