import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/support_message_model.dart';
import '../../data/repositories/chat_support_repository.dart';
import 'messages_state.dart';

part 'messages_notifier.g.dart';

@riverpod
class MessagesNotifier extends _$MessagesNotifier {
  late ChatSupportRepository _repository;

  @override
  MessagesState build(String ticketId) {
    _repository = ref.watch(chatSupportRepositoryProvider);
    loadMessages();
    return const MessagesState.initial();
  }

  Future<void> loadMessages() async {
    state = const MessagesState.loading();
    final result = await _repository.getTicketMessages(ticketId);

    if (result.failure != null) {
      state = MessagesState.error(failure: result.failure!);
    } else {
      final data = result.data!;
      final items = (data['items'] as List<dynamic>?)
              ?.map((e) => SupportMessageModel.fromJson(
                  e as Map<String, dynamic>))
              .toList() ??
          [];

      state = MessagesState.loaded(
        messages: items,
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool? ?? false,
      );
    }
  }

  Future<void> loadMore() async {
    final currentState = state;
    if (currentState is! MessagesLoaded ||
        !currentState.hasMore ||
        currentState.isLoadingMore) {
      return;
    }

    state = currentState.copyWith(isLoadingMore: true);

    final result = await _repository.getTicketMessages(
      ticketId,
      cursor: currentState.nextCursor,
    );

    if (result.failure != null) {
      state = currentState.copyWith(isLoadingMore: false);
    } else {
      final data = result.data!;
      final newItems = (data['items'] as List<dynamic>?)
              ?.map((e) => SupportMessageModel.fromJson(
                  e as Map<String, dynamic>))
              .toList() ??
          [];

      state = MessagesState.loaded(
        messages: [...currentState.messages, ...newItems],
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool? ?? false,
      );
    }
  }

  Future<void> sendMessage(String content, {int messageType = 0}) async {
    final result = await _repository.sendMessage(
      ticketId,
      content: content,
      messageType: messageType,
    );

    if (result.data != null) {
      final currentState = state;
      if (currentState is MessagesLoaded) {
        state = currentState.copyWith(
          messages: [result.data!, ...currentState.messages],
        );
      }
    }
  }

  void addRealtimeMessage(SupportMessageModel message) {
    final currentState = state;
    if (currentState is MessagesLoaded) {
      // Avoid duplicates
      final exists = currentState.messages.any((m) => m.id == message.id);
      if (!exists) {
        state = currentState.copyWith(
          messages: [message, ...currentState.messages],
        );
      }
    }
  }
}
