import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_restaurant_model.dart';

part 'admin_restaurants_state.freezed.dart';

@freezed
sealed class AdminRestaurantsState with _$AdminRestaurantsState {
  const factory AdminRestaurantsState.initial() = AdminRestaurantsInitial;
  const factory AdminRestaurantsState.loading() = AdminRestaurantsLoading;
  const factory AdminRestaurantsState.loaded({
    required List<AdminRestaurantModel> restaurants,
    required int totalCount,
    required int page,
    required int totalPages,
    @Default(false) bool isLoadingMore,
  }) = AdminRestaurantsLoaded;
  const factory AdminRestaurantsState.error({required Failure failure}) =
      AdminRestaurantsError;
}
