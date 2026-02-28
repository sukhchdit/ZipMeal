import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../providers/tip_notifier.dart';

class TipDeliverySheet extends ConsumerStatefulWidget {
  const TipDeliverySheet({required this.orderId, super.key});

  final String orderId;

  @override
  ConsumerState<TipDeliverySheet> createState() => _TipDeliverySheetState();
}

class _TipDeliverySheetState extends ConsumerState<TipDeliverySheet> {
  static const _presets = [20, 30, 50, 100];

  int _selectedAmount = 0;
  final _customController = TextEditingController();
  bool _isCustom = false;

  @override
  void dispose() {
    _customController.dispose();
    super.dispose();
  }

  void _selectPreset(int amount) {
    setState(() {
      _selectedAmount = amount;
      _isCustom = false;
      _customController.clear();
    });
  }

  void _onCustomChanged(String value) {
    final parsed = int.tryParse(value) ?? 0;
    setState(() {
      _isCustom = true;
      _selectedAmount = parsed;
    });
  }

  Future<void> _submit() async {
    if (_selectedAmount <= 0) return;

    final amountPaise = _selectedAmount * 100;
    final success = await ref
        .read(tipNotifierProvider.notifier)
        .submitTip(orderId: widget.orderId, amountPaise: amountPaise);

    if (!mounted) return;

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Thank you for the \u20B9$_selectedAmount tip!'),
          backgroundColor: AppColors.success,
        ),
      );
      Navigator.of(context).pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to submit tip. Please try again.'),
          backgroundColor: AppColors.error,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final tipState = ref.watch(tipNotifierProvider);
    final isLoading = tipState is AsyncLoading;

    return Padding(
      padding: EdgeInsets.only(
        left: 24,
        right: 24,
        top: 24,
        bottom: MediaQuery.of(context).viewInsets.bottom + 24,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.volunteer_activism, color: AppColors.primary),
              const SizedBox(width: 8),
              Text(
                'Tip your delivery partner',
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            'Your tip goes directly to the delivery partner.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
          const SizedBox(height: 16),

          // Preset chips
          Wrap(
            spacing: 8,
            children: _presets.map((amount) {
              final isSelected = !_isCustom && _selectedAmount == amount;
              return ChoiceChip(
                label: Text('\u20B9$amount'),
                selected: isSelected,
                onSelected: (_) => _selectPreset(amount),
                selectedColor: AppColors.primary.withValues(alpha: 0.15),
                labelStyle: TextStyle(
                  color: isSelected ? AppColors.primary : null,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),

          const SizedBox(height: 16),

          // Custom amount
          TextField(
            controller: _customController,
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            decoration: InputDecoration(
              prefixText: '\u20B9 ',
              hintText: 'Custom amount',
              border: const OutlineInputBorder(),
              contentPadding:
                  const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
              suffixText: _isCustom && _selectedAmount > 500 ? 'Max \u20B9500' : null,
              suffixStyle: const TextStyle(color: AppColors.error, fontSize: 12),
            ),
            onChanged: _onCustomChanged,
          ),

          const SizedBox(height: 20),

          // Submit button
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _selectedAmount > 0 && _selectedAmount <= 500 && !isLoading
                  ? _submit
                  : null,
              child: isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : Text(
                      _selectedAmount > 0
                          ? 'Submit \u20B9$_selectedAmount Tip'
                          : 'Submit Tip',
                    ),
            ),
          ),
        ],
      ),
    );
  }
}
