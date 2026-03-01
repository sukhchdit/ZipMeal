import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../data/models/promotion_model.dart';
import '../../data/repositories/promotions_repository.dart';

class CreatePromotionScreen extends ConsumerStatefulWidget {
  const CreatePromotionScreen({this.promotion, super.key});

  final PromotionModel? promotion;

  @override
  ConsumerState<CreatePromotionScreen> createState() =>
      _CreatePromotionScreenState();
}

class _CreatePromotionScreenState extends ConsumerState<CreatePromotionScreen> {
  final _formKey = GlobalKey<FormState>();
  bool get _isEditing => widget.promotion != null;

  late int _promotionType;
  late int _discountType;
  late final TextEditingController _titleController;
  late final TextEditingController _descriptionController;
  late final TextEditingController _imageUrlController;
  late final TextEditingController _discountValueController;
  late final TextEditingController _maxDiscountController;
  late final TextEditingController _minOrderAmountController;
  late final TextEditingController _displayOrderController;
  late final TextEditingController _comboPriceController;
  late final TextEditingController _startTimeController;
  late final TextEditingController _endTimeController;

  DateTimeRange? _dateRange;
  final List<int> _selectedDays = [];
  bool _isSaving = false;

  static const _dayLabels = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  @override
  void initState() {
    super.initState();
    final p = widget.promotion;
    _promotionType = p?.promotionType ?? 0;
    _discountType = p?.discountType ?? 1;
    _titleController = TextEditingController(text: p?.title ?? '');
    _descriptionController = TextEditingController(text: p?.description ?? '');
    _imageUrlController = TextEditingController(text: p?.imageUrl ?? '');
    _discountValueController =
        TextEditingController(text: p != null ? '${p.discountValue}' : '');
    _maxDiscountController = TextEditingController(
        text: p?.maxDiscount != null ? '${p!.maxDiscount}' : '');
    _minOrderAmountController = TextEditingController(
        text: p?.minOrderAmount != null ? '${p!.minOrderAmount}' : '');
    _displayOrderController =
        TextEditingController(text: '${p?.displayOrder ?? 0}');
    _comboPriceController = TextEditingController(
        text: p?.comboPrice != null ? '${p!.comboPrice}' : '');
    _startTimeController =
        TextEditingController(text: p?.recurringStartTime ?? '');
    _endTimeController =
        TextEditingController(text: p?.recurringEndTime ?? '');

    if (p != null) {
      _dateRange = DateTimeRange(
        start: DateTime.tryParse(p.validFrom) ?? DateTime.now(),
        end: DateTime.tryParse(p.validUntil) ??
            DateTime.now().add(const Duration(days: 7)),
      );
      _selectedDays.addAll(p.recurringDaysOfWeek);
    }
  }

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    _imageUrlController.dispose();
    _discountValueController.dispose();
    _maxDiscountController.dispose();
    _minOrderAmountController.dispose();
    _displayOrderController.dispose();
    _comboPriceController.dispose();
    _startTimeController.dispose();
    _endTimeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(_isEditing ? 'Edit Promotion' : 'Create Promotion'),
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Promotion type selector
            Text('Promotion Type',
                style: theme.textTheme.titleSmall
                    ?.copyWith(fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            SegmentedButton<int>(
              segments: const [
                ButtonSegment(value: 0, label: Text('Flash Deal')),
                ButtonSegment(value: 1, label: Text('Happy Hour')),
                ButtonSegment(value: 2, label: Text('Combo')),
              ],
              selected: {_promotionType},
              onSelectionChanged: _isEditing
                  ? null
                  : (v) => setState(() => _promotionType = v.first),
            ),
            const SizedBox(height: 16),

            // Title
            TextFormField(
              controller: _titleController,
              decoration: const InputDecoration(
                labelText: 'Title',
                hintText: 'e.g., Weekend Flash Sale',
              ),
              validator: (v) =>
                  v == null || v.isEmpty ? 'Title is required' : null,
            ),
            const SizedBox(height: 12),

            // Description
            TextFormField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description (optional)',
              ),
              maxLines: 2,
            ),
            const SizedBox(height: 12),

            // Image URL
            TextFormField(
              controller: _imageUrlController,
              decoration: const InputDecoration(
                labelText: 'Image URL (optional)',
              ),
            ),
            const SizedBox(height: 16),

            // Discount config
            Text('Discount',
                style: theme.textTheme.titleSmall
                    ?.copyWith(fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            Row(
              children: [
                Expanded(
                  child: DropdownButtonFormField<int>(
                    value: _discountType,
                    items: const [
                      DropdownMenuItem(value: 1, child: Text('Percentage')),
                      DropdownMenuItem(value: 2, child: Text('Flat Amount')),
                    ],
                    onChanged: (v) => setState(() => _discountType = v ?? 1),
                    decoration:
                        const InputDecoration(labelText: 'Discount Type'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _discountValueController,
                    keyboardType: TextInputType.number,
                    decoration: InputDecoration(
                      labelText: _discountType == 1 ? '% Off' : 'Amount (paise)',
                    ),
                    validator: (v) =>
                        v == null || v.isEmpty ? 'Required' : null,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _maxDiscountController,
                    keyboardType: TextInputType.number,
                    decoration: const InputDecoration(
                      labelText: 'Max Discount (paise, optional)',
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: _minOrderAmountController,
                    keyboardType: TextInputType.number,
                    decoration: const InputDecoration(
                      labelText: 'Min Order (paise, optional)',
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),

            // Date range
            Text('Validity Period',
                style: theme.textTheme.titleSmall
                    ?.copyWith(fontWeight: FontWeight.w600)),
            const SizedBox(height: 8),
            OutlinedButton.icon(
              onPressed: _pickDateRange,
              icon: const Icon(Icons.date_range),
              label: Text(_dateRange != null
                  ? '${_formatDate(_dateRange!.start)} — ${_formatDate(_dateRange!.end)}'
                  : 'Select dates'),
            ),
            const SizedBox(height: 12),

            // Display order
            TextFormField(
              controller: _displayOrderController,
              keyboardType: TextInputType.number,
              decoration:
                  const InputDecoration(labelText: 'Display Order'),
            ),
            const SizedBox(height: 16),

            // Happy Hour specific
            if (_promotionType == 1) ...[
              Text('Happy Hour Schedule',
                  style: theme.textTheme.titleSmall
                      ?.copyWith(fontWeight: FontWeight.w600)),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _startTimeController,
                      decoration: const InputDecoration(
                        labelText: 'Start Time',
                        hintText: '15:00',
                      ),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: TextFormField(
                      controller: _endTimeController,
                      decoration: const InputDecoration(
                        labelText: 'End Time',
                        hintText: '18:00',
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Wrap(
                spacing: 8,
                children: List.generate(7, (i) {
                  final selected = _selectedDays.contains(i);
                  return FilterChip(
                    label: Text(_dayLabels[i]),
                    selected: selected,
                    onSelected: (v) {
                      setState(() {
                        if (v) {
                          _selectedDays.add(i);
                        } else {
                          _selectedDays.remove(i);
                        }
                      });
                    },
                  );
                }),
              ),
              const SizedBox(height: 16),
            ],

            // Combo specific
            if (_promotionType == 2) ...[
              Text('Combo Price',
                  style: theme.textTheme.titleSmall
                      ?.copyWith(fontWeight: FontWeight.w600)),
              const SizedBox(height: 8),
              TextFormField(
                controller: _comboPriceController,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: 'Combo Price (paise)',
                  hintText: '29900',
                ),
              ),
              const SizedBox(height: 16),
            ],

            const SizedBox(height: 24),

            // Save button
            SizedBox(
              height: 48,
              child: FilledButton(
                onPressed: _isSaving ? null : _save,
                child: _isSaving
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white),
                      )
                    : Text(_isEditing ? 'Update Promotion' : 'Create Promotion'),
              ),
            ),
            const SizedBox(height: 40),
          ],
        ),
      ),
    );
  }

  Future<void> _pickDateRange() async {
    final picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime.now().subtract(const Duration(days: 1)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
      initialDateRange: _dateRange,
    );
    if (picked != null) setState(() => _dateRange = picked);
  }

  String _formatDate(DateTime d) =>
      '${d.day}/${d.month}/${d.year}';

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    if (_dateRange == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please select validity dates')),
      );
      return;
    }

    setState(() => _isSaving = true);

    final data = <String, dynamic>{
      'title': _titleController.text.trim(),
      'description': _descriptionController.text.trim().isEmpty
          ? null
          : _descriptionController.text.trim(),
      'imageUrl': _imageUrlController.text.trim().isEmpty
          ? null
          : _imageUrlController.text.trim(),
      'promotionType': _promotionType,
      'discountType': _discountType,
      'discountValue': int.tryParse(_discountValueController.text) ?? 0,
      'maxDiscount': int.tryParse(_maxDiscountController.text),
      'minOrderAmount': int.tryParse(_minOrderAmountController.text),
      'validFrom': _dateRange!.start.toUtc().toIso8601String(),
      'validUntil': _dateRange!.end.toUtc().toIso8601String(),
      'displayOrder': int.tryParse(_displayOrderController.text) ?? 0,
      'recurringStartTime': _promotionType == 1 && _startTimeController.text.isNotEmpty
          ? _startTimeController.text
          : null,
      'recurringEndTime': _promotionType == 1 && _endTimeController.text.isNotEmpty
          ? _endTimeController.text
          : null,
      'recurringDaysOfWeek': _promotionType == 1 ? _selectedDays : null,
      'comboPrice':
          _promotionType == 2 ? int.tryParse(_comboPriceController.text) : null,
      'menuItems': widget.promotion?.menuItems
              .map((m) => {
                    'menuItemId': m.menuItemId,
                    'quantity': m.quantity,
                  })
              .toList() ??
          [],
    };

    final repo = ref.read(promotionsRepositoryProvider);

    if (_isEditing) {
      final failure = await repo.updatePromotion(widget.promotion!.id, data);
      setState(() => _isSaving = false);
      if (failure == null && mounted) {
        Navigator.of(context).pop(true);
      } else if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(failure?.toString() ?? 'Update failed')),
        );
      }
    } else {
      final result = await repo.createPromotion(data);
      setState(() => _isSaving = false);
      if (result.failure == null && mounted) {
        Navigator.of(context).pop(true);
      } else if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
              content:
                  Text(result.failure?.toString() ?? 'Creation failed')),
        );
      }
    }
  }
}
