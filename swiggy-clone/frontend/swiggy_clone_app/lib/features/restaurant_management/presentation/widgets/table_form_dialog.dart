import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class TableFormDialog extends StatefulWidget {
  const TableFormDialog({
    required this.onSubmit,
    this.initialTableNumber,
    this.initialCapacity,
    this.initialFloorSection,
    super.key,
  });

  final Future<void> Function(Map<String, dynamic> data) onSubmit;
  final String? initialTableNumber;
  final int? initialCapacity;
  final String? initialFloorSection;

  @override
  State<TableFormDialog> createState() => _TableFormDialogState();
}

class _TableFormDialogState extends State<TableFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _tableNumberController;
  late final TextEditingController _capacityController;
  late final TextEditingController _floorSectionController;
  bool _isSubmitting = false;

  bool get _isEdit => widget.initialTableNumber != null;

  @override
  void initState() {
    super.initState();
    _tableNumberController =
        TextEditingController(text: widget.initialTableNumber ?? '');
    _capacityController = TextEditingController(
        text: (widget.initialCapacity ?? 4).toString());
    _floorSectionController =
        TextEditingController(text: widget.initialFloorSection ?? '');
  }

  @override
  void dispose() {
    _tableNumberController.dispose();
    _capacityController.dispose();
    _floorSectionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: EdgeInsets.only(
        left: 16,
        right: 16,
        top: 16,
        bottom: MediaQuery.of(context).viewInsets.bottom + 16,
      ),
      child: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Text(
              _isEdit ? 'Edit Table' : 'Add Table',
              style: Theme.of(context)
                  .textTheme
                  .titleMedium
                  ?.copyWith(fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _tableNumberController,
              decoration: const InputDecoration(
                labelText: 'Table Number',
                hintText: 'e.g., T1, A-12',
              ),
              textCapitalization: TextCapitalization.characters,
              validator: (v) =>
                  v == null || v.trim().isEmpty ? 'Required' : null,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: _capacityController,
              decoration: const InputDecoration(
                labelText: 'Capacity',
                hintText: 'Number of seats',
              ),
              keyboardType: TextInputType.number,
              validator: (v) {
                if (v == null || v.trim().isEmpty) return 'Required';
                final n = int.tryParse(v.trim());
                if (n == null || n < 1 || n > 20) return '1-20';
                return null;
              },
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: _floorSectionController,
              decoration: const InputDecoration(
                labelText: 'Floor / Section (optional)',
                hintText: 'e.g., Ground Floor, Rooftop',
              ),
            ),
            const SizedBox(height: 20),
            FilledButton(
              onPressed: _isSubmitting ? null : _submit,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                padding: const EdgeInsets.symmetric(vertical: 14),
              ),
              child: _isSubmitting
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(
                          strokeWidth: 2, color: Colors.white),
                    )
                  : Text(_isEdit ? 'Update' : 'Add Table'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isSubmitting = true);

    final data = <String, dynamic>{
      'tableNumber': _tableNumberController.text.trim(),
      'capacity': int.parse(_capacityController.text.trim()),
    };
    final floor = _floorSectionController.text.trim();
    if (floor.isNotEmpty) data['floorSection'] = floor;

    await widget.onSubmit(data);

    if (mounted) setState(() => _isSubmitting = false);
  }
}
