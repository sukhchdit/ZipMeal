import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/customer_restaurant_model.dart';

part 'restaurant_browse_state.freezed.dart';

@freezed
sealed class RestaurantBrowseState with _$RestaurantBrowseState {
  const factory RestaurantBrowseState.initial() = RestaurantBrowseInitial;
  const factory RestaurantBrowseState.loading() = RestaurantBrowseLoading;
  const factory RestaurantBrowseState.loaded({
    required List<CustomerRestaurantModel> restaurants,
    required bool hasMore,
    String? nextCursor,
  }) = RestaurantBrowseLoaded;
  const factory RestaurantBrowseState.error({required Failure failure}) =
      RestaurantBrowseError;
}
