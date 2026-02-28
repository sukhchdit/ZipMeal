import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/public_restaurant_detail_model.dart';

part 'public_restaurant_detail_state.freezed.dart';

@freezed
sealed class PublicRestaurantDetailState with _$PublicRestaurantDetailState {
  const factory PublicRestaurantDetailState.initial() =
      PublicRestaurantDetailInitial;
  const factory PublicRestaurantDetailState.loading() =
      PublicRestaurantDetailLoading;
  const factory PublicRestaurantDetailState.loaded({
    required PublicRestaurantDetailModel restaurant,
    required bool isFavourited,
  }) = PublicRestaurantDetailLoaded;
  const factory PublicRestaurantDetailState.error({required Failure failure}) =
      PublicRestaurantDetailError;
}
