import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/dispute_repository.dart';
import 'dispute_detail_state.dart';

part 'dispute_detail_notifier.g.dart';

@riverpod
class DisputeDetailNotifier extends _$DisputeDetailNotifier {
  late DisputeRepository _repo;

  @override
  DisputeDetailState build(String disputeId) {
    _repo = ref.watch(disputeRepositoryProvider);
    return const DisputeDetailState.initial();
  }

  Future<void> load() async {
    state = const DisputeDetailState.loading();

    final detailResult = await _repo.getDisputeDetail(disputeId);
    if (detailResult.failure != null) {
      state = DisputeDetailState.error(message: detailResult.failure!.message);
      return;
    }

    final messagesResult = await _repo.getDisputeMessages(disputeId);
    if (messagesResult.failure != null) {
      state =
          DisputeDetailState.error(message: messagesResult.failure!.message);
      return;
    }

    state = DisputeDetailState.loaded(
      dispute: detailResult.data!,
      messages: messagesResult.data ?? [],
      nextCursor: messagesResult.nextCursor,
      hasMore: messagesResult.hasMore,
    );
  }

  Future<void> loadMoreMessages() async {
    final current = state;
    if (current is! DisputeDetailLoaded ||
        !current.hasMore ||
        current.isLoadingMore) {
      return;
    }
    state = current.copyWith(isLoadingMore: true);
    final result = await _repo.getDisputeMessages(
      disputeId,
      cursor: current.nextCursor,
    );
    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }
    state = current.copyWith(
      messages: [...current.messages, ...result.data ?? []],
      nextCursor: result.nextCursor,
      hasMore: result.hasMore,
      isLoadingMore: false,
    );
  }

  Future<void> sendMessage(String content) async {
    final current = state;
    if (current is! DisputeDetailLoaded || current.isSending) return;

    state = current.copyWith(isSending: true);
    final result = await _repo.addMessage(
      disputeId: disputeId,
      content: content,
    );
    if (result.failure != null) {
      state = current.copyWith(isSending: false);
      return;
    }
    state = current.copyWith(
      messages: [result.data!, ...current.messages],
      isSending: false,
    );
  }
}
