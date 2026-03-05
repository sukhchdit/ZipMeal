import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/data_point_model.dart';
import '../../data/models/forecast_point_model.dart';

class ForecastLineChart extends StatelessWidget {
  const ForecastLineChart({
    super.key,
    required this.title,
    required this.historicalData,
    required this.forecastData,
  });

  final String title;
  final List<DataPointModel> historicalData;
  final List<ForecastPointModel> forecastData;

  static const _historicalColor = Color(0xFF4CAF50);
  static const _forecastColor = Color(0xFF2196F3);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (historicalData.isEmpty && forecastData.isEmpty) {
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

    final histLen = historicalData.length;
    final totalLen = histLen + forecastData.length;

    // Build spots for historical line
    final historicalSpots = historicalData
        .asMap()
        .entries
        .map((e) => FlSpot(e.key.toDouble(), e.value.value))
        .toList();

    // Build spots for forecast line
    final forecastSpots = forecastData
        .asMap()
        .entries
        .map((e) => FlSpot(
            (histLen + e.key).toDouble(), e.value.predictedValue))
        .toList();

    // Connect historical end to forecast start
    if (historicalSpots.isNotEmpty && forecastSpots.isNotEmpty) {
      forecastSpots.insert(0, historicalSpots.last);
    }

    // Confidence band spots
    final lowerSpots = forecastData
        .asMap()
        .entries
        .map(
            (e) => FlSpot((histLen + e.key).toDouble(), e.value.lowerBound))
        .toList();
    final upperSpots = forecastData
        .asMap()
        .entries
        .map(
            (e) => FlSpot((histLen + e.key).toDouble(), e.value.upperBound))
        .toList();

    if (historicalSpots.isNotEmpty && lowerSpots.isNotEmpty) {
      lowerSpots.insert(0, historicalSpots.last);
      upperSpots.insert(
          0,
          FlSpot(
              historicalSpots.last.x, historicalSpots.last.y));
    }

    // Calculate max Y
    var maxY = 0.0;
    for (final s in historicalSpots) {
      if (s.y > maxY) maxY = s.y;
    }
    for (final f in forecastData) {
      if (f.upperBound > maxY) maxY = f.upperBound;
    }
    if (maxY == 0) maxY = 1;

    // Build all labels for bottom axis
    final allLabels = <String>[
      ...historicalData.map((d) => d.label),
      ...forecastData.map((f) => f.label),
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(title,
            style: theme.textTheme.titleSmall
                ?.copyWith(fontWeight: FontWeight.w600)),
        const SizedBox(height: 12),
        SizedBox(
          height: 220,
          child: LineChart(
            LineChartData(
              gridData: FlGridData(
                show: true,
                drawVerticalLine: false,
                horizontalInterval: maxY / 4,
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
                    getTitlesWidget: (value, meta) => SideTitleWidget(
                      axisSide: meta.axisSide,
                      child: Text(
                        _compactNumber(value),
                        style: theme.textTheme.labelSmall?.copyWith(
                          color: AppColors.textTertiaryLight,
                        ),
                      ),
                    ),
                  ),
                ),
                bottomTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    reservedSize: 28,
                    interval: _bottomInterval(totalLen),
                    getTitlesWidget: (value, meta) {
                      final idx = value.toInt();
                      if (idx < 0 || idx >= allLabels.length) {
                        return const SizedBox.shrink();
                      }
                      final label = allLabels[idx];
                      final short = label.length > 5
                          ? label.substring(label.length - 5)
                          : label;
                      return SideTitleWidget(
                        axisSide: meta.axisSide,
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
                topTitles:
                    const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                rightTitles:
                    const AxisTitles(sideTitles: SideTitles(showTitles: false)),
              ),
              borderData: FlBorderData(show: false),
              extraLinesData: ExtraLinesData(
                verticalLines: [
                  if (histLen > 0)
                    VerticalLine(
                      x: (histLen - 1).toDouble(),
                      color: AppColors.textTertiaryLight,
                      strokeWidth: 1,
                      dashArray: [6, 4],
                    ),
                ],
              ),
              lineBarsData: [
                // Confidence band upper
                if (upperSpots.isNotEmpty)
                  LineChartBarData(
                    spots: upperSpots,
                    isCurved: true,
                    preventCurveOverShooting: true,
                    color: Colors.transparent,
                    barWidth: 0,
                    dotData: const FlDotData(show: false),
                    belowBarData: BarAreaData(show: false),
                  ),
                // Confidence band lower
                if (lowerSpots.isNotEmpty)
                  LineChartBarData(
                    spots: lowerSpots,
                    isCurved: true,
                    preventCurveOverShooting: true,
                    color: Colors.transparent,
                    barWidth: 0,
                    dotData: const FlDotData(show: false),
                    belowBarData: BarAreaData(show: false),
                  ),
                // Historical line
                if (historicalSpots.isNotEmpty)
                  LineChartBarData(
                    spots: historicalSpots,
                    isCurved: true,
                    preventCurveOverShooting: true,
                    color: _historicalColor,
                    barWidth: 2.5,
                    dotData: FlDotData(
                      show: historicalData.length <= 14,
                      getDotPainter: (_, __, ___, ____) => FlDotCirclePainter(
                        radius: 3,
                        color: _historicalColor,
                        strokeWidth: 0,
                      ),
                    ),
                    belowBarData: BarAreaData(
                      show: true,
                      color: _historicalColor.withValues(alpha: 0.1),
                    ),
                  ),
                // Forecast line
                if (forecastSpots.isNotEmpty)
                  LineChartBarData(
                    spots: forecastSpots,
                    isCurved: true,
                    preventCurveOverShooting: true,
                    color: _forecastColor,
                    barWidth: 2.5,
                    dashArray: [8, 4],
                    dotData: const FlDotData(show: false),
                    belowBarData: BarAreaData(
                      show: true,
                      color: _forecastColor.withValues(alpha: 0.08),
                    ),
                  ),
              ],
              betweenBarsData: [
                if (upperSpots.isNotEmpty && lowerSpots.isNotEmpty)
                  BetweenBarsData(
                    fromIndex: 0,
                    toIndex: 1,
                    color: _forecastColor.withValues(alpha: 0.12),
                  ),
              ],
              lineTouchData: LineTouchData(
                touchTooltipData: LineTouchTooltipData(
                  getTooltipItems: (spots) => spots.map((spot) {
                    final idx = spot.x.toInt();
                    final label =
                        idx < allLabels.length ? allLabels[idx] : '';
                    return LineTooltipItem(
                      '$label\n${_compactNumber(spot.y)}',
                      const TextStyle(
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
        const SizedBox(height: 8),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _LegendItem(color: _historicalColor, label: 'Historical'),
            const SizedBox(width: 16),
            _LegendItem(color: _forecastColor, label: 'Forecast'),
          ],
        ),
        const SizedBox(height: 16),
      ],
    );
  }

  static double _bottomInterval(int count) {
    if (count <= 7) return 1;
    if (count <= 14) return 2;
    if (count <= 31) return 5;
    return (count / 6).ceilToDouble();
  }

  static String _compactNumber(double value) {
    if (value >= 10000000) return '${(value / 10000000).toStringAsFixed(1)}Cr';
    if (value >= 100000) return '${(value / 100000).toStringAsFixed(1)}L';
    if (value >= 1000) return '${(value / 1000).toStringAsFixed(1)}K';
    return value.toStringAsFixed(0);
  }
}

class _LegendItem extends StatelessWidget {
  const _LegendItem({required this.color, required this.label});

  final Color color;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 12,
          height: 3,
          decoration: BoxDecoration(
            color: color,
            borderRadius: BorderRadius.circular(2),
          ),
        ),
        const SizedBox(width: 4),
        Text(
          label,
          style: Theme.of(context).textTheme.labelSmall,
        ),
      ],
    );
  }
}
