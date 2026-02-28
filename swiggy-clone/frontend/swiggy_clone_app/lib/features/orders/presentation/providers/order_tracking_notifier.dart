import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/network/signalr_provider.dart';
import '../../../deliveries/presentation/providers/delivery_tracking_notifier.dart';
import 'order_detail_notifier.dart';

part 'order_tracking_notifier.g.dart';

/// Subscribes to real-time SignalR updates for a specific order.
///
/// When an OrderStatusChanged event arrives, the [OrderDetailNotifier] is
/// refreshed. When a DeliveryLocationUpdated event arrives, coordinates are
/// surgically patched into [DeliveryTrackingNotifier] without a full REST
/// round-trip.
@riverpod
class OrderTrackingNotifier extends _$OrderTrackingNotifier {
  @override
  void build(String orderId) {
    final signalR = ref.watch(signalRServiceProvider);

    signalR.subscribeToOrder(orderId);

    // Wire up SignalR callbacks
    signalR.onOrderStatusChanged = (data) {
      final eventOrderId = data['orderId'] as String?;
      if (eventOrderId == orderId) {
        ref.read(orderDetailNotifierProvider(orderId).notifier).loadDetail();
      }
    };

    signalR.onDeliveryLocationUpdated = (data) {
      final eventOrderId = data['orderId'] as String?;
      if (eventOrderId == orderId) {
        final lat = data['latitude'] as double?;
        final lng = data['longitude'] as double?;
        if (lat != null && lng != null) {
          ref
              .read(deliveryTrackingNotifierProvider(orderId).notifier)
              .updateLocation(lat, lng);
        }
      }
    };

    ref.onDispose(() {
      signalR.unsubscribeFromOrder(orderId);
    });
  }
}
