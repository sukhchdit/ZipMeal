import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/network/signalr_provider.dart';
import 'order_detail_notifier.dart';

part 'order_tracking_notifier.g.dart';

/// Subscribes to real-time SignalR updates for a specific order.
///
/// When an OrderStatusChanged or DeliveryLocationUpdated event arrives for
/// this order, the [OrderDetailNotifier] is refreshed.
@riverpod
class OrderTrackingNotifier extends _$OrderTrackingNotifier {
  @override
  void build(String orderId) {
    final signalR = ref.watch(signalRServiceProvider);

    signalR.subscribeToOrder(orderId);

    // Wire up SignalR callbacks to refresh order detail
    signalR.onOrderStatusChanged = (data) {
      final eventOrderId = data['orderId'] as String?;
      if (eventOrderId == orderId) {
        ref.read(orderDetailNotifierProvider(orderId).notifier).loadDetail();
      }
    };

    signalR.onDeliveryLocationUpdated = (data) {
      final eventOrderId = data['orderId'] as String?;
      if (eventOrderId == orderId) {
        ref.read(orderDetailNotifierProvider(orderId).notifier).loadDetail();
      }
    };

    ref.onDispose(() {
      signalR.unsubscribeFromOrder(orderId);
    });
  }
}
