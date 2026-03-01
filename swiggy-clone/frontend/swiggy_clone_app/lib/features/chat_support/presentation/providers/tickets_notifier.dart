import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/support_ticket_model.dart';
import '../../data/repositories/chat_support_repository.dart';
import 'tickets_state.dart';

part 'tickets_notifier.g.dart';

@riverpod
class TicketsNotifier extends _$TicketsNotifier {
  late ChatSupportRepository _repository;

  @override
  TicketsState build() {
    _repository = ref.watch(chatSupportRepositoryProvider);
    loadTickets();
    return const TicketsState.initial();
  }

  Future<void> loadTickets() async {
    state = const TicketsState.loading();
    final result = await _repository.getMyTickets();

    if (result.failure != null) {
      state = TicketsState.error(failure: result.failure!);
    } else {
      final data = result.data!;
      final items = (data['items'] as List<dynamic>?)
              ?.map((e) => SupportTicketSummaryModel.fromJson(
                  e as Map<String, dynamic>))
              .toList() ??
          [];

      state = TicketsState.loaded(
        tickets: items,
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool? ?? false,
      );
    }
  }

  Future<void> loadMore() async {
    final currentState = state;
    if (currentState is! TicketsLoaded ||
        !currentState.hasMore ||
        currentState.isLoadingMore) {
      return;
    }

    state = currentState.copyWith(isLoadingMore: true);

    final result = await _repository.getMyTickets(
      cursor: currentState.nextCursor,
    );

    if (result.failure != null) {
      state = currentState.copyWith(isLoadingMore: false);
    } else {
      final data = result.data!;
      final newItems = (data['items'] as List<dynamic>?)
              ?.map((e) => SupportTicketSummaryModel.fromJson(
                  e as Map<String, dynamic>))
              .toList() ??
          [];

      state = TicketsState.loaded(
        tickets: [...currentState.tickets, ...newItems],
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool? ?? false,
      );
    }
  }
}
