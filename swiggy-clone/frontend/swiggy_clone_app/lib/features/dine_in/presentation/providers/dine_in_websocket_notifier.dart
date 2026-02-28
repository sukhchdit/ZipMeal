import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/network/signalr_provider.dart';
import 'dine_in_session_notifier.dart';
import 'session_orders_notifier.dart';

part 'dine_in_websocket_notifier.g.dart';

/// Manages the SignalR hub subscription for a dine-in session and dispatches
/// received events to the appropriate notifiers.
@riverpod
class DineInWebSocketNotifier extends _$DineInWebSocketNotifier {
  @override
  void build(String sessionId) {
    final signalR = ref.watch(signalRServiceProvider);

    signalR.joinDineInSession(sessionId);

    signalR.onDineInEvent = (data) {
      final eventSessionId = data['sessionId'] as String?;
      if (eventSessionId != sessionId) return;
      _handleEvent(data, sessionId);
    };

    ref.onDispose(() {
      signalR.leaveDineInSession(sessionId);
    });
  }

  void _handleEvent(Map<String, dynamic> event, String sessionId) {
    final type = event['eventType'] as String?;
    switch (type) {
      case 'MemberJoined':
      case 'MemberLeft':
      case 'BillRequested':
      case 'SessionEnded':
        ref
            .read(dineInSessionNotifierProvider(sessionId).notifier)
            .loadSession();
      case 'OrderPlaced':
      case 'OrderStatusChanged':
        ref
            .read(sessionOrdersNotifierProvider(sessionId).notifier)
            .loadOrders();
      case 'SessionStarted':
        ref
            .read(dineInSessionNotifierProvider(sessionId).notifier)
            .loadSession();
    }
  }
}
