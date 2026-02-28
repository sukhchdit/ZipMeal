import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'home_feed_state.dart';

part 'home_feed_notifier.g.dart';

@riverpod
class HomeFeedNotifier extends _$HomeFeedNotifier {
  late DiscoveryRepository _repository;

  @override
  HomeFeedState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    loadFeed();
    return const HomeFeedState.initial();
  }

  Future<void> loadFeed({String? city}) async {
    state = const HomeFeedState.loading();
    final result = await _repository.getHomeFeed(city: city);
    if (result.failure != null) {
      state = HomeFeedState.error(failure: result.failure!);
    } else {
      state = HomeFeedState.loaded(feed: result.data!);
    }
  }
}
