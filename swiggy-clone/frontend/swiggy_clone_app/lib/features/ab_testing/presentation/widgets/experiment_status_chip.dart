import 'package:flutter/material.dart';

class ExperimentStatusChip extends StatelessWidget {
  const ExperimentStatusChip({super.key, required this.status});

  final int status;

  @override
  Widget build(BuildContext context) {
    final (label, color) = _statusInfo(status);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        label,
        style: Theme.of(context).textTheme.labelSmall?.copyWith(
              color: color,
              fontWeight: FontWeight.w600,
            ),
      ),
    );
  }

  static (String, Color) _statusInfo(int status) => switch (status) {
        0 => ('Draft', Colors.blueGrey),
        1 => ('Active', Colors.green),
        2 => ('Paused', Colors.orange),
        3 => ('Completed', Colors.blue),
        4 => ('Archived', Colors.grey),
        _ => ('Unknown', Colors.grey),
      };

  static String statusLabel(int status) => _statusInfo(status).$1;
}
