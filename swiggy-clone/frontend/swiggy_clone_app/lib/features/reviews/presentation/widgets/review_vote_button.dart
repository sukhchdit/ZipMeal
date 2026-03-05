import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../providers/review_vote_notifier.dart';

class ReviewVoteButton extends ConsumerWidget {
  const ReviewVoteButton({
    required this.reviewId,
    required this.helpfulCount,
    required this.hasVoted,
    super.key,
  });

  final String reviewId;
  final int helpfulCount;
  final bool? hasVoted;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final votes = ref.watch(reviewVoteNotifierProvider);
    final isVoted = votes.containsKey(reviewId)
        ? votes[reviewId]!
        : (hasVoted ?? false);
    final displayCount = votes.containsKey(reviewId)
        ? (votes[reviewId]!
            ? helpfulCount + (hasVoted == true ? 0 : 1)
            : helpfulCount - (hasVoted == true ? 1 : 0))
        : helpfulCount;

    return InkWell(
      onTap: () => ref.read(reviewVoteNotifierProvider.notifier).toggleVote(
            reviewId: reviewId,
            isHelpful: true,
            currentlyVoted: isVoted ? true : null,
          ),
      borderRadius: BorderRadius.circular(16),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              isVoted ? Icons.thumb_up : Icons.thumb_up_outlined,
              size: 16,
              color: isVoted ? AppColors.primary : AppColors.textTertiaryLight,
            ),
            if (displayCount > 0) ...[
              const SizedBox(width: 4),
              Text(
                '$displayCount',
                style: TextStyle(
                  fontSize: 12,
                  color: isVoted
                      ? AppColors.primary
                      : AppColors.textSecondaryLight,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
