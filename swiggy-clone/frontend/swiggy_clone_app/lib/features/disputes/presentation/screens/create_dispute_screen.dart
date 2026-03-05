import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../routing/route_names.dart';
import '../providers/create_dispute_notifier.dart';
import '../providers/create_dispute_state.dart';
import '../widgets/issue_type_selector.dart';

class CreateDisputeScreen extends ConsumerStatefulWidget {
  const CreateDisputeScreen({super.key});

  @override
  ConsumerState<CreateDisputeScreen> createState() =>
      _CreateDisputeScreenState();
}

class _CreateDisputeScreenState extends ConsumerState<CreateDisputeScreen> {
  final _formKey = GlobalKey<FormState>();
  final _orderIdController = TextEditingController();
  final _descriptionController = TextEditingController();
  int? _selectedIssueType;

  @override
  void dispose() {
    _orderIdController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(createDisputeNotifierProvider);
    final l10n = context.l10n;

    ref.listen(createDisputeNotifierProvider, (_, next) {
      next.map(
        initial: (_) {},
        submitting: (_) {},
        success: (s) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text(l10n.disputeCreated)),
          );
          context.pushReplacement(
              RouteNames.disputeDetailPath(s.dispute.id));
        },
        error: (e) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text(e.message)),
          );
        },
      );
    });

    final isSubmitting = state is CreateDisputeSubmitting;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.reportOrderIssue)),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Order ID
            Text(l10n.selectOrder,
                style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 8),
            TextFormField(
              controller: _orderIdController,
              decoration: InputDecoration(
                hintText: 'Enter Order ID',
                border: const OutlineInputBorder(),
                prefixIcon: const Icon(Icons.receipt_long),
              ),
              validator: (v) =>
                  v == null || v.trim().isEmpty ? 'Order ID is required' : null,
            ),
            const SizedBox(height: 24),

            // Issue Type
            Text(l10n.selectIssueType,
                style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 8),
            IssueTypeSelector(
              selectedType: _selectedIssueType,
              onSelected: (type) =>
                  setState(() => _selectedIssueType = type),
            ),
            const SizedBox(height: 24),

            // Description
            Text(l10n.issueDescription,
                style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 8),
            TextFormField(
              controller: _descriptionController,
              maxLines: 5,
              maxLength: 2000,
              decoration: InputDecoration(
                hintText: l10n.issueDescriptionHint,
                border: const OutlineInputBorder(),
                alignLabelWithHint: true,
              ),
              validator: (v) => v == null || v.trim().isEmpty
                  ? 'Description is required'
                  : null,
            ),
            const SizedBox(height: 32),

            // Submit
            FilledButton(
              onPressed: isSubmitting ? null : _submit,
              child: isSubmitting
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : Text(l10n.submitDispute),
            ),
          ],
        ),
      ),
    );
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    if (_selectedIssueType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(context.l10n.selectIssueType)),
      );
      return;
    }
    ref.read(createDisputeNotifierProvider.notifier).submit(
          orderId: _orderIdController.text.trim(),
          issueType: _selectedIssueType!,
          description: _descriptionController.text.trim(),
        );
  }
}
