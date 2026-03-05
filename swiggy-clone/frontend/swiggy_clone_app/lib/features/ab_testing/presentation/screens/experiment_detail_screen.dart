import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/experiment_model.dart';
import '../providers/experiment_detail_notifier.dart';
import '../providers/experiment_detail_state.dart';
import '../widgets/experiment_status_chip.dart';

class ExperimentDetailScreen extends ConsumerStatefulWidget {
  const ExperimentDetailScreen({super.key, required this.experimentId});

  final String experimentId;

  @override
  ConsumerState<ExperimentDetailScreen> createState() =>
      _ExperimentDetailScreenState();
}

class _ExperimentDetailScreenState
    extends ConsumerState<ExperimentDetailScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(experimentDetailNotifierProvider(widget.experimentId).notifier)
          .load(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state =
        ref.watch(experimentDetailNotifierProvider(widget.experimentId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Experiment Detail')),
      body: switch (state) {
        ExperimentDetailInitial() || ExperimentDetailLoading() =>
          const AppLoadingWidget(),
        ExperimentDetailError(:final message) => AppErrorWidget(
            message: message,
            onRetry: () => ref
                .read(experimentDetailNotifierProvider(widget.experimentId)
                    .notifier)
                .load(),
          ),
        ExperimentDetailLoaded(
          :final experiment,
          :final isPerformingAction,
        ) =>
          RefreshIndicator(
            onRefresh: () => ref
                .read(experimentDetailNotifierProvider(widget.experimentId)
                    .notifier)
                .load(),
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  _ExperimentInfoCard(experiment: experiment),
                  const SizedBox(height: 16),
                  _ActionButtons(
                    status: experiment.status,
                    isPerformingAction: isPerformingAction,
                    onActivate: () => ref
                        .read(experimentDetailNotifierProvider(
                                widget.experimentId)
                            .notifier)
                        .activate(),
                    onPause: () => ref
                        .read(experimentDetailNotifierProvider(
                                widget.experimentId)
                            .notifier)
                        .pause(),
                    onComplete: () => ref
                        .read(experimentDetailNotifierProvider(
                                widget.experimentId)
                            .notifier)
                        .complete(),
                  ),
                  const SizedBox(height: 16),
                  FilledButton.tonalIcon(
                    onPressed: () => context.push(
                      RouteNames.adminExperimentResultsPath(
                          widget.experimentId),
                    ),
                    icon: const Icon(Icons.bar_chart),
                    label: const Text('View Results'),
                  ),
                  const SizedBox(height: 24),
                  Text(
                    'Variants',
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 8),
                  ...experiment.variants.map(
                    (v) => _VariantTile(variant: v),
                  ),
                  const SizedBox(height: 24),
                ],
              ),
            ),
          ),
      },
    );
  }
}

class _ExperimentInfoCard extends StatelessWidget {
  const _ExperimentInfoCard({required this.experiment});

  final ExperimentModel experiment;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    experiment.name,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                ExperimentStatusChip(status: experiment.status),
              ],
            ),
            const SizedBox(height: 4),
            Text(
              experiment.key,
              style: theme.textTheme.bodySmall?.copyWith(
                fontFamily: 'monospace',
                color: theme.colorScheme.outline,
              ),
            ),
            if (experiment.description != null) ...[
              const SizedBox(height: 12),
              Text(experiment.description!, style: theme.textTheme.bodyMedium),
            ],
            if (experiment.targetAudience != null) ...[
              const SizedBox(height: 8),
              _InfoRow(label: 'Target Audience', value: experiment.targetAudience!),
            ],
            if (experiment.goalDescription != null) ...[
              const SizedBox(height: 8),
              _InfoRow(label: 'Goal', value: experiment.goalDescription!),
            ],
            if (experiment.startDate != null || experiment.endDate != null) ...[
              const SizedBox(height: 8),
              _InfoRow(
                label: 'Period',
                value:
                    '${_fmtDate(experiment.startDate)} - ${_fmtDate(experiment.endDate)}',
              ),
            ],
          ],
        ),
      ),
    );
  }

  String _fmtDate(String? isoDate) {
    if (isoDate == null) return 'N/A';
    final dt = DateTime.tryParse(isoDate);
    if (dt == null) return isoDate;
    return '${dt.day}/${dt.month}/${dt.year}';
  }
}

class _InfoRow extends StatelessWidget {
  const _InfoRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          width: 120,
          child: Text(
            label,
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.outline,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        Expanded(child: Text(value, style: theme.textTheme.bodySmall)),
      ],
    );
  }
}

class _ActionButtons extends StatelessWidget {
  const _ActionButtons({
    required this.status,
    required this.isPerformingAction,
    required this.onActivate,
    required this.onPause,
    required this.onComplete,
  });

  final int status;
  final bool isPerformingAction;
  final VoidCallback onActivate;
  final VoidCallback onPause;
  final VoidCallback onComplete;

  @override
  Widget build(BuildContext context) {
    if (isPerformingAction) {
      return const Center(child: CircularProgressIndicator());
    }

    return Row(
      children: [
        // Draft(0) or Paused(2) → can Activate
        if (status == 0 || status == 2)
          Expanded(
            child: FilledButton.icon(
              onPressed: onActivate,
              icon: const Icon(Icons.play_arrow),
              label: const Text('Activate'),
              style: FilledButton.styleFrom(backgroundColor: Colors.green),
            ),
          ),
        if ((status == 0 || status == 2) && status == 1)
          const SizedBox(width: 8),
        // Active(1) → can Pause
        if (status == 1) ...[
          Expanded(
            child: FilledButton.icon(
              onPressed: onPause,
              icon: const Icon(Icons.pause),
              label: const Text('Pause'),
              style: FilledButton.styleFrom(backgroundColor: Colors.orange),
            ),
          ),
          const SizedBox(width: 8),
        ],
        // Active(1) or Paused(2) → can Complete
        if (status == 1 || status == 2)
          Expanded(
            child: FilledButton.tonalIcon(
              onPressed: onComplete,
              icon: const Icon(Icons.check),
              label: const Text('Complete'),
            ),
          ),
      ],
    );
  }
}

class _VariantTile extends StatelessWidget {
  const _VariantTile({required this.variant});

  final ExperimentVariantModel variant;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(
                        variant.name,
                        style: theme.textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      if (variant.isControl) ...[
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
                    variant.key,
                    style: theme.textTheme.bodySmall?.copyWith(
                      fontFamily: 'monospace',
                      color: theme.colorScheme.outline,
                    ),
                  ),
                ],
              ),
            ),
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
              decoration: BoxDecoration(
                color: theme.colorScheme.primaryContainer,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Text(
                '${variant.allocationPercent}%',
                style: theme.textTheme.labelLarge?.copyWith(
                  color: theme.colorScheme.onPrimaryContainer,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
