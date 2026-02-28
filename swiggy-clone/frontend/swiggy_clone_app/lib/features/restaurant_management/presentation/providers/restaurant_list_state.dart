import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_summary_model.dart';

part 'restaurant_list_state.freezed.dart';

@freezed
sealed class RestaurantListState with _$RestaurantListState {
  const factory RestaurantListState.initial() = RestaurantListInitial;
  const factory RestaurantListState.loading() = RestaurantListLoading;
  const factory RestaurantListState.loaded({
    required List<RestaurantSummaryModel> restaurants,
  }) = RestaurantListLoaded;
  const factory RestaurantListState.error({required Failure failure}) =
      RestaurantListError;
}
