import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/customer_restaurant_model.dart';
import '../../data/repositories/discovery_repository.dart';
import 'restaurant_browse_state.dart';

part 'restaurant_browse_notifier.g.dart';

@riverpod
class RestaurantBrowseNotifier extends _$RestaurantBrowseNotifier {
  late DiscoveryRepository _repository;

  String? _city;
  String? _cuisineId;
  bool? _isVegOnly;
  double? _minRating;
  int? _maxCostForTwo;
  String? _sortBy;

  @override
  RestaurantBrowseState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    return const RestaurantBrowseState.initial();
  }

  Future<void> browse({
    String? city,
    String? cuisineId,
    bool? isVegOnly,
    double? minRating,
    int? maxCostForTwo,
    String? sortBy,
  }) async {
    _city = city;
    _cuisineId = cuisineId;
    _isVegOnly = isVegOnly;
    _minRating = minRating;
    _maxCostForTwo = maxCostForTwo;
    _sortBy = sortBy;

    state = const RestaurantBrowseState.loading();

    final result = await _repository.browseRestaurants(
      city: _city,
      cuisineId: _cuisineId,
      isVegOnly: _isVegOnly,
      minRating: _minRating,
      maxCostForTwo: _maxCostForTwo,
      sortBy: _sortBy,
    );

    if (result.failure != null) {
      state = RestaurantBrowseState.error(failure: result.failure!);
    } else {
      state = RestaurantBrowseState.loaded(
        restaurants: result.data!.items,
        hasMore: result.data!.hasMore,
        nextCursor: result.data!.nextCursor,
      );
    }
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! RestaurantBrowseLoaded || !current.hasMore) return;

    final result = await _repository.browseRestaurants(
      city: _city,
      cuisineId: _cuisineId,
      isVegOnly: _isVegOnly,
      minRating: _minRating,
      maxCostForTwo: _maxCostForTwo,
      sortBy: _sortBy,
      cursor: current.nextCursor,
    );

    if (result.failure != null) return; // Silently fail for pagination

    state = RestaurantBrowseState.loaded(
      restaurants: [...current.restaurants, ...result.data!.items],
      hasMore: result.data!.hasMore,
      nextCursor: result.data!.nextCursor,
    );
  }
}
