import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/review_repository.dart';

part 'review_vote_notifier.g.dart';

@riverpod
class ReviewVoteNotifier extends _$ReviewVoteNotifier {
  @override
  Map<String, bool> build() => {};

  Future<void> toggleVote({
    required String reviewId,
    required bool isHelpful,
    bool? currentlyVoted,
  }) async {
    final repository = ref.read(reviewRepositoryProvider);

    if (currentlyVoted == true) {
      // Remove vote
      state = {...state, reviewId: false};
      final result = await repository.removeVote(reviewId: reviewId);
      if (result.failure != null) {
        state = {...state}..remove(reviewId);
      }
    } else {
      // Add/update vote
      state = {...state, reviewId: true};
      final result = await repository.voteReview(
        reviewId: reviewId,
        isHelpful: isHelpful,
      );
      if (result.failure != null) {
        state = {...state}..remove(reviewId);
      }
    }
  }
}
