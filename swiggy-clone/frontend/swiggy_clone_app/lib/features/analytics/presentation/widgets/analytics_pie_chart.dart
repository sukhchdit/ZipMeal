import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/named_value_model.dart';

class AnalyticsPieChart extends StatelessWidget {
  const AnalyticsPieChart({
    super.key,
    required this.title,
    required this.items,
    this.colors,
  });

  final String title;
  final List<NamedValueModel> items;
  final List<Color>? colors;

  static const _defaultColors = [
    AppColors.primary,
    AppColors.success,
    AppColors.info,
    AppColors.warning,
    AppColors.error,
    Colors.deepPurple,
    Colors.teal,
    Colors.pink,
    Colors.indigo,
    Colors.brown,
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (items.isEmpty) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: theme.textTheme.titleSmall?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            height: 80,
            child: Center(
              child: Text(
                'No data available',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textTertiaryLight,
                ),
              ),
            ),
          ),
        ],
      );
    }

    final colorList = colors ?? _defaultColors;
    final total = items.fold<double>(0, (sum, i) => sum + i.value);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            SizedBox(
              width: 140,
              height: 140,
              child: PieChart(
                PieChartData(
                  sectionsSpace: 2,
                  centerSpaceRadius: 30,
                  sections: items.asMap().entries.map((e) {
                    final color = colorList[e.key % colorList.length];
                    final pct =
                        total > 0 ? (e.value.value / total * 100) : 0.0;
                    return PieChartSectionData(
                      value: e.value.value,
                      color: color,
                      radius: 35,
                      title: pct >= 5 ? '${pct.toStringAsFixed(0)}%' : '',
                      titleStyle: const TextStyle(
                        fontSize: 10,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    );
                  }).toList(),
                ),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: items.asMap().entries.map((e) {
                  final color = colorList[e.key % colorList.length];
                  return Padding(
                    padding: const EdgeInsets.only(bottom: 4),
                    child: Row(
                      children: [
                        Container(
                          width: 10,
                          height: 10,
                          decoration: BoxDecoration(
                            color: color,
                            shape: BoxShape.circle,
                          ),
                        ),
                        const SizedBox(width: 6),
                        Expanded(
                          child: Text(
                            e.value.name,
                            style: theme.textTheme.labelSmall,
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        Text(
                          e.value.value.toStringAsFixed(0),
                          style: theme.textTheme.labelSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                  );
                }).toList(),
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
      ],
    );
  }
}
