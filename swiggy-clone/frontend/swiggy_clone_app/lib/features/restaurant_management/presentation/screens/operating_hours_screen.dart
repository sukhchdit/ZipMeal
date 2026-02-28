import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/operating_hours_model.dart';
import '../providers/operating_hours_notifier.dart';
import '../providers/operating_hours_state.dart';
import '../widgets/operating_hour_row.dart';

/// Displays and allows editing of a restaurant's weekly operating hours.
///
/// Shows a row per day (Sunday–Saturday) with open/close time pickers and
/// a "Closed" toggle. Changes are saved in bulk via the upsert API.
class OperatingHoursScreen extends ConsumerStatefulWidget {
  const OperatingHoursScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  ConsumerState<OperatingHoursScreen> createState() =>
      _OperatingHoursScreenState();
}

class _OperatingHoursScreenState extends ConsumerState<OperatingHoursScreen> {
  static const _dayNames = [
    'Sun',
    'Mon',
    'Tue',
    'Wed',
    'Thu',
    'Fri',
    'Sat',
  ];

  /// Mutable copy of hours for editing. Index = dayOfWeek (0–6).
  final List<_DayHours> _days = List.generate(
    7,
    (i) => _DayHours(dayOfWeek: i),
  );

  bool _initialized = false;
  bool _isSaving = false;

  void _populateFromState(List<OperatingHoursModel> hours) {
    if (_initialized) return;
    for (final h in hours) {
      if (h.dayOfWeek >= 0 && h.dayOfWeek < 7) {
        _days[h.dayOfWeek] = _DayHours(
          dayOfWeek: h.dayOfWeek,
          openTime: _parseTime(h.openTime),
          closeTime: _parseTime(h.closeTime),
          isClosed: h.isClosed,
        );
      }
    }
    _initialized = true;
  }

  TimeOfDay? _parseTime(String? value) {
    if (value == null || value.isEmpty) return null;
    final parts = value.split(':');
    if (parts.length < 2) return null;
    return TimeOfDay(
      hour: int.tryParse(parts[0]) ?? 0,
      minute: int.tryParse(parts[1]) ?? 0,
    );
  }

  String _formatTime(TimeOfDay? time) {
    if (time == null) return '';
    return '${time.hour.toString().padLeft(2, '0')}:${time.minute.toString().padLeft(2, '0')}';
  }

  Future<void> _save() async {
    setState(() => _isSaving = true);

    final hoursList = _days.map((d) => <String, dynamic>{
          'dayOfWeek': d.dayOfWeek,
          'openTime': d.isClosed ? null : _formatTime(d.openTime),
          'closeTime': d.isClosed ? null : _formatTime(d.closeTime),
          'isClosed': d.isClosed,
        }).toList();

    final success = await ref
        .read(operatingHoursNotifierProvider(widget.restaurantId).notifier)
        .upsertHours({'hours': hoursList});

    if (!mounted) return;
    setState(() => _isSaving = false);

    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Operating hours saved!'),
          backgroundColor: AppColors.success,
        ),
      );
      context.pop();
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to save hours. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final state =
        ref.watch(operatingHoursNotifierProvider(widget.restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Operating Hours'),
      ),
      body: switch (state) {
        OperatingHoursInitial() || OperatingHoursLoading() =>
          const AppLoadingWidget(message: 'Loading hours...'),
        OperatingHoursError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(operatingHoursNotifierProvider(widget.restaurantId)
                    .notifier)
                .loadHours(),
          ),
        OperatingHoursLoaded(:final hours) => _buildForm(theme, hours),
      },
    );
  }

  Widget _buildForm(ThemeData theme, List<OperatingHoursModel> hours) {
    _populateFromState(hours);

    return Column(
      children: [
        Expanded(
          child: ListView.separated(
            padding: const EdgeInsets.all(16),
            itemCount: 7,
            separatorBuilder: (_, __) => const Divider(height: 1),
            itemBuilder: (context, index) {
              final day = _days[index];
              return OperatingHourRow(
                dayName: _dayNames[index],
                openTime: day.openTime,
                closeTime: day.closeTime,
                isClosed: day.isClosed,
                onClosedChanged: (v) =>
                    setState(() => _days[index] = day.copyWith(isClosed: v)),
                onOpenTimeChanged: (t) =>
                    setState(() => _days[index] = day.copyWith(openTime: t)),
                onCloseTimeChanged: (t) =>
                    setState(() => _days[index] = day.copyWith(closeTime: t)),
              );
            },
          ),
        ),

        // Save button
        SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: FilledButton(
              onPressed: _isSaving ? null : _save,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                minimumSize: const Size.fromHeight(52),
              ),
              child: _isSaving
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text('Save Hours'),
            ),
          ),
        ),
      ],
    );
  }
}

/// Simple data class holding mutable hour values per day for the form.
class _DayHours {
  _DayHours({
    required this.dayOfWeek,
    this.openTime,
    this.closeTime,
    this.isClosed = false,
  });

  final int dayOfWeek;
  final TimeOfDay? openTime;
  final TimeOfDay? closeTime;
  final bool isClosed;

  _DayHours copyWith({
    TimeOfDay? openTime,
    TimeOfDay? closeTime,
    bool? isClosed,
  }) =>
      _DayHours(
        dayOfWeek: dayOfWeek,
        openTime: openTime ?? this.openTime,
        closeTime: closeTime ?? this.closeTime,
        isClosed: isClosed ?? this.isClosed,
      );
}
