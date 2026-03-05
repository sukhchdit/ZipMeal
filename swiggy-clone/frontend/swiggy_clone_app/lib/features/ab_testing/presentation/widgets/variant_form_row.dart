import 'package:flutter/material.dart';

class VariantFormRow extends StatelessWidget {
  const VariantFormRow({
    super.key,
    required this.index,
    required this.keyController,
    required this.nameController,
    required this.allocationController,
    required this.configJsonController,
    required this.isControl,
    required this.onControlChanged,
    required this.onRemove,
    this.canRemove = true,
  });

  final int index;
  final TextEditingController keyController;
  final TextEditingController nameController;
  final TextEditingController allocationController;
  final TextEditingController configJsonController;
  final bool isControl;
  final ValueChanged<bool> onControlChanged;
  final VoidCallback onRemove;
  final bool canRemove;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  'Variant ${index + 1}',
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
                const Spacer(),
                if (canRemove)
                  IconButton(
                    icon: const Icon(Icons.close, size: 20),
                    onPressed: onRemove,
                    tooltip: 'Remove variant',
                    visualDensity: VisualDensity.compact,
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: keyController,
                    decoration: const InputDecoration(
                      labelText: 'Key',
                      hintText: 'e.g. control',
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    validator: (v) =>
                        (v == null || v.trim().isEmpty) ? 'Required' : null,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: TextFormField(
                    controller: nameController,
                    decoration: const InputDecoration(
                      labelText: 'Name',
                      hintText: 'e.g. Control Group',
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    validator: (v) =>
                        (v == null || v.trim().isEmpty) ? 'Required' : null,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                SizedBox(
                  width: 120,
                  child: TextFormField(
                    controller: allocationController,
                    decoration: const InputDecoration(
                      labelText: 'Allocation %',
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    keyboardType: TextInputType.number,
                    validator: (v) {
                      if (v == null || v.trim().isEmpty) return 'Required';
                      final n = int.tryParse(v.trim());
                      if (n == null || n < 0 || n > 100) {
                        return '0-100';
                      }
                      return null;
                    },
                  ),
                ),
                const SizedBox(width: 16),
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Switch(
                      value: isControl,
                      onChanged: (v) => onControlChanged(v),
                    ),
                    Text(
                      'Control',
                      style: theme.textTheme.bodySmall,
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: configJsonController,
              decoration: const InputDecoration(
                labelText: 'Config JSON (optional)',
                hintText: '{"feature_flag": true}',
                border: OutlineInputBorder(),
                isDense: true,
              ),
              maxLines: 2,
            ),
          ],
        ),
      ),
    );
  }
}
