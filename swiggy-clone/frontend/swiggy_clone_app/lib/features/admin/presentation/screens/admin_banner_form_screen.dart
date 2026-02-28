import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/admin_banner_model.dart';
import '../providers/admin_banners_notifier.dart';

/// Create / edit form for a promotional banner.
class AdminBannerFormScreen extends ConsumerStatefulWidget {
  const AdminBannerFormScreen({super.key, this.banner});

  final AdminBannerModel? banner;

  @override
  ConsumerState<AdminBannerFormScreen> createState() =>
      _AdminBannerFormScreenState();
}

class _AdminBannerFormScreenState
    extends ConsumerState<AdminBannerFormScreen> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _titleController;
  late final TextEditingController _imageUrlController;
  late final TextEditingController _deepLinkController;
  late final TextEditingController _displayOrderController;
  late DateTime _validFrom;
  late DateTime _validUntil;
  bool _isSaving = false;

  bool get _isEditing => widget.banner != null;

  @override
  void initState() {
    super.initState();
    final b = widget.banner;
    _titleController = TextEditingController(text: b?.title ?? '');
    _imageUrlController = TextEditingController(text: b?.imageUrl ?? '');
    _deepLinkController = TextEditingController(text: b?.deepLink ?? '');
    _displayOrderController =
        TextEditingController(text: (b?.displayOrder ?? 0).toString());
    _validFrom = b?.validFrom ?? DateTime.now();
    _validUntil =
        b?.validUntil ?? DateTime.now().add(const Duration(days: 30));
  }

  @override
  void dispose() {
    _titleController.dispose();
    _imageUrlController.dispose();
    _deepLinkController.dispose();
    _displayOrderController.dispose();
    super.dispose();
  }

  Future<void> _pickDate({required bool isFrom}) async {
    final initial = isFrom ? _validFrom : _validUntil;
    final picked = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: DateTime(2020),
      lastDate: DateTime(2030),
    );
    if (picked != null) {
      setState(() {
        if (isFrom) {
          _validFrom = picked;
        } else {
          _validUntil = picked;
        }
      });
    }
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;

    if (_validFrom.isAfter(_validUntil)) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('"Valid From" must be before "Valid Until"')),
      );
      return;
    }

    setState(() => _isSaving = true);

    final data = {
      'title': _titleController.text.trim(),
      'imageUrl': _imageUrlController.text.trim(),
      'deepLink': _deepLinkController.text.trim().isEmpty
          ? null
          : _deepLinkController.text.trim(),
      'displayOrder': int.parse(_displayOrderController.text.trim()),
      'validFrom': _validFrom.toUtc().toIso8601String(),
      'validUntil': _validUntil.toUtc().toIso8601String(),
    };

    final notifier = ref.read(adminBannersNotifierProvider.notifier);
    final success = _isEditing
        ? await notifier.updateBanner(widget.banner!.id, data)
        : await notifier.createBanner(data);

    if (!mounted) return;
    setState(() => _isSaving = false);

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            _isEditing ? 'Banner updated' : 'Banner created',
          ),
        ),
      );
      Navigator.of(context).pop(true);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to save banner')),
      );
    }
  }

  String _formatDate(DateTime dt) {
    return '${dt.day.toString().padLeft(2, '0')}/${dt.month.toString().padLeft(2, '0')}/${dt.year}';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(_isEditing ? 'Edit Banner' : 'Create Banner'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Title
              TextFormField(
                controller: _titleController,
                decoration: const InputDecoration(
                  labelText: 'Title *',
                  hintText: 'e.g. Summer Sale',
                ),
                maxLength: 200,
                validator: (v) =>
                    (v == null || v.trim().isEmpty) ? 'Title is required' : null,
              ),
              const SizedBox(height: 12),

              // Image URL
              TextFormField(
                controller: _imageUrlController,
                decoration: const InputDecoration(
                  labelText: 'Image URL *',
                  hintText: 'https://...',
                ),
                maxLength: 500,
                validator: (v) => (v == null || v.trim().isEmpty)
                    ? 'Image URL is required'
                    : null,
              ),
              const SizedBox(height: 8),

              // Image Preview
              if (_imageUrlController.text.trim().isNotEmpty)
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Image.network(
                    _imageUrlController.text.trim(),
                    height: 160,
                    width: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => Container(
                      height: 160,
                      decoration: BoxDecoration(
                        color: AppColors.primaryLight,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: const Center(
                        child: Icon(Icons.broken_image,
                            size: 48, color: AppColors.textTertiaryLight),
                      ),
                    ),
                  ),
                )
              else
                Container(
                  height: 120,
                  decoration: BoxDecoration(
                    color: AppColors.primaryLight,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Center(
                    child: Text('Enter URL to preview image'),
                  ),
                ),
              const SizedBox(height: 16),

              // Deep Link
              TextFormField(
                controller: _deepLinkController,
                decoration: const InputDecoration(
                  labelText: 'Deep Link (optional)',
                  hintText: '/restaurant/abc123',
                ),
                maxLength: 500,
              ),
              const SizedBox(height: 12),

              // Display Order
              TextFormField(
                controller: _displayOrderController,
                decoration: const InputDecoration(
                  labelText: 'Display Order',
                  hintText: '0',
                ),
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                validator: (v) {
                  if (v == null || v.trim().isEmpty) {
                    return 'Display order is required';
                  }
                  final n = int.tryParse(v.trim());
                  if (n == null || n < 0) return 'Must be 0 or greater';
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Date Pickers
              Text('Validity Period',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  )),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: _DatePickerTile(
                      label: 'From',
                      date: _validFrom,
                      formattedDate: _formatDate(_validFrom),
                      onTap: () => _pickDate(isFrom: true),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _DatePickerTile(
                      label: 'Until',
                      date: _validUntil,
                      formattedDate: _formatDate(_validUntil),
                      onTap: () => _pickDate(isFrom: false),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 32),

              // Save Button
              FilledButton.icon(
                onPressed: _isSaving ? null : _save,
                icon: _isSaving
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.save),
                label: Text(_isEditing ? 'Update Banner' : 'Create Banner'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _DatePickerTile extends StatelessWidget {
  const _DatePickerTile({
    required this.label,
    required this.date,
    required this.formattedDate,
    required this.onTap,
  });

  final String label;
  final DateTime date;
  final String formattedDate;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 14),
        decoration: BoxDecoration(
          border: Border.all(color: theme.dividerColor),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(label,
                      style: theme.textTheme.labelSmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      )),
                  const SizedBox(height: 2),
                  Text(formattedDate,
                      style: theme.textTheme.bodyMedium),
                ],
              ),
            ),
            const Icon(Icons.calendar_today, size: 18),
          ],
        ),
      ),
    );
  }
}
