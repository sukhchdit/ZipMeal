import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/dietary_profile_notifier.dart';

class DietaryProfileScreen extends ConsumerStatefulWidget {
  const DietaryProfileScreen({super.key});

  @override
  ConsumerState<DietaryProfileScreen> createState() =>
      _DietaryProfileScreenState();
}

class _DietaryProfileScreenState
    extends ConsumerState<DietaryProfileScreen> {
  final Set<int> _selectedAllergens = {};
  final Set<int> _selectedDietaryTags = {};
  double _spiceLevel = 0;
  bool _initialized = false;
  bool _isSaving = false;

  static const _allergenNames = [
    'Gluten', 'Dairy', 'Nuts', 'Peanuts', 'Shellfish', 'Soy', 'Eggs',
    'Fish', 'Sesame', 'Mustard', 'Celery', 'Lupin', 'Molluscs', 'Sulfites',
  ];

  static const _dietaryTagNames = [
    'Vegan', 'Gluten-Free', 'Dairy-Free', 'Nut-Free', 'Keto',
    'Halal', 'Jain', 'Organic', 'Sugar-Free', 'High Protein',
  ];

  static const _spiceLevelNames = ['None', 'Mild', 'Medium', 'Hot', 'Extra Hot'];

  @override
  Widget build(BuildContext context) {
    final profileState = ref.watch(dietaryProfileNotifierProvider);
    final theme = Theme.of(context);
    final l10n = AppLocalizations.of(context)!;

    // Initialize from loaded profile
    profileState.whenData((profile) {
      if (!_initialized) {
        _selectedAllergens.addAll(profile.allergenAlerts);
        _selectedDietaryTags.addAll(profile.dietaryPreferences);
        _spiceLevel = (profile.maxSpiceLevel ?? 0).toDouble();
        _initialized = true;
      }
    });

    return Scaffold(
      appBar: AppBar(title: Text(l10n.dietaryProfile)),
      body: profileState.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (e, _) => Center(child: Text(l10n.somethingWentWrong)),
        data: (_) => ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // ── Allergen Alerts ──
            Text(
              l10n.allergenAlerts,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              l10n.allergenWarningMessage,
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: List.generate(_allergenNames.length, (i) {
                final selected = _selectedAllergens.contains(i);
                return FilterChip(
                  label: Text(_allergenNames[i]),
                  selected: selected,
                  onSelected: (v) {
                    setState(() {
                      if (v) {
                        _selectedAllergens.add(i);
                      } else {
                        _selectedAllergens.remove(i);
                      }
                    });
                  },
                  selectedColor: AppColors.warning.withValues(alpha: 0.2),
                  checkmarkColor: AppColors.warning,
                );
              }),
            ),

            const Divider(height: 32),

            // ── Dietary Preferences ──
            Text(
              l10n.dietaryPreferences,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: List.generate(_dietaryTagNames.length, (i) {
                final selected = _selectedDietaryTags.contains(i);
                return FilterChip(
                  label: Text(_dietaryTagNames[i]),
                  selected: selected,
                  onSelected: (v) {
                    setState(() {
                      if (v) {
                        _selectedDietaryTags.add(i);
                      } else {
                        _selectedDietaryTags.remove(i);
                      }
                    });
                  },
                  selectedColor: AppColors.primaryLight,
                  checkmarkColor: AppColors.primary,
                );
              }),
            ),

            const Divider(height: 32),

            // ── Spice Level ──
            Text(
              l10n.maxSpiceLevel,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Expanded(
                  child: Slider(
                    value: _spiceLevel,
                    min: 0,
                    max: 4,
                    divisions: 4,
                    label: _spiceLevelNames[_spiceLevel.toInt()],
                    activeColor: _spiceColor(_spiceLevel.toInt()),
                    onChanged: (v) => setState(() => _spiceLevel = v),
                  ),
                ),
                SizedBox(
                  width: 80,
                  child: Text(
                    _spiceLevelNames[_spiceLevel.toInt()],
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                      color: _spiceColor(_spiceLevel.toInt()),
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 32),

            // ── Save Button ──
            FilledButton(
              onPressed: _isSaving ? null : _save,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                minimumSize: const Size.fromHeight(48),
              ),
              child: _isSaving
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : Text(
                      l10n.saveDietaryProfile,
                      style: const TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }

  Color _spiceColor(int level) => switch (level) {
        0 => AppColors.textTertiaryLight,
        1 => AppColors.success,
        2 => AppColors.warning,
        3 => AppColors.error,
        4 => const Color(0xFF8B0000),
        _ => AppColors.textTertiaryLight,
      };

  Future<void> _save() async {
    setState(() => _isSaving = true);
    final success =
        await ref.read(dietaryProfileNotifierProvider.notifier).save(
              allergenAlerts: _selectedAllergens.toList()..sort(),
              dietaryPreferences: _selectedDietaryTags.toList()..sort(),
              maxSpiceLevel: _spiceLevel.toInt(),
            );
    if (mounted) {
      setState(() => _isSaving = false);
      final l10n = AppLocalizations.of(context)!;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(success
              ? l10n.saveDietaryProfile
              : l10n.somethingWentWrong),
          backgroundColor: success ? AppColors.success : AppColors.error,
        ),
      );
      if (success) Navigator.of(context).pop();
    }
  }
}
