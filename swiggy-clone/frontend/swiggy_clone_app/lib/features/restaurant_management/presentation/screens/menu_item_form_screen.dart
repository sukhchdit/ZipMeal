import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/menu_item_model.dart';
import '../../data/repositories/restaurant_repository.dart';
import '../providers/menu_items_notifier.dart';

/// Full-featured form for creating or editing a menu item, including
/// inline management of variants and addons.
///
/// When [itemId] is provided via `extra`, the screen loads the existing item
/// and operates in edit mode. Otherwise it creates a new item.
class MenuItemFormScreen extends ConsumerStatefulWidget {
  const MenuItemFormScreen({
    required this.restaurantId,
    this.categoryId,
    this.itemId,
    super.key,
  });

  final String restaurantId;
  final String? categoryId;
  final String? itemId;

  bool get isEditing => itemId != null;

  @override
  ConsumerState<MenuItemFormScreen> createState() => _MenuItemFormScreenState();
}

class _MenuItemFormScreenState extends ConsumerState<MenuItemFormScreen> {
  final _formKey = GlobalKey<FormState>();

  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _priceController = TextEditingController();
  final _discountedPriceController = TextEditingController();
  final _prepTimeController = TextEditingController(text: '15');
  final _sortOrderController = TextEditingController(text: '0');

  bool _isVeg = true;
  bool _isAvailable = true;
  bool _isBestseller = false;
  bool _isSubmitting = false;
  bool _isLoadingItem = false;

  // Variants & addons managed inline
  final List<_VariantFormData> _variants = [];
  final List<_AddonFormData> _addons = [];

  MenuItemModel? _existingItem;

  @override
  void initState() {
    super.initState();
    if (widget.isEditing) {
      _loadExistingItem();
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    _priceController.dispose();
    _discountedPriceController.dispose();
    _prepTimeController.dispose();
    _sortOrderController.dispose();
    super.dispose();
  }

  Future<void> _loadExistingItem() async {
    setState(() => _isLoadingItem = true);
    final repo = ref.read(restaurantRepositoryProvider);
    final result = await repo.getMenuItemById(
      restaurantId: widget.restaurantId,
      itemId: widget.itemId!,
    );

    if (!mounted) return;

    if (result.failure != null) {
      setState(() => _isLoadingItem = false);
      return;
    }

    final item = result.data!;
    _existingItem = item;
    _nameController.text = item.name;
    _descriptionController.text = item.description ?? '';
    _priceController.text = (item.price / 100).toStringAsFixed(0);
    if (item.discountedPrice != null) {
      _discountedPriceController.text =
          (item.discountedPrice! / 100).toStringAsFixed(0);
    }
    _prepTimeController.text = item.preparationTimeMin.toString();
    _sortOrderController.text = item.sortOrder.toString();
    _isVeg = item.isVeg;
    _isAvailable = item.isAvailable;
    _isBestseller = item.isBestseller;

    _variants
      ..clear()
      ..addAll(item.variants.map((v) => _VariantFormData(
            id: v.id,
            name: v.name,
            priceAdjustment: (v.priceAdjustment / 100).toStringAsFixed(0),
            isDefault: v.isDefault,
            isAvailable: v.isAvailable,
          )));

    _addons
      ..clear()
      ..addAll(item.addons.map((a) => _AddonFormData(
            id: a.id,
            name: a.name,
            price: (a.price / 100).toStringAsFixed(0),
            isVeg: a.isVeg,
            isAvailable: a.isAvailable,
            maxQuantity: a.maxQuantity.toString(),
          )));

    setState(() => _isLoadingItem = false);
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isSubmitting = true);

    final priceRupees = int.tryParse(_priceController.text.trim()) ?? 0;
    final discountRupees =
        int.tryParse(_discountedPriceController.text.trim());

    final data = <String, dynamic>{
      'name': _nameController.text.trim(),
      'description': _descriptionController.text.trim(),
      'price': priceRupees * 100, // convert to paise
      if (discountRupees != null) 'discountedPrice': discountRupees * 100,
      'isVeg': _isVeg,
      'isAvailable': _isAvailable,
      'isBestseller': _isBestseller,
      'preparationTimeMin':
          int.tryParse(_prepTimeController.text.trim()) ?? 15,
      'sortOrder': int.tryParse(_sortOrderController.text.trim()) ?? 0,
      if (widget.categoryId != null) 'categoryId': widget.categoryId,
      'variants': _variants
          .map((v) => {
                'name': v.name,
                'priceAdjustment':
                    (int.tryParse(v.priceAdjustment) ?? 0) * 100,
                'isDefault': v.isDefault,
                'isAvailable': v.isAvailable,
              })
          .toList(),
      'addons': _addons
          .map((a) => {
                'name': a.name,
                'price': (int.tryParse(a.price) ?? 0) * 100,
                'isVeg': a.isVeg,
                'isAvailable': a.isAvailable,
                'maxQuantity': int.tryParse(a.maxQuantity) ?? 5,
              })
          .toList(),
    };

    bool success;
    if (widget.isEditing) {
      final notifier = ref.read(
          menuItemsNotifierProvider(widget.restaurantId, _existingItem!.categoryId)
              .notifier);
      success = await notifier.updateItem(widget.itemId!, data);
    } else {
      final notifier = ref.read(
          menuItemsNotifierProvider(widget.restaurantId, widget.categoryId!)
              .notifier);
      success = await notifier.createItem(data);
    }

    if (!mounted) return;
    setState(() => _isSubmitting = false);

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            widget.isEditing ? 'Item updated!' : 'Item created!',
          ),
          backgroundColor: AppColors.success,
        ),
      );
      context.pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Operation failed. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  // ─────────────────────── Variant Helpers ─────────────────────────

  void _addVariant() {
    setState(() => _variants.add(_VariantFormData()));
  }

  void _removeVariant(int index) {
    setState(() => _variants.removeAt(index));
  }

  // ─────────────────────── Addon Helpers ──────────────────────────

  void _addAddon() {
    setState(() => _addons.add(_AddonFormData()));
  }

  void _removeAddon(int index) {
    setState(() => _addons.removeAt(index));
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (_isLoadingItem) {
      return Scaffold(
        appBar: AppBar(title: const Text('Menu Item')),
        body: const AppLoadingWidget(message: 'Loading item...'),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: Text(widget.isEditing ? 'Edit Item' : 'New Item'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // ──── Basic Fields ────
              Text(
                'Basic Information',
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 12),

              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(
                  labelText: 'Item Name *',
                  prefixIcon: Icon(Icons.fastfood_outlined),
                ),
                textCapitalization: TextCapitalization.words,
                validator: (v) => v == null || v.trim().isEmpty
                    ? 'Name is required'
                    : null,
              ),
              const SizedBox(height: 12),

              TextFormField(
                controller: _descriptionController,
                decoration: const InputDecoration(
                  labelText: 'Description',
                  prefixIcon: Icon(Icons.notes_outlined),
                ),
                maxLines: 3,
                minLines: 1,
              ),
              const SizedBox(height: 12),

              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _priceController,
                      decoration: const InputDecoration(
                        labelText: 'Price (\u20B9) *',
                        prefixIcon: Icon(Icons.currency_rupee),
                      ),
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                      ],
                      validator: (v) => v == null || v.trim().isEmpty
                          ? 'Price is required'
                          : null,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _discountedPriceController,
                      decoration: const InputDecoration(
                        labelText: 'Discounted (\u20B9)',
                        prefixIcon: Icon(Icons.local_offer_outlined),
                      ),
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),

              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _prepTimeController,
                      decoration: const InputDecoration(
                        labelText: 'Prep Time (min)',
                        prefixIcon: Icon(Icons.timer_outlined),
                      ),
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                      ],
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _sortOrderController,
                      decoration: const InputDecoration(
                        labelText: 'Sort Order',
                        prefixIcon: Icon(Icons.sort),
                      ),
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),

              // ──── Toggles ────
              SwitchListTile(
                title: const Text('Vegetarian'),
                value: _isVeg,
                onChanged: (v) => setState(() => _isVeg = v),
                activeColor: AppColors.success,
                contentPadding: EdgeInsets.zero,
                secondary: Icon(
                  _isVeg ? Icons.eco : Icons.set_meal,
                  color: _isVeg ? AppColors.success : AppColors.error,
                ),
              ),
              SwitchListTile(
                title: const Text('Available'),
                value: _isAvailable,
                onChanged: (v) => setState(() => _isAvailable = v),
                activeColor: AppColors.success,
                contentPadding: EdgeInsets.zero,
              ),
              SwitchListTile(
                title: const Text('Bestseller'),
                value: _isBestseller,
                onChanged: (v) => setState(() => _isBestseller = v),
                activeColor: AppColors.primary,
                contentPadding: EdgeInsets.zero,
              ),
              const Divider(height: 32),

              // ──── Variants Section ────
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Variants',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  TextButton.icon(
                    onPressed: _addVariant,
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Add'),
                  ),
                ],
              ),
              const SizedBox(height: 8),

              if (_variants.isEmpty)
                Padding(
                  padding: const EdgeInsets.only(bottom: 8),
                  child: Text(
                    'No variants. Add variants for size options (e.g., Half, Full).',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ),

              ..._variants.asMap().entries.map((entry) {
                final i = entry.key;
                final v = entry.value;
                return Card(
                  margin: const EdgeInsets.only(bottom: 8),
                  child: Padding(
                    padding: const EdgeInsets.all(12),
                    child: Column(
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: TextFormField(
                                initialValue: v.name,
                                decoration: const InputDecoration(
                                  labelText: 'Variant Name *',
                                  isDense: true,
                                ),
                                onChanged: (val) => v.name = val,
                                validator: (val) =>
                                    val == null || val.trim().isEmpty
                                        ? 'Required'
                                        : null,
                              ),
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: TextFormField(
                                initialValue: v.priceAdjustment,
                                decoration: const InputDecoration(
                                  labelText: '+/- Price (\u20B9)',
                                  isDense: true,
                                ),
                                keyboardType: TextInputType.number,
                                onChanged: (val) => v.priceAdjustment = val,
                              ),
                            ),
                            IconButton(
                              icon: const Icon(Icons.close, size: 20),
                              onPressed: () => _removeVariant(i),
                              color: AppColors.error,
                            ),
                          ],
                        ),
                        Row(
                          children: [
                            Checkbox(
                              value: v.isDefault,
                              onChanged: (val) => setState(
                                  () => v.isDefault = val ?? false),
                            ),
                            const Text('Default'),
                            const SizedBox(width: 16),
                            Checkbox(
                              value: v.isAvailable,
                              onChanged: (val) => setState(
                                  () => v.isAvailable = val ?? true),
                            ),
                            const Text('Available'),
                          ],
                        ),
                      ],
                    ),
                  ),
                );
              }),

              const Divider(height: 32),

              // ──── Addons Section ────
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Addons',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  TextButton.icon(
                    onPressed: _addAddon,
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Add'),
                  ),
                ],
              ),
              const SizedBox(height: 8),

              if (_addons.isEmpty)
                Padding(
                  padding: const EdgeInsets.only(bottom: 8),
                  child: Text(
                    'No addons. Add extras like cheese, toppings, etc.',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ),

              ..._addons.asMap().entries.map((entry) {
                final i = entry.key;
                final a = entry.value;
                return Card(
                  margin: const EdgeInsets.only(bottom: 8),
                  child: Padding(
                    padding: const EdgeInsets.all(12),
                    child: Column(
                      children: [
                        Row(
                          children: [
                            Expanded(
                              child: TextFormField(
                                initialValue: a.name,
                                decoration: const InputDecoration(
                                  labelText: 'Addon Name *',
                                  isDense: true,
                                ),
                                onChanged: (val) => a.name = val,
                                validator: (val) =>
                                    val == null || val.trim().isEmpty
                                        ? 'Required'
                                        : null,
                              ),
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: TextFormField(
                                initialValue: a.price,
                                decoration: const InputDecoration(
                                  labelText: 'Price (\u20B9)',
                                  isDense: true,
                                ),
                                keyboardType: TextInputType.number,
                                onChanged: (val) => a.price = val,
                              ),
                            ),
                            IconButton(
                              icon: const Icon(Icons.close, size: 20),
                              onPressed: () => _removeAddon(i),
                              color: AppColors.error,
                            ),
                          ],
                        ),
                        Row(
                          children: [
                            Checkbox(
                              value: a.isVeg,
                              onChanged: (val) =>
                                  setState(() => a.isVeg = val ?? true),
                            ),
                            const Text('Veg'),
                            const SizedBox(width: 16),
                            Checkbox(
                              value: a.isAvailable,
                              onChanged: (val) =>
                                  setState(() => a.isAvailable = val ?? true),
                            ),
                            const Text('Available'),
                            const Spacer(),
                            SizedBox(
                              width: 60,
                              child: TextFormField(
                                initialValue: a.maxQuantity,
                                decoration: const InputDecoration(
                                  labelText: 'Max',
                                  isDense: true,
                                ),
                                keyboardType: TextInputType.number,
                                inputFormatters: [
                                  FilteringTextInputFormatter.digitsOnly,
                                ],
                                onChanged: (val) => a.maxQuantity = val,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                );
              }),

              const SizedBox(height: 32),

              // ──── Submit ────
              FilledButton(
                onPressed: _isSubmitting ? null : _submit,
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  minimumSize: const Size.fromHeight(52),
                ),
                child: _isSubmitting
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: Colors.white,
                        ),
                      )
                    : Text(widget.isEditing ? 'Update Item' : 'Create Item'),
              ),
              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }
}

/// Mutable variant data used during form editing.
class _VariantFormData {
  _VariantFormData({
    this.id,
    this.name = '',
    this.priceAdjustment = '0',
    this.isDefault = false,
    this.isAvailable = true,
  });

  final String? id;
  String name;
  String priceAdjustment;
  bool isDefault;
  bool isAvailable;
}

/// Mutable addon data used during form editing.
class _AddonFormData {
  _AddonFormData({
    this.id,
    this.name = '',
    this.price = '0',
    this.isVeg = true,
    this.isAvailable = true,
    this.maxQuantity = '5',
  });

  final String? id;
  String name;
  String price;
  bool isVeg;
  bool isAvailable;
  String maxQuantity;
}
