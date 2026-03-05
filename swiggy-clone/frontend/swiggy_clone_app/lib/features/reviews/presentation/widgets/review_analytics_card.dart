import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/review_analytics_model.dart';

class ReviewAnalyticsCard extends StatelessWidget {
  const ReviewAnalyticsCard({required this.analytics, super.key});

  final ReviewAnalyticsModel analytics;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.all(16),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Review Analytics',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            // Average rating and total
            Row(
              children: [
                Column(
                  children: [
                    Text(
                      analytics.averageRating.toStringAsFixed(1),
                      style: theme.textTheme.headlineLarge?.copyWith(
                        fontWeight: FontWeight.bold,
                        color: AppColors.rating,
                      ),
                    ),
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: List.generate(
                          5,
                          (i) => Icon(
                                i < analytics.averageRating.round()
                                    ? Icons.star
                                    : Icons.star_border,
                                size: 16,
                                color: AppColors.rating,
                              )),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      '${analytics.totalReviews} reviews',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                  ],
                ),
                const SizedBox(width: 24),
                Expanded(
                    child: _RatingDistribution(
                        distribution: analytics.ratingDistribution,
                        total: analytics.totalReviews)),
              ],
            ),
            const SizedBox(height: 12),
            // Photo reviews
            Text(
              '${analytics.photoReviewsCount} reviews with photos',
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _RatingDistribution extends StatelessWidget {
  const _RatingDistribution({required this.distribution, required this.total});
  final Map<String, int> distribution;
  final int total;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      children: List.generate(5, (index) {
        final star = 5 - index;
        final count = distribution['$star'] ?? 0;
        final fraction = total > 0 ? count / total : 0.0;
        return Padding(
          padding: const EdgeInsets.symmetric(vertical: 2),
          child: Row(
            children: [
              Text('$star', style: theme.textTheme.bodySmall),
              const SizedBox(width: 4),
              const Icon(Icons.star, size: 12, color: AppColors.rating),
              const SizedBox(width: 8),
              Expanded(
                child: LinearProgressIndicator(
                  value: fraction,
                  backgroundColor: Colors.grey.shade200,
                  color: AppColors.rating,
                  minHeight: 8,
                  borderRadius: BorderRadius.circular(4),
                ),
              ),
              const SizedBox(width: 8),
              SizedBox(
                width: 24,
                child: Text('$count',
                    style: theme.textTheme.bodySmall,
                    textAlign: TextAlign.end),
              ),
            ],
          ),
        );
      }),
    );
  }
}
