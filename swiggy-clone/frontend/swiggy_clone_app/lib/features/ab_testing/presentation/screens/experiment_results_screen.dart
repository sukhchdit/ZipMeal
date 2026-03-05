import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/experiment_results_notifier.dart';
import '../providers/experiment_results_state.dart';
import '../widgets/variant_stats_card.dart';

class ExperimentResultsScreen extends ConsumerStatefulWidget {
  const ExperimentResultsScreen({super.key, required this.experimentId});

  final String experimentId;

  @override
  ConsumerState<ExperimentResultsScreen> createState() =>
      _ExperimentResultsScreenState();
}

class _ExperimentResultsScreenState
    extends ConsumerState<ExperimentResultsScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(
              experimentResultsNotifierProvider(widget.experimentId).notifier)
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state =
        ref.watch(experimentResultsNotifierProvider(widget.experimentId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Experiment Results'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => ref
                .read(experimentResultsNotifierProvider(widget.experimentId)
                    .notifier)
                .refresh(),
          ),
        ],
      ),
      body: switch (state) {
        ExperimentResultsInitial() || ExperimentResultsLoading() =>
          const AppLoadingWidget(),
        ExperimentResultsError(:final message) => AppErrorWidget(
            message: message,
            onRetry: () => ref
                .read(experimentResultsNotifierProvider(widget.experimentId)
                    .notifier)
                .load(),
          ),
        ExperimentResultsLoaded(:final stats) => RefreshIndicator(
            onRefresh: () => ref
                .read(experimentResultsNotifierProvider(widget.experimentId)
                    .notifier)
                .refresh(),
            child: ListView(
              padding: const EdgeInsets.only(top: 8, bottom: 32),
              children: [
                Padding(
                  padding:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                  child: Text(
                    'Computed at: ${_formatDateTime(stats.computedAt)}',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.outline,
                    ),
                  ),
                ),
                if (stats.variants.isEmpty)
                  Padding(
                    padding: const EdgeInsets.all(32),
                    child: Center(
                      child: Column(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Icon(Icons.bar_chart,
                              size: 64, color: theme.colorScheme.outline),
                          const SizedBox(height: 16),
                          Text(
                            'No statistics available yet',
                            style: theme.textTheme.bodyLarge,
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'Statistics are computed periodically',
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: theme.colorScheme.outline,
                            ),
                          ),
                        ],
                      ),
                    ),
                  )
                else
                  ...stats.variants.map(
                    (variant) => VariantStatsCard(stats: variant),
                  ),
              ],
            ),
          ),
      },
    );
  }

  String _formatDateTime(String isoDate) {
    final dt = DateTime.tryParse(isoDate);
    if (dt == null) return isoDate;
    final local = dt.toLocal();
    return '${local.day}/${local.month}/${local.year} ${local.hour.toString().padLeft(2, '0')}:${local.minute.toString().padLeft(2, '0')}';
  }
}
