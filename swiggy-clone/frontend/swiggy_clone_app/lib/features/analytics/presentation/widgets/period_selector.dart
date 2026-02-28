import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class PeriodSelector extends StatelessWidget {
  const PeriodSelector({
    super.key,
    required this.selected,
    required this.onChanged,
  });

  final String selected;
  final ValueChanged<String> onChanged;

  static const _periods = [
    (value: 'daily', label: 'Daily'),
    (value: 'weekly', label: 'Weekly'),
    (value: 'monthly', label: 'Monthly'),
  ];

  @override
  Widget build(BuildContext context) {
    return SegmentedButton<String>(
      segments: _periods
          .map((p) => ButtonSegment<String>(
                value: p.value,
                label: Text(p.label),
              ))
          .toList(),
      selected: {selected},
      onSelectionChanged: (s) => onChanged(s.first),
      style: SegmentedButton.styleFrom(
        selectedForegroundColor: Colors.white,
        selectedBackgroundColor: AppColors.primary,
      ),
    );
  }
}
