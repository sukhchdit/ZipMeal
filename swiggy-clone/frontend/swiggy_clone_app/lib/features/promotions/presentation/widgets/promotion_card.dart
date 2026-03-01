import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/promotion_model.dart';

class PromotionCard extends StatelessWidget {
  const PromotionCard({
    required this.promotion,
    required this.onTap,
    required this.onToggle,
    super.key,
  });

  final PromotionModel promotion;
  final VoidCallback onTap;
  final ValueChanged<bool> onToggle;

  static const _typeLabels = ['Flash Deal', 'Happy Hour', 'Combo Offer'];
  static const _typeColors = [AppColors.error, AppColors.rating, AppColors.info];
  static const _typeIcons = [
    Icons.flash_on,
    Icons.access_time_filled,
    Icons.local_offer,
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final typeIndex = promotion.promotionType.clamp(0, 2);
    final color = _typeColors[typeIndex];
    final discountText = promotion.discountType == 1
        ? '${promotion.discountValue}% OFF'
        : '\u20B9${promotion.discountValue ~/ 100} OFF';

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              // Type icon
              Container(
                width: 44,
                height: 44,
                decoration: BoxDecoration(
                  color: color.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(_typeIcons[typeIndex], color: color, size: 22),
              ),
              const SizedBox(width: 12),

              // Details
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: color.withValues(alpha: 0.12),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            _typeLabels[typeIndex],
                            style: theme.textTheme.labelSmall?.copyWith(
                              color: color,
                              fontWeight: FontWeight.bold,
                              fontSize: 10,
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Text(
                          discountText,
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: AppColors.success,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Text(
                      promotion.title,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      '${promotion.menuItems.length} items',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                    ),
                  ],
                ),
              ),

              // Active toggle
              Switch.adaptive(
                value: promotion.isActive,
                onChanged: onToggle,
                activeColor: AppColors.primary,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
