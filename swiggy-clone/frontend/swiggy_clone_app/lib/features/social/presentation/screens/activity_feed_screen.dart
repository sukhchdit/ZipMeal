import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../providers/activity_feed_notifier.dart';
import '../providers/activity_feed_state.dart';
import '../widgets/activity_feed_card.dart';

class ActivityFeedScreen extends ConsumerWidget {
  const ActivityFeedScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final feedState = ref.watch(activityFeedNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.activityFeed),
      ),
      body: feedState.map(
        initial: (_) => const Center(child: CircularProgressIndicator()),
        loading: (_) => const Center(child: CircularProgressIndicator()),
        loaded: (loaded) => _LoadedFeed(loaded: loaded, ref: ref),
        error: (_) => _buildErrorOrEmpty(context, ref, feedState),
      ),
    );
  }

  Widget _buildErrorOrEmpty(
      BuildContext context, WidgetRef ref, ActivityFeedState feedState) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.dynamic_feed_outlined, size: 64, color: Colors.grey),
          const SizedBox(height: 16),
          Text(
            context.l10n.noActivityYet,
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: 8),
          Text(
            context.l10n.followPeopleToSeeActivity,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.grey,
                ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 24),
          OutlinedButton(
            onPressed: () =>
                ref.read(activityFeedNotifierProvider.notifier).loadFeed(),
            child: Text(context.l10n.retry),
          ),
        ],
      ),
    );
  }
}

class _LoadedFeed extends StatefulWidget {
  const _LoadedFeed({required this.loaded, required this.ref});

  final ActivityFeedLoaded loaded;
  final WidgetRef ref;

  @override
  State<_LoadedFeed> createState() => _LoadedFeedState();
}

class _LoadedFeedState extends State<_LoadedFeed> {
  final _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 200) {
      widget.ref.read(activityFeedNotifierProvider.notifier).loadMore();
    }
  }

  @override
  Widget build(BuildContext context) {
    if (widget.loaded.items.isEmpty) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(Icons.dynamic_feed_outlined,
                size: 64, color: Colors.grey),
            const SizedBox(height: 16),
            Text(
              context.l10n.noActivityYet,
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            Text(
              context.l10n.followPeopleToSeeActivity,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Colors.grey,
                  ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () =>
          widget.ref.read(activityFeedNotifierProvider.notifier).loadFeed(),
      child: ListView.builder(
        controller: _scrollController,
        padding: const EdgeInsets.symmetric(vertical: 8),
        itemCount:
            widget.loaded.items.length + (widget.loaded.isLoadingMore ? 1 : 0),
        itemBuilder: (context, index) {
          if (index == widget.loaded.items.length) {
            return const Padding(
              padding: EdgeInsets.all(16),
              child: Center(child: CircularProgressIndicator()),
            );
          }
          return ActivityFeedCard(item: widget.loaded.items[index]);
        },
      ),
    );
  }
}
