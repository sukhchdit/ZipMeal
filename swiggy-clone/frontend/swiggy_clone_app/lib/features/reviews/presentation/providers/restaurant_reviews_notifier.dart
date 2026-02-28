import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/review_repository.dart';
import 'restaurant_reviews_state.dart';

part 'restaurant_reviews_notifier.g.dart';

@riverpod
class RestaurantReviewsNotifier extends _$RestaurantReviewsNotifier {
  late ReviewRepository _repository;

  @override
  RestaurantReviewsState build(String restaurantId) {
    _repository = ref.watch(reviewRepositoryProvider);
    _loadReviews(1);
    return const RestaurantReviewsState.loading();
  }

  Future<void> _loadReviews(int page) async {
    final result = await _repository.getRestaurantReviews(
      restaurantId: restaurantId,
      page: page,
    );
    if (result.failure != null) {
      state = RestaurantReviewsState.error(failure: result.failure!);
    } else {
      state = RestaurantReviewsState.loaded(
        reviews: result.items!,
        totalCount: result.totalCount ?? 0,
        currentPage: page,
      );
    }
  }

  Future<void> loadPage(int page) async {
    state = const RestaurantReviewsState.loading();
    await _loadReviews(page);
  }

  Future<void> refresh() async {
    state = const RestaurantReviewsState.loading();
    await _loadReviews(1);
  }
}
