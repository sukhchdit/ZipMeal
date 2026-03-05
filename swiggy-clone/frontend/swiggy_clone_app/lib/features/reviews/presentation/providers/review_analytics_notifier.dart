import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/review_repository.dart';
import 'review_analytics_state.dart';

part 'review_analytics_notifier.g.dart';

@riverpod
class ReviewAnalyticsNotifier extends _$ReviewAnalyticsNotifier {
  @override
  ReviewAnalyticsState build(String restaurantId) {
    _load();
    return const ReviewAnalyticsState.loading();
  }

  Future<void> _load() async {
    final repository = ref.read(reviewRepositoryProvider);
    final result = await repository.getReviewAnalytics(
      restaurantId: restaurantId,
    );

    if (result.failure != null) {
      state = ReviewAnalyticsState.error(failure: result.failure!);
    } else {
      state = ReviewAnalyticsState.loaded(analytics: result.data!);
    }
  }

  Future<void> refresh() async {
    state = const ReviewAnalyticsState.loading();
    await _load();
  }
}
