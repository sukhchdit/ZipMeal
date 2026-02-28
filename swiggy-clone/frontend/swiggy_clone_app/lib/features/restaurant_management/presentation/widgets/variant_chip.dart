import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/menu_item_model.dart';

/// A small chip showing variant name and price adjustment.
/// Shows a delete icon when [onDelete] is provided.
class VariantChip extends StatelessWidget {
  const VariantChip({
    required this.variant,
    this.onDelete,
    super.key,
  });

  final MenuItemVariantModel variant;
  final VoidCallback? onDelete;

  String _formatAdjustment(int paise) {
    final rupees = paise / 100;
    if (paise == 0) return 'Base';
    final sign = paise > 0 ? '+' : '';
    return '$sign\u20B9${rupees == rupees.roundToDouble() ? rupees.toInt() : rupees.toStringAsFixed(2)}';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Chip(
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            variant.name,
            style: theme.textTheme.labelMedium?.copyWith(
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(width: 4),
          Text(
            _formatAdjustment(variant.priceAdjustment),
            style: theme.textTheme.labelSmall?.copyWith(
              color: AppColors.primary,
              fontWeight: FontWeight.w600,
            ),
          ),
          if (variant.isDefault) ...[
            const SizedBox(width: 4),
            Icon(Icons.star, size: 12, color: AppColors.warning),
          ],
        ],
      ),
      deleteIcon: onDelete != null
          ? const Icon(Icons.close, size: 16)
          : null,
      onDeleted: onDelete,
      backgroundColor: variant.isAvailable
          ? AppColors.primaryLight
          : AppColors.shimmerBase,
      side: BorderSide.none,
      visualDensity: VisualDensity.compact,
    );
  }
}
