import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/network/signalr_service.dart';
import '../../../../core/storage/secure_storage_service.dart';
import '../../data/models/support_message_model.dart';
import 'messages_notifier.dart';
import 'typing_indicator_notifier.dart';

part 'chat_websocket_notifier.g.dart';

@riverpod
class ChatWebSocketNotifier extends _$ChatWebSocketNotifier {
  SignalRService? _signalR;

  @override
  bool build(String ticketId) {
    final secureStorage = ref.watch(secureStorageServiceProvider);
    _signalR = SignalRService(secureStorage: secureStorage);

    _connect();

    ref.onDispose(() {
      _signalR?.leaveChatTicket(ticketId);
      _signalR?.disconnect();
    });

    return false; // connected state
  }

  Future<void> _connect() async {
    await _signalR?.connect();

    _signalR?.onNewChatMessage = (data) {
      final messageDetails = data['messageDetails'] as Map<String, dynamic>?;
      if (messageDetails != null) {
        final message = SupportMessageModel.fromJson(messageDetails);
        ref
            .read(messagesNotifierProvider(ticketId).notifier)
            .addRealtimeMessage(message);
      }
    };

    _signalR?.onTypingIndicator = (data) {
      final isTyping = data['isTyping'] as bool? ?? false;
      ref
          .read(typingIndicatorNotifierProvider(ticketId).notifier)
          .setTyping(isTyping);
    };

    await _signalR?.joinChatTicket(ticketId);
    state = true;
  }

  Future<void> sendTypingIndicator(bool isTyping) async {
    await _signalR?.sendTypingIndicator(ticketId, isTyping);
  }
}
