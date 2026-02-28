import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../../customer_discovery/data/models/public_restaurant_detail_model.dart';

part 'dine_in_menu_state.freezed.dart';

@freezed
sealed class DineInMenuState with _$DineInMenuState {
  const factory DineInMenuState.initial() = DineInMenuInitial;
  const factory DineInMenuState.loading() = DineInMenuLoading;
  const factory DineInMenuState.loaded({
    required PublicRestaurantDetailModel restaurant,
  }) = DineInMenuLoaded;
  const factory DineInMenuState.error({required Failure failure}) =
      DineInMenuError;
}
