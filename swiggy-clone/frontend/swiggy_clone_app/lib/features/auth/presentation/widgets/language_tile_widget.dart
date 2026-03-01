import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

/// A radio-style list tile for selecting a language in the language picker.
class LanguageTileWidget extends StatelessWidget {
  const LanguageTileWidget({
    super.key,
    required this.languageName,
    required this.nativeName,
    required this.isSelected,
    required this.onTap,
  });

  final String languageName;
  final String nativeName;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return RadioListTile<bool>(
      value: true,
      groupValue: isSelected ? true : null,
      onChanged: (_) => onTap(),
      activeColor: AppColors.primary,
      title: Text(
        languageName,
        style: theme.textTheme.bodyLarge?.copyWith(
          fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
        ),
      ),
      subtitle: Text(
        nativeName,
        style: theme.textTheme.bodyMedium?.copyWith(
          color: AppColors.textSecondaryLight,
        ),
      ),
    );
  }
}
