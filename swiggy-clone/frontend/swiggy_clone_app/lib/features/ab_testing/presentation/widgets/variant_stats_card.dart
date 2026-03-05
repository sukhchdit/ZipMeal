import 'package:flutter/material.dart';

import '../../data/models/experiment_stats_model.dart';

class VariantStatsCard extends StatelessWidget {
  const VariantStatsCard({super.key, required this.stats});

  final VariantStatsModel stats;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Text(
                            stats.variantName,
                            style: theme.textTheme.titleSmall?.copyWith(
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          if (stats.isControl) ...[
                            const SizedBox(width: 8),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 6, vertical: 2),
                              decoration: BoxDecoration(
                                color: Colors.blue.withValues(alpha: 0.12),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                'Control',
                                style: theme.textTheme.labelSmall?.copyWith(
                                  color: Colors.blue,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ),
                          ],
                        ],
                      ),
                      const SizedBox(height: 2),
                      Text(
                        stats.variantKey,
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: theme.colorScheme.outline,
                          fontFamily: 'monospace',
                        ),
                      ),
                    ],
                  ),
                ),
                if (stats.isSignificant == true)
                  Icon(Icons.check_circle, color: Colors.green, size: 24)
                else if (stats.isSignificant == false)
                  Icon(Icons.remove_circle_outline,
                      color: Colors.grey, size: 24),
              ],
            ),
            const Divider(height: 24),
            Row(
              children: [
                _MetricTile(
                  label: 'Exposures',
                  value: stats.exposures.toString(),
                ),
                _MetricTile(
                  label: 'Conversions',
                  value: stats.conversions.toString(),
                ),
                _MetricTile(
                  label: 'Conv. Rate',
                  value: '${(stats.conversionRate * 100).toStringAsFixed(2)}%',
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                if (stats.relativeLift != null)
                  _MetricTile(
                    label: 'Lift',
                    value: '${stats.relativeLift! >= 0 ? '+' : ''}${(stats.relativeLift! * 100).toStringAsFixed(1)}%',
                    valueColor:
                        stats.relativeLift! >= 0 ? Colors.green : Colors.red,
                    icon: stats.relativeLift! >= 0
                        ? Icons.arrow_upward
                        : Icons.arrow_downward,
                  ),
                if (stats.zScore != null)
                  _MetricTile(
                    label: 'Z-Score',
                    value: stats.zScore!.toStringAsFixed(3),
                  ),
                if (stats.pValue != null)
                  _MetricTile(
                    label: 'P-Value',
                    value: stats.pValue!.toStringAsFixed(4),
                    valueColor: (stats.pValue! < 0.05)
                        ? Colors.green
                        : theme.colorScheme.onSurface,
                  ),
              ],
            ),
            if (stats.isSignificant != null) ...[
              const SizedBox(height: 8),
              Container(
                width: double.infinity,
                padding:
                    const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                decoration: BoxDecoration(
                  color: stats.isSignificant!
                      ? Colors.green.withValues(alpha: 0.08)
                      : Colors.orange.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Text(
                  stats.isSignificant!
                      ? 'Statistically significant (p < 0.05)'
                      : 'Not yet statistically significant',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: stats.isSignificant! ? Colors.green : Colors.orange,
                    fontWeight: FontWeight.w500,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}

class _MetricTile extends StatelessWidget {
  const _MetricTile({
    required this.label,
    required this.value,
    this.valueColor,
    this.icon,
  });

  final String label;
  final String value;
  final Color? valueColor;
  final IconData? icon;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Expanded(
      child: Column(
        children: [
          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (icon != null) ...[
                Icon(icon, size: 14, color: valueColor),
                const SizedBox(width: 2),
              ],
              Text(
                value,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: valueColor,
                ),
              ),
            ],
          ),
          const SizedBox(height: 2),
          Text(
            label,
            style: theme.textTheme.labelSmall?.copyWith(
              color: theme.colorScheme.outline,
            ),
          ),
        ],
      ),
    );
  }
}
