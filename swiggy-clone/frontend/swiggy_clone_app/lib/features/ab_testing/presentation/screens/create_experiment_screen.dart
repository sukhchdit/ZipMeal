import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../providers/create_experiment_notifier.dart';
import '../providers/create_experiment_state.dart';
import '../widgets/variant_form_row.dart';

class CreateExperimentScreen extends ConsumerStatefulWidget {
  const CreateExperimentScreen({super.key});

  @override
  ConsumerState<CreateExperimentScreen> createState() =>
      _CreateExperimentScreenState();
}

class _CreateExperimentScreenState
    extends ConsumerState<CreateExperimentScreen> {
  final _formKey = GlobalKey<FormState>();
  final _keyController = TextEditingController();
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _targetAudienceController = TextEditingController();
  final _goalDescriptionController = TextEditingController();
  DateTime? _startDate;
  DateTime? _endDate;

  final List<_VariantData> _variants = [];

  @override
  void initState() {
    super.initState();
    // Start with 2 variants: control + treatment
    _addVariant(isControl: true);
    _addVariant();
  }

  void _addVariant({bool isControl = false}) {
    _variants.add(_VariantData(
      keyController: TextEditingController(),
      nameController: TextEditingController(),
      allocationController: TextEditingController(text: '50'),
      configJsonController: TextEditingController(),
      isControl: isControl,
    ));
    setState(() {});
  }

  void _removeVariant(int index) {
    if (_variants.length <= 2) return;
    _variants[index].dispose();
    _variants.removeAt(index);
    setState(() {});
  }

  void _setControl(int index, bool value) {
    setState(() {
      for (int i = 0; i < _variants.length; i++) {
        _variants[i].isControl = (i == index && value);
      }
    });
  }

  @override
  void dispose() {
    _keyController.dispose();
    _nameController.dispose();
    _descriptionController.dispose();
    _targetAudienceController.dispose();
    _goalDescriptionController.dispose();
    for (final v in _variants) {
      v.dispose();
    }
    super.dispose();
  }

  Future<void> _pickDate(bool isStart) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime.now().subtract(const Duration(days: 30)),
      lastDate: DateTime.now().add(const Duration(days: 365)),
    );
    if (picked != null) {
      setState(() {
        if (isStart) {
          _startDate = picked;
        } else {
          _endDate = picked;
        }
      });
    }
  }

  String? _validateForm() {
    final allocations = _variants
        .map((v) => int.tryParse(v.allocationController.text.trim()) ?? 0)
        .toList();
    final sum = allocations.fold(0, (a, b) => a + b);
    if (sum != 100) {
      return 'Variant allocations must sum to 100% (currently $sum%)';
    }

    final controlCount = _variants.where((v) => v.isControl).length;
    if (controlCount != 1) {
      return 'Exactly one variant must be marked as control';
    }

    return null;
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;

    final validationError = _validateForm();
    if (validationError != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(validationError)),
      );
      return;
    }

    final variants = _variants
        .map((v) => {
              'key': v.keyController.text.trim(),
              'name': v.nameController.text.trim(),
              'allocationPercent':
                  int.parse(v.allocationController.text.trim()),
              if (v.configJsonController.text.trim().isNotEmpty)
                'configJson': v.configJsonController.text.trim(),
              'isControl': v.isControl,
            })
        .toList();

    ref.read(createExperimentNotifierProvider.notifier).submit(
          key: _keyController.text.trim(),
          name: _nameController.text.trim(),
          description: _descriptionController.text.trim().isNotEmpty
              ? _descriptionController.text.trim()
              : null,
          targetAudience: _targetAudienceController.text.trim().isNotEmpty
              ? _targetAudienceController.text.trim()
              : null,
          startDate: _startDate?.toUtc().toIso8601String(),
          endDate: _endDate?.toUtc().toIso8601String(),
          goalDescription: _goalDescriptionController.text.trim().isNotEmpty
              ? _goalDescriptionController.text.trim()
              : null,
          variants: variants,
        );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(createExperimentNotifierProvider);
    final isSubmitting = state is CreateExperimentSubmitting;

    ref.listen(createExperimentNotifierProvider, (_, next) {
      next.map(
        initial: (_) {},
        submitting: (_) {},
        success: (s) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Experiment created successfully')),
          );
          context.pop();
        },
        error: (e) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text(e.message)),
          );
        },
      );
    });

    return Scaffold(
      appBar: AppBar(title: const Text('Create Experiment')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            TextFormField(
              controller: _keyController,
              decoration: const InputDecoration(
                labelText: 'Experiment Key *',
                hintText: 'e.g. homepage_hero_redesign',
                border: OutlineInputBorder(),
              ),
              validator: (v) =>
                  (v == null || v.trim().isEmpty) ? 'Required' : null,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Name *',
                hintText: 'e.g. Homepage Hero Redesign',
                border: OutlineInputBorder(),
              ),
              validator: (v) =>
                  (v == null || v.trim().isEmpty) ? 'Required' : null,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _targetAudienceController,
              decoration: const InputDecoration(
                labelText: 'Target Audience',
                hintText: 'e.g. new_users, premium_subscribers',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _goalDescriptionController,
              decoration: const InputDecoration(
                labelText: 'Goal Description',
                hintText: 'e.g. Increase click-through rate',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: InkWell(
                    onTap: () => _pickDate(true),
                    child: InputDecorator(
                      decoration: const InputDecoration(
                        labelText: 'Start Date',
                        border: OutlineInputBorder(),
                        suffixIcon: Icon(Icons.calendar_today),
                      ),
                      child: Text(
                        _startDate != null
                            ? '${_startDate!.day}/${_startDate!.month}/${_startDate!.year}'
                            : 'Select',
                        style: Theme.of(context).textTheme.bodyMedium,
                      ),
                    ),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: InkWell(
                    onTap: () => _pickDate(false),
                    child: InputDecorator(
                      decoration: const InputDecoration(
                        labelText: 'End Date',
                        border: OutlineInputBorder(),
                        suffixIcon: Icon(Icons.calendar_today),
                      ),
                      child: Text(
                        _endDate != null
                            ? '${_endDate!.day}/${_endDate!.month}/${_endDate!.year}'
                            : 'Select',
                        style: Theme.of(context).textTheme.bodyMedium,
                      ),
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            Row(
              children: [
                Text(
                  'Variants',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                ),
                const Spacer(),
                TextButton.icon(
                  onPressed: () => _addVariant(),
                  icon: const Icon(Icons.add, size: 18),
                  label: const Text('Add Variant'),
                ),
              ],
            ),
            const SizedBox(height: 8),
            ...List.generate(_variants.length, (index) {
              final v = _variants[index];
              return VariantFormRow(
                index: index,
                keyController: v.keyController,
                nameController: v.nameController,
                allocationController: v.allocationController,
                configJsonController: v.configJsonController,
                isControl: v.isControl,
                onControlChanged: (val) => _setControl(index, val),
                onRemove: () => _removeVariant(index),
                canRemove: _variants.length > 2,
              );
            }),
            const SizedBox(height: 24),
            FilledButton(
              onPressed: isSubmitting ? null : _submit,
              child: isSubmitting
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Create Experiment'),
            ),
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }
}

class _VariantData {
  _VariantData({
    required this.keyController,
    required this.nameController,
    required this.allocationController,
    required this.configJsonController,
    this.isControl = false,
  });

  final TextEditingController keyController;
  final TextEditingController nameController;
  final TextEditingController allocationController;
  final TextEditingController configJsonController;
  bool isControl;

  void dispose() {
    keyController.dispose();
    nameController.dispose();
    allocationController.dispose();
    configJsonController.dispose();
  }
}
