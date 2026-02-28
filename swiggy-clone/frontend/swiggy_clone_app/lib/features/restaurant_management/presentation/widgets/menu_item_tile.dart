import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/menu_item_model.dart';

/// A card showing a menu item with: veg/non-veg indicator, name, price
/// (with discount), availability toggle, bestseller badge, and optional image.
class MenuItemTile extends StatelessWidget {
  const MenuItemTile({
    required this.item,
    required this.onTap,
    this.onAvailabilityChanged,
    super.key,
  });

  final MenuItemModel item;
  final VoidCallback onTap;
  final ValueChanged<bool>? onAvailabilityChanged;

  String _formatPrice(int paise) {
    final rupees = paise / 100;
    return rupees == rupees.roundToDouble()
        ? rupees.toInt().toString()
        : rupees.toStringAsFixed(2);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Veg / Non-veg indicator
              Container(
                width: 16,
                height: 16,
                margin: const EdgeInsets.only(top: 2),
                decoration: BoxDecoration(
                  border: Border.all(
                    color: item.isVeg ? AppColors.success : AppColors.error,
                    width: 1.5,
                  ),
                  borderRadius: BorderRadius.circular(3),
                ),
                child: Center(
                  child: Container(
                    width: 8,
                    height: 8,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: item.isVeg ? AppColors.success : AppColors.error,
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 10),

              // Name, price, badges
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Name + bestseller badge
                    Row(
                      children: [
                        Expanded(
                          child: Text(
                            item.name,
                            style: theme.textTheme.titleSmall?.copyWith(
                              fontWeight: FontWeight.w600,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        if (item.isBestseller)
                          Container(
                            margin: const EdgeInsets.only(left: 6),
                            padding: const EdgeInsets.symmetric(
                                horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: AppColors.primary.withValues(alpha: 0.12),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Text(
                              'Bestseller',
                              style: theme.textTheme.labelSmall?.copyWith(
                                color: AppColors.primary,
                                fontWeight: FontWeight.bold,
                                fontSize: 10,
                              ),
                            ),
                          ),
                      ],
                    ),
                    const SizedBox(height: 4),

                    // Description
                    if (item.description != null &&
                        item.description!.isNotEmpty) ...[
                      Text(
                        item.description!,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 4),
                    ],

                    // Price
                    Row(
                      children: [
                        if (item.discountedPrice != null &&
                            item.discountedPrice! < item.price) ...[
                          Text(
                            '\u20B9${_formatPrice(item.price)}',
                            style: theme.textTheme.bodySmall?.copyWith(
                              decoration: TextDecoration.lineThrough,
                              color: AppColors.textTertiaryLight,
                            ),
                          ),
                          const SizedBox(width: 4),
                          Text(
                            '\u20B9${_formatPrice(item.discountedPrice!)}',
                            style: theme.textTheme.bodyMedium?.copyWith(
                              fontWeight: FontWeight.w600,
                              color: AppColors.success,
                            ),
                          ),
                        ] else
                          Text(
                            '\u20B9${_formatPrice(item.price)}',
                            style: theme.textTheme.bodyMedium?.copyWith(
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        const SizedBox(width: 8),
                        Icon(
                          Icons.timer_outlined,
                          size: 14,
                          color: AppColors.textTertiaryLight,
                        ),
                        const SizedBox(width: 2),
                        Text(
                          '${item.preparationTimeMin} min',
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: AppColors.textTertiaryLight,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),

              const SizedBox(width: 8),

              // Image + availability
              Column(
                children: [
                  // Thumbnail
                  ClipRRect(
                    borderRadius: BorderRadius.circular(8),
                    child: item.imageUrl != null && item.imageUrl!.isNotEmpty
                        ? CachedNetworkImage(
                            imageUrl: item.imageUrl!,
                            width: 60,
                            height: 60,
                            fit: BoxFit.cover,
                            placeholder: (_, __) => Container(
                              width: 60,
                              height: 60,
                              color: AppColors.shimmerBase,
                            ),
                            errorWidget: (_, __, ___) => Container(
                              width: 60,
                              height: 60,
                              color: AppColors.primaryLight,
                              child: const Icon(Icons.fastfood,
                                  color: AppColors.primary, size: 24),
                            ),
                          )
                        : Container(
                            width: 60,
                            height: 60,
                            decoration: BoxDecoration(
                              color: AppColors.primaryLight,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: const Icon(Icons.fastfood,
                                color: AppColors.primary, size: 24),
                          ),
                  ),
                  const SizedBox(height: 4),
                  // Availability toggle
                  SizedBox(
                    height: 24,
                    child: Switch(
                      value: item.isAvailable,
                      onChanged: onAvailabilityChanged,
                      activeColor: AppColors.success,
                      materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
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
