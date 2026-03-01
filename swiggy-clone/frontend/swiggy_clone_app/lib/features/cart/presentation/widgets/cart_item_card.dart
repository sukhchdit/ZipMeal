import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/cart_model.dart';

class CartItemCard extends StatelessWidget {
  const CartItemCard({
    required this.item,
    required this.onQuantityChanged,
    required this.onRemove,
    super.key,
  });

  final CartItemModel item;
  final ValueChanged<int> onQuantityChanged;
  final VoidCallback onRemove;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Item details
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  item.itemName,
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
                if (item.variantName != null) ...[
                  const SizedBox(height: 2),
                  Text(
                    item.variantName!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                ],
                if (item.addons.isNotEmpty) ...[
                  const SizedBox(height: 2),
                  Text(
                    item.addons.map((a) => a.addonName).join(', '),
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
                if (item.specialInstructions != null &&
                    item.specialInstructions!.isNotEmpty) ...[
                  const SizedBox(height: 2),
                  Text(
                    item.specialInstructions!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                      fontStyle: FontStyle.italic,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
                const SizedBox(height: 4),
                Text(
                  '\u20B9${item.totalPrice ~/ 100}',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ),

          const SizedBox(width: 12),

          // Quantity controls
          Container(
            decoration: BoxDecoration(
              border: Border.all(color: AppColors.primary),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                _QuantityButton(
                  icon: item.quantity == 1 ? Icons.delete_outline : Icons.remove,
                  onPressed: () {
                    if (item.quantity == 1) {
                      onRemove();
                    } else {
                      onQuantityChanged(item.quantity - 1);
                    }
                  },
                  color: item.quantity == 1 ? AppColors.error : AppColors.primary,
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  child: Text(
                    '${item.quantity}',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppColors.primary,
                    ),
                  ),
                ),
                _QuantityButton(
                  icon: Icons.add,
                  onPressed: () => onQuantityChanged(item.quantity + 1),
                  color: AppColors.primary,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _QuantityButton extends StatelessWidget {
  const _QuantityButton({
    required this.icon,
    required this.onPressed,
    required this.color,
  });

  final IconData icon;
  final VoidCallback onPressed;
  final Color color;

  @override
  Widget build(BuildContext context) => SizedBox(
        width: 32,
        height: 32,
        child: IconButton(
          icon: Icon(icon, size: 16, color: color),
          onPressed: onPressed,
          padding: EdgeInsets.zero,
          constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
        ),
      );
}
