import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'restaurant_search_state.dart';

part 'restaurant_search_notifier.g.dart';

@riverpod
class RestaurantSearchNotifier extends _$RestaurantSearchNotifier {
  late DiscoveryRepository _repository;

  @override
  RestaurantSearchState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    return const RestaurantSearchState.initial();
  }

  Future<void> search(String term, {String? city}) async {
    if (term.trim().length < 2) {
      state = const RestaurantSearchState.initial();
      return;
    }

    state = const RestaurantSearchState.loading();

    final result =
        await _repository.searchRestaurants(term: term, city: city);

    if (result.failure != null) {
      state = RestaurantSearchState.error(failure: result.failure!);
    } else if (result.data!.isEmpty) {
      state = const RestaurantSearchState.empty();
    } else {
      state = RestaurantSearchState.loaded(results: result.data!);
    }
  }

  void clear() => state = const RestaurantSearchState.initial();
}
