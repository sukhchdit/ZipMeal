import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/experiments_list_notifier.dart';
import '../providers/experiments_list_state.dart';
import '../widgets/experiment_card.dart';

class ExperimentsListScreen extends ConsumerStatefulWidget {
  const ExperimentsListScreen({super.key});

  @override
  ConsumerState<ExperimentsListScreen> createState() =>
      _ExperimentsListScreenState();
}

class _ExperimentsListScreenState extends ConsumerState<ExperimentsListScreen> {
  int? _selectedStatus;

  static const _statusFilters = <(int?, String)>[
    (null, 'All'),
    (0, 'Draft'),
    (1, 'Active'),
    (2, 'Paused'),
    (3, 'Completed'),
  ];

  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref
          .read(experimentsListNotifierProvider.notifier)
          .loadExperiments(),
    );
  }

  void _onFilterChanged(int? status) {
    setState(() => _selectedStatus = status);
    ref
        .read(experimentsListNotifierProvider.notifier)
        .loadExperiments(statusFilter: status);
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(experimentsListNotifierProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('A/B Experiments')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => context.push(RouteNames.adminCreateExperiment),
        icon: const Icon(Icons.add),
        label: const Text('New Experiment'),
      ),
      body: Column(
        children: [
          SizedBox(
            height: 48,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              itemCount: _statusFilters.length,
              separatorBuilder: (_, __) => const SizedBox(width: 8),
              itemBuilder: (context, index) {
                final (statusValue, label) = _statusFilters[index];
                final isSelected = _selectedStatus == statusValue;
                return FilterChip(
                  label: Text(label),
                  selected: isSelected,
                  onSelected: (_) => _onFilterChanged(statusValue),
                );
              },
            ),
          ),
          Expanded(
            child: switch (state) {
              ExperimentsListInitial() || ExperimentsListLoading() =>
                const AppLoadingWidget(),
              ExperimentsListError(:final message) => AppErrorWidget(
                  message: message,
                  onRetry: () => ref
                      .read(experimentsListNotifierProvider.notifier)
                      .loadExperiments(statusFilter: _selectedStatus),
                ),
              ExperimentsListLoaded(
                :final experiments,
                :final isLoadingMore
              ) =>
                experiments.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.science_outlined,
                                size: 64,
                                color: Theme.of(context).colorScheme.outline),
                            const SizedBox(height: 16),
                            Text(
                              'No experiments found',
                              style: Theme.of(context).textTheme.bodyLarge,
                            ),
                            const SizedBox(height: 8),
                            FilledButton.icon(
                              onPressed: () => context
                                  .push(RouteNames.adminCreateExperiment),
                              icon: const Icon(Icons.add),
                              label: const Text('Create one'),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: () => ref
                            .read(experimentsListNotifierProvider.notifier)
                            .loadExperiments(statusFilter: _selectedStatus),
                        child: NotificationListener<ScrollNotification>(
                          onNotification: (notification) {
                            if (notification is ScrollEndNotification &&
                                notification.metrics.extentAfter < 200) {
                              ref
                                  .read(
                                      experimentsListNotifierProvider.notifier)
                                  .loadMore();
                            }
                            return false;
                          },
                          child: ListView.builder(
                            padding: const EdgeInsets.only(bottom: 80),
                            itemCount:
                                experiments.length + (isLoadingMore ? 1 : 0),
                            itemBuilder: (context, index) {
                              if (index == experiments.length) {
                                return const Padding(
                                  padding: EdgeInsets.all(16),
                                  child:
                                      Center(child: CircularProgressIndicator()),
                                );
                              }
                              final experiment = experiments[index];
                              return ExperimentCard(
                                experiment: experiment,
                                onTap: () => context.push(
                                  RouteNames.adminExperimentDetailPath(
                                      experiment.id),
                                ),
                              );
                            },
                          ),
                        ),
                      ),
            },
          ),
        ],
      ),
    );
  }
}
