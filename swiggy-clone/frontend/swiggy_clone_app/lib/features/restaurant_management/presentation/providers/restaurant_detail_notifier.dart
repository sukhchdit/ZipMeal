import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'restaurant_detail_state.dart';

part 'restaurant_detail_notifier.g.dart';

@riverpod
class RestaurantDetailNotifier extends _$RestaurantDetailNotifier {
  late RestaurantRepository _repository;

  @override
  RestaurantDetailState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadRestaurant();
    return const RestaurantDetailState.initial();
  }

  Future<void> loadRestaurant() async {
    state = const RestaurantDetailState.loading();
    final result = await _repository.getRestaurantById(id: restaurantId);
    if (result.failure != null) {
      state = RestaurantDetailState.error(failure: result.failure!);
    } else {
      state = RestaurantDetailState.loaded(restaurant: result.data!);
    }
  }

  Future<bool> updateRestaurant(Map<String, dynamic> data) async {
    final result =
        await _repository.updateRestaurant(id: restaurantId, data: data);
    if (result.failure != null) return false;
    state = RestaurantDetailState.loaded(restaurant: result.data!);
    return true;
  }

  Future<bool> toggleAcceptingOrders(bool value) async {
    final result =
        await _repository.toggleAcceptingOrders(id: restaurantId, value: value);
    if (result.failure != null) return false;
    await loadRestaurant();
    return true;
  }

  Future<bool> toggleDineIn(bool value) async {
    final result =
        await _repository.toggleDineIn(id: restaurantId, value: value);
    if (result.failure != null) return false;
    await loadRestaurant();
    return true;
  }
}
