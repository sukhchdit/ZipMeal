import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/dispute_repository.dart';
import 'disputes_state.dart';

part 'disputes_notifier.g.dart';

@riverpod
class DisputesNotifier extends _$DisputesNotifier {
  late DisputeRepository _repo;

  @override
  DisputesState build() {
    _repo = ref.watch(disputeRepositoryProvider);
    return const DisputesState.initial();
  }

  Future<void> loadDisputes() async {
    state = const DisputesState.loading();
    final result = await _repo.getMyDisputes();
    if (result.failure != null) {
      state = DisputesState.error(message: result.failure!.message);
      return;
    }
    state = DisputesState.loaded(
      disputes: result.data ?? [],
      nextCursor: result.nextCursor,
      hasMore: result.hasMore,
    );
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! DisputesLoaded || !current.hasMore || current.isLoadingMore) {
      return;
    }
    state = current.copyWith(isLoadingMore: true);
    final result = await _repo.getMyDisputes(cursor: current.nextCursor);
    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }
    state = current.copyWith(
      disputes: [...current.disputes, ...result.data ?? []],
      nextCursor: result.nextCursor,
      hasMore: result.hasMore,
      isLoadingMore: false,
    );
  }
}
