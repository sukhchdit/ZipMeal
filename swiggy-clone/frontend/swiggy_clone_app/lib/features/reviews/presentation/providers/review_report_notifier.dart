import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/review_repository.dart';

part 'review_report_notifier.g.dart';

@riverpod
class ReviewReportNotifier extends _$ReviewReportNotifier {
  @override
  AsyncValue<void> build() => const AsyncData(null);

  Future<bool> reportReview({
    required String reviewId,
    required String reason,
    String? description,
  }) async {
    state = const AsyncLoading();
    final repository = ref.read(reviewRepositoryProvider);
    final result = await repository.reportReview(
      reviewId: reviewId,
      reason: reason,
      description: description,
    );
    if (result.failure != null) {
      state = AsyncError(result.failure!, StackTrace.current);
      return false;
    }
    state = const AsyncData(null);
    return true;
  }
}
