import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../customer_discovery/data/models/public_restaurant_detail_model.dart';
import '../../../restaurant_management/data/models/menu_item_model.dart';
import '../providers/session_orders_notifier.dart';

class DineInMenuItemDetailSheet extends ConsumerStatefulWidget {
  const DineInMenuItemDetailSheet({
    required this.item,
    required this.sessionId,
    super.key,
  });

  final MenuItemModel item;
  final String sessionId;

  @override
  ConsumerState<DineInMenuItemDetailSheet> createState() =>
      _DineInMenuItemDetailSheetState();
}

class _DineInMenuItemDetailSheetState
    extends ConsumerState<DineInMenuItemDetailSheet> {
  int _quantity = 1;
  String? _selectedVariantId;
  final Set<String> _selectedAddonIds = {};
  bool _isPlacing = false;

  @override
  void initState() {
    super.initState();
    // Select default variant if available
    final defaultVariant = widget.item.variants
        .where((v) => v.isDefault)
        .firstOrNull;
    if (defaultVariant != null) {
      _selectedVariantId = defaultVariant.id;
    }
  }

  int get _calculatedPrice {
    var price = widget.item.discountedPrice ?? widget.item.price;

    // Add variant adjustment
    if (_selectedVariantId != null) {
      final variant = widget.item.variants
          .where((v) => v.id == _selectedVariantId)
          .firstOrNull;
      if (variant != null) price += variant.priceAdjustment;
    }

    // Add addons
    for (final addonId in _selectedAddonIds) {
      final addon =
          widget.item.addons.where((a) => a.id == addonId).firstOrNull;
      if (addon != null) price += addon.price;
    }

    return price * _quantity;
  }

  Future<void> _placeOrder() async {
    setState(() => _isPlacing = true);

    final addons = _selectedAddonIds
        .map((id) => {'addonId': id, 'quantity': 1})
        .toList();

    final items = [
      {
        'menuItemId': widget.item.id,
        if (_selectedVariantId != null) 'variantId': _selectedVariantId,
        'quantity': _quantity,
        'addons': addons,
      }
    ];

    final success = await ref
        .read(sessionOrdersNotifierProvider(widget.sessionId).notifier)
        .placeOrder(items: items);

    if (!mounted) return;
    setState(() => _isPlacing = false);

    if (success) {
      Navigator.of(context).pop();
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Order placed!')),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to place order. Try again.')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final item = widget.item;

    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      expand: false,
      builder: (context, scrollController) {
        return Container(
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
          ),
          child: Column(
            children: [
              // Handle
              Container(
                margin: const EdgeInsets.only(top: 8),
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Expanded(
                child: ListView(
                  controller: scrollController,
                  padding: const EdgeInsets.all(16),
                  children: [
                    // Item image
                    if (item.imageUrl != null)
                      ClipRRect(
                        borderRadius: BorderRadius.circular(12),
                        child: Image.network(
                          item.imageUrl!,
                          height: 200,
                          width: double.infinity,
                          fit: BoxFit.cover,
                        ),
                      ),
                    const SizedBox(height: 12),
                    // Item name and price
                    Row(
                      children: [
                        if (item.isVeg)
                          Container(
                            margin: const EdgeInsets.only(right: 8),
                            padding: const EdgeInsets.all(2),
                            decoration: BoxDecoration(
                              border:
                                  Border.all(color: Colors.green, width: 1.5),
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: const Icon(Icons.circle,
                                size: 8, color: Colors.green),
                          ),
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
                    Text(
                      '\u20B9${(item.price / 100).toStringAsFixed(0)}',
                      style: theme.textTheme.titleMedium?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                    if (item.description != null) ...[
                      const SizedBox(height: 8),
                      Text(
                        item.description!,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                    // Variants
                    if (item.variants.isNotEmpty) ...[
                      const SizedBox(height: 16),
                      Text('Choose Variant',
                          style: theme.textTheme.titleSmall
                              ?.copyWith(fontWeight: FontWeight.w600)),
                      ...item.variants.map((variant) => RadioListTile<String>(
                            value: variant.id,
                            groupValue: _selectedVariantId,
                            title: Text(variant.name),
                            subtitle: variant.priceAdjustment != 0
                                ? Text(
                                    '${variant.priceAdjustment > 0 ? "+" : ""}\u20B9${(variant.priceAdjustment / 100).toStringAsFixed(0)}')
                                : null,
                            onChanged: (v) =>
                                setState(() => _selectedVariantId = v),
                            dense: true,
                          )),
                    ],
                    // Addons
                    if (item.addons.isNotEmpty) ...[
                      const SizedBox(height: 16),
                      Text('Add-ons',
                          style: theme.textTheme.titleSmall
                              ?.copyWith(fontWeight: FontWeight.w600)),
                      ...item.addons.map((addon) => CheckboxListTile(
                            value: _selectedAddonIds.contains(addon.id),
                            title: Text(addon.name),
                            subtitle: Text(
                                '+\u20B9${(addon.price / 100).toStringAsFixed(0)}'),
                            onChanged: (v) => setState(() {
                              if (v == true) {
                                _selectedAddonIds.add(addon.id);
                              } else {
                                _selectedAddonIds.remove(addon.id);
                              }
                            }),
                            dense: true,
                          )),
                    ],
                    // Quantity
                    const SizedBox(height: 16),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        IconButton.outlined(
                          onPressed: _quantity > 1
                              ? () => setState(() => _quantity--)
                              : null,
                          icon: const Icon(Icons.remove),
                        ),
                        Padding(
                          padding: const EdgeInsets.symmetric(horizontal: 16),
                          child: Text(
                            '$_quantity',
                            style: theme.textTheme.titleLarge
                                ?.copyWith(fontWeight: FontWeight.bold),
                          ),
                        ),
                        IconButton.outlined(
                          onPressed: _quantity < 20
                              ? () => setState(() => _quantity++)
                              : null,
                          icon: const Icon(Icons.add),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              // Bottom button
              SafeArea(
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: _isPlacing ? null : _placeOrder,
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(vertical: 14),
                      ),
                      child: _isPlacing
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                color: Colors.white,
                              ),
                            )
                          : Text(
                              'Place Order \u2022 \u20B9${(_calculatedPrice / 100).toStringAsFixed(0)}',
                              style: const TextStyle(
                                fontSize: 16,
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                    ),
                  ),
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
