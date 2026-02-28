import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

/// A row widget with: day name label, time pickers for open/close, and a
/// closed toggle switch.
class OperatingHourRow extends StatelessWidget {
  const OperatingHourRow({
    required this.dayName,
    required this.openTime,
    required this.closeTime,
    required this.isClosed,
    required this.onOpenTimeChanged,
    required this.onCloseTimeChanged,
    required this.onClosedChanged,
    super.key,
  });

  final String dayName;
  final TimeOfDay? openTime;
  final TimeOfDay? closeTime;
  final bool isClosed;
  final ValueChanged<TimeOfDay> onOpenTimeChanged;
  final ValueChanged<TimeOfDay> onCloseTimeChanged;
  final ValueChanged<bool> onClosedChanged;

  String _formatTime(TimeOfDay? time) {
    if (time == null) return '--:--';
    final hour = time.hourOfPeriod == 0 ? 12 : time.hourOfPeriod;
    final minute = time.minute.toString().padLeft(2, '0');
    final period = time.period == DayPeriod.am ? 'AM' : 'PM';
    return '$hour:$minute $period';
  }

  Future<void> _pickTime(
    BuildContext context,
    TimeOfDay? initialTime,
    ValueChanged<TimeOfDay> onChanged,
  ) async {
    final picked = await showTimePicker(
      context: context,
      initialTime: initialTime ?? const TimeOfDay(hour: 9, minute: 0),
    );
    if (picked != null) {
      onChanged(picked);
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Row(
        children: [
          // Day name
          SizedBox(
            width: 48,
            child: Text(
              dayName,
              style: theme.textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          const SizedBox(width: 8),

          // Open time
          Expanded(
            child: _TimeButton(
              label: _formatTime(openTime),
              enabled: !isClosed,
              onTap: isClosed
                  ? null
                  : () => _pickTime(context, openTime, onOpenTimeChanged),
            ),
          ),
          const SizedBox(width: 8),

          Text(
            'to',
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
          const SizedBox(width: 8),

          // Close time
          Expanded(
            child: _TimeButton(
              label: _formatTime(closeTime),
              enabled: !isClosed,
              onTap: isClosed
                  ? null
                  : () => _pickTime(context, closeTime, onCloseTimeChanged),
            ),
          ),
          const SizedBox(width: 8),

          // Closed toggle
          Column(
            children: [
              SizedBox(
                height: 28,
                child: Switch(
                  value: isClosed,
                  onChanged: onClosedChanged,
                  activeColor: AppColors.error,
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
              ),
              Text(
                'Closed',
                style: theme.textTheme.labelSmall?.copyWith(
                  color: isClosed
                      ? AppColors.error
                      : AppColors.textTertiaryLight,
                  fontSize: 10,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}

class _TimeButton extends StatelessWidget {
  const _TimeButton({
    required this.label,
    required this.enabled,
    this.onTap,
  });

  final String label;
  final bool enabled;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
        decoration: BoxDecoration(
          border: Border.all(
            color: enabled
                ? AppColors.borderLight
                : AppColors.shimmerBase,
          ),
          borderRadius: BorderRadius.circular(8),
          color: enabled ? null : AppColors.shimmerBase.withValues(alpha: 0.3),
        ),
        child: Center(
          child: Text(
            label,
            style: theme.textTheme.bodySmall?.copyWith(
              color: enabled
                  ? AppColors.textPrimaryLight
                  : AppColors.textDisabledLight,
            ),
          ),
        ),
      ),
    );
  }
}
