import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class ReviewReportDialog extends StatefulWidget {
  const ReviewReportDialog({super.key});

  @override
  State<ReviewReportDialog> createState() => _ReviewReportDialogState();
}

class _ReviewReportDialogState extends State<ReviewReportDialog> {
  String? _selectedReason;
  final _descriptionController = TextEditingController();

  static const _reasons = [
    ('Spam', 'Spam'),
    ('Inappropriate', 'Inappropriate'),
    ('FakeReview', 'Fake Review'),
    ('Harassment', 'Harassment'),
    ('Other', 'Other'),
  ];

  @override
  void dispose() {
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: EdgeInsets.only(
        left: 16,
        right: 16,
        top: 16,
        bottom: MediaQuery.of(context).viewInsets.bottom + 16,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Report Review',
            style: theme.textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          ..._reasons.map((r) => RadioListTile<String>(
                value: r.$1,
                groupValue: _selectedReason,
                title: Text(r.$2),
                onChanged: (v) => setState(() => _selectedReason = v),
                contentPadding: EdgeInsets.zero,
                dense: true,
                activeColor: AppColors.primary,
              )),
          const SizedBox(height: 8),
          TextField(
            controller: _descriptionController,
            maxLines: 3,
            maxLength: 1000,
            decoration: const InputDecoration(
              hintText: 'Additional details (optional)',
              border: OutlineInputBorder(),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _selectedReason == null
                  ? null
                  : () => Navigator.pop(context, {
                        'reason': _selectedReason,
                        'description': _descriptionController.text.isNotEmpty
                            ? _descriptionController.text
                            : null,
                      }),
              style: FilledButton.styleFrom(backgroundColor: AppColors.primary),
              child: const Text('Submit Report'),
            ),
          ),
        ],
      ),
    );
  }
}
