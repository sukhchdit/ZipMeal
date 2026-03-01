import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/social_repository.dart';
import 'activity_feed_state.dart';

part 'activity_feed_notifier.g.dart';

@riverpod
class ActivityFeedNotifier extends _$ActivityFeedNotifier {
  late SocialRepository _repository;

  @override
  ActivityFeedState build() {
    _repository = ref.watch(socialRepositoryProvider);
    loadFeed();
    return const ActivityFeedState.initial();
  }

  Future<void> loadFeed() async {
    state = const ActivityFeedState.loading();
    final result = await _repository.getActivityFeed();
    if (result.failure != null) {
      state = ActivityFeedState.error(failure: result.failure!);
    } else {
      final data = result.data!;
      state = ActivityFeedState.loaded(
        items: data.items,
        nextCursor: data.nextCursor,
        hasMore: data.hasMore,
      );
    }
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! ActivityFeedLoaded || !current.hasMore || current.isLoadingMore) {
      return;
    }

    state = current.copyWith(isLoadingMore: true);

    final result = await _repository.getActivityFeed(
      cursor: current.nextCursor,
    );

    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
    } else {
      final data = result.data!;
      state = ActivityFeedState.loaded(
        items: [...current.items, ...data.items],
        nextCursor: data.nextCursor,
        hasMore: data.hasMore,
      );
    }
  }
}
