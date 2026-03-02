import 'package:flutter/material.dart';

import '../../data/models/funnel_stage_model.dart';

class FunnelChart extends StatelessWidget {
  const FunnelChart({
    super.key,
    required this.title,
    required this.stages,
  });

  final String title;
  final List<FunnelStageModel> stages;

  static const _colors = [
    Color(0xFF4CAF50),
    Color(0xFF66BB6A),
    Color(0xFF8BC34A),
    Color(0xFFCDDC39),
    Color(0xFFFFC107),
    Color(0xFFFF9800),
    Color(0xFFFF7043),
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (stages.isEmpty) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title,
              style: theme.textTheme.titleSmall
                  ?.copyWith(fontWeight: FontWeight.w600)),
          const SizedBox(height: 12),
          const SizedBox(
            height: 120,
            child: Center(child: Text('No data available')),
          ),
        ],
      );
    }

    final maxCount =
        stages.map((s) => s.count).reduce((a, b) => a > b ? a : b);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title,
            style: theme.textTheme.titleSmall
                ?.copyWith(fontWeight: FontWeight.w600)),
        const SizedBox(height: 12),
        ...stages.asMap().entries.map((entry) {
          final index = entry.key;
          final stage = entry.value;
          final fraction = maxCount > 0 ? stage.count / maxCount : 0.0;
          final color = _colors[index % _colors.length];

          return Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Row(
              children: [
                SizedBox(
                  width: 120,
                  child: Text(
                    stage.stage,
                    style: theme.textTheme.bodySmall,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: LayoutBuilder(
                    builder: (context, constraints) {
                      return Container(
                        height: 28,
                        width: constraints.maxWidth * fraction,
                        decoration: BoxDecoration(
                          color: color,
                          borderRadius: BorderRadius.circular(4),
                        ),
                        alignment: Alignment.centerLeft,
                        padding: const EdgeInsets.symmetric(horizontal: 8),
                        child: Text(
                          _formatCount(stage.count),
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.w600,
                          ),
                          overflow: TextOverflow.ellipsis,
                        ),
                      );
                    },
                  ),
                ),
                const SizedBox(width: 8),
                Container(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                  decoration: BoxDecoration(
                    color: color.withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    '${stage.conversionRate.toStringAsFixed(1)}%',
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: color,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          );
        }),
        const SizedBox(height: 16),
      ],
    );
  }

  static String _formatCount(int value) {
    if (value >= 10000000) return '${(value / 10000000).toStringAsFixed(1)}Cr';
    if (value >= 100000) return '${(value / 100000).toStringAsFixed(1)}L';
    if (value >= 1000) return '${(value / 1000).toStringAsFixed(1)}K';
    return value.toString();
  }
}
