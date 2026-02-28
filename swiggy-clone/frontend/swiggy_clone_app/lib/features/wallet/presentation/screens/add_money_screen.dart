import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../providers/wallet_notifier.dart';
import '../providers/wallet_state.dart';

class AddMoneyScreen extends ConsumerStatefulWidget {
  const AddMoneyScreen({super.key});

  @override
  ConsumerState<AddMoneyScreen> createState() => _AddMoneyScreenState();
}

class _AddMoneyScreenState extends ConsumerState<AddMoneyScreen> {
  final _formKey = GlobalKey<FormState>();
  final _amountController = TextEditingController();

  static const _quickAmounts = [100, 200, 500, 1000];

  @override
  void dispose() {
    _amountController.dispose();
    super.dispose();
  }

  Future<void> _handleAddMoney() async {
    if (!_formKey.currentState!.validate()) return;

    final amount = int.tryParse(_amountController.text);
    if (amount == null || amount <= 0) return;

    final amountPaise = amount * 100;
    final success =
        await ref.read(walletNotifierProvider.notifier).addMoney(amountPaise);

    if (mounted) {
      if (success) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('\u20B9$amount added to wallet')),
        );
        context.pop();
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Failed to add money. Please try again.')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final walletState = ref.watch(walletNotifierProvider);
    final theme = Theme.of(context);

    final currentBalance = switch (walletState) {
      WalletLoaded(:final wallet) => wallet.balancePaise,
      _ => 0,
    };
    final isAdding = switch (walletState) {
      WalletLoaded(:final isAddingMoney) => isAddingMoney,
      _ => false,
    };
    final balanceRupees = (currentBalance / 100).toStringAsFixed(2);

    return Scaffold(
      appBar: AppBar(title: const Text('Add Money')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Current balance
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: AppColors.primaryLight,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Current Balance',
                      style: theme.textTheme.bodyLarge,
                    ),
                    Text(
                      '\u20B9$balanceRupees',
                      style: theme.textTheme.titleLarge?.copyWith(
                        fontWeight: FontWeight.bold,
                        color: AppColors.primary,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 24),

              // Amount input
              TextFormField(
                controller: _amountController,
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                decoration: const InputDecoration(
                  prefixText: '\u20B9 ',
                  labelText: 'Amount',
                  hintText: 'Enter amount',
                  border: OutlineInputBorder(),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter an amount';
                  }
                  final amount = int.tryParse(value);
                  if (amount == null || amount <= 0) {
                    return 'Please enter a valid amount';
                  }
                  if (amount > 10000) {
                    return 'Maximum amount is \u20B910,000';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Quick amount chips
              Wrap(
                spacing: 8,
                runSpacing: 8,
                children: _quickAmounts
                    .map(
                      (amount) => ActionChip(
                        label: Text('\u20B9$amount'),
                        onPressed: () {
                          _amountController.text = amount.toString();
                        },
                        backgroundColor: AppColors.primaryLight,
                        labelStyle: const TextStyle(color: AppColors.primary),
                      ),
                    )
                    .toList(),
              ),
              const SizedBox(height: 32),

              // Add money button
              FilledButton(
                onPressed: isAdding ? null : _handleAddMoney,
                style: FilledButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                  backgroundColor: AppColors.primary,
                ),
                child: isAdding
                    ? const SizedBox(
                        width: 20,
                        height: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: Colors.white,
                        ),
                      )
                    : const Text('Add Money'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
