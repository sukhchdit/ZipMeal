import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/restaurant_summary_model.dart';

/// A Material card showing restaurant summary: logo, name, city, rating stars,
/// status chip, and accepting-orders indicator.
///
/// Tapping the card invokes [onTap], which should navigate to the dashboard.
class RestaurantCard extends StatelessWidget {
  const RestaurantCard({
    required this.restaurant,
    required this.onTap,
    super.key,
  });

  final RestaurantSummaryModel restaurant;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Logo
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: restaurant.logoUrl != null &&
                        restaurant.logoUrl!.isNotEmpty
                    ? CachedNetworkImage(
                        imageUrl: restaurant.logoUrl!,
                        width: 64,
                        height: 64,
                        fit: BoxFit.cover,
                        placeholder: (_, __) => Container(
                          width: 64,
                          height: 64,
                          color: AppColors.shimmerBase,
                          child: const Icon(Icons.restaurant,
                              color: Colors.white54),
                        ),
                        errorWidget: (_, __, ___) => Container(
                          width: 64,
                          height: 64,
                          color: AppColors.primaryLight,
                          child: const Icon(Icons.restaurant,
                              color: AppColors.primary),
                        ),
                      )
                    : Container(
                        width: 64,
                        height: 64,
                        color: AppColors.primaryLight,
                        child: const Icon(Icons.restaurant,
                            color: AppColors.primary, size: 32),
                      ),
              ),
              const SizedBox(width: 12),

              // Info
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      restaurant.name,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 4),
                    if (restaurant.city != null)
                      Text(
                        restaurant.city!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    const SizedBox(height: 6),
                    Row(
                      children: [
                        // Rating
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.rating,
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const Icon(Icons.star,
                                  size: 12, color: Colors.white),
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
                        const SizedBox(width: 6),
                        Text(
                          '(${restaurant.totalRatings})',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: AppColors.textTertiaryLight,
                          ),
                        ),
                        const Spacer(),

                        // Status chip
                        _StatusChip(status: restaurant.status),
                      ],
                    ),
                  ],
                ),
              ),

              const SizedBox(width: 8),

              // Accepting orders indicator
              Column(
                children: [
                  Icon(
                    restaurant.isAcceptingOrders
                        ? Icons.storefront
                        : Icons.store_outlined,
                    color: restaurant.isAcceptingOrders
                        ? AppColors.success
                        : AppColors.textDisabledLight,
                    size: 20,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    restaurant.isAcceptingOrders ? 'Open' : 'Closed',
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: restaurant.isAcceptingOrders
                          ? AppColors.success
                          : AppColors.textDisabledLight,
                      fontSize: 10,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _StatusChip extends StatelessWidget {
  const _StatusChip({required this.status});

  final String status;

  @override
  Widget build(BuildContext context) {
    final Color color;
    switch (status.toLowerCase()) {
      case 'active':
        color = AppColors.success;
      case 'pending':
        color = AppColors.warning;
      case 'suspended':
        color = AppColors.error;
      default:
        color = AppColors.textTertiaryLight;
    }

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withValues(alpha: 0.4)),
      ),
      child: Text(
        status,
        style: Theme.of(context).textTheme.labelSmall?.copyWith(
              color: color,
              fontWeight: FontWeight.w600,
            ),
      ),
    );
  }
}
