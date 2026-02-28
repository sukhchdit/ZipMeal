import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/review_repository.dart';
import 'review_submit_state.dart';

part 'review_submit_notifier.g.dart';

@riverpod
class ReviewSubmitNotifier extends _$ReviewSubmitNotifier {
  late ReviewRepository _repository;

  @override
  ReviewSubmitState build() {
    _repository = ref.watch(reviewRepositoryProvider);
    return const ReviewSubmitState.initial();
  }

  Future<void> submitReview({
    required String orderId,
    required int rating,
    String? reviewText,
    int? deliveryRating,
    required bool isAnonymous,
    List<String> photoUrls = const [],
  }) async {
    state = const ReviewSubmitState.submitting();
    final result = await _repository.submitReview(
      orderId: orderId,
      rating: rating,
      reviewText: reviewText,
      deliveryRating: deliveryRating,
      isAnonymous: isAnonymous,
      photoUrls: photoUrls,
    );
    if (result.failure != null) {
      state = ReviewSubmitState.error(failure: result.failure!);
    } else {
      state = ReviewSubmitState.submitted(review: result.data!);
    }
  }
}
