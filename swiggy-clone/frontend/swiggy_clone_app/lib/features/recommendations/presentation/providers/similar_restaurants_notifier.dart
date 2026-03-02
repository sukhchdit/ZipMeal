import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/recommended_restaurant_model.dart';
import '../../data/repositories/recommendations_repository.dart';

part 'similar_restaurants_notifier.freezed.dart';
part 'similar_restaurants_notifier.g.dart';

@freezed
sealed class SimilarRestaurantsState with _$SimilarRestaurantsState {
  const factory SimilarRestaurantsState.initial() = SimilarRestaurantsInitial;
  const factory SimilarRestaurantsState.loading() = SimilarRestaurantsLoading;
  const factory SimilarRestaurantsState.loaded({
    required List<RecommendedRestaurantModel> restaurants,
  }) = SimilarRestaurantsLoaded;
  const factory SimilarRestaurantsState.error({required Failure failure}) =
      SimilarRestaurantsError;
}

@riverpod
class SimilarRestaurantsNotifier extends _$SimilarRestaurantsNotifier {
  @override
  SimilarRestaurantsState build(String restaurantId) {
    _load();
    return const SimilarRestaurantsState.loading();
  }

  Future<void> _load() async {
    final repository = ref.read(recommendationsRepositoryProvider);
    final result = await repository.getSimilarRestaurants(
      restaurantId: restaurantId,
    );

    if (result.failure != null) {
      state = SimilarRestaurantsState.error(failure: result.failure!);
    } else {
      state = SimilarRestaurantsState.loaded(restaurants: result.data!);
    }
  }

  Future<void> refresh() async {
    state = const SimilarRestaurantsState.loading();
    await _load();
  }
}
