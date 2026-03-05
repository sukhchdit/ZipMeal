import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/network/signalr_provider.dart';
import 'group_cart_notifier.dart';
import 'group_order_notifier.dart';

part 'group_order_websocket_notifier.g.dart';

@riverpod
class GroupOrderWebSocketNotifier extends _$GroupOrderWebSocketNotifier {
  @override
  void build(String groupOrderId) {
    final signalR = ref.watch(signalRServiceProvider);
    signalR.joinGroupOrder(groupOrderId);

    signalR.onGroupOrderEvent = (data) {
      final eventGroupOrderId = data['groupOrderId'] as String?;
      if (eventGroupOrderId != groupOrderId) return;
      _handleEvent(data, groupOrderId);
    };

    ref.onDispose(() {
      signalR.leaveGroupOrder(groupOrderId);
    });
  }

  void _handleEvent(Map<String, dynamic> event, String groupOrderId) {
    final type = event['eventType'] as String?;
    switch (type) {
      case 'ParticipantJoined':
      case 'ParticipantLeft':
      case 'ParticipantReady':
        ref
            .read(groupOrderNotifierProvider(groupOrderId).notifier)
            .loadGroupOrder();
      case 'CartUpdated':
        ref
            .read(groupCartNotifierProvider(groupOrderId).notifier)
            .loadCart();
        ref
            .read(groupOrderNotifierProvider(groupOrderId).notifier)
            .loadGroupOrder();
      case 'GroupOrderFinalized':
      case 'GroupOrderCancelled':
      case 'GroupOrderExpired':
        ref
            .read(groupOrderNotifierProvider(groupOrderId).notifier)
            .loadGroupOrder();
    }
  }
}
