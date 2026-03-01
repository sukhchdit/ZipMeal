import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../cart/presentation/providers/cart_notifier.dart';
import '../../../restaurant_management/data/models/menu_item_model.dart';

/// Bottom sheet showing full menu item details with variants, addons,
/// and an add-to-cart button.
class MenuItemDetailSheet extends ConsumerStatefulWidget {
  const MenuItemDetailSheet({
    required this.item,
    required this.restaurantId,
    super.key,
  });

  final MenuItemModel item;
  final String restaurantId;

  @override
  ConsumerState<MenuItemDetailSheet> createState() =>
      _MenuItemDetailSheetState();
}

class _MenuItemDetailSheetState extends ConsumerState<MenuItemDetailSheet> {
  String? _selectedVariantId;
  final Set<String> _selectedAddonIds = {};
  int _quantity = 1;
  bool _isAdding = false;
  final _specialInstructionsController = TextEditingController();

  static const _allergenNames = [
    'Gluten', 'Dairy', 'Nuts', 'Peanuts', 'Shellfish', 'Soy', 'Eggs',
    'Fish', 'Sesame', 'Mustard', 'Celery', 'Lupin', 'Molluscs', 'Sulfites',
  ];

  static const _dietaryTagNames = [
    'Vegan', 'Gluten-Free', 'Dairy-Free', 'Nut-Free', 'Keto',
    'Halal', 'Jain', 'Organic', 'Sugar-Free', 'High Protein',
  ];

  static const _dietaryTagColors = [
    Color(0xFF4CAF50), Color(0xFF009688), Color(0xFF03A9F4), Color(0xFF8BC34A),
    Color(0xFF9C27B0), Color(0xFF607D8B), Color(0xFFFF9800), Color(0xFF4CAF50),
    Color(0xFF00BCD4), Color(0xFFE91E63),
  ];

  MenuItemModel get item => widget.item;

  @override
  void dispose() {
    _specialInstructionsController.dispose();
    super.dispose();
  }

  @override
  void initState() {
    super.initState();
    // Select default variant if available
    final defaultVariant = item.variants.where((v) => v.isDefault).firstOrNull;
    _selectedVariantId = defaultVariant?.id ?? item.variants.firstOrNull?.id;
  }

  int get _totalPrice {
    var base = item.discountedPrice ?? item.price;

    // Add variant adjustment
    if (_selectedVariantId != null) {
      final variant =
          item.variants.where((v) => v.id == _selectedVariantId).firstOrNull;
      if (variant != null) base += variant.priceAdjustment;
    }

    // Add addons
    for (final addonId in _selectedAddonIds) {
      final addon =
          item.addons.where((a) => a.id == addonId).firstOrNull;
      if (addon != null) base += addon.price;
    }

    return base * _quantity;
  }

  Future<void> _addToCart() async {
    if (_isAdding) return;
    setState(() => _isAdding = true);

    final addons = _selectedAddonIds
        .map((id) => {'addonId': id, 'quantity': 1})
        .toList();

    final instructions = _specialInstructionsController.text.trim();
    final result = await ref.read(cartNotifierProvider.notifier).addToCart(
          restaurantId: widget.restaurantId,
          menuItemId: item.id,
          variantId: _selectedVariantId,
          quantity: _quantity,
          specialInstructions: instructions.isEmpty ? null : instructions,
          addons: addons,
        );

    if (!mounted) return;

    if (result.success) {
      Navigator.of(context).pop();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Added to cart!'),
          backgroundColor: AppColors.success,
        ),
      );
    } else if (result.errorCode == 'DIFFERENT_RESTAURANT') {
      // Show confirmation dialog to clear cart and retry
      final shouldClear = await showDialog<bool>(
        context: context,
        builder: (ctx) => AlertDialog(
          title: const Text('Replace cart items?'),
          content: const Text(
            'Your cart contains items from a different restaurant. '
            'Would you like to clear the cart and add this item?',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(false),
              child: const Text('Cancel'),
            ),
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(true),
              child: const Text('Clear & Add'),
            ),
          ],
        ),
      );

      if (shouldClear == true && mounted) {
        await ref.read(cartNotifierProvider.notifier).clearCart();
        final retryResult =
            await ref.read(cartNotifierProvider.notifier).addToCart(
                  restaurantId: widget.restaurantId,
                  menuItemId: item.id,
                  variantId: _selectedVariantId,
                  quantity: _quantity,
                  specialInstructions: instructions.isEmpty ? null : instructions,
                  addons: addons,
                );
        if (mounted && retryResult.success) {
          Navigator.of(context).pop();
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Added to cart!'),
              backgroundColor: AppColors.success,
            ),
          );
        }
      }
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to add to cart.'),
          backgroundColor: AppColors.error,
        ),
      );
    }

    if (mounted) setState(() => _isAdding = false);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      minChildSize: 0.4,
      maxChildSize: 0.9,
      expand: false,
      builder: (context, scrollController) => Column(
        children: [
          // Handle
          Center(
            child: Container(
              margin: const EdgeInsets.only(top: 8),
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: AppColors.dividerLight,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),

          Expanded(
            child: ListView(
              controller: scrollController,
              padding: const EdgeInsets.all(16),
              children: [
                // ── Image ──
                if (item.imageUrl != null)
                  ClipRRect(
                    borderRadius: BorderRadius.circular(12),
                    child: Image.network(
                      item.imageUrl!,
                      height: 200,
                      width: double.infinity,
                      fit: BoxFit.cover,
                      errorBuilder: (_, __, ___) => const SizedBox.shrink(),
                    ),
                  ),
                const SizedBox(height: 16),

                // ── Name + Veg indicator ──
                Row(
                  children: [
                    Icon(
                      item.isVeg ? Icons.circle : Icons.change_history,
                      size: 16,
                      color: item.isVeg ? AppColors.success : AppColors.error,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        item.name,
                        style: theme.textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),

                // ── Price ──
                Row(
                  children: [
                    Text(
                      '\u20B9${(item.discountedPrice ?? item.price) ~/ 100}',
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    if (item.discountedPrice != null) ...[
                      const SizedBox(width: 8),
                      Text(
                        '\u20B9${item.price ~/ 100}',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: AppColors.textTertiaryLight,
                          decoration: TextDecoration.lineThrough,
                        ),
                      ),
                    ],
                  ],
                ),

                if (item.description != null &&
                    item.description!.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Text(
                    item.description!,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                ],

                // ── Allergen badges ──
                if (item.allergens.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 6,
                    runSpacing: 6,
                    children: item.allergens.map((a) => Chip(
                      label: Text(
                        a < _allergenNames.length ? _allergenNames[a] : 'Unknown',
                        style: const TextStyle(fontSize: 11),
                      ),
                      backgroundColor: Colors.amber.withValues(alpha: 0.15),
                      side: const BorderSide(color: Colors.amber),
                      visualDensity: VisualDensity.compact,
                      materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                      padding: const EdgeInsets.symmetric(horizontal: 4),
                    )).toList(),
                  ),
                ],

                // ── Dietary tag chips ──
                if (item.dietaryTags.isNotEmpty) ...[
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 6,
                    runSpacing: 6,
                    children: item.dietaryTags.map((t) {
                      final color = t < _dietaryTagColors.length
                          ? _dietaryTagColors[t]
                          : AppColors.primary;
                      return Chip(
                        label: Text(
                          t < _dietaryTagNames.length ? _dietaryTagNames[t] : 'Unknown',
                          style: TextStyle(fontSize: 11, color: color),
                        ),
                        backgroundColor: color.withValues(alpha: 0.1),
                        side: BorderSide(color: color.withValues(alpha: 0.3)),
                        visualDensity: VisualDensity.compact,
                        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        padding: const EdgeInsets.symmetric(horizontal: 4),
                      );
                    }).toList(),
                  ),
                ],

                // ── Spice indicator + Calorie badge ──
                if (item.spiceLevel > 0 || item.calorieCount != null) ...[
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      if (item.spiceLevel > 0)
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: List.generate(
                            item.spiceLevel,
                            (_) => const Icon(Icons.local_fire_department,
                                size: 16, color: Colors.deepOrange),
                          ),
                        ),
                      if (item.spiceLevel > 0 && item.calorieCount != null)
                        const SizedBox(width: 12),
                      if (item.calorieCount != null)
                        Container(
                          padding: const EdgeInsets.symmetric(
                              horizontal: 8, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppColors.info.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: Text(
                            '${item.calorieCount} kcal',
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: AppColors.info,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                    ],
                  ),
                ],

                // ── Variants ──
                if (item.variants.isNotEmpty) ...[
                  const Divider(height: 32),
                  Text(
                    'Choose Variant',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 8),
                  ...item.variants.map((variant) => RadioListTile<String>(
                        value: variant.id,
                        groupValue: _selectedVariantId,
                        onChanged: (v) =>
                            setState(() => _selectedVariantId = v),
                        title: Text(variant.name),
                        subtitle: variant.priceAdjustment != 0
                            ? Text(
                                '${variant.priceAdjustment > 0 ? '+' : ''}\u20B9${variant.priceAdjustment ~/ 100}',
                                style: theme.textTheme.bodySmall,
                              )
                            : null,
                        dense: true,
                        contentPadding: EdgeInsets.zero,
                        activeColor: AppColors.primary,
                      )),
                ],

                // ── Addons ──
                if (item.addons.isNotEmpty) ...[
                  const Divider(height: 32),
                  Text(
                    'Add Extras',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 8),
                  ...item.addons.map((addon) => CheckboxListTile(
                        value: _selectedAddonIds.contains(addon.id),
                        onChanged: (v) {
                          setState(() {
                            if (v == true) {
                              _selectedAddonIds.add(addon.id);
                            } else {
                              _selectedAddonIds.remove(addon.id);
                            }
                          });
                        },
                        title: Row(
                          children: [
                            Icon(
                              addon.isVeg
                                  ? Icons.circle
                                  : Icons.change_history,
                              size: 12,
                              color: addon.isVeg
                                  ? AppColors.success
                                  : AppColors.error,
                            ),
                            const SizedBox(width: 6),
                            Expanded(child: Text(addon.name)),
                          ],
                        ),
                        subtitle: Text(
                          '+\u20B9${addon.price ~/ 100}',
                          style: theme.textTheme.bodySmall,
                        ),
                        dense: true,
                        contentPadding: EdgeInsets.zero,
                        activeColor: AppColors.primary,
                      )),
                ],

                // ── Special Instructions ──
                const Divider(height: 32),
                Text(
                  'Special Instructions',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                TextFormField(
                  controller: _specialInstructionsController,
                  maxLength: 500,
                  maxLines: 3,
                  decoration: InputDecoration(
                    hintText: 'e.g. No onions, extra spicy, allergies...',
                    border: const OutlineInputBorder(),
                    isDense: true,
                    contentPadding: const EdgeInsets.all(12),
                    hintStyle: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ),
              ],
            ),
          ),

          // ── Bottom bar: Quantity + Add to cart ──
          SafeArea(
            child: Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: theme.scaffoldBackgroundColor,
                boxShadow: [
                  BoxShadow(
                    color: AppColors.shadow,
                    blurRadius: 8,
                    offset: const Offset(0, -2),
                  ),
                ],
              ),
              child: Row(
                children: [
                  // Quantity selector
                  Container(
                    decoration: BoxDecoration(
                      border: Border.all(color: AppColors.borderLight),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Row(
                      children: [
                        IconButton(
                          icon: const Icon(Icons.remove, size: 18),
                          onPressed: _quantity > 1
                              ? () => setState(() => _quantity--)
                              : null,
                          constraints: const BoxConstraints(
                              minWidth: 36, minHeight: 36),
                          padding: EdgeInsets.zero,
                        ),
                        Text(
                          '$_quantity',
                          style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        IconButton(
                          icon: const Icon(Icons.add, size: 18),
                          onPressed: () => setState(() => _quantity++),
                          constraints: const BoxConstraints(
                              minWidth: 36, minHeight: 36),
                          padding: EdgeInsets.zero,
                          color: AppColors.primary,
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 16),

                  // Add to cart button
                  Expanded(
                    child: FilledButton(
                      onPressed: _isAdding ? null : _addToCart,
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        minimumSize: const Size.fromHeight(48),
                      ),
                      child: _isAdding
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : Text(
                              'Add  \u20B9${_totalPrice ~/ 100}',
                              style: const TextStyle(
                                fontWeight: FontWeight.bold,
                                fontSize: 16,
                              ),
                            ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
