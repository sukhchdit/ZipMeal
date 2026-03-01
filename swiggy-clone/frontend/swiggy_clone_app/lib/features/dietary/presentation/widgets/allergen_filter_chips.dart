import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

/// Horizontal scrollable row of dietary filter chips.
/// Used on the restaurant detail screen to filter menu items.
class AllergenFilterChips extends StatelessWidget {
  const AllergenFilterChips({
    required this.selectedFilters,
    required this.onChanged,
    super.key,
  });

  final Set<String> selectedFilters;
  final ValueChanged<Set<String>> onChanged;

  static const _filters = [
    ('vegOnly', 'Veg Only', Icons.eco),
    ('vegan', 'Vegan', Icons.spa),
    ('glutenFree', 'Gluten-Free', Icons.no_food),
    ('jain', 'Jain', Icons.grass),
    ('keto', 'Keto', Icons.fitness_center),
    ('halal', 'Halal', Icons.verified),
  ];

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 44,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 16),
        itemCount: _filters.length,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, index) {
          final (key, label, icon) = _filters[index];
          final selected = selectedFilters.contains(key);
          return FilterChip(
            avatar: Icon(icon, size: 16),
            label: Text(label),
            selected: selected,
            onSelected: (v) {
              final newSet = Set<String>.from(selectedFilters);
              if (v) {
                newSet.add(key);
              } else {
                newSet.remove(key);
              }
              onChanged(newSet);
            },
            selectedColor: AppColors.primaryLight,
            checkmarkColor: AppColors.primary,
            visualDensity: VisualDensity.compact,
          );
        },
      ),
    );
  }
}
