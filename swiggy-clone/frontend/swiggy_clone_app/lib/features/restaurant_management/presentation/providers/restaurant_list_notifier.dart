import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'restaurant_list_state.dart';

part 'restaurant_list_notifier.g.dart';

@riverpod
class RestaurantListNotifier extends _$RestaurantListNotifier {
  late RestaurantRepository _repository;

  @override
  RestaurantListState build() {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadRestaurants();
    return const RestaurantListState.initial();
  }

  Future<void> loadRestaurants() async {
    state = const RestaurantListState.loading();
    final result = await _repository.getMyRestaurants();
    if (result.failure != null) {
      state = RestaurantListState.error(failure: result.failure!);
    } else {
      state = RestaurantListState.loaded(restaurants: result.data!);
    }
  }
}
