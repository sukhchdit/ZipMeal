import 'package:flutter/material.dart';

import '../../../../core/extensions/l10n_extensions.dart';

class IssueTypeSelector extends StatelessWidget {
  const IssueTypeSelector({
    super.key,
    required this.selectedType,
    required this.onSelected,
  });

  final int? selectedType;
  final ValueChanged<int> onSelected;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final types = [
      (0, l10n.issueWrongItems, Icons.swap_horiz),
      (1, l10n.issueMissingItems, Icons.remove_shopping_cart),
      (2, l10n.issueQualityIssue, Icons.thumb_down_outlined),
      (3, l10n.issueLateDelivery, Icons.access_time),
      (4, l10n.issueNeverDelivered, Icons.cancel_outlined),
      (5, l10n.issueWrongOrder, Icons.shuffle),
      (6, l10n.issueDamagedPackaging, Icons.broken_image_outlined),
      (7, l10n.issueOther, Icons.help_outline),
    ];

    return Wrap(
      spacing: 10,
      runSpacing: 10,
      children: types.map((t) {
        final (value, label, icon) = t;
        final isSelected = selectedType == value;
        return ChoiceChip(
          avatar: Icon(icon, size: 18),
          label: Text(label),
          selected: isSelected,
          onSelected: (_) => onSelected(value),
        );
      }).toList(),
    );
  }
}
