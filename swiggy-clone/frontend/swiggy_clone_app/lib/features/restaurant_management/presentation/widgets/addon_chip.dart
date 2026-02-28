import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/menu_item_model.dart';

/// A small chip showing addon name, price, and veg indicator.
/// Shows a delete icon when [onDelete] is provided.
class AddonChip extends StatelessWidget {
  const AddonChip({
    required this.addon,
    this.onDelete,
    super.key,
  });

  final MenuItemAddonModel addon;
  final VoidCallback? onDelete;

  String _formatPrice(int paise) {
    final rupees = paise / 100;
    return rupees == rupees.roundToDouble()
        ? rupees.toInt().toString()
        : rupees.toStringAsFixed(2);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Chip(
      avatar: Container(
        width: 12,
        height: 12,
        decoration: BoxDecoration(
          border: Border.all(
            color: addon.isVeg ? AppColors.success : AppColors.error,
            width: 1.5,
          ),
          borderRadius: BorderRadius.circular(2),
        ),
        child: Center(
          child: Container(
            width: 6,
            height: 6,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: addon.isVeg ? AppColors.success : AppColors.error,
            ),
          ),
        ),
      ),
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text(
            addon.name,
            style: theme.textTheme.labelMedium?.copyWith(
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(width: 4),
          Text(
            '+\u20B9${_formatPrice(addon.price)}',
            style: theme.textTheme.labelSmall?.copyWith(
              color: AppColors.primary,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
      deleteIcon: onDelete != null
          ? const Icon(Icons.close, size: 16)
          : null,
      onDeleted: onDelete,
      backgroundColor: addon.isAvailable
          ? AppColors.primaryLight
          : AppColors.shimmerBase,
      side: BorderSide.none,
      visualDensity: VisualDensity.compact,
    );
  }
}
