import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/data_point_model.dart';

class AnalyticsLineChart extends StatelessWidget {
  const AnalyticsLineChart({
    super.key,
    required this.title,
    required this.dataPoints,
    this.lineColor = AppColors.primary,
    this.formatValue,
  });

  final String title;
  final List<DataPointModel> dataPoints;
  final Color lineColor;
  final String Function(double)? formatValue;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (dataPoints.isEmpty) {
      return _EmptyChart(title: title);
    }

    final spots = dataPoints
        .asMap()
        .entries
        .map((e) => FlSpot(e.key.toDouble(), e.value.value))
        .toList();

    final maxY = dataPoints.map((d) => d.value).reduce(
          (a, b) => a > b ? a : b,
        );

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
          height: 200,
          child: LineChart(
            LineChartData(
              gridData: FlGridData(
                show: true,
                drawVerticalLine: false,
                horizontalInterval: maxY > 0 ? maxY / 4 : 1,
                getDrawingHorizontalLine: (value) => FlLine(
                  color: AppColors.borderLight,
                  strokeWidth: 0.5,
                ),
              ),
              titlesData: FlTitlesData(
                leftTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    reservedSize: 48,
                    getTitlesWidget: (value, meta) {
                      final text = formatValue?.call(value) ??
                          _compactNumber(value);
                      return SideTitleWidget(
                        meta: meta,
                        child: Text(
                          text,
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: AppColors.textTertiaryLight,
                          ),
                        ),
                      );
                    },
                  ),
                ),
                bottomTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    reservedSize: 28,
                    interval: _bottomInterval,
                    getTitlesWidget: (value, meta) {
                      final idx = value.toInt();
                      if (idx < 0 || idx >= dataPoints.length) {
                        return const SizedBox.shrink();
                      }
                      final label = dataPoints[idx].label;
                      // Show shortened label
                      final short = label.length > 5
                          ? label.substring(label.length - 5)
                          : label;
                      return SideTitleWidget(
                        meta: meta,
                        child: Text(
                          short,
                          style: theme.textTheme.labelSmall?.copyWith(
                            color: AppColors.textTertiaryLight,
                            fontSize: 9,
                          ),
                        ),
                      );
                    },
                  ),
                ),
                topTitles: const AxisTitles(
                  sideTitles: SideTitles(showTitles: false),
                ),
                rightTitles: const AxisTitles(
                  sideTitles: SideTitles(showTitles: false),
                ),
              ),
              borderData: FlBorderData(show: false),
              lineBarsData: [
                LineChartBarData(
                  spots: spots,
                  isCurved: true,
                  preventCurveOverShooting: true,
                  color: lineColor,
                  barWidth: 2.5,
                  dotData: FlDotData(
                    show: dataPoints.length <= 14,
                    getDotPainter: (_, __, ___, ____) => FlDotCirclePainter(
                      radius: 3,
                      color: lineColor,
                      strokeWidth: 0,
                    ),
                  ),
                  belowBarData: BarAreaData(
                    show: true,
                    color: lineColor.withValues(alpha: 0.1),
                  ),
                ),
              ],
              lineTouchData: LineTouchData(
                touchTooltipData: LineTouchTooltipData(
                  getTooltipItems: (spots) => spots.map((spot) {
                    final idx = spot.x.toInt();
                    final label =
                        idx < dataPoints.length ? dataPoints[idx].label : '';
                    final val =
                        formatValue?.call(spot.y) ?? _compactNumber(spot.y);
                    return LineTooltipItem(
                      '$label\n$val',
                      TextStyle(
                        color: Colors.white,
                        fontSize: 12,
                        fontWeight: FontWeight.w600,
                      ),
                    );
                  }).toList(),
                ),
              ),
            ),
          ),
        ),
        const SizedBox(height: 16),
      ],
    );
  }

  double get _bottomInterval {
    if (dataPoints.length <= 7) return 1;
    if (dataPoints.length <= 14) return 2;
    if (dataPoints.length <= 31) return 5;
    return (dataPoints.length / 6).ceilToDouble();
  }

  static String _compactNumber(double value) {
    if (value >= 10000000) return '${(value / 10000000).toStringAsFixed(1)}Cr';
    if (value >= 100000) return '${(value / 100000).toStringAsFixed(1)}L';
    if (value >= 1000) return '${(value / 1000).toStringAsFixed(1)}K';
    return value.toStringAsFixed(0);
  }
}

class _EmptyChart extends StatelessWidget {
  const _EmptyChart({required this.title});

  final String title;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.w600,
              ),
        ),
        const SizedBox(height: 12),
        SizedBox(
          height: 120,
          child: Center(
            child: Text(
              'No data available',
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
            ),
          ),
        ),
      ],
    );
  }
}
