import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class CategorySelectorWidget extends StatelessWidget {
  const CategorySelectorWidget({
    super.key,
    required this.selectedCategory,
    required this.onChanged,
  });

  final int selectedCategory;
  final ValueChanged<int> onChanged;

  static const _categories = [
    (value: 0, label: 'General'),
    (value: 1, label: 'Order Issue'),
    (value: 2, label: 'Payment Issue'),
    (value: 3, label: 'Delivery Issue'),
    (value: 4, label: 'Account Issue'),
    (value: 5, label: 'Other'),
  ];

  @override
  Widget build(BuildContext context) {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: _categories.map((cat) {
        final isSelected = selectedCategory == cat.value;
        return ChoiceChip(
          label: Text(cat.label),
          selected: isSelected,
          onSelected: (_) => onChanged(cat.value),
          selectedColor: AppColors.primary.withAlpha(30),
          labelStyle: TextStyle(
            color: isSelected ? AppColors.primary : null,
            fontWeight: isSelected ? FontWeight.w600 : null,
          ),
        );
      }).toList(),
    );
  }
}
