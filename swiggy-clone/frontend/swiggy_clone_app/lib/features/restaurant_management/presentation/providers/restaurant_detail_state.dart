import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_model.dart';

part 'restaurant_detail_state.freezed.dart';

@freezed
sealed class RestaurantDetailState with _$RestaurantDetailState {
  const factory RestaurantDetailState.initial() = RestaurantDetailInitial;
  const factory RestaurantDetailState.loading() = RestaurantDetailLoading;
  const factory RestaurantDetailState.loaded({
    required RestaurantModel restaurant,
  }) = RestaurantDetailLoaded;
  const factory RestaurantDetailState.error({required Failure failure}) =
      RestaurantDetailError;
}
