import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/review_model.dart';
import '../providers/restaurant_reviews_notifier.dart';
import '../providers/restaurant_reviews_state.dart';

/// Embeddable reviews section for the restaurant detail screen.
class RestaurantReviewsSection extends ConsumerWidget {
  const RestaurantReviewsSection({required this.restaurantId, super.key});

  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state =
        ref.watch(restaurantReviewsNotifierProvider(restaurantId));
    final theme = Theme.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16),
          child: Text(
            'Reviews',
            style: theme.textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        const SizedBox(height: 8),
        switch (state) {
          RestaurantReviewsInitial() ||
          RestaurantReviewsLoading() =>
            const Padding(
              padding: EdgeInsets.all(16),
              child: Center(child: CircularProgressIndicator()),
            ),
          RestaurantReviewsError(:final failure) => Padding(
              padding: const EdgeInsets.all(16),
              child: Text(failure.message,
                  style: TextStyle(color: AppColors.error)),
            ),
          RestaurantReviewsLoaded(
            :final reviews,
            :final totalCount,
          ) =>
            reviews.isEmpty
                ? const Padding(
                    padding: EdgeInsets.all(16),
                    child: Text('No reviews yet. Be the first to review!'),
                  )
                : Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        child: Text(
                          '$totalCount review${totalCount == 1 ? '' : 's'}',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: AppColors.textSecondaryLight,
                          ),
                        ),
                      ),
                      const SizedBox(height: 8),
                      ...reviews.map((review) => _ReviewCard(review: review)),
                    ],
                  ),
        },
      ],
    );
  }
}

class _ReviewCard extends StatelessWidget {
  const _ReviewCard({required this.review});

  final ReviewModel review;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.borderLight),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header: name + rating
          Row(
            children: [
              Expanded(
                child: Text(
                  review.reviewerName ?? 'Anonymous',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
              Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.star, size: 16, color: AppColors.rating),
                  const SizedBox(width: 2),
                  Text(
                    '${review.rating}',
                    style: theme.textTheme.labelMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ],
          ),

          if (review.reviewText != null && review.reviewText!.isNotEmpty) ...[
            const SizedBox(height: 6),
            Text(
              review.reviewText!,
              style: theme.textTheme.bodyMedium,
            ),
          ],

          if (review.deliveryRating != null) ...[
            const SizedBox(height: 4),
            Row(
              children: [
                const Icon(Icons.delivery_dining, size: 14,
                    color: AppColors.textTertiaryLight),
                const SizedBox(width: 4),
                Text(
                  'Delivery: ${review.deliveryRating}/5',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
                ),
              ],
            ),
          ],

          // Restaurant reply
          if (review.restaurantReply != null &&
              review.restaurantReply!.isNotEmpty) ...[
            const SizedBox(height: 8),
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: AppColors.primary.withValues(alpha: 0.06),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Restaurant Reply',
                    style: theme.textTheme.labelSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppColors.primary,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    review.restaurantReply!,
                    style: theme.textTheme.bodySmall,
                  ),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }
}
