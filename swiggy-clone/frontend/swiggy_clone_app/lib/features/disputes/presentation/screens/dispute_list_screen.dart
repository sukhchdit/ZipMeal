import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/disputes_notifier.dart';
import '../providers/disputes_state.dart';
import '../widgets/dispute_card.dart';

class DisputeListScreen extends ConsumerStatefulWidget {
  const DisputeListScreen({super.key});

  @override
  ConsumerState<DisputeListScreen> createState() => _DisputeListScreenState();
}

class _DisputeListScreenState extends ConsumerState<DisputeListScreen> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
        () => ref.read(disputesNotifierProvider.notifier).loadDisputes());
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(disputesNotifierProvider);
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.disputes)),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () async {
          final result = await context.push(RouteNames.createDispute);
          if (result == true) {
            ref.read(disputesNotifierProvider.notifier).loadDisputes();
          }
        },
        icon: const Icon(Icons.report_problem_outlined),
        label: Text(l10n.reportIssue),
      ),
      body: switch (state) {
        DisputesInitial() || DisputesLoading() => const AppLoadingWidget(),
        DisputesError(:final message) => AppErrorWidget(
            message: message,
            onRetry: () =>
                ref.read(disputesNotifierProvider.notifier).loadDisputes(),
          ),
        DisputesLoaded(:final disputes, :final hasMore, :final isLoadingMore) =>
          disputes.isEmpty
              ? Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.gavel_outlined,
                          size: 64,
                          color: Theme.of(context)
                              .colorScheme
                              .onSurfaceVariant),
                      const SizedBox(height: 16),
                      Text(l10n.noDisputesYet,
                          style: Theme.of(context).textTheme.titleMedium),
                      const SizedBox(height: 8),
                      Text(l10n.noDisputesDescription,
                          style: Theme.of(context).textTheme.bodyMedium),
                      const SizedBox(height: 24),
                      FilledButton.icon(
                        onPressed: () =>
                            context.push(RouteNames.createDispute),
                        icon: const Icon(Icons.report_problem_outlined),
                        label: Text(l10n.reportIssue),
                      ),
                    ],
                  ),
                )
              : RefreshIndicator(
                  onRefresh: () => ref
                      .read(disputesNotifierProvider.notifier)
                      .loadDisputes(),
                  child: NotificationListener<ScrollNotification>(
                    onNotification: (notification) {
                      if (notification is ScrollEndNotification &&
                          notification.metrics.pixels >=
                              notification.metrics.maxScrollExtent - 200 &&
                          hasMore &&
                          !isLoadingMore) {
                        ref
                            .read(disputesNotifierProvider.notifier)
                            .loadMore();
                      }
                      return false;
                    },
                    child: ListView.builder(
                      padding: const EdgeInsets.only(top: 8, bottom: 88),
                      itemCount: disputes.length + (isLoadingMore ? 1 : 0),
                      itemBuilder: (context, index) {
                        if (index == disputes.length) {
                          return const Padding(
                            padding: EdgeInsets.all(16),
                            child:
                                Center(child: CircularProgressIndicator()),
                          );
                        }
                        final dispute = disputes[index];
                        return DisputeCard(
                          dispute: dispute,
                          onTap: () => context.push(
                              RouteNames.disputeDetailPath(dispute.id)),
                        );
                      },
                    ),
                  ),
                ),
      },
    );
  }
}
