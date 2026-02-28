import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';

/// A "Rate your experience" banner card shown for delivered orders
/// that don't yet have a review.
class ReviewPromptBanner extends StatelessWidget {
  const ReviewPromptBanner({
    required this.orderId,
    required this.restaurantName,
    super.key,
  });

  final String orderId;
  final String restaurantName;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Container(
      margin: const EdgeInsets.only(top: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.rating.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.rating.withValues(alpha: 0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(Icons.star_outline, color: AppColors.rating, size: 24),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Rate your experience',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            'Help others by sharing your experience with this order.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton(
              onPressed: () => context.push(
                RouteNames.submitReviewPath(orderId),
                extra: {'restaurantName': restaurantName},
              ),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppColors.rating,
                side: BorderSide(color: AppColors.rating),
              ),
              child: const Text('Write a Review'),
            ),
          ),
        ],
      ),
    );
  }
}
