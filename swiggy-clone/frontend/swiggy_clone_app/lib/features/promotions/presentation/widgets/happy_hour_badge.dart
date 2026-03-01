import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class HappyHourBadge extends StatelessWidget {
  const HappyHourBadge({
    required this.discountValue,
    required this.discountType,
    this.startTime,
    this.endTime,
    super.key,
  });

  final int discountValue;
  final int discountType;
  final String? startTime;
  final String? endTime;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final discountText = discountType == 1
        ? '$discountValue% OFF'
        : '\u20B9${discountValue ~/ 100} OFF';

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        gradient: LinearGradient(
          colors: [
            AppColors.rating.withValues(alpha: 0.15),
            AppColors.rating.withValues(alpha: 0.08),
          ],
        ),
        borderRadius: BorderRadius.circular(6),
        border: Border.all(color: AppColors.rating.withValues(alpha: 0.3)),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.access_time_filled,
              size: 14, color: AppColors.rating),
          const SizedBox(width: 4),
          Text(
            'Happy Hour $discountText',
            style: theme.textTheme.labelSmall?.copyWith(
              color: AppColors.rating,
              fontWeight: FontWeight.bold,
              fontSize: 10,
            ),
          ),
          if (startTime != null && endTime != null) ...[
            const SizedBox(width: 4),
            Text(
              '$startTime–$endTime',
              style: theme.textTheme.labelSmall?.copyWith(
                color: AppColors.rating,
                fontSize: 9,
              ),
            ),
          ],
        ],
      ),
    );
  }
}
