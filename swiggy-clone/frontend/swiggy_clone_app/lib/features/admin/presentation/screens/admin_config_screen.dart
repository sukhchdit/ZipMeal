import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/admin_platform_config_model.dart';
import '../providers/admin_config_notifier.dart';
import '../providers/admin_config_state.dart';

/// Admin screen for viewing and editing platform configuration (fees, tax, etc.).
class AdminConfigScreen extends ConsumerStatefulWidget {
  const AdminConfigScreen({super.key});

  @override
  ConsumerState<AdminConfigScreen> createState() => _AdminConfigScreenState();
}

class _AdminConfigScreenState extends ConsumerState<AdminConfigScreen> {
  final _formKey = GlobalKey<FormState>();
  final _deliveryFeeController = TextEditingController();
  final _packagingChargeController = TextEditingController();
  final _taxRateController = TextEditingController();
  final _freeDeliveryController = TextEditingController();
  bool _freeDeliveryEnabled = false;
  bool _formInitialized = false;

  @override
  void dispose() {
    _deliveryFeeController.dispose();
    _packagingChargeController.dispose();
    _taxRateController.dispose();
    _freeDeliveryController.dispose();
    super.dispose();
  }

  void _populateForm(AdminPlatformConfigModel config) {
    _deliveryFeeController.text =
        (config.deliveryFeePaise / 100).toStringAsFixed(2);
    _packagingChargeController.text =
        (config.packagingChargePaise / 100).toStringAsFixed(2);
    _taxRateController.text = config.taxRatePercent.toStringAsFixed(2);
    _freeDeliveryEnabled = config.freeDeliveryThresholdPaise != null;
    _freeDeliveryController.text = config.freeDeliveryThresholdPaise != null
        ? (config.freeDeliveryThresholdPaise! / 100).toStringAsFixed(2)
        : '';
    _formInitialized = true;
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;

    final deliveryFeePaise =
        (double.parse(_deliveryFeeController.text.trim()) * 100).round();
    final packagingChargePaise =
        (double.parse(_packagingChargeController.text.trim()) * 100).round();
    final taxRatePercent =
        double.parse(_taxRateController.text.trim());
    final freeDeliveryThresholdPaise = _freeDeliveryEnabled &&
            _freeDeliveryController.text.trim().isNotEmpty
        ? (double.parse(_freeDeliveryController.text.trim()) * 100).round()
        : null;

    final success = await ref
        .read(adminConfigNotifierProvider.notifier)
        .updateConfig(
          deliveryFeePaise: deliveryFeePaise,
          packagingChargePaise: packagingChargePaise,
          taxRatePercent: taxRatePercent,
          freeDeliveryThresholdPaise: freeDeliveryThresholdPaise,
        );

    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          success ? 'Configuration updated' : 'Failed to update configuration',
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(adminConfigNotifierProvider);
    final theme = Theme.of(context);

    // Populate form when loaded/saved
    if (!_formInitialized) {
      if (state is AdminConfigLoaded) {
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (mounted) setState(() => _populateForm(state.config));
        });
      } else if (state is AdminConfigSaved) {
        WidgetsBinding.instance.addPostFrameCallback((_) {
          if (mounted) setState(() => _populateForm(state.config));
        });
      }
    }

    final isBusy = state is AdminConfigInitial ||
        state is AdminConfigLoading ||
        state is AdminConfigSaving;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Platform Config'),
        actions: [
          if (_formInitialized)
            IconButton(
              icon: const Icon(Icons.save),
              onPressed: isBusy ? null : _save,
            ),
        ],
      ),
      body: switch (state) {
        AdminConfigInitial() || AdminConfigLoading() =>
          const AppLoadingWidget(message: 'Loading configuration...'),
        AdminConfigError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(adminConfigNotifierProvider.notifier)
                .loadConfig(),
          ),
        AdminConfigSaving() => const AppLoadingWidget(message: 'Saving...'),
        AdminConfigLoaded() || AdminConfigSaved() => _buildForm(theme, state),
      },
    );
  }

  Widget _buildForm(ThemeData theme, AdminConfigState state) {
    final config = state is AdminConfigLoaded
        ? state.config
        : (state as AdminConfigSaved).config;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Delivery Fee
            TextFormField(
              controller: _deliveryFeeController,
              decoration: const InputDecoration(
                labelText: 'Delivery Fee (\u20B9)',
                prefixText: '\u20B9 ',
                hintText: '0.00',
              ),
              keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
              inputFormatters: [
                FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d{0,2}')),
              ],
              validator: (v) {
                if (v == null || v.trim().isEmpty) return 'Required';
                if (double.tryParse(v.trim()) == null) return 'Invalid amount';
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Packaging Charge
            TextFormField(
              controller: _packagingChargeController,
              decoration: const InputDecoration(
                labelText: 'Packaging Charge (\u20B9)',
                prefixText: '\u20B9 ',
                hintText: '0.00',
              ),
              keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
              inputFormatters: [
                FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d{0,2}')),
              ],
              validator: (v) {
                if (v == null || v.trim().isEmpty) return 'Required';
                if (double.tryParse(v.trim()) == null) return 'Invalid amount';
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Tax Rate
            TextFormField(
              controller: _taxRateController,
              decoration: const InputDecoration(
                labelText: 'Tax Rate (%)',
                suffixText: '%',
                hintText: '0.00',
              ),
              keyboardType:
                  const TextInputType.numberWithOptions(decimal: true),
              inputFormatters: [
                FilteringTextInputFormatter.allow(RegExp(r'^\d*\.?\d{0,2}')),
              ],
              validator: (v) {
                if (v == null || v.trim().isEmpty) return 'Required';
                final rate = double.tryParse(v.trim());
                if (rate == null) return 'Invalid rate';
                if (rate < 0 || rate > 100) return 'Must be 0–100';
                return null;
              },
            ),
            const SizedBox(height: 16),

            // Free Delivery Threshold Toggle
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              title: const Text('Free Delivery Threshold'),
              subtitle: const Text(
                'Enable to set a minimum order amount for free delivery',
              ),
              value: _freeDeliveryEnabled,
              onChanged: (v) => setState(() => _freeDeliveryEnabled = v),
            ),

            // Free Delivery Threshold Amount
            if (_freeDeliveryEnabled) ...[
              const SizedBox(height: 8),
              TextFormField(
                controller: _freeDeliveryController,
                decoration: const InputDecoration(
                  labelText: 'Free Delivery Above (\u20B9)',
                  prefixText: '\u20B9 ',
                  hintText: '0.00',
                ),
                keyboardType:
                    const TextInputType.numberWithOptions(decimal: true),
                inputFormatters: [
                  FilteringTextInputFormatter.allow(
                      RegExp(r'^\d*\.?\d{0,2}')),
                ],
                validator: (v) {
                  if (!_freeDeliveryEnabled) return null;
                  if (v == null || v.trim().isEmpty) return 'Required';
                  if (double.tryParse(v.trim()) == null) {
                    return 'Invalid amount';
                  }
                  return null;
                },
              ),
            ],

            const SizedBox(height: 24),

            // Last Updated
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: AppColors.primaryLight,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  const Icon(Icons.info_outline,
                      size: 18, color: AppColors.primary),
                  const SizedBox(width: 8),
                  Text(
                    'Last updated: ${_formatDateTime(config.updatedAt)}',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.primary,
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 24),

            // Save Button
            FilledButton.icon(
              onPressed: _save,
              icon: const Icon(Icons.save),
              label: const Text('Save Configuration'),
            ),
          ],
        ),
      ),
    );
  }

  String _formatDateTime(DateTime dt) {
    final local = dt.toLocal();
    return '${local.day.toString().padLeft(2, '0')}/${local.month.toString().padLeft(2, '0')}/${local.year} '
        '${local.hour.toString().padLeft(2, '0')}:${local.minute.toString().padLeft(2, '0')}';
  }
}
