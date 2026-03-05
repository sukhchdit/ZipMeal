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
    final isDark = theme.brightness == Brightness.dark;
    final secondaryTextColor =
        isDark ? AppColors.textSecondaryDark : AppColors.textSecondaryLight;
    final tertiaryTextColor =
        isDark ? AppColors.textTertiaryDark : AppColors.textTertiaryLight;
    final placeholderBg =
        isDark ? AppColors.surfaceDark : AppColors.shimmerBase;

    return Card(
      margin: EdgeInsets.zero,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // ── Banner / Logo ──
            Expanded(
              flex: 3,
              child: Container(
                color: placeholderBg,
                child: restaurant.bannerUrl != null
                    ? Image.network(
                        restaurant.bannerUrl!,
                        fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) =>
                            const Center(child: Icon(Icons.restaurant, size: 40)),
                      )
                    : Center(
                        child: Icon(Icons.restaurant, size: 40,
                            color: tertiaryTextColor)),
              ),
            ),

            // ── Info Section ──
            Expanded(
              flex: 2,
              child: Padding(
                padding: const EdgeInsets.all(10),
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
                                horizontal: 4, vertical: 1),
                            decoration: BoxDecoration(
                              color: AppColors.success.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              'VEG',
                              style: theme.textTheme.labelSmall?.copyWith(
                                color: AppColors.success,
                                fontWeight: FontWeight.bold,
                                fontSize: 9,
                              ),
                            ),
                          ),
                      ],
                    ),
                    const SizedBox(height: 2),

                    // ── Cuisines ──
                    if (restaurant.cuisines.isNotEmpty)
                      Text(
                        restaurant.cuisines.join(', '),
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: secondaryTextColor,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),

                    const Spacer(),

                    // ── Info chips ──
                    Row(
                      children: [
                        // Rating
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 5, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.rating,
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const Icon(Icons.star, size: 11, color: Colors.white),
                              const SizedBox(width: 2),
                              Text(
                                restaurant.averageRating.toStringAsFixed(1),
                                style: theme.textTheme.labelSmall?.copyWith(
                                  color: Colors.white,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 11,
                                ),
                              ),
                            ],
                          ),
                        ),
                        const SizedBox(width: 6),

                        // Delivery time
                        if (restaurant.avgDeliveryTimeMin != null) ...[
                          Icon(Icons.access_time, size: 13,
                              color: secondaryTextColor),
                          const SizedBox(width: 2),
                          Text(
                            '${restaurant.avgDeliveryTimeMin} min',
                            style: theme.textTheme.labelSmall?.copyWith(
                              color: secondaryTextColor,
                            ),
                          ),
                        ],

                        const Spacer(),

                        // Closed indicator
                        if (!restaurant.isAcceptingOrders)
                          Container(
                            padding: const EdgeInsets.symmetric(
                                horizontal: 5, vertical: 1),
                            decoration: BoxDecoration(
                              color: AppColors.error.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              'CLOSED',
                              style: theme.textTheme.labelSmall?.copyWith(
                                color: AppColors.error,
                                fontWeight: FontWeight.bold,
                                fontSize: 9,
                              ),
                            ),
                          ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
