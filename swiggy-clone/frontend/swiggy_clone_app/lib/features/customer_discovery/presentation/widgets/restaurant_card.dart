import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/customer_restaurant_model.dart';

/// A card widget displaying a restaurant summary for customer listing.
class CustomerRestaurantCard extends StatelessWidget {
  const CustomerRestaurantCard({
    required this.restaurant,
    required this.onTap,
    super.key,
  });

  final CustomerRestaurantModel restaurant;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // ── Banner / Logo ──
            Container(
              height: 140,
              color: AppColors.shimmerBase,
              child: restaurant.bannerUrl != null
                  ? Image.network(
                      restaurant.bannerUrl!,
                      fit: BoxFit.cover,
                      errorBuilder: (_, __, ___) =>
                          const Center(child: Icon(Icons.restaurant, size: 48)),
                    )
                  : const Center(
                      child: Icon(Icons.restaurant, size: 48,
                          color: AppColors.textTertiaryLight)),
            ),

            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // ── Name + Veg badge ──
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          restaurant.name,
                          style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      if (restaurant.isVegOnly)
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.success.withValues(alpha: 0.12),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            'VEG',
                            style: theme.textTheme.labelSmall?.copyWith(
                              color: AppColors.success,
                              fontWeight: FontWeight.bold,
                              fontSize: 10,
                            ),
                          ),
                        ),
                    ],
                  ),
                  const SizedBox(height: 4),

                  // ── Cuisines ──
                  if (restaurant.cuisines.isNotEmpty)
                    Text(
                      restaurant.cuisines.join(', '),
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  const SizedBox(height: 8),

                  // ── Info chips ──
                  Row(
                    children: [
                      // Rating
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 6, vertical: 3),
                        decoration: BoxDecoration(
                          color: AppColors.rating,
                          borderRadius: BorderRadius.circular(4),
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const Icon(Icons.star, size: 12, color: Colors.white),
                            const SizedBox(width: 2),
                            Text(
                              restaurant.averageRating.toStringAsFixed(1),
                              style: theme.textTheme.labelSmall?.copyWith(
                                color: Colors.white,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(width: 8),

                      // Delivery time
                      if (restaurant.avgDeliveryTimeMin != null) ...[
                        Icon(Icons.access_time, size: 14,
                            color: AppColors.textSecondaryLight),
                        const SizedBox(width: 2),
                        Text(
                          '${restaurant.avgDeliveryTimeMin} min',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: AppColors.textSecondaryLight,
                          ),
                        ),
                        const SizedBox(width: 8),
                      ],

                      // Cost for two
                      if (restaurant.avgCostForTwo != null) ...[
                        Text(
                          '\u20B9${restaurant.avgCostForTwo! ~/ 100} for two',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: AppColors.textSecondaryLight,
                          ),
                        ),
                      ],

                      const Spacer(),

                      // Closed indicator
                      if (!restaurant.isAcceptingOrders)
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.error.withValues(alpha: 0.12),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            'CLOSED',
                            style: theme.textTheme.labelSmall?.copyWith(
                              color: AppColors.error,
                              fontWeight: FontWeight.bold,
                              fontSize: 10,
                            ),
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
