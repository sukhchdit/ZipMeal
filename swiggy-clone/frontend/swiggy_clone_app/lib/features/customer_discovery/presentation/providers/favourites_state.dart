import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/customer_restaurant_model.dart';

part 'favourites_state.freezed.dart';

@freezed
sealed class FavouritesState with _$FavouritesState {
  const factory FavouritesState.initial() = FavouritesInitial;
  const factory FavouritesState.loading() = FavouritesLoading;
  const factory FavouritesState.loaded({
    required List<CustomerRestaurantModel> restaurants,
  }) = FavouritesLoaded;
  const factory FavouritesState.error({required Failure failure}) =
      FavouritesError;
}
