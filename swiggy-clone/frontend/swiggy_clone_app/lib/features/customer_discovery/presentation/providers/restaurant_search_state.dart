import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/customer_restaurant_model.dart';

part 'restaurant_search_state.freezed.dart';

@freezed
sealed class RestaurantSearchState with _$RestaurantSearchState {
  const factory RestaurantSearchState.initial() = RestaurantSearchInitial;
  const factory RestaurantSearchState.loading() = RestaurantSearchLoading;
  const factory RestaurantSearchState.loaded({
    required List<CustomerRestaurantModel> results,
  }) = RestaurantSearchLoaded;
  const factory RestaurantSearchState.empty() = RestaurantSearchEmpty;
  const factory RestaurantSearchState.error({required Failure failure}) =
      RestaurantSearchError;
}
